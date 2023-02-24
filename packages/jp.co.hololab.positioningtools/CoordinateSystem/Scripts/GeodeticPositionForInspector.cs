using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HoloLab.PositioningTools.GeographicCoordinate;

namespace HoloLab.PositioningTools.CoordinateSystem
{
    [Serializable]
    public struct GeodeticPositionForInspector
    {
        [SerializeField]
        private double latitude;

        public double Latitude => latitude;

        [SerializeField]
        private double longitude;

        public double Longitude => longitude;

        [SerializeField]
        private double ellipsoidalHeight;

        public double EllipsoidalHeight => ellipsoidalHeight;

        public GeodeticPositionForInspector(GeodeticPosition geodeticPosition)
        {
            latitude = geodeticPosition.Latitude;
            longitude = geodeticPosition.Longitude;
            ellipsoidalHeight = geodeticPosition.EllipsoidalHeight;
        }

        public GeodeticPosition ToGeodeticPosition()
        {
            return new GeodeticPosition(latitude, longitude, ellipsoidalHeight);
        }
    }
}