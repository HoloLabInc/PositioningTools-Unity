using HoloLab.PositioningTools.GeographicCoordinate;
using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace Tests
{
    public class GeographicCoordinateConversionTest
    {
        private (double latitude, double longitude, double ellipsoidalHeight,
            double originLatitude, double originLongitude, double originEllipsoidalHeight,
            double east, double north, double up)[]
            geodeticAndEnuTestCases =
            {
                (
                    0.0, 0.0, 0.0,
                    0.0, 0.0, 0.0,
                    0.0, 0.0, 0.0
                ),
                (
                    0.0, 0.0, 1.0,
                    0.0, 0.0, 0.0,
                    0.0, 0.0, 1.0
                ),
                (
                    0.0, 0.0, 0.0,
                    0.0, 0.0, 1.0,
                    0.0, 0.0, -1.0
                ),
                (
                    1.0, 0.0, 0.0,
                    0.0, 0.0, 0.0,
                    0.0, 110568.77482456666, -964.9195715896785
                ),
                (
                    0.0, 1.0, 0.0,
                    0.0, 0.0, 0.0,
                    111313.83923667614, 0.0, -971.4211582997814
                ),
                (
                    35.68944, 139.69167, 0,
                    35.68944, 139.69167, 0,
                    0.0, 0.0, 0.0
                ),
                (
                    35.68944, 139.69167, 1,
                    35.68944, 139.69167, 0,
                    0.0, 0.0, 1.0
                ),
                (
                    36.68944, 139.69167, 0,
                    35.68944, 139.69167, 0,
                    -5.093170329928398e-11, 110956.89140579224, -968.3331423887212
                ),
                (
                    36.68944, 140.69167, 0,
                    35.68944, 139.69167, 0,
                    89367.83192407408, 111411.87919076698, -1601.7619714090688
                ),
            };

        [Test]
        public void GeodeticToEcefTest()
        {
            var testCases = new (double latitude, double longitude, double ellipsoidalHeight, double x, double y, double z)[]
            {
                (
                    0.0, 0.0, 0.0,
                    6378137.0, 0.0, 0.0
                ),
                (
                    0.0, 0.0, 1.0,
                    6378138.0, 0.0, 0.0
                ),
                (
                    1.0, 0.0, 0.0,
                    6377172.08042841, 0.0, 110568.77482456666
                ),
                (
                    0.0, 1.0, 0.0,
                    6377165.5788417, 111313.83923667614, 0.0
                ),
                (
                    35.68944, 139.69167, 0,
                    -3954845.4939313377, 3354941.5159737933, 3700259.381112877
                ),
                (
                    35.68944, 139.69167, 1,
                    -3954846.11328736, 3354942.0413807244, 3700259.964504406
                )
            };
            foreach (var (latitude, longitude, ellipsoidalHeight, x, y, z) in testCases)
            {
                var ecefPosition = GeographicCoordinateConversion.GeodeticToEcef(latitude, longitude, ellipsoidalHeight);

                var expectedPosition = new EcefPosition(x, y, z);
                AssertAreApproximatelyEqual(expectedPosition, ecefPosition);
            }
        }

        [Test]
        public void EcefToGeodeticTest()
        {
            var testCases = new (double latitude, double longitude, double ellipsoidalHeight)[]
            {
                (
                    0.0, 0.0, 0.0
                ),
                (
                    0.0, 0.0, 1.0
                ),
                (
                    1.0, 0.0, 0.0
                ),
                (
                    0.0, 1.0, 0.0
                ),
                (
                    35.68944, 139.69167, 0
                ),
                (
                    35.68944, 139.69167, 1
                )
            };
            foreach (var (latitude, longitude, ellipsoidalHeight) in testCases)
            {
                var ecefPosition = GeographicCoordinateConversion.GeodeticToEcef(latitude, longitude, ellipsoidalHeight);
                var geodeticPosition = GeographicCoordinateConversion.EcefToGeodetic(ecefPosition.X, ecefPosition.Y, ecefPosition.Z);

                var expectedPosition = new GeodeticPosition(latitude, longitude, ellipsoidalHeight);
                AssertAreApproximatelyEqual(expectedPosition, geodeticPosition);
            }
        }

        [Test]
        public void GeodeticToEnuTest()
        {
            foreach (var (latitude, longitude, ellipsoidalHeight, originLatitude, originLongitude, originEllipsoidalHeight, east, north, up) in geodeticAndEnuTestCases)
            {
                var enuPosition = GeographicCoordinateConversion.GeodeticToEnu(latitude, longitude, ellipsoidalHeight, originLatitude, originLongitude, originEllipsoidalHeight);

                var expectedPosition = new EnuPosition(east, north, up);
                AssertAreApproximatelyEqual(expectedPosition, enuPosition);
            }
        }

        [Test]
        public void EnuToGeodeticTest()
        {
            foreach (var (latitude, longitude, ellipsoidalHeight, originLatitude, originLongitude, originEllipsoidalHeight, east, north, up) in geodeticAndEnuTestCases)
            {
                var geodeticPosition = GeographicCoordinateConversion.EnuToGeodetic(east, north, up, originLatitude, originLongitude, originEllipsoidalHeight);

                var expectedPosition = new GeodeticPosition(latitude, longitude, ellipsoidalHeight);
                AssertAreApproximatelyEqual(expectedPosition, geodeticPosition);
            }
        }

        [Test]
        public void EcefToEnuTest()
        {
            foreach (var (latitude, longitude, ellipsoidalHeight, originLatitude, originLongitude, originEllipsoidalHeight, east, north, up) in geodeticAndEnuTestCases)
            {
                var ecefPosition = GeographicCoordinateConversion.GeodeticToEcef(latitude, longitude, ellipsoidalHeight);
                var enuPosition = GeographicCoordinateConversion.EcefToEnu(ecefPosition.X, ecefPosition.Y, ecefPosition.Z, originLatitude, originLongitude, originEllipsoidalHeight);

                var expectedPosition = new EnuPosition(east, north, up);
                AssertAreApproximatelyEqual(expectedPosition, enuPosition);
            }
        }

        [Test]
        public void EnuToEcefTest()
        {
            foreach (var (latitude, longitude, ellipsoidalHeight, originLatitude, originLongitude, originEllipsoidalHeight, east, north, up) in geodeticAndEnuTestCases)
            {
                var ecefPosition = GeographicCoordinateConversion.EnuToEcef(east, north, up, originLatitude, originLongitude, originEllipsoidalHeight);
                var geodeticPosition = GeographicCoordinateConversion.EcefToGeodetic(ecefPosition);

                var expectedPosition = new GeodeticPosition(latitude, longitude, ellipsoidalHeight);
                AssertAreApproximatelyEqual(expectedPosition, geodeticPosition);
            }
        }

        private void AssertAreApproximatelyEqual(EcefPosition expected, EcefPosition actual, double delta = 1e-6)
        {
            Assert.AreEqual(expected.X, actual.X, delta);
            Assert.AreEqual(expected.Y, actual.Y, delta);
            Assert.AreEqual(expected.Z, actual.Z, delta);
        }

        private void AssertAreApproximatelyEqual(EnuPosition expected, EnuPosition actual, double delta = 1e-6)
        {
            Assert.AreEqual(expected.East, actual.East, delta);
            Assert.AreEqual(expected.North, actual.North, delta);
            Assert.AreEqual(expected.Up, actual.Up, delta);
        }

        private void AssertAreApproximatelyEqual(GeodeticPosition expected, GeodeticPosition actual, double delta = 1e-6)
        {
            Assert.AreEqual(expected.Latitude, actual.Latitude, delta);
            Assert.AreEqual(expected.Longitude, actual.Longitude, delta);
            Assert.AreEqual(expected.EllipsoidalHeight, actual.EllipsoidalHeight, delta);
        }
    }
}