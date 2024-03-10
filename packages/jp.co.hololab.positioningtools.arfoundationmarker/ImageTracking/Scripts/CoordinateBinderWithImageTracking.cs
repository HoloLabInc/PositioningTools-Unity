using System.Collections;
using System.Collections.Generic;
using HoloLab.PositioningTools.CoordinateSystem;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

namespace HoloLab.PositioningTools.ARFoundationMarker
{
    public class CoordinateBinderWithImageTracking : MonoBehaviour
    {
        private ARTrackedImageManager arTrackedImageManager;
        private CoordinateManager coordinateManager;

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

        private void Start()
        {

        }


        private void ARTrackedImageManager_trackedImagesChanged(ARTrackedImagesChangedEventArgs eventArgs)
        {
            Debug.Log("changed");
            foreach (var newImage in eventArgs.added)
            {
                if (IsTrackingReliable(newImage))
                {
                    BindSpaceCoordinates(newImage);
                }
            }

            foreach (var updatedImage in eventArgs.updated)
            {
                Debug.Log("Updated image: " + updatedImage.referenceImage.name);
                if (IsTrackingReliable(updatedImage))
                {
                    BindSpaceCoordinates(updatedImage);
                }
            }

            foreach (var removedImage in eventArgs.removed)
            {
                // UnbindSpaceCoordinates(removedImage);
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

        private bool IsTrackingReliable(ARTrackedImage arTrackedImage)
        {
            return true;
        }

        private const string spaceType = SpaceOrigin.SpaceTypeMarker;

        private static SpaceBinding ARTrackedImageToSpaceBinding(ARTrackedImage trackedImage)
        {
            var imageTransform = trackedImage.transform;
            var pose = new Pose(imageTransform.position, imageTransform.rotation);
            // var spaceId = trackedImage.name;
            // Debug.Log(spaceId);
            Debug.Log(trackedImage.referenceImage.name);
            var spaceId = trackedImage.referenceImage.name;
            var spaceBinding = new SpaceBinding(pose, spaceType, spaceId);
            return spaceBinding;
        }
    }
}
