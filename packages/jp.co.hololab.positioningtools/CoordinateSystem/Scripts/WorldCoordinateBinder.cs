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

        private void Start()
        {
            if (IsBindingValid())
            {
                Bind();
            }
        }

        private void Bind()
        {
            var spaceCoordinateManager = CoordinateManager.Instance;
            var gPosition = this.geodeticPosition.ToGeodeticPosition();
            var rotation = Quaternion.AngleAxis(northHeading, Vector3.up);
            var gPose = new GeodeticPose(gPosition, rotation);

            var binding = new WorldBinding(transform, gPose);
            spaceCoordinateManager.BindCoordinates(binding);
        }
    }
}