using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace HoloLab.PositioningTools.CoordinateSystem.Samples
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
            Vector3 position = Vector3.zero;
            Vector3 eulerAngles = Vector3.zero;

            if (worldBinding.Transform != null)
            {
                position = worldBinding.Transform.position;
                eulerAngles = worldBinding.Transform.eulerAngles;
            }
            else if (worldBinding.ApplicationPose.HasValue)
            {
                position = worldBinding.ApplicationPose.Value.position;
                eulerAngles = worldBinding.ApplicationPose.Value.rotation.eulerAngles;
            }

            var geodeticPose = worldBinding.GeodeticPose;
            var geodeticPosition = geodeticPose.GeodeticPosition;

            var builder = new StringBuilder();
            builder.AppendLine($"[UnityPose] Position: {position}, EnuRotation: {eulerAngles}");
            builder.AppendLine($"[GeodeticPose]");
            builder.AppendLine($"Latitude: {geodeticPosition.Latitude}");
            builder.AppendLine($"Longitude: {geodeticPosition.Longitude}");
            builder.AppendLine($"EllipsoidalHeight: {geodeticPosition.EllipsoidalHeight}, EnuRotation: {geodeticPose.EnuRotation.eulerAngles}");

            text.text = builder.ToString();
        }
    }
}