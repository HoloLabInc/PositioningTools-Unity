using UnityEngine;

namespace HoloLab.PositioningTools.CoordinateSystem
{
    /// <summary>
    /// Binding between Unity coordinates and geographic coordinates
    /// </summary>
    public class WorldBinding
    {
        /// <summary>
        /// World binding pose in Unity coordinate
        ///
        /// If Pose is null, please use Transform value.
        /// </summary>
        public Pose? ApplicationPose { get; }

        /// <summary>
        /// World binding transform in Unity coordinate
        ///
        /// If Transform is null, please use Pose value.
        /// </summary>
        public Transform Transform { get; }

        public GeodeticPose GeodeticPose { get; }

        public WorldBinding(Pose applicationPose, GeodeticPose geodeticPose)
        {
            ApplicationPose = applicationPose;
            GeodeticPose = geodeticPose;
        }

        public WorldBinding(Transform transform, GeodeticPose geodeticPose)
        {
            Transform = transform;
            GeodeticPose = geodeticPose;
        }
    }
}