#if QRTRACKING_PRESENT
using System.Collections.Generic;
using HoloLab.ARFoundationQRTracking;
using HoloLab.PositioningTools.CoordinateSerialization;
using HoloLab.PositioningTools.CoordinateSystem;
#endif

using UnityEngine;

namespace HoloLab.PositioningTools.ARFoundationMarker
{
    public class CoordinateBinderWithQRTracking : MonoBehaviour
    {
#if QRTRACKING_PRESENT
        private CoordinateManager coordinateManager;
        private ARFoundationQRTracker qrTracker;

        private readonly Dictionary<ARTrackedQRImage, ICoordinateInfo> coordinateInfoDictionary = new Dictionary<ARTrackedQRImage, ICoordinateInfo>();
        private readonly CoordinateSerializer coordinateSerializer = new CoordinateSerializer();

        private static readonly float sin45 = Mathf.Sin(45 * Mathf.Deg2Rad);
        private const string spaceType = SpaceOrigin.SpaceTypeMarker;

        private void Start()
        {
            coordinateManager = CoordinateManager.Instance;

            qrTracker = FindObjectOfType<ARFoundationQRTracker>();
            if (qrTracker == null)
            {
#if ARFOUNDATION_5_OR_NEWER
                Debug.LogError($"{nameof(ARFoundationQRTracker)} not found in scene. Please add {nameof(ARFoundationQRTracker)} to XR Origin");
#else
                Debug.LogError($"{nameof(ARFoundationQRTracker)} not found in scene. Please add {nameof(ARFoundationQRTracker)} to AR Session Origin");
#endif
            }
            else
            {
                qrTracker.OnTrackedQRImagesChanged += QRTracker_OnTrackedQRImagesChanged;
            }
        }

        private void QRTracker_OnTrackedQRImagesChanged(ARTrackedQRImagesChangedEventArgs eventArgs)
        {
            foreach (var addedQR in eventArgs.Added)
            {
                if (coordinateSerializer.TryDeserialize(addedQR.Text, out var coordinateInfo))
                {
                    coordinateInfoDictionary[addedQR] = coordinateInfo;
                }

                if (IsAccuratelyTracked(addedQR))
                {
                    BindSpaceCoordinates(addedQR);
                    BindWorldCoordinates(addedQR);
                }
            }

            foreach (var updatedQR in eventArgs.Updated)
            {
                if (IsAccuratelyTracked(updatedQR))
                {
                    BindSpaceCoordinates(updatedQR);
                    BindWorldCoordinates(updatedQR);
                }
            }

            foreach (var removedQR in eventArgs.Removed)
            {
                // Space binding
                var spaceBinding = ARTrackedQRImageToSpaceBinding(removedQR);
                coordinateManager.UnbindSpace(spaceBinding);

                // World binding
                coordinateInfoDictionary.Remove(removedQR);
            }
        }

        private void BindSpaceCoordinates(ARTrackedQRImage qr)
        {
            var spaceBinding = ARTrackedQRImageToSpaceBinding(qr);
            coordinateManager.BindSpace(spaceBinding);
        }

        private void BindWorldCoordinates(ARTrackedQRImage qr)
        {
            if (coordinateInfoDictionary.TryGetValue(qr, out var coordinateInfo) == false)
            {
                return;
            }

            if (TryConvertARTrackedQRImageToWorldBinding(qr, coordinateInfo, out var worldBinding))
            {
                coordinateManager.BindCoordinates(worldBinding);
            }
        }

        private static bool IsAccuratelyTracked(ARTrackedQRImage qr)
        {
            return qr.TrackingReliable && qr.ARTrackedImage.trackingState == UnityEngine.XR.ARSubsystems.TrackingState.Tracking;
        }

        private static SpaceBinding ARTrackedQRImageToSpaceBinding(ARTrackedQRImage qr)
        {
            var qrTransform = qr.transform;
            var pose = new Pose(qrTransform.position, qrTransform.rotation);
            var spaceBinding = new SpaceBinding(pose, spaceType, qr.Text);
            return spaceBinding;
        }

        private static bool TryConvertARTrackedQRImageToWorldBinding(ARTrackedQRImage qr, ICoordinateInfo coordinateInfo, out WorldBinding worldBinding)
        {
            switch (coordinateInfo)
            {
                case GeodeticPositionWithHeading geodeticPositionWithHeading:
                    {
                        var qrTransform = qr.transform;
                        var qrPosition = qrTransform.position;

                        Quaternion rotation;
                        var qrUp = qrTransform.up;
                        if (qrUp.y > sin45)
                        {
                            var forward = qrTransform.forward;
                            forward.y = 0;
                            rotation = Quaternion.LookRotation(forward, Vector3.up);
                        }
                        else
                        {
                            var forward = -qrUp;
                            forward.y = 0;
                            rotation = Quaternion.LookRotation(forward, Vector3.up);
                        }

                        var pose = new Pose(qrPosition, rotation);

                        var geodeticRotation = Quaternion.AngleAxis(geodeticPositionWithHeading.Heading, Vector3.up);
                        var geodeticPose = new GeodeticPose(geodeticPositionWithHeading.GeodeticPosition, geodeticRotation);

                        worldBinding = new WorldBinding(pose, geodeticPose);
                        return true;
                    }
            }

            worldBinding = null;
            return false;
        }
#endif
    }
}

