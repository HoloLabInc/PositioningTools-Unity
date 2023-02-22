using System;

namespace HoloLab.PositioningTools.LocationServiceImplementation
{
    internal static class TimeStampConverter
    {
        public static DateTimeOffset TimeStampToDateTimeOffset(double timestamp)
        {
            return DateTimeOffset.FromUnixTimeSeconds((long)timestamp);
        }
    }
}