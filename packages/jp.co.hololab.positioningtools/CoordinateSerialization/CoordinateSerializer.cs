using System;
using System.Collections;
using System.Collections.Generic;

namespace HoloLab.PositioningTools.CoordinateSerialization
{
    public class CoordinateSerializer : ICoordinateSerializer
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

    public interface ICoordinateInfo { }
}