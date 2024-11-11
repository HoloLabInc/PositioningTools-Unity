using System;
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
        private PositionSettingModeType positionSettingMode = PositionSettingModeType.GeodeticPosition;

        public PositionSettingModeType PositionSettingMode
        {
            set
            {
                if (positionSettingMode == value)
                {
                    return;
                }

                var previousMode = positionSettingMode;
                positionSettingMode = value;

                if (previousMode == PositionSettingModeType.Transform)
                {
                    SyncGeodeticPoseAndUnityPose(GetLatestWorldBinding(), PositionSettingModeType.Transform);
                }
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
                UpdateGeodeticPoseInTransformMode();

                geodeticPosition = new GeodeticPositionForInspector(value);

                // Update transform
                SyncGeodeticPoseAndUnityPose(GetLatestWorldBinding(), PositionSettingModeType.GeodeticPosition);
            }
            get
            {
                UpdateGeodeticPoseInTransformMode();

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
                UpdateGeodeticPoseInTransformMode();

                enuRotation = value;

                // Update transform
                SyncGeodeticPoseAndUnityPose(GetLatestWorldBinding(), PositionSettingModeType.GeodeticPosition);
            }
            get
            {
                UpdateGeodeticPoseInTransformMode();
                return enuRotation;
            }
        }

        /// <summary>
        /// Whether to fix the rotation angle to "EnuRotation".
        /// </summary>
        public bool BindRotation { get; set; } = true;

        private CoordinateManager coordinateManager;

        private WorldBinding latestWorldBinding;

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

            coordinateManager = CoordinateManager.Instance;
            coordinateManager.OnCoordinatesBound += OnCoordinatesBound;
            gameObject.SetActive(false);

            var worldBinding = GetLatestWorldBinding();
            if (worldBinding != null)
            {
                OnCoordinatesBound(worldBinding);
            }
        }

        private void Update()
        {
            if (Application.isPlaying)
            {
                if (transform.hasChanged)
                {
                    if (latestWorldBinding != null)
                    {
                        SyncGeodeticPoseAndUnityPose(latestWorldBinding);
                    }
                    transform.hasChanged = false;
                }
            }

#if UNITY_EDITOR
            // Update position in Edit mode.
            if (!Application.isPlaying)
            {
                UpdateBindingInEditMode();
            }
#endif
        }

        private void OnDestroy()
        {
            if (!Application.isPlaying)
            {
                return;
            }

            if (coordinateManager != null)
            {
                coordinateManager.OnCoordinatesBound -= OnCoordinatesBound;
            }
        }

        [Obsolete("This method is obsolete.")]
        public void BindCoordinates(WorldBinding worldBinding)
        {
            UpdateTransformWithGeodeticPosition(worldBinding);
        }

#if UNITY_EDITOR
        private void UpdateBindingInEditMode()
        {
            WorldBinding worldBinding;

            var parentBinder = GetComponentInParent<WorldCoordinateBinder>();
            if (parentBinder != null)
            {
                worldBinding = parentBinder.TransformWorldBinding;
            }
            else if (coordinateManager == null)
            {
                coordinateManager = FindObjectOfType<CoordinateManager>();

                if (coordinateManager == null)
                {
                    return;
                }
                worldBinding = coordinateManager.WorldBindingInEditor;
            }
            else
            {
                return;
            }

            SyncGeodeticPoseAndUnityPose(worldBinding);
        }
#endif

        private WorldBinding GetLatestWorldBinding()
        {
            if (latestWorldBinding != null)
            {
                return latestWorldBinding;
            }

            var coordinateManager = CoordinateManager.Instance;
            if (coordinateManager != null)
            {
                return coordinateManager.LatestWorldBinding;
            }

            return null;
        }


        internal void SyncGeodeticPoseAndUnityPose(WorldBinding worldBinding)
        {
            SyncGeodeticPoseAndUnityPose(worldBinding, positionSettingMode);
        }

        internal void SyncGeodeticPoseAndUnityPose(WorldBinding worldBinding, PositionSettingModeType syncMode)
        {
            latestWorldBinding = worldBinding;

            switch (syncMode)
            {
                case PositionSettingModeType.Transform:
                    // If in "transform to lat/lon" mode, update latitude and longitude、
                    UpdateGeodeticPositionWithCurrentPosition(worldBinding);
                    break;
                case PositionSettingModeType.GeodeticPosition:
                    // If in "lat/lon to transform" mode, update transform.
                    UpdateTransformWithGeodeticPosition(worldBinding);
                    break;
            }
        }

        private void UpdateGeodeticPoseInTransformMode()
        {
            if (positionSettingMode == PositionSettingModeType.Transform)
            {
                SyncGeodeticPoseAndUnityPose(GetLatestWorldBinding(), PositionSettingModeType.Transform);
            }
        }

        private void UpdateTransformWithGeodeticPosition(WorldBinding worldBinding)
        {
            if (worldBinding == null)
            {
                return;
            }

            var gp = geodeticPosition.ToGeodeticPosition();
            var geodeticPose = new GeodeticPose(gp, enuRotation);

            Pose pose;

            if (worldBinding.Transform != null)
            {
                // Bind coordinates by transform
                if (worldBinding.Transform.lossyScale != Vector3.one)
                {
                    if (IsDescendantOf(transform, worldBinding.Transform) == false)
                    {
                        Debug.LogWarning("When the scale of binding is not Vector3.one, WorldCoordinateOrigin should be the descendant of binding transform.");
                    }
                }

                pose = GetUnityPoseWithBoundTransform(geodeticPose, worldBinding.GeodeticPose, worldBinding.Transform);
            }
            else if (worldBinding.ApplicationPose.HasValue)
            {
                // Bind coordinates by pose
                pose = GetUnityPoseWithBoundPoint(geodeticPose, worldBinding.GeodeticPose, worldBinding.ApplicationPose.Value);
            }
            else
            {
                Debug.LogError($"WorldBinding should have {nameof(worldBinding.Transform)} or {nameof(worldBinding.ApplicationPose)}");
                return;
            }

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
            SyncGeodeticPoseAndUnityPose(worldBinding);
            gameObject.SetActive(true);
        }

        private void UpdateGeodeticPositionWithCurrentPosition(WorldBinding worldBinding)
        {
            if (worldBinding == null)
            {
                return;
            }

            var pose = new Pose(transform.position, transform.rotation);

            GeodeticPose geodeticPose;

            if (worldBinding.Transform != null)
            {
                // Update position by transform
                geodeticPose = WorldCoordinateUtils.GetGeodeticPoseWithBoundTransform(pose, worldBinding.GeodeticPose, worldBinding.Transform);
            }
            else if (worldBinding.ApplicationPose.HasValue)
            {
                // Update position by pose
                geodeticPose = WorldCoordinateUtils.GetGeodeticPoseWithBoundPoint(pose, worldBinding.GeodeticPose, worldBinding.ApplicationPose.Value);
            }
            else
            {
                Debug.LogError($"WorldBinding should have {nameof(worldBinding.Transform)} or {nameof(worldBinding.ApplicationPose)}");
                return;
            }

            geodeticPosition = new GeodeticPositionForInspector(geodeticPose.GeodeticPosition);
            enuRotation = geodeticPose.EnuRotation;
        }

        internal static Pose GetUnityPoseWithBoundPoint(GeodeticPose targetPose, GeodeticPose boundPoseInWorld, Pose boundPoseInUnity)
        {
            var enuPose = GetEnuPoseWithBoundPose(targetPose, boundPoseInWorld);
            return enuPose.GetTransformedBy(boundPoseInUnity);
        }

        internal static Pose GetUnityPoseWithBoundPoint(GeodeticPosition targetPosition, GeodeticPose boundPoseInWorld, Pose boundPoseInUnity)
        {
            var targetPose = new GeodeticPose(targetPosition, Quaternion.identity);
            return GetUnityPoseWithBoundPoint(targetPose, boundPoseInWorld, boundPoseInUnity);
        }

        /// <summary>
        /// Convert geodetic pose to unity pose with bound point.
        /// </summary>
        /// <param name="targetPose"></param>
        /// <param name="boundPoseInWorld"></param>
        /// <param name="boundPoseInUnity"></param>
        /// <returns></returns>
        internal static Pose GetUnityPoseWithBoundTransform(GeodeticPose targetPose, GeodeticPose boundPoseInWorld,
            Transform boundTransformInUnity)
        {
            var enuPose = GetEnuPoseWithBoundPose(targetPose, boundPoseInWorld);
            return enuPose.GetTransformedBy(boundTransformInUnity);
        }

        private static Pose GetEnuPoseWithBoundPose(GeodeticPose targetPose, GeodeticPose boundPoseInWorld)
        {
            // ENU position seen from the bound point.
            var enuPosition =
                GeographicCoordinateConversion.GeodeticToEnu(targetPose.GeodeticPosition, boundPoseInWorld.GeodeticPosition);
            var boundPoseInWorldRotation = Quaternion.Inverse(boundPoseInWorld.EnuRotation);

            var enuPose = new Pose(boundPoseInWorldRotation * enuPosition.ToUnityVector(), boundPoseInWorldRotation * targetPose.EnuRotation);
            return enuPose;
        }

        private static bool IsDescendantOf(Transform child, Transform parent)
        {
            if (child == null || parent == null)
            {
                return false;
            }

            Transform current = child;
            while (current != null)
            {
                if (current == parent)
                {
                    return true;
                }
                current = current.parent;
            }
            return false;
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