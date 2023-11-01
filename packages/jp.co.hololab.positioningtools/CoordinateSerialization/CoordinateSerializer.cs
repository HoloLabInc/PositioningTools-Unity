using System;
using System.Collections;
using System.Collections.Generic;

namespace HoloLab.PositioningTools.CoordinateSerialization
{
    public class CoordinateSerializer : ICoordinateSerializer
    {
        private readonly IReadOnlyList<ICoordinateSerializer> serializers;

        private readonly List<ICoordinateSerializer> defaultSerializers = new List<ICoordinateSerializer>()
        {
            new GeodeticCoordinateJsonSerializer(),
        };

        public CoordinateSerializer()
        {
            this.serializers = defaultSerializers;
        }

        public CoordinateSerializer(IReadOnlyList<ICoordinateSerializer> serializers)
        {
            this.serializers = serializers;
        }

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
}