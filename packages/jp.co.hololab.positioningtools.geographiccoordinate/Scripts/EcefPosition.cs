using System;

namespace HoloLab.PositioningTools.GeographicCoordinate
{
    public readonly struct EcefPosition : IEquatable<EcefPosition>
    {
        public EcefPosition(double x, double y, double z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public double X { get; }
        public double Y { get; }
        public double Z { get; }

        public override string ToString()
        {
            return $"EcefPosition ({X:F1} {Y:F1} {Z:F1})";
        }

        public bool Equals(EcefPosition other)
        {
            return X.Equals(other.X) && Y.Equals(other.Y) && Z.Equals(other.Z);
        }

        public override bool Equals(object obj)
        {
            return obj is EcefPosition other && Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = X.GetHashCode();
                hashCode = (hashCode * 397) ^ Y.GetHashCode();
                hashCode = (hashCode * 397) ^ Z.GetHashCode();
                return hashCode;
            }
        }

        public static bool operator ==(EcefPosition lhs, EcefPosition rhs) => lhs.Equals(rhs);

        public static bool operator !=(EcefPosition lhs, EcefPosition rhs) => !(lhs == rhs);
    }
}