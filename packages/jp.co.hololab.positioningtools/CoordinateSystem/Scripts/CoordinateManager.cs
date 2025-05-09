using System;
using UnityEngine;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace HoloLab.PositioningTools.CoordinateSystem
{
    public class CoordinateManager : MonoBehaviour
    {
        [SerializeField]
        [Tooltip("Geodetic position that serves as the reference when placing objects in Editor.")]
        private GeodeticPositionForInspector geodeticPositionInEditMode;

        public WorldBinding WorldBindingInEditor
        {
            get
            {
                var pose = new Pose(transform.position, transform.rotation);
                var geodeticPose = new GeodeticPose(
                    geodeticPositionInEditMode.ToGeodeticPosition(),
                    Quaternion.identity);
                return new WorldBinding(pose, geodeticPose);
            }
        }

        private WorldBinding latestWorldBinding;
        public WorldBinding LatestWorldBinding => latestWorldBinding;

        private List<SpaceBinding> spaceBindingList = new List<SpaceBinding>();
        public List<SpaceBinding> SpaceBindingList => spaceBindingList;

        public event Action<WorldBinding> OnCoordinatesBound;

        public event Action<SpaceBinding> OnSpaceBound;

        public event Action<SpaceBinding> OnSpaceLost;

        private static CoordinateManager instance;
        public static CoordinateManager Instance
        {
            get
            {
                if (instance == null)
                {
#if UNITY_6000_0_OR_NEWER
                    instance = FindFirstObjectByType<CoordinateManager>();
#else
                    instance = FindObjectOfType<CoordinateManager>();
#endif
                    if (instance == null)
                    {
                        var instanceObject = new GameObject("CoordinateManager");
                        instance = instanceObject.AddComponent<CoordinateManager>();
                    }
                }
                return instance;
            }
        }

        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
            }
            else if (instance != this)
            {
                Debug.LogWarning("Multiple CoordinateManager found in scene.");
            }
        }

        public void BindCoordinates(WorldBinding worldBinding)
        {
            latestWorldBinding = worldBinding;
            OnCoordinatesBound?.Invoke(worldBinding);
        }

        /// <summary>
        /// Bind the Unity coordinates and the space coordinates.
        /// </summary>
        /// <param name="spaceBinding"></param>
        public void BindSpace(SpaceBinding spaceBinding)
        {
            // 同一のスペースが登録済みの場合は削除
            spaceBindingList.RemoveAll(x => x.SpaceType == spaceBinding.SpaceType && x.SpaceId == spaceBinding.SpaceId);
            spaceBindingList.Add(spaceBinding);

            OnSpaceBound?.Invoke(spaceBinding);
        }

        /// <summary>
        /// Unbind the Unity coordinates and the space coordinates.
        /// </summary>
        /// <param name="spaceId"></param>
        /// <param name="spaceType"></param>
        public void UnbindSpace(SpaceBinding spaceBinding)
        {
            spaceBindingList.RemoveAll(x => x.SpaceType == spaceBinding.SpaceType && x.SpaceId == spaceBinding.SpaceId);

            OnSpaceLost?.Invoke(spaceBinding);
        }

        public bool TryConvertUnityPoseToGeodeticPose(Pose poseInUnitySpace, out GeodeticPose geodeticPose)
        {
            var worldBinding = latestWorldBinding;
            if (worldBinding == null)
            {
                geodeticPose = null;
                return false;
            }

            if (worldBinding.Transform != null)
            {
                geodeticPose = WorldCoordinateUtils.GetGeodeticPoseWithBoundTransform(poseInUnitySpace, worldBinding.GeodeticPose, worldBinding.Transform);
                return true;
            }
            else if (worldBinding.ApplicationPose.HasValue)
            {
                geodeticPose = WorldCoordinateUtils.GetGeodeticPoseWithBoundPoint(poseInUnitySpace, worldBinding.GeodeticPose, worldBinding.ApplicationPose.Value);
                return true;
            }
            else
            {
                geodeticPose = null;
                return false;
            }
        }

#if UNITY_EDITOR
        [CustomEditor(typeof(CoordinateManager))]
        private class SpaceCoordinateManagerEditor : Editor
        {
            public override void OnInspectorGUI()
            {
                base.OnInspectorGUI();

                var spaceCoordinateManager = target as CoordinateManager;

                if (spaceCoordinateManager == null)
                {
                    return;
                }

                if (Application.isPlaying)
                {
                    return;
                }

                var worldBinding = spaceCoordinateManager.WorldBindingInEditor;
                if (GUILayout.Button("Align objects in scene"))
                {
#if UNITY_6000_0_OR_NEWER
                    var worldCoordinateOrigins = FindObjectsByType<WorldCoordinateOrigin>(FindObjectsSortMode.None);
#else
                    var worldCoordinateOrigins = FindObjectsOfType<WorldCoordinateOrigin>();
#endif
                    foreach (var worldCoordinateOrigin in worldCoordinateOrigins)
                    {
                        worldCoordinateOrigin.SyncGeodeticPoseAndUnityPose(worldBinding);
                    }
                }
            }
        }
#endif
    }
}