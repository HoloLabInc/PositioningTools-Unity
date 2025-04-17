using HoloLab.QuestCameraTools.QR;
using System.Collections.Generic;
using UnityEngine;

namespace HoloLab.PositioningTools.QuestCameraTools.QR
{
    public class CoordinateBinderWithQRTracking : MonoBehaviour
    {
        [SerializeField]
        private QRTrackerCoordinateBinder qrTrackerCoordinateBinderPrefab;

        private QuestQRTracking qrTracking;

        private readonly Dictionary<string, QRTrackerCoordinateBinder> qrObjects = new Dictionary<string, QRTrackerCoordinateBinder>();

        private void Start()
        {
            qrTracking = FindFirstObjectByType<QuestQRTracking>();
            if (qrTracking == null)
            {
                Debug.LogError($"{nameof(QuestQRTracking)} not found in the scene.");
                return;
            }

            qrTracking.OnQRCodeDetected += OnQRCodeDetected;
        }

        private void OnDestroy()
        {
            if (qrTracking != null)
            {
                qrTracking.OnQRCodeDetected -= OnQRCodeDetected;
            }

            foreach (var qrObject in qrObjects.Values)
            {
                if (qrObject != null)
                {
                    Destroy(qrObject.gameObject);
                }
            }
            qrObjects.Clear();
        }

        private void OnQRCodeDetected(List<QRCodeDetectedInfo> infoList)
        {
            foreach (var info in infoList)
            {
                if (qrObjects.ContainsKey(info.Text))
                {
                    continue;
                }

                var qrObject = Instantiate(qrTrackerCoordinateBinderPrefab, transform);
                qrObject.SetQRCodeDetectedInfo(info);

                qrObjects[info.Text] = qrObject;
            }
        }
    }
}

