using HoloLab.PositioningTools.CoordinateSystem;
using HoloLab.PositioningTools.GeographicCoordinate;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoordinateSerializer
{
    private readonly List<ICoordinateSerializer> serializers = new List<ICoordinateSerializer>()
    {
        new GeodeticCoordinateJsonSerializer(),
    };

    public bool TryDeserialize(string text, out ICoordinateInfo coordinate)
    {
        foreach (var serializer in serializers)
        {
            if (serializer.TryDeserialize(text, out coordinate))
            {
                return true;
            }
        }

        coordinate = null;
        return false;
    }

    public bool TrySerialize(ICoordinateInfo coordinate, out string text)
    {
        foreach (var serializer in serializers)
        {
            if (serializer.TrySerialize(coordinate, out text))
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
    bool TryDeserialize(string text, out ICoordinateInfo coordinate);

    bool TrySerialize(ICoordinateInfo coordinate, out string text);
}

public class GeodeticCoordinateJsonSerializer : ICoordinateSerializer
{
    public bool TryDeserialize(string text, out ICoordinateInfo coordinate)
    {
        try
        {
            var geodeticPositionObject = JsonConvert.DeserializeObject<GeodeticPositionJsonNetObject>(text);
            coordinate = geodeticPositionObject.ToCoordinateInfo();
            return true;
        }
        catch (Exception)
        {
            // Do nothing
        }
        coordinate = null;
        return false;
    }

    public bool TrySerialize(ICoordinateInfo coordinate, out string text)
    {
        switch (coordinate)
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
    public GeodeticPosition GeodeticPosition;
    public float Heading;
}

internal class GeodeticPositionJsonNetObject
{
    public double Latitude { set; get; }

    private double Lat { set { Latitude = value; } }

    public double Longitude { set; get; }
    public double EllipsoidalHeight { set; get; }

    public float Heading { set; get; }

    public ICoordinateInfo ToCoordinateInfo()
    {
        var geodeticPosition = new GeodeticPosition(Latitude, Longitude, EllipsoidalHeight);

        return new GeodeticPositionWithHeading()
        {
            GeodeticPosition = geodeticPosition,
            Heading = Heading
        };
    }
}