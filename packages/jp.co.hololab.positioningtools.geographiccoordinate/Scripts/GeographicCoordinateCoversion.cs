using System;

namespace HoloLab.PositioningTools.GeographicCoordinate
{
    public static class GeographicCoordinateConversion
    {
        // ref: https://www.enri.go.jp/~fks442/K_MUSEN/1st/1st060428rev2.pdf
        private const double A = 6378137;
        private const double FInv = 298.257223563;
        private const double F = 1 / FInv;

        private const double B = A * (1 - F);

        /// <summary>
        /// A^2
        /// </summary>
        private const double A2 = A * A;

        /// <summary>
        /// B^2
        /// </summary>
        private const double B2 = B * B;

        /// <summary>
        /// e^2
        /// </summary>
        private const double E2 = F * (2 - F);

        /// <summary>
        /// e´^2
        /// </summary>
        private const double EPrime2 = (A * A - B * B) / (B * B);

        private const double Pi = 3.1415926535898;

        private const double Deg2Rad = Pi / 180;
        private const double Rad2Deg = 180 / Pi;

        #region Geodetic -> ECEF

        /// <summary>
        /// Convert geodetic position to ECEF position
        /// </summary>
        /// <param name="latitude"></param>
        /// <param name="longitude"></param>
        /// <param name="ellipsoidalHeight"></param>
        /// <returns></returns>
        public static EcefPosition GeodeticToEcef(double latitude, double longitude, double ellipsoidalHeight)
        {
            var phi = latitude * Deg2Rad;
            var lambda = longitude * Deg2Rad;
            var n = CalcN(phi);

            var cosPhi = Math.Cos(phi);
            var x = (n + ellipsoidalHeight) * cosPhi * Math.Cos(lambda);
            var y = (n + ellipsoidalHeight) * cosPhi * Math.Sin(lambda);
            var z = (n * Math.Pow(1 - F, 2) + ellipsoidalHeight) * Math.Sin(phi);

            return new EcefPosition(x, y, z);
        }

        /// <summary>
        /// Convert geodetic position to ECEF position
        /// </summary>
        /// <param name="geodeticPosition"></param>
        /// <returns></returns>
        public static EcefPosition GeodeticToEcef(GeodeticPosition geodeticPosition)
        {
            return GeodeticToEcef(geodeticPosition.Latitude, geodeticPosition.Longitude, geodeticPosition.EllipsoidalHeight);
        }

        #endregion

        #region ECEF -> Geodetic

        /// <summary>
        /// Convert ECEF position to geodetic position
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        /// <returns></returns>
        public static GeodeticPosition EcefToGeodetic(double x, double y, double z)
        {
            var p = Math.Sqrt(x * x + y * y);
            var theta = Math.Atan2(z * A, p * B);

            var sinTheta = Math.Sin(theta);
            var cosTheta = Math.Cos(theta);
            var phi = Math.Atan2(z + EPrime2 * B * Math.Pow(sinTheta, 3), p - E2 * A * Math.Pow(cosTheta, 3));
            var lambda = Math.Atan2(y, x);
            var ellipsoidalHeight = (p / Math.Cos(phi)) - CalcN(phi);

            var latitude = phi * Rad2Deg;
            var longitude = lambda * Rad2Deg;
            return new GeodeticPosition(latitude, longitude, ellipsoidalHeight);
        }

        /// <summary>
        /// Convert ECEF position to geodetic position
        /// </summary>
        /// <param name="ecefPosition"></param>
        /// <returns></returns>
        public static GeodeticPosition EcefToGeodetic(EcefPosition ecefPosition)
        {
            return EcefToGeodetic(ecefPosition.X, ecefPosition.Y, ecefPosition.Z);
        }

        #endregion

        #region Geodetic -> ENU

        /// <summary>
        /// Convert geodetic position to ENU position
        /// </summary>
        /// <param name="latitude"></param>
        /// <param name="longitude"></param>
        /// <param name="ellipsoidalHeight"></param>
        /// <param name="originLatitude"></param>
        /// <param name="originLongitude"></param>
        /// <param name="originEllipsoidalHeight"></param>
        /// <returns></returns>
        public static EnuPosition GeodeticToEnu(double latitude, double longitude, double ellipsoidalHeight, double originLatitude, double originLongitude, double originEllipsoidalHeight)
        {
            var ecefPosition = GeodeticToEcef(latitude, longitude, ellipsoidalHeight);

            return EcefToEnu(ecefPosition.X, ecefPosition.Y, ecefPosition.Z, originLatitude, originLongitude, originEllipsoidalHeight);
        }

        /// <summary>
        /// Convert geodetic position to ENU position
        /// </summary>
        /// <param name="geodeticPosition"></param>
        /// <param name="originPosition"></param>
        /// <returns></returns>
        public static EnuPosition GeodeticToEnu(GeodeticPosition geodeticPosition, GeodeticPosition originPosition)
        {
            return GeodeticToEnu(geodeticPosition.Latitude, geodeticPosition.Longitude, geodeticPosition.EllipsoidalHeight,
                originPosition.Latitude, originPosition.Longitude, originPosition.EllipsoidalHeight);
        }

        #endregion

        #region ENU -> Geodetic

        /// <summary>
        /// Convert ENU position to geodetic position
        /// </summary>
        /// <param name="east"></param>
        /// <param name="north"></param>
        /// <param name="up"></param>
        /// <param name="originLatitude"></param>
        /// <param name="originLongitude"></param>
        /// <param name="originEllipsoidalHeight"></param>
        /// <returns></returns>
        public static GeodeticPosition EnuToGeodetic(double east, double north, double up, double originLatitude, double originLongitude, double originEllipsoidalHeight)
        {
            var ecefPosition = EnuToEcef(east, north, up, originLatitude, originLongitude, originEllipsoidalHeight);
            return EcefToGeodetic(ecefPosition);
        }

        /// <summary>
        /// Convert ENU position to geodetic position
        /// </summary>
        /// <param name="enuPosition"></param>
        /// <param name="originPosition"></param>
        /// <returns></returns>
        public static GeodeticPosition EnuToGeodetic(EnuPosition enuPosition, GeodeticPosition originPosition)
        {
            return EnuToGeodetic(enuPosition.East, enuPosition.North, enuPosition.Up,
                originPosition.Latitude, originPosition.Longitude, originPosition.EllipsoidalHeight);
        }

        #endregion

        #region ECEF -> ENU

        /// <summary>
        /// Convert ECEF position to ENU position
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        /// <param name="originLatitude"></param>
        /// <param name="originLongitude"></param>
        /// <param name="originEllipsoidalHeight"></param>
        /// <returns></returns>
        public static EnuPosition EcefToEnu(double x, double y, double z, double originLatitude, double originLongitude, double originEllipsoidalHeight)
        {
            var originEcefPosition = GeodeticToEcef(originLatitude, originLongitude, originEllipsoidalHeight);

            var deltaX = x - originEcefPosition.X;
            var deltaY = y - originEcefPosition.Y;
            var deltaZ = z - originEcefPosition.Z;

            return EcefDeltaToEnu(deltaX, deltaY, deltaZ, originLatitude, originLongitude);
        }

        /// <summary>
        /// Convert ECEF position to ENU position
        /// </summary>
        /// <param name="ecefPosition"></param>
        /// <param name="originPosition"></param>
        /// <returns></returns>
        public static EnuPosition EcefToEnu(EcefPosition ecefPosition, GeodeticPosition originPosition)
        {
            return EcefToEnu(ecefPosition.X, ecefPosition.Y, ecefPosition.Z,
                originPosition.Latitude, originPosition.Longitude, originPosition.EllipsoidalHeight);
        }

        #endregion

        #region ENU -> ECEF

        /// <summary>
        /// Convert ENU position to ECEF position
        /// </summary>
        /// <param name="east"></param>
        /// <param name="north"></param>
        /// <param name="up"></param>
        /// <param name="originLatitude"></param>
        /// <param name="originLongitude"></param>
        /// <param name="originEllipsoidalHeight"></param>
        /// <returns></returns>
        public static EcefPosition EnuToEcef(double east, double north, double up, double originLatitude, double originLongitude, double originEllipsoidalHeight)
        {
            var (deltaX, deltaY, deltaZ) = EnuToEcefDelta(east, north, up, originLatitude, originLongitude);
            var originEcefPosition = GeodeticToEcef(originLatitude, originLongitude, originEllipsoidalHeight);
            return new EcefPosition(originEcefPosition.X + deltaX, originEcefPosition.Y + deltaY, originEcefPosition.Z + deltaZ);
        }

        /// <summary>
        /// Convert ENU position to ECEF position
        /// </summary>
        /// <param name="enuPosition"></param>
        /// <param name="originPosition"></param>
        /// <returns></returns>
        public static EcefPosition EnuToEcef(EnuPosition enuPosition, GeodeticPosition originPosition)
        {
            return EnuToEcef(enuPosition.East, enuPosition.North, enuPosition.Up,
                originPosition.Latitude, originPosition.Longitude, originPosition.EllipsoidalHeight);
        }

        #endregion

        private static EnuPosition EcefDeltaToEnu(double deltaX, double deltaY, double deltaZ, double originLatitude, double originLongitude)
        {
            // |  0 1 0 | | sin(phi) 0 -cos(phi) | |  cos(lambda) sin(lambda) 0 | | x |
            // | -1 0 0 | |        0 1         0 | | -sin(lambda) cos(lambda) 0 | | y |
            // |  0 0 1 | | cos(phi) 0  sin(phi) | |            0           0 1 | | z |
            //
            //   |         0 1        0 | |  x cos(lambda) + y sin(lambda) |
            // = | -sin(phi) 0 cos(phi) | | -x sin(lambda) + y cos(lambda) |
            //   |  cos(phi) 0 sin(phi) | |                              z |


            var phi = originLatitude * Deg2Rad;
            var cosPhi = Math.Cos(phi);
            var sinPhi = Math.Sin(phi);

            var lambda = originLongitude * Deg2Rad;
            var cosLambda = Math.Cos(lambda);
            var sinLambda = Math.Sin(lambda);

            var t = deltaX * cosLambda + deltaY * sinLambda;
            var u = -deltaX * sinLambda + deltaY * cosLambda;

            var east = u;
            var north = -sinPhi * t + cosPhi * deltaZ;
            var up = cosPhi * t + sinPhi * deltaZ;

            return new EnuPosition(east, north, up);
        }

        private static (double deltaX, double deltaY, double deltaZ) EnuToEcefDelta(double east, double north, double up, double originLatitude, double originLongitude)
        {
            // | cos(lambda) -sin(lambda) 0 | |  sin(phi) 0 cos(phi) | | 0 -1 0 | | east  |
            // | sin(lambda)  cos(lambda) 0 | |         0 1        0 | | 1  0 0 | | north |
            // |           0            0 1 | | -cos(phi) 0 sin(phi) | | 0  0 1 | | up    |
            //
            //   | cos(lambda) -sin(lambda) 0 | | -north sin(phi) + up cos(phi) (= t) |
            // = | sin(lambda)  cos(lambda) 0 | |                          east       |
            //   |           0            0 1 | |  north cos(phi) + up sin(phi) (= u) |


            var phi = originLatitude * Deg2Rad;
            var cosPhi = Math.Cos(phi);
            var sinPhi = Math.Sin(phi);

            var lambda = originLongitude * Deg2Rad;
            var cosLambda = Math.Cos(lambda);
            var sinLambda = Math.Sin(lambda);

            var t = -north * sinPhi + up * cosPhi;
            var u = north * cosPhi + up * sinPhi;

            var x = cosLambda * t - sinLambda * east;
            var y = sinLambda * t + cosLambda * east;
            var z = u;

            return (x, y, z);
        }

        private static double CalcN(double phi)
        {
            var sinPhi = Math.Sin(phi);
            var cosPhi = Math.Cos(phi);
            return A2 / Math.Sqrt(A2 * cosPhi * cosPhi + B2 * sinPhi * sinPhi);
        }
    }
}