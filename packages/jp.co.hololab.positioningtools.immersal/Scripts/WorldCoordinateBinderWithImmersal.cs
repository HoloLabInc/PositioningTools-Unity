using System;
using System.Collections;
using HoloLab.PositioningTools.CoordinateSystem;
using HoloLab.PositioningTools.GeographicCoordinate;
using Immersal.AR;
using UnityEngine;
using ImmersalCore = Immersal.Core;

namespace HoloLab.PositioningTools.Immersal
{
    public class WorldCoordinateBinderWithImmersal : MonoBehaviour
    {
        private LocalizerBase arLocalizer;
        private CoordinateManager coordinateManager;

        private void Start()
        {
            coordinateManager = CoordinateManager.Instance;
            arLocalizer = GetComponentInChildren<LocalizerBase>();
            arLocalizer.OnPoseFound += OnPoseFound;
        }

        private void OnPoseFound(LocalizerPose localizerPose)
        {
            BindSpaceCoordinate(localizerPose);
            BindWorldCoordinate(localizerPose);
        }

        private void BindSpaceCoordinate(LocalizerPose localizerPose)
        {
            var spaceType = SpaceOrigin.SpaceTypeImmersal;
            var mapId = arLocalizer.lastLocalizedMapId.ToString();

            if (ARSpace.mapIdToMap.TryGetValue(arLocalizer.lastLocalizedMapId, out var arMap))
            {
                var spaceBinding = new SpaceBinding(arMap.transform, spaceType, mapId);
                coordinateManager.BindSpace(spaceBinding);
            }
        }

        private void BindWorldCoordinate(LocalizerPose localizerPose)
        {
            var ecefPosition = UnityPositionToEcef(Vector3.zero, localizerPose);
            var geodeticPosition = GeographicCoordinateConversion.EcefToGeodetic(ecefPosition);

            var forwardEnu = UnityPositionToEnu(Vector3.forward, localizerPose, geodeticPosition);

            // For the vertical direction, use the Y-axis of the Unity application
            forwardEnu.y = 0;

            var applicationPose = new Pose(Vector3.zero, Quaternion.identity);
            var geodeticPose = new GeodeticPose(
                geodeticPosition,
                Quaternion.LookRotation(forwardEnu, Vector3.up)
            );
            var binding = new WorldBinding(applicationPose, geodeticPose);
            coordinateManager.BindCoordinates(binding);
        }

        private static Vector3 UnityPositionToEnu(Vector3 position, LocalizerPose localizerPose, GeodeticPosition geodeticPosition)
        {
            var ecefPosition = UnityPositionToEcef(position, localizerPose);

            var enuPosition = GeographicCoordinateConversion.EcefToEnu(
                ecefPosition.X, ecefPosition.Y, ecefPosition.Z,
                geodeticPosition.Latitude, geodeticPosition.Longitude, geodeticPosition.EllipsoidalHeight);

            return enuPosition.ToUnityVector();
        }

        private static EcefPosition UnityPositionToEcef(Vector3 position, LocalizerPose localizerPose)
        {
            Vector3 positionInMap = localizerPose.matrix.MultiplyPoint(position);

            var ecefArray = new double[3];
            ImmersalCore.PosMapToEcef(ecefArray, ARHelper.SwitchHandedness(positionInMap), localizerPose.mapToEcef);
            var ecefPosition = new EcefPosition(ecefArray[0], ecefArray[1], ecefArray[2]);
            return ecefPosition;
        }
    }
}