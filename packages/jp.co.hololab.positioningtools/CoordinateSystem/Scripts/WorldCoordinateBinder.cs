using HoloLab.PositioningTools.GeographicCoordinate;
using System.Collections;
using UnityEngine;

namespace HoloLab.PositioningTools.CoordinateSystem
{
    /// <summary>
    /// A component for binding between Unity coordinates and geographic coordinates.
    /// </summary>
    public class WorldCoordinateBinder : BinderComponentBase
    {
        [SerializeField]
        [Tooltip("latitude/longitude/height")]
        private GeodeticPositionForInspector geodeticPosition;

        [SerializeField]
        private float northHeading;

        public GeodeticPosition GeodeticPosition
        {
            get
            {
                return geodeticPosition.ToGeodeticPosition();
            }
            set
            {
                geodeticPosition = new GeodeticPositionForInspector(value);
                BindIfBindingValid();
            }
        }

        public float NorthHeading
        {
            get
            {
                return northHeading;
            }
            set
            {
                northHeading = value;
                BindIfBindingValid();
            }
        }

        private void Start()
        {
            BindIfBindingValid();
        }

        private void BindIfBindingValid()
        {
            if (IsBindingValid())
            {
                Bind();
            }
        }

        private void Bind()
        {
            var gPosition = geodeticPosition.ToGeodeticPosition();
            var rotation = Quaternion.AngleAxis(northHeading, Vector3.up);
            var gPose = new GeodeticPose(gPosition, rotation);

            var binding = new WorldBinding(transform, gPose);

            var spaceCoordinateManager = CoordinateManager.Instance;
            spaceCoordinateManager.BindCoordinates(binding);
        }
    }
}