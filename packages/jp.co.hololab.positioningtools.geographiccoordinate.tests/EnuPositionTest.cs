using HoloLab.PositioningTools.GeographicCoordinate;
using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace Tests
{
    public class EnuPositionTest
    {
        [Test]
        public void EqualsTest()
        {
            var position = new EnuPosition(1, 2, 3);
            var samePosition = new EnuPosition(1, 2, 3);

            var position2 = new EnuPosition(4, 4, 4);

            Assert.IsTrue(position.Equals(position));
            Assert.IsTrue(position == samePosition);
            Assert.IsFalse(position != samePosition);

            Assert.IsFalse(position.Equals(position2));
            Assert.IsFalse(position == position2);
            Assert.IsTrue(position != position2);
        }
    }
}