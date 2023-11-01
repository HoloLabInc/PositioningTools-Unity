using System;
using System.Collections;
using System.Collections.Generic;

namespace HoloLab.PositioningTools.CoordinateSerialization
{
    interface ICoordinateSerializer
    {
        bool TryDeserialize(string text, out ICoordinateInfo coordinateInfo);

        bool TrySerialize(ICoordinateInfo coordinateInfo, out string text);
    }

    public interface ICoordinateInfo { }
}