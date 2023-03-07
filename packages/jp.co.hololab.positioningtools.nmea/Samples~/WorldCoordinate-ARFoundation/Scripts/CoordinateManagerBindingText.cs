using HoloLab.PositioningTools.CoordinateSystem;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace HoloLab.PositioningTools.Nmea.Samples
{
    public class CoordinateManagerBindingText : MonoBehaviour
    {
        [SerializeField]
        private Text text = null;

        private void Start()
        {
            CoordinateManager.Instance.OnCoordinatesBound += OnCoordinatesBound;
        }

        private void OnCoordinatesBound(WorldBinding worldBinding)
        {
            var pose = worldBinding.ApplicationPose;
            var geodeticPose = worldBinding.GeodeticPose;
            var geodeticPosition = geodeticPose.GeodeticPosition;

            var builder = new StringBuilder();
            builder.AppendLine($"[UnityPose] Position: {pose.position}, EnuRotation: {pose.rotation.eulerAngles}");
            builder.AppendLine($"[GeodeticPose]");
            builder.AppendLine($"Latitude: {geodeticPosition.Latitude}");
            builder.AppendLine($"Longitude: {geodeticPosition.Longitude}");
            builder.AppendLine($"EllipsoidalHeight: {geodeticPosition.EllipsoidalHeight}, EnuRotation: {geodeticPose.EnuRotation.eulerAngles}");

            text.text = builder.ToString();
        }
    }
}