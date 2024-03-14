using HoloLab.PositioningTools.CoordinateSystem;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

namespace HoloLab.PositioningTools.ARFoundationMarker
{
    public class CoordinateBinderWithImageTracking : MonoBehaviour
    {
        private ARTrackedImageManager arTrackedImageManager;
        private CoordinateManager coordinateManager;

        private const string spaceType = SpaceOrigin.SpaceTypeMarker;

        private void Awake()
        {
            arTrackedImageManager = FindObjectOfType<ARTrackedImageManager>();
            if (arTrackedImageManager == null)
            {
                Debug.LogError($"{nameof(ARTrackedImageManager)} not found in scene. Please add {nameof(ARTrackedImageManager)} to AR Session Origin");
            }

            coordinateManager = CoordinateManager.Instance;
        }

        private void OnEnable()
        {
            if (arTrackedImageManager != null)
            {
                arTrackedImageManager.trackedImagesChanged += ARTrackedImageManager_trackedImagesChanged;
            }
        }

        private void OnDisable()
        {
            if (arTrackedImageManager != null)
            {
                arTrackedImageManager.trackedImagesChanged -= ARTrackedImageManager_trackedImagesChanged;
            }
        }

        private void ARTrackedImageManager_trackedImagesChanged(ARTrackedImagesChangedEventArgs eventArgs)
        {
            foreach (var newImage in eventArgs.added)
            {
                if (IsTrackingReliable(newImage))
                {
                    BindSpaceCoordinates(newImage);
                }
            }

            foreach (var updatedImage in eventArgs.updated)
            {
                if (IsTrackingReliable(updatedImage))
                {
                    BindSpaceCoordinates(updatedImage);
                }
            }

            foreach (var removedImage in eventArgs.removed)
            {
                UnbindSpaceCoordinates(removedImage);
            }
        }

        private void BindSpaceCoordinates(ARTrackedImage trackedImage)
        {
            var spaceBinding = ARTrackedImageToSpaceBinding(trackedImage);
            if (string.IsNullOrEmpty(spaceBinding.SpaceId) == false)
            {
                coordinateManager.BindSpace(spaceBinding);
            }
        }

        private void UnbindSpaceCoordinates(ARTrackedImage trackedImage)
        {
            var spaceBinding = ARTrackedImageToSpaceBinding(trackedImage);
            if (string.IsNullOrEmpty(spaceBinding.SpaceId) == false)
            {
                coordinateManager.UnbindSpace(spaceBinding);
            }
        }

        private static bool IsTrackingReliable(ARTrackedImage arTrackedImage)
        {
#if UNITY_IOS || UNITY_VISIONOS
            if (arTrackedImage.referenceImage.specifySize)
            {
                return true;
            }
            return ScaleEstimatedInIOS();
#else
            return true;
#endif
        }

        private static bool ScaleEstimatedInIOS(ARTrackedImage arTrackedImage)
        {
#if QRTRACKING_PRESENT && (UNITY_IOS && UNITY_VISIONOS)
            var estimatedScale = HoloLab.ARFoundationQRTracking.iOS.ARKitImageScaleEstimationInterop.GetEstimatedScale(ARTrackedImage);
            return estimatedScale != 1.0;
#else
            Debug.LogWarning("ARFoundationQRTracking is needed to use scale estimation in iOS");
            return false;
#endif
        }

        private static SpaceBinding ARTrackedImageToSpaceBinding(ARTrackedImage trackedImage)
        {
            var imageTransform = trackedImage.transform;
            var pose = new Pose(imageTransform.position, imageTransform.rotation);
            var spaceId = trackedImage.referenceImage.name;
            var spaceBinding = new SpaceBinding(pose, spaceType, spaceId);
            return spaceBinding;
        }
    }
}

