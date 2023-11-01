using HoloLab.PositioningTools.CoordinateSystem;
using HoloLab.PositioningTools.GeographicCoordinate;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HoloLab.PositioningTools.CoordinateSerialization
{

    public class CoordinateSerializer
    {
        private readonly List<ICoordinateSerializer> serializers = new List<ICoordinateSerializer>()
    {
        new GeodeticCoordinateJsonSerializer(),
    };

        public bool TryDeserialize(string text, out ICoordinateInfo coordinateInfo)
        {
            foreach (var serializer in serializers)
            {
                if (serializer.TryDeserialize(text, out coordinateInfo))
                {
                    return true;
                }
            }

            coordinateInfo = null;
            return false;
        }

        public bool TrySerialize(ICoordinateInfo coordinateInfo, out string text)
        {
            foreach (var serializer in serializers)
            {
                if (serializer.TrySerialize(coordinateInfo, out text))
                {
                    return true;
                }
            }

            text = null;
            return false;
        }
    }

    interface ICoordinateSerializer
    {
        bool TryDeserialize(string text, out ICoordinateInfo coordinateInfo);

        bool TrySerialize(ICoordinateInfo coordinateInfo, out string text);
    }

    public class GeodeticCoordinateJsonSerializer : ICoordinateSerializer
    {
        public bool TryDeserialize(string text, out ICoordinateInfo coordinateInfo)
        {
            try
            {
                var geodeticPositionObject = JsonConvert.DeserializeObject<GeodeticPositionJsonNetObject>(text);
                coordinateInfo = geodeticPositionObject.ToCoordinateInfo();
                return true;
            }
            catch (Exception e)
            {
                Debug.Log(e);
                // Do nothing
            }
            coordinateInfo = null;
            return false;
        }

        public bool TrySerialize(ICoordinateInfo coordinateInfo, out string text)
        {
            switch (coordinateInfo)
            {
                case GeodeticPositionWithHeading geodeticPositionWithHeading:
                    return TrySerialize(geodeticPositionWithHeading, out text);
                default:
                    text = null;
                    return false;
            }
        }

        public bool TrySerialize(GeodeticPositionWithHeading coordinate, out string text)
        {
            text = null;
            return false;
        }
    }

    public interface ICoordinateInfo { }

    public class GeodeticPositionWithHeading : ICoordinateInfo
    {
        public GeodeticPosition GeodeticPosition { get; }
        public float Heading { get; }

        public GeodeticPositionWithHeading(GeodeticPosition geodeticPosition, float heading)
        {
            GeodeticPosition = geodeticPosition;
            Heading = heading;
        }
    }

    internal class GeodeticPositionJsonNetObject
    {
        public double Latitude { set; get; }
        public double Lat { set { Latitude = value; } }

        public double Longitude { set; get; }
        public double Long { set { Longitude = value; } }
        public double Lon { set { Longitude = value; } }
        public double Lng { set { Longitude = value; } }

        public double EllipsoidalHeight { set; get; }
        public double Height { set { EllipsoidalHeight = value; } }

        public float Heading { set; get; }

        public ICoordinateInfo ToCoordinateInfo()
        {
            var geodeticPosition = new GeodeticPosition(Latitude, Longitude, EllipsoidalHeight);
            return new GeodeticPositionWithHeading(geodeticPosition, Heading);
        }
    }
}