using System.Collections;
using UnityEngine;

namespace HoloLab.PositioningTools.CoordinateSystem
{
    /// <summary>
    /// A component for binding between Unity coordinates and geographic coordinates.
    /// </summary>
    public class WorldCoordinateBinder : MonoBehaviour
    {
        private enum RuntimeType
        {
            None = 0,
            Editor = 1,
            Player = 2,
            EditorAndPlayer = 3,
        }

        [SerializeField]
        [Tooltip("latitude/longitude/height")]
        private GeodeticPositionForInspector geodeticPosition;

        [SerializeField]
        private float northHeading;

        [SerializeField]
        private RuntimeType runtimeType = RuntimeType.Editor;

        private void Start()
        {
            StartCoroutine(Bind());
        }

        private IEnumerator Bind()
        {
            // Delay by one frame to wait for event registration.
            yield return null;

            var spaceCoordinateManager = CoordinateManager.Instance;
            var pose = new Pose(transform.position, transform.rotation);
            var gPosition = this.geodeticPosition.ToGeodeticPosition();
            var rotation = Quaternion.AngleAxis(northHeading, Vector3.up);
            var gPose = new GeodeticPose(gPosition, rotation);

            var binding = new WorldBinding(pose, gPose);
            spaceCoordinateManager.BindCoordinates(binding);
        }

        private bool IsBindingValid()
        {
#if UNITY_EDITOR
            switch (runtimeType)
            {
                case RuntimeType.Editor:
                case RuntimeType.EditorAndPlayer:
                    return true;
                default:
                    return false;
            }
#else
            switch (runtimeType)
            {
                case RuntimeType.Player:
                case RuntimeType.EditorAndPlayer:
                    return true;
                default:
                    return false;
            }
#endif
        }
    }
}