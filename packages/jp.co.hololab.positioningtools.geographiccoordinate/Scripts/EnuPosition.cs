using System;

namespace HoloLab.PositioningTools.GeographicCoordinate
{
    public readonly struct EnuPosition : IEquatable<EnuPosition>
    {
        public EnuPosition(double east, double north, double up)
        {
            East = east;
            North = north;
            Up = up;
        }

        public double East { get; }
        public double North { get; }
        public double Up { get; }

        public override string ToString()
        {
            return $"EnuPosition ({East:F1} {North:F1} {Up:F1})";
        }

        public bool Equals(EnuPosition other)
        {
            return East.Equals(other.East) && North.Equals(other.North) && Up.Equals(other.Up);
        }

        public override bool Equals(object obj)
        {
            return obj is EnuPosition other && Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = East.GetHashCode();
                hashCode = (hashCode * 397) ^ North.GetHashCode();
                hashCode = (hashCode * 397) ^ Up.GetHashCode();
                return hashCode;
            }
        }

        public static bool operator ==(EnuPosition lhs, EnuPosition rhs) => lhs.Equals(rhs);

        public static bool operator !=(EnuPosition lhs, EnuPosition rhs) => !(lhs == rhs);
    }
}