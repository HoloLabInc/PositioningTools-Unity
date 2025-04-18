using HoloLab.PositioningTools.CoordinateSerialization;
using HoloLab.PositioningTools.CoordinateSystem;
using HoloLab.QuestCameraTools.QR;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace HoloLab.PositioningTools.QuestCameraTools.QR
{
    [RequireComponent(typeof(QRTracker))]
    public class QRTrackerCoordinateBinder : MonoBehaviour
    {
        private CoordinateManager coordinateManager;
        private QRTracker qrTracker;

        private ICoordinateInfo coordinateInfo;

        private readonly CoordinateSerializer coordinateSerializer = new CoordinateSerializer();

        private static readonly float sin45 = Mathf.Sin(45 * Mathf.Deg2Rad);
        private const string spaceType = SpaceOrigin.SpaceTypeMarker;

        private void Awake()
        {
            coordinateManager = CoordinateManager.Instance;

            qrTracker = GetComponent<QRTracker>();
            qrTracker.OnFirstDetected += OnFirstDetected;
            qrTracker.OnDetected += OnDetected;
        }

        public void SetQRCodeDetectedInfo(QRCodeDetectedInfo info)
        {
            qrTracker.TargetQRText = info.Text;
            coordinateSerializer.TryDeserialize(info.Text, out coordinateInfo);
        }

        private void OnFirstDetected(QRCodeDetectedInfo info)
        {
            BindSpaceCoordinate(info);
        }

        private void OnDetected(QRCodeDetectedInfo info)
        {
            BindWorldCoordinate(info);
        }

        private void BindSpaceCoordinate(QRCodeDetectedInfo info)
        {
            var spaceBinding = new SpaceBinding(transform, spaceType, info.Text);
            coordinateManager.BindSpace(spaceBinding);
        }

        private void BindWorldCoordinate(QRCodeDetectedInfo info)
        {
            if (coordinateInfo == null)
            {
                return;
            }

            if (TryConvertQRCodeDetectedInfoToWorldBinding(info, coordinateInfo, out var worldBinding))
            {
                coordinateManager.BindCoordinates(worldBinding);
            }
        }

        private static bool TryConvertQRCodeDetectedInfoToWorldBinding(QRCodeDetectedInfo qrInfo, ICoordinateInfo coordinateInfo, out WorldBinding worldBinding)
        {
            switch (coordinateInfo)
            {
                case GeodeticPositionWithHeading geodeticPositionWithHeading:
                    {
                        var qrPose = qrInfo.Pose;
                        var qrPosition = qrPose.position;
                        var qrRotation = qrPose.rotation;

                        Quaternion rotation;
                        var qrUp = qrRotation * Vector3.up;
                        if (qrUp.y > sin45)
                        {
                            var forward = qrRotation * Vector3.forward;
                            forward.y = 0;
                            rotation = Quaternion.LookRotation(forward, Vector3.up);
                        }
                        else
                        {
                            var forward = -qrUp;
                            forward.y = 0;
                            rotation = Quaternion.LookRotation(forward, Vector3.up);
                        }

                        var pose = new Pose(qrPosition, rotation);

                        var geodeticRotation = Quaternion.AngleAxis(geodeticPositionWithHeading.Heading, Vector3.up);
                        var geodeticPosition = geodeticPositionWithHeading.GeodeticPosition;
                        var geodeticPose = new GeodeticPose(geodeticPosition, geodeticRotation);

                        worldBinding = new WorldBinding(pose, geodeticPose);
                        return true;
                    }
            }

            worldBinding = null;
            return false;
        }
    }
}

