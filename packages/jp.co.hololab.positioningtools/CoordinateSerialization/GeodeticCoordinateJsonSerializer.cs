using HoloLab.PositioningTools.GeographicCoordinate;
using System;
using System.Collections;
using System.Collections.Generic;
#if JSONNET_PRESENT
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
#else
using UnityEngine;
#endif

namespace HoloLab.PositioningTools.CoordinateSerialization
{
    public class GeodeticCoordinateJsonSerializer : ICoordinateSerializer
    {
        public GeodeticCoordinateJsonSerializer()
        {
#if !JSONNET_PRESENT
            Debug.LogWarning($"Please import \"com.unity.nuget.newtonsoft-json\" package if you want to use ${nameof(GeodeticCoordinateJsonSerializer)}.");
#endif
        }
        public bool TryDeserialize(string text, out ICoordinateInfo coordinateInfo)
        {
#if JSONNET_PRESENT
            try
            {
                var geodeticPositionObject = JsonConvert.DeserializeObject<GeodeticCoordinateJsonNetObject>(text);
                coordinateInfo = geodeticPositionObject.ToCoordinateInfo();
                return true;
            }
            catch (Exception)
            {
                // Do nothing
            }
#endif
            coordinateInfo = null;
            return false;
        }

        public bool TrySerialize(ICoordinateInfo coordinateInfo, out string text)
        {
#if JSONNET_PRESENT
            switch (coordinateInfo)
            {
                case GeodeticPositionWithHeading geodeticPositionWithHeading:
                    return TrySerialize(geodeticPositionWithHeading, out text);
            }
#endif
            text = null;
            return false;
        }

        public bool TrySerialize(GeodeticPositionWithHeading coordinate, out string text)
        {
#if JSONNET_PRESENT
            var contractResolver = new DefaultContractResolver
            {
                NamingStrategy = new CamelCaseNamingStrategy()
            };
            var jsonSerializerSettings = new JsonSerializerSettings
            {
                ContractResolver = contractResolver,
                NullValueHandling = NullValueHandling.Ignore
            };
            var jsonObject = GeodeticCoordinateJsonNetObject.Create(coordinate);
            try
            {
                text = JsonConvert.SerializeObject(jsonObject, jsonSerializerSettings);
                return true;
            }
            catch (Exception)
            {
                // Do nothing
            }
#endif
            text = null;
            return false;
        }
    }

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

    internal class GeodeticCoordinateJsonNetObject
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

        public static GeodeticCoordinateJsonNetObject Create(GeodeticPositionWithHeading geodeticPositionWithHeadings)
        {
            var geodeticPosition = geodeticPositionWithHeadings.GeodeticPosition;
            return new GeodeticCoordinateJsonNetObject()
            {
                Latitude = geodeticPosition.Latitude,
                Longitude = geodeticPosition.Longitude,
                EllipsoidalHeight = geodeticPosition.EllipsoidalHeight,
                Heading = geodeticPositionWithHeadings.Heading
            };
        }

        public ICoordinateInfo ToCoordinateInfo()
        {
            var geodeticPosition = new GeodeticPosition(Latitude, Longitude, EllipsoidalHeight);
            return new GeodeticPositionWithHeading(geodeticPosition, Heading);
        }
    }
}