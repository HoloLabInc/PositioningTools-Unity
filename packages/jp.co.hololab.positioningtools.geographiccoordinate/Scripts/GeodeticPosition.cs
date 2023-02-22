using System;

namespace HoloLab.PositioningTools.GeographicCoordinate
{
    public readonly struct GeodeticPosition : IEquatable<GeodeticPosition>
    {
        public GeodeticPosition(double latitude, double longitude, double ellipsoidalHeight)
        {
            Latitude = latitude;
            Longitude = longitude;
            EllipsoidalHeight = ellipsoidalHeight;
        }

        public double Latitude { get; }
        public double Longitude { get; }
        public double EllipsoidalHeight { get; }

        public override string ToString()
        {
            return $"GeodeticPosition ({Latitude:F1} {Longitude:F1} {EllipsoidalHeight:F1})";
        }

        public bool Equals(GeodeticPosition other)
        {
            return Latitude.Equals(other.Latitude) && Longitude.Equals(other.Longitude) && EllipsoidalHeight.Equals(other.EllipsoidalHeight);
        }

        public override bool Equals(object obj)
        {
            return obj is GeodeticPosition other && Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = Latitude.GetHashCode();
                hashCode = (hashCode * 397) ^ Longitude.GetHashCode();
                hashCode = (hashCode * 397) ^ EllipsoidalHeight.GetHashCode();
                return hashCode;
            }
        }

        public static bool operator ==(GeodeticPosition lhs, GeodeticPosition rhs) => lhs.Equals(rhs);

        public static bool operator !=(GeodeticPosition lhs, GeodeticPosition rhs) => !(lhs == rhs);
    }
}