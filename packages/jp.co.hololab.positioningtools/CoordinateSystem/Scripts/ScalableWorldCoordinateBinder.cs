using System.Collections;
using UnityEngine;

namespace HoloLab.PositioningTools.CoordinateSystem
{
    /// <summary>
    /// A component for binding between Unity coordinates and geographic coordinates.
    /// </summary>
    public class ScalableWorldCoordinateBinder : MonoBehaviour
    {
        [SerializeField]
        [Tooltip("latitude/longitude/height")]
        private GeodeticPositionForInspector geodeticPosition;

        [SerializeField]
        private float northHeading;

        private void Start()
        {
            StartCoroutine(Bind());
        }

        private IEnumerator Bind()
        {
            // Delay by one frame to wait for event registration.
            yield return null;

            var spaceCoordinateManager = CoordinateManager.Instance;
            // var pose = new Pose(transform.position, transform.rotation);
            var gPosition = this.geodeticPosition.ToGeodeticPosition();
            var rotation = Quaternion.AngleAxis(northHeading, Vector3.up);
            var gPose = new GeodeticPose(gPosition, rotation);

            var binding = new WorldBinding(transform, gPose);
            spaceCoordinateManager.BindCoordinates(binding);
        }
    }
}