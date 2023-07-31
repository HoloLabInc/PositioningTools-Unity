using HoloLab.PositioningTools.GeographicCoordinate;
using System;
using System.Collections;
using UnityEngine;

namespace HoloLab.PositioningTools.CoordinateSystem
{
    public class WorldCoordinateBinderWithLocationService : MonoBehaviour
    {
        [SerializeField]
        private CardinalDirectionServiceComponent cardinalDirectionService = null;

        [SerializeField]
        private GeographicLocationServiceComponent geographicLocationService = null;

        [Tooltip("Transform of device pose\nIf null, use the pose of the main camera.")]
        [SerializeField]
        private Transform deviceTransform = null;
        public Transform DeviceTransform
        {
            get
            {
                return deviceTransform;
            }
            set
            {
                if (value != null)
                {
                    deviceTransform = value;
                }
                else
                {
                    deviceTransform = Camera.main.transform;
                }
            }
        }

        [Tooltip("Whether to automatically update the reference point when the location information is updated.")]
        [SerializeField]
        private bool autoUpdateReferencePoint = false;
        public bool AutoUpdateReferencePoint
        {
            get => autoUpdateReferencePoint;
            set => autoUpdateReferencePoint = value;
        }

        private GeographicLocation? latestLocation;
        private CardinalDirection? latestDirection;

        private WorldBinding referencePointBinding;
        private WorldBinding subReferencePointBinding;

        public event Action<WorldBinding> OnReferencePointBound;
        public event Action<WorldBinding> OnSubReferencePointBound;

        private void Start()
        {
            if (deviceTransform == null)
            {
                deviceTransform = Camera.main.transform;
            }

            cardinalDirectionService.OnDirectionUpdated += CardinalDirectionService_OnDirectionUpdated;
            geographicLocationService.OnLocationUpdated += GeographicLocationService_OnLocationUpdated;
        }

        private void GeographicLocationService_OnLocationUpdated(GeographicLocation geographicLocation)
        {
            latestLocation = geographicLocation;
            if (autoUpdateReferencePoint)
            {
                BindReferencePoint();
            }
        }

        private void CardinalDirectionService_OnDirectionUpdated(CardinalDirection cardinalDirection)
        {
            latestDirection = cardinalDirection;
            if (autoUpdateReferencePoint)
            {
                BindReferencePoint();
            }
        }

        /// <summary>
        /// Set the reference point.
        /// </summary>
        public void BindReferencePoint()
        {
            if (latestLocation == null || latestDirection == null)
            {
                return;
            }

            referencePointBinding = GetWorldBinding();
            OnReferencePointBound?.Invoke(referencePointBinding);

            Bind();
        }

        public void UnbindReferencePoint()
        {
            referencePointBinding = null;
            Bind();
        }

        /// <summary>
        /// Set the sub reference point.
        /// The sub reference point is used to specify the orientation of the reference point.
        /// </summary>
        public void BindSubReferencePoint()
        {
            if (latestLocation == null || latestDirection == null)
            {
                return;
            }

            subReferencePointBinding = GetWorldBinding();
            OnSubReferencePointBound?.Invoke(subReferencePointBinding);

            Bind();
        }

        public void UnbindSubReferencePoint()
        {
            subReferencePointBinding = null;
            Bind();
        }

        private void Bind()
        {
            var spaceCoordinateManager = CoordinateManager.Instance;

            if (referencePointBinding == null)
            {
                return;
            }

            // If the sub reference point is not specified, only the referene point is used.
            WorldBinding binding;
            if (subReferencePointBinding == null)
            {
                binding = referencePointBinding;
            }
            else
            {
                binding = CalcWorldBindingWithSubReferencePoint(referencePointBinding, subReferencePointBinding);
            }
            spaceCoordinateManager.BindCoordinates(binding);
        }

        internal static WorldBinding CalcWorldBindingWithSubReferencePoint(WorldBinding referencePoint, WorldBinding subReferencePoint)
        {
            var forward = subReferencePoint.ApplicationPose.Value.position - referencePoint.ApplicationPose.Value.position;
            forward.y = 0;
            var cameraRotation = Quaternion.LookRotation(forward, Vector3.up);

            var applicationPose = new Pose(referencePoint.ApplicationPose.Value.position, cameraRotation);

            var enu = GeographicCoordinateConversion.GeodeticToEnu(subReferencePoint.GeodeticPose.GeodeticPosition, referencePoint.GeodeticPose.GeodeticPosition);
            var headingDegrees = Mathf.Atan2((float)enu.East, (float)enu.North) * Mathf.Rad2Deg;

            var geodeticPose = new GeodeticPose(referencePoint.GeodeticPose.GeodeticPosition, Quaternion.AngleAxis(headingDegrees, Vector3.up));

            return new WorldBinding(applicationPose, geodeticPose);
        }

        private WorldBinding GetWorldBinding()
        {
            var devicePose = GetDevicePose();
            var geodeticPose = GetGeodeticPose();

            if (geodeticPose == null)
            {
                return null;
            }

            return new WorldBinding(devicePose, geodeticPose);
        }

        private GeodeticPose GetGeodeticPose()
        {
            if (latestLocation == null || latestDirection == null)
            {
                return null;
            }

            var location = latestLocation.Value;
            var geodeticPosition = new GeodeticPosition(location.Latitude, location.Longitude, location.Height);

            var geodeticRotation = Quaternion.AngleAxis(latestDirection.Value.HeadingDegrees, Vector3.up);
            var geodeticPose = new GeodeticPose(geodeticPosition, geodeticRotation);
            return geodeticPose;
        }

        private Pose GetDevicePose()
        {
            // TODO: Support for various orientations.
            // Currently, considering the case where the screen is vertical.
            var forward = deviceTransform.forward;
            forward.y = 0;
            var rotation = Quaternion.LookRotation(forward, Vector3.up);
            var pose = new Pose(deviceTransform.position, rotation);
            return pose;
        }

        private Quaternion LookRotationRight(Vector3 right)
        {
            var rotation = Quaternion.LookRotation(right, Vector3.up);
            rotation = rotation * Quaternion.AngleAxis(90, Vector3.up);
            return rotation;
        }
    }
}