using HoloLab.PositioningTools.CoordinateSystem;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

#if VUFORIA_PRESENT
using Vuforia;
#endif

namespace HoloLab.PositioningTools.Vuforia
{
    public class SpaceBinderWithVuforiaAreaTarget : MonoBehaviour
    {
#if VUFORIA_PRESENT
        public Action<ObserverBehaviour, TargetStatus> OnTrackingStatusChanged;
#endif

        private void Start()
        {
#if VUFORIA_PRESENT
            // Get AreaTargetBehaviour in the scene
            var areaTargetsInScene = GameObject.FindObjectsOfType<AreaTargetBehaviour>();
            foreach (var areaTargetBehaviour in areaTargetsInScene)
            {
                areaTargetBehaviour.OnTargetStatusChanged += AreaTargetBehaviour_OnTargetStatusChanged;
            }
#else
            Debug.LogWarning("Vuforia Engine is not installed");
#endif
        }

        public void LoadAreaTarget(string dataSetXmlPath, string dataSetName = "")
        {
            if (string.IsNullOrEmpty(dataSetName))
            {
                dataSetName = Path.GetFileNameWithoutExtension(dataSetXmlPath);
            }

#if VUFORIA_PRESENT
            var areaTargetBehaviour = VuforiaBehaviour.Instance.ObserverFactory.CreateAreaTarget(dataSetXmlPath, dataSetName);

            // TODO Requires investigation.
            // In the case of HoloLens, if the following line is not called, the alignment will not match after tracking loss.
            // In the case of iOS, if the following line is called, the camera image will not be displayed on the screen.
#if WINDOWS_UWP
            areaTargetBehaviour.gameObject.AddComponent<DefaultObserverEventHandler>();
#endif

            areaTargetBehaviour.OnTargetStatusChanged += AreaTargetBehaviour_OnTargetStatusChanged;
#else
            Debug.LogWarning("Vuforia Engine is not installed");
#endif
        }

#if VUFORIA_PRESENT
        private void AreaTargetBehaviour_OnTargetStatusChanged(ObserverBehaviour observerBehaviour, TargetStatus targetStatus)
        {
            var mapId = observerBehaviour.TargetName;

            var status = targetStatus.Status;
            if (status == Status.NO_POSE || status == Status.LIMITED)
            {
                UnbindSpace(mapId);
            }
            else
            {
                BindSpace(mapId, observerBehaviour.transform);
            }

            OnTrackingStatusChanged?.Invoke(observerBehaviour, targetStatus);
        }
#endif

        private void BindSpace(string mapId, Transform mapTransform)
        {
            var spaceType = SpaceOrigin.SpaceTypeVuforiaAreaTarget;

            var spaceBinding = new SpaceBinding(mapTransform, spaceType, mapId);
            var coordinateManager = CoordinateManager.Instance;
            coordinateManager.BindSpace(spaceBinding);
        }

        private void UnbindSpace(string mapId)
        {
            var spaceType = SpaceOrigin.SpaceTypeVuforiaAreaTarget;

            var spaceBinding = new SpaceBinding(null, spaceType, mapId);
            var coordinateManager = CoordinateManager.Instance;
            coordinateManager.UnbindSpace(spaceBinding);
        }
    }
}
