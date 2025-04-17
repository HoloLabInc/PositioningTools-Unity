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

        private const string spaceType = SpaceOrigin.SpaceTypeMarker;

        private void Awake()
        {
            coordinateManager = CoordinateManager.Instance;

            qrTracker = GetComponent<QRTracker>();
            qrTracker.OnFirstDetected += OnFirstDetected;
        }

        public void SetQRCodeDetectedInfo(QRCodeDetectedInfo info)
        {
            qrTracker.TargetQRText = info.Text;
        }

        private void OnFirstDetected(QRCodeDetectedInfo info)
        {
            BindSpaceCoordinate(info);

            // TODO: BindWorldCoordinate
        }

        private void BindSpaceCoordinate(QRCodeDetectedInfo info)
        {
            var spaceBinding = new SpaceBinding(transform, spaceType, info.Text);
            coordinateManager.BindSpace(spaceBinding);
        }
    }
}

