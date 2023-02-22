using UnityEngine;

namespace HoloLab.PositioningTools.GeographicCoordinate
{
    public static class EnuPositionExtension
    {
        public static Vector3 ToUnityVector(this EnuPosition enuPosition)
        {
            return new Vector3((float)enuPosition.East, (float)enuPosition.Up, (float)enuPosition.North);
        }
    }
}