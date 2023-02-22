using System;

namespace HoloLab.PositioningTools
{
    public readonly struct CardinalDirection
    {
        public CardinalDirection(float headingDegrees, DateTimeOffset dateTimeOffset)
        {
            HeadingDegrees = headingDegrees;
            DateTimeOffset = dateTimeOffset;
        }

        public float HeadingDegrees { get; }

        public DateTimeOffset DateTimeOffset { get; }

        public override string ToString()
        {
            return $"CompassData ({HeadingDegrees:F4})";
        }
    }
}