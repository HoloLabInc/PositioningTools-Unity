using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using HoloLab.PositioningTools.CoordinateSerialization;
using HoloLab.PositioningTools.GeographicCoordinate;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class GeodeticCoordinateJsonSerializerTests
{
    // [Test]
    public void TrySerialize()
    {
        var geodeticPosition = new GeodeticPosition(1.0, 2.0, 3.0);
        var heading = 10.0f;
        var coordinate = new GeodeticPositionWithHeading(geodeticPosition, heading);

        var serializer = new GeodeticCoordinateJsonSerializer();
        var result = serializer.TrySerialize(coordinate, out var text);

        Assert.That(result, Is.True);

        Assert.That(text, Is.EqualTo("{\"latitude\":1.0}"));
    }

    //implement test cases
    private static IEnumerable<object[]> DeserializeTestCases()
    {
        return new List<object[]> {
            new object[] {
@"
{
    ""latitude"": 1.0,
    ""longitude"": 2.0,
    ""ellipsoidalHeight"": 3.0,
    ""heading"": 10.0
}",
                new GeodeticPositionWithHeading(new GeodeticPosition(1.0, 2.0, 3.0), 10.0f)
            },
            new object[] {
@"
{
    ""latitude"": 1.0,
    ""longitude"": 2.0,
    ""ellipsoidalHeight"": 3.0,
}",
                new GeodeticPositionWithHeading(new GeodeticPosition(1.0, 2.0, 3.0), 0.0f)
            },
            new object[] {
@"
{
    ""lat"": 1.0,
    ""lon"": 2.0,
    ""height"": 3.0,
    ""heading"": 10.0
}",
                new GeodeticPositionWithHeading(new GeodeticPosition(1.0, 2.0, 3.0), 10.0f)
            },
            new object[] {
@"
{
    ""lat"": 1.0,
    ""lng"": 2.0,
    ""height"": 3.0,
    ""heading"": 10.0
}",
                new GeodeticPositionWithHeading(new GeodeticPosition(1.0, 2.0, 3.0), 10.0f)
            },
            new object[] {
@"
{
    ""lat"": 1.0,
    ""long"": 2.0,
    ""height"": 3.0,
    ""heading"": 10.0
}",
                new GeodeticPositionWithHeading(new GeodeticPosition(1.0, 2.0, 3.0), 10.0f)
            }
        };
    }

    [TestCaseSource(nameof(DeserializeTestCases))]
    public void TryDeserialize(string json, GeodeticPositionWithHeading expected)
    {
        var serializer = new GeodeticCoordinateJsonSerializer();
        var result = serializer.TryDeserialize(json, out var coordinateInfo);

        Assert.That(result, Is.True);

        var geodeticPositionWithHeading = coordinateInfo as GeodeticPositionWithHeading;
        Assert.That(geodeticPositionWithHeading, Is.Not.Null);
        Assert.That(geodeticPositionWithHeading.GeodeticPosition, Is.EqualTo(expected.GeodeticPosition));
        Assert.That(geodeticPositionWithHeading.Heading, Is.EqualTo(expected.Heading));
    }
}
