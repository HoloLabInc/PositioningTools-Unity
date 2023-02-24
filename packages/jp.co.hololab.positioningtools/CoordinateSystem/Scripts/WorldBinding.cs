using UnityEngine;

namespace HoloLab.PositioningTools.CoordinateSystem
{
    /// <summary>
    /// Binding between Unity coordinates and geographic coordinates
    /// </summary>
    public class WorldBinding
    {
        public Pose ApplicationPose { get; }
        public GeodeticPose GeodeticPose { get; }

        public WorldBinding(Pose applicationPose, GeodeticPose geodeticPose)
        {
            ApplicationPose = applicationPose;
            GeodeticPose = geodeticPose;
        }
    }
}