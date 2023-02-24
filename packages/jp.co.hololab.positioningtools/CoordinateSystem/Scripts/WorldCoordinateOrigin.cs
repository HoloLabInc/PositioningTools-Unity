﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HoloLab.PositioningTools.GeographicCoordinate;
#if UNITY_EDITOR
using UnityEditor;

#endif

namespace HoloLab.PositioningTools.CoordinateSystem
{
    [ExecuteAlways]
    public class WorldCoordinateOrigin : MonoBehaviour
    {
        [SerializeField]
        private PositionSettingModeType positionSettingMode;

        public PositionSettingModeType PositionSettingMode
        {
            set
            {
                positionSettingMode = value;
            }
            get
            {
                return positionSettingMode;
            }
        }

        [SerializeField]
        [Tooltip("latitude/longitude/ellipsoidal height")]
        private GeodeticPositionForInspector geodeticPosition;

        public GeodeticPosition GeodeticPosition
        {
            set
            {
                geodeticPosition = new GeodeticPositionForInspector(value);
            }
            get
            {
                return geodeticPosition.ToGeodeticPosition();
            }
        }

        [SerializeField]
        [Tooltip("Rotation in ENU coordinates")]
        private Quaternion enuRotation;

        public Quaternion EnuRotation
        {
            set
            {
                enuRotation = value;
            }
            get
            {
                return enuRotation;
            }
        }

        /// <summary>
        /// Whether to fix the rotation angle to "EnuRotation".
        /// </summary>
        public bool BindRotation { get; set; } = true;

        private CoordinateManager spaceCoordinateManager;

        public enum PositionSettingModeType
        {
            Transform = 0,
            GeodeticPosition
        }

        private void Start()
        {
            if (!Application.isPlaying)
            {
                return;
            }

            spaceCoordinateManager = CoordinateManager.Instance;
            spaceCoordinateManager.OnCoordinatesBound += OnCoordinatesBound;
            gameObject.SetActive(false);

            var latestWorldBinding = spaceCoordinateManager.LatestWorldBinding;
            if (latestWorldBinding != null)
            {
                OnCoordinatesBound(latestWorldBinding);
            }
        }

#if UNITY_EDITOR
        private void Update()
        {
            // Update position in Edit mode.
            if (!Application.isPlaying)
            {
                UpdateBinding();
            }
        }
#endif

        private void OnDestroy()
        {
            if (!Application.isPlaying)
            {
                return;
            }
            spaceCoordinateManager.OnCoordinatesBound -= OnCoordinatesBound;
        }

#if UNITY_EDITOR
        public void UpdateBinding()
        {
            if (spaceCoordinateManager == null)
            {
                spaceCoordinateManager = FindObjectOfType<CoordinateManager>();

                if (spaceCoordinateManager == null)
                {
                    return;
                }
            }

            var worldBinding = spaceCoordinateManager.WorldBindingInEditor;

            switch (positionSettingMode)
            {
                case PositionSettingModeType.Transform:
                    // If in "transform to lat/lon" mode, update latitude and longitude、
                    UpdateGeodeticPositionWithCurrentPosition(worldBinding);
                    break;
                case PositionSettingModeType.GeodeticPosition:
                    // If in "lat/lon to transform" mode, update transform.
                    BindCoordinates(worldBinding);
                    break;
            }
        }

#endif

        public void BindCoordinates(WorldBinding worldBinding)
        {
            var gp = geodeticPosition.ToGeodeticPosition();
            var geodeticPose = new GeodeticPose(gp, enuRotation);
            var pose = GetUnityPoseWithBoundPoint(geodeticPose, worldBinding.GeodeticPose, worldBinding.ApplicationPose);

            if (BindRotation)
            {
                transform.SetPositionAndRotation(pose.position, pose.rotation);
            }
            else
            {
                transform.position = pose.position;
            }
        }

        private void OnCoordinatesBound(WorldBinding worldBinding)
        {
            BindCoordinates(worldBinding);
            gameObject.SetActive(true);
        }

        private void UpdateGeodeticPositionWithCurrentPosition(WorldBinding worldBinding)
        {
            var pose = new Pose(transform.position, transform.rotation);
            var gp = GetGeodeticPoseWithBoundPoint(pose, worldBinding.GeodeticPose, worldBinding.ApplicationPose);
            geodeticPosition = new GeodeticPositionForInspector(gp.GeodeticPosition);
            enuRotation = gp.EnuRotation;
        }

        internal static Pose GetUnityPoseWithBoundPoint(GeodeticPose targetPose, GeodeticPose boundPoseInWorld,
            Pose boundPoseInUnity)
        {
            // ENU position seen from the bound point.
            var enuPosition =
                GeographicCoordinateConversion.GeodeticToEnu(targetPose.GeodeticPosition, boundPoseInWorld.GeodeticPosition);
            var boundPoseInWorldRotation = Quaternion.Inverse(boundPoseInWorld.EnuRotation);

            var enuPose = new Pose(boundPoseInWorldRotation * enuPosition.ToUnityVector(), boundPoseInWorldRotation * targetPose.EnuRotation);
            return enuPose.GetTransformedBy(boundPoseInUnity);
        }

        internal static Pose GetUnityPoseWithBoundPoint(GeodeticPosition targetPosition, GeodeticPose boundPoseInWorld,
            Pose boundPoseInUnity)
        {
            var targetPose = new GeodeticPose(targetPosition, Quaternion.identity);
            return GetUnityPoseWithBoundPoint(targetPose, boundPoseInWorld, boundPoseInUnity);
        }

        internal static GeodeticPose GetGeodeticPoseWithBoundPoint(Pose unityPose,
            GeodeticPose boundPoseInWorld, Pose boundPoseInUnity)
        {
            var unityPosition = unityPose.position;
            var unityRotation = unityPose.rotation;

            // Calculates the position in the ENU coordinate of "boundPose".
            var enuPositionVector = Quaternion.Inverse(boundPoseInUnity.rotation) * (unityPosition - boundPoseInUnity.position);
            var enuPosition = new EnuPosition(enuPositionVector.x, enuPositionVector.z, enuPositionVector.y);

            // Convert to latitude and longitude.
            var geodeticPosition =
                GeographicCoordinateConversion.EnuToGeodetic(enuPosition, boundPoseInWorld.GeodeticPosition);

            // Calculates the rotation in the ENU coordinate of "boundPose".
            // TODO: We should actually calculate "boundPoseInWorld.EnuRotation".
            // Additionally, we need to calculate the rotation in the ENU coordinate of "geodeticPosition".
            var enuRotation = Quaternion.Inverse(boundPoseInUnity.rotation) * unityRotation;
            return new GeodeticPose(geodeticPosition, enuRotation);
        }

#if UNITY_EDITOR
        [CustomEditor(typeof(WorldCoordinateOrigin))]
        private class WorldCoordinateOriginEditor : Editor
        {
            private SerializedProperty settingModeProperty;
            private SerializedProperty geodeticPositionProperty;
            private SerializedProperty enuRotationProperty;

            private void OnEnable()
            {
                settingModeProperty = serializedObject.FindProperty(nameof(positionSettingMode));
                geodeticPositionProperty = serializedObject.FindProperty(nameof(geodeticPosition));
                enuRotationProperty = serializedObject.FindProperty(nameof(enuRotation));
            }

            public override void OnInspectorGUI()
            {
                serializedObject.Update();
                // base.OnInspectorGUI();

                var worldCoordinateOrigin = target as WorldCoordinateOrigin;

                if (worldCoordinateOrigin == null)
                {
                    return;
                }

                EditorGUILayout.PropertyField(settingModeProperty);

                var editable = settingModeProperty.intValue == (int)PositionSettingModeType.GeodeticPosition;

                // Not editable if "editable" is false
                EditorGUI.BeginDisabledGroup(!editable);
                EditorGUILayout.PropertyField(geodeticPositionProperty);

                DrawEnuRotationField();

                EditorGUI.EndDisabledGroup();

                serializedObject.ApplyModifiedProperties();
            }

            private void DrawEnuRotationField()
            {
                var previousEnuEuler = enuRotationProperty.quaternionValue.eulerAngles;

                // Display up to the third decimal place.
                var x = float.Parse(previousEnuEuler.x.ToString("f3"));
                var y = float.Parse(previousEnuEuler.y.ToString("f3"));
                var z = float.Parse(previousEnuEuler.z.ToString("f3"));
                var enuEuler = EditorGUILayout.Vector3Field("EnuRotation", new Vector3(x, y, z));

                enuRotationProperty.quaternionValue = Quaternion.Euler(enuEuler);
            }
        }
#endif
    }
}