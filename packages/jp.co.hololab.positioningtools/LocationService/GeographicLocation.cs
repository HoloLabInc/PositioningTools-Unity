using System;

namespace HoloLab.PositioningTools
{
    public readonly struct GeographicLocation
    {
        public GeographicLocation(double latitude, double longitude, double height, DateTimeOffset dateTimeOffset)
        {
            Latitude = latitude;
            Longitude = longitude;
            Height = height;
            DateTimeOffset = dateTimeOffset;
        }

        public double Latitude { get; }

        public double Longitude { get; }

        public double Height { get; }

        public DateTimeOffset DateTimeOffset { get; }

        public override string ToString()
        {
            return $"LocationData ({Latitude:F4} {Longitude:F4} {Height:F1})";
        }
    }
}