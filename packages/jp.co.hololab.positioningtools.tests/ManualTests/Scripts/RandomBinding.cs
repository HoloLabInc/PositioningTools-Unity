using HoloLab.PositioningTools.GeographicCoordinate;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Random = UnityEngine.Random;

namespace HoloLab.PositioningTools.CoordinateSystem.Tests
{
    public class RandomBinding : MonoBehaviour
    {
        [SerializeField]
        private WorldCoordinateOrigin origin;

        [SerializeField]
        private GeodeticPositionForInspector geodeticPosition;

        private async void Awake()
        {
            while (Application.isPlaying)
            {
                await Task.Delay(5000);
                BindTest();
            }
        }

        public void BindTest()
        {
            var position = Random.insideUnitSphere;
            //var headingDegrees = Random.Range(0, 360);
            var headingDegrees = 10;

            Bind(position, headingDegrees);
        }

        public void Bind(Vector3 position, float headingDegrees)
        {
            // TODO: Support for various orientations.
            // Currently, considering the case where the screen is vertical.
            // var cameraTransform = mainCamera.transform;

            transform.rotation = Quaternion.AngleAxis(headingDegrees, Vector3.up);
            transform.position = position;

            var forward = transform.forward;
            forward.y = 0;
            var rotation = Quaternion.LookRotation(forward, Vector3.up);
            var pose = new Pose(transform.position, rotation);

            var location = GeographicCoordinateConversion.EnuToGeodetic(
                new EnuPosition(position.x, position.z, position.y),
                this.geodeticPosition.ToGeodeticPosition());
            var gp = new GeodeticPosition(location.Latitude, location.Longitude, location.EllipsoidalHeight);

            var geodeticRotation = Quaternion.AngleAxis(headingDegrees, Vector3.up);
            var geodeticPose = new GeodeticPose(gp, geodeticRotation);
            var spaceCoordinateManager = CoordinateManager.Instance;

            var binding = new WorldBinding(pose, geodeticPose);
            spaceCoordinateManager.BindCoordinates(binding);
        }
    }
}