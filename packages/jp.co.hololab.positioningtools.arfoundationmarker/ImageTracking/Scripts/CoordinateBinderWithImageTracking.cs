using HoloLab.PositioningTools.CoordinateSystem;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

namespace HoloLab.PositioningTools.ARFoundationMarker
{
    public class CoordinateBinderWithImageTracking : MonoBehaviour
    {
        private ARTrackedImageManager arTrackedImageManager;
        private CoordinateManager coordinateManager;

#if QRTRACKING_PRESENT
        private HoloLab.ARFoundationQRTracking.iOS.EnableScaleEstimationForARKit enableScaleEstimationForARKit;
#endif

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

        private void Start()
        {
#if QRTRACKING_PRESENT
            enableScaleEstimationForARKit = FindObjectOfType<HoloLab.ARFoundationQRTracking.iOS.EnableScaleEstimationForARKit>();
#endif
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

        private bool IsTrackingReliable(ARTrackedImage arTrackedImage)
        {
#if !UNITY_EDITOR && (UNITY_IOS || UNITY_VISIONOS)
            if (ScaleEstimationEnabledInIOS())
            {
                return ScaleEstimatedInIOS(arTrackedImage);
            }
            else
            {
                return true;
            }
#else
            return true;
#endif
        }

        private bool ScaleEstimationEnabledInIOS()
        {
#if QRTRACKING_PRESENT
            return enableScaleEstimationForARKit != null;
#else
            return false;
#endif
        }

        private static bool ScaleEstimatedInIOS(ARTrackedImage arTrackedImage)
        {
#if QRTRACKING_PRESENT && (UNITY_IOS || UNITY_VISIONOS)
            var estimatedScale = HoloLab.ARFoundationQRTracking.iOS.ARKitImageScaleEstimationInterop.GetEstimatedScale(arTrackedImage);
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
