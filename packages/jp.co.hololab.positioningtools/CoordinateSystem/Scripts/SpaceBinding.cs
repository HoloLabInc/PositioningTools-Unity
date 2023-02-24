using UnityEngine;

namespace HoloLab.PositioningTools.CoordinateSystem
{
    public class SpaceBinding
    {
        /// <summary>
        /// Space origin in Unity coordinate
        /// 
        /// If Pose is null, please use Transform value.
        /// </summary>
        public Pose? Pose { get; }

        /// <summary>
        /// Space origin in Unity coordinate
        /// 
        /// If Transform is null, please use Pose value.
        /// </summary>
        public Transform Transform { get; }

        /// <summary>
        /// Space ID
        /// </summary>
        public string SpaceId { get; }

        /// <summary>
        /// Space Type (e.g. Marker, Immersal, VAT)
        /// </summary>
        public string SpaceType { get; }

        public SpaceBinding(Pose pose, string spaceType, string spaceId)
        {
            Pose = pose;
            SpaceType = spaceType;
            SpaceId = spaceId;
        }

        public SpaceBinding(Transform transform, string spaceType, string spaceId)
        {
            Transform = transform;
            SpaceType = spaceType;
            SpaceId = spaceId;
        }
    }
}