using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HoloLab.PositioningTools.GeographicCoordinate;

namespace HoloLab.PositioningTools.CoordinateSystem
{
    internal static class WorldCoordinateUtils
    {
        internal static GeodeticPose GetGeodeticPoseWithBoundPoint(Pose unityPose, GeodeticPose boundPoseInWorld, Pose boundPoseInUnity)
        {
            var relativePosition = Quaternion.Inverse(boundPoseInUnity.rotation) * (unityPose.position - boundPoseInUnity.position);
            var relativeRotation = Quaternion.Inverse(boundPoseInUnity.rotation) * unityPose.rotation;

            return GetGeodeticPoseWithRelativePose(relativePosition, relativeRotation, boundPoseInWorld);
        }

        internal static GeodeticPose GetGeodeticPoseWithBoundTransform(Pose unityPose, GeodeticPose boundPoseInWorld, Transform boundTransformInUnity)
        {
            var relativePosition = boundTransformInUnity.InverseTransformPoint(unityPose.position);
            var relativeRotation = Quaternion.Inverse(boundTransformInUnity.rotation) * unityPose.rotation;

            return GetGeodeticPoseWithRelativePose(relativePosition, relativeRotation, boundPoseInWorld);
        }

        private static GeodeticPose GetGeodeticPoseWithRelativePose(Vector3 relativePosition, Quaternion relativeRotation, GeodeticPose boundPoseInWorld)
        {
            var enuPositionUnity = boundPoseInWorld.EnuRotation * relativePosition;
            var enuPosition = new EnuPosition(enuPositionUnity.x, enuPositionUnity.z, enuPositionUnity.y);

            var enuRotation = boundPoseInWorld.EnuRotation * relativeRotation;
            var geodeticPosition = GeographicCoordinateConversion.EnuToGeodetic(enuPosition, boundPoseInWorld.GeodeticPosition);

            return new GeodeticPose(geodeticPosition, enuRotation);
        }
    }
}
