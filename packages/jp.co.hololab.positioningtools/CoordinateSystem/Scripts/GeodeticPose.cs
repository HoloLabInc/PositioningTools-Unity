using HoloLab.PositioningTools.GeographicCoordinate;
using UnityEngine;

namespace HoloLab.PositioningTools.CoordinateSystem
{
    public class GeodeticPose
    {
        public GeodeticPosition GeodeticPosition { get; }
        public Quaternion EnuRotation { get; }

        public GeodeticPose(GeodeticPosition geodeticPosition, Quaternion enuRotation)
        {
            GeodeticPosition = geodeticPosition;
            EnuRotation = enuRotation;
        }
    }
}