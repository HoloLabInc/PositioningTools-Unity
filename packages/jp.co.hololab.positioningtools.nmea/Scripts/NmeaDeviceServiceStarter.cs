using System.Linq;
using UnityEngine;

namespace HoloLab.PositioningTools.Nmea
{
    public class NmeaDeviceServiceStarter : MonoBehaviour
    {
        [SerializeField]
        private NmeaDeviceServiceComponent nmeaDeviceServiceComponent;

        [SerializeField]
        private string deviceDisplayName;

        private void Start()
        {
            var devices = nmeaDeviceServiceComponent.GetDeviceList();
            var device = devices.FirstOrDefault(x => x.DisplayName == deviceDisplayName);

            if (device == null)
            {
                Debug.LogWarning($"Device {deviceDisplayName} not found");
            }
            else
            {
                nmeaDeviceServiceComponent.SelectDevice(device);
                nmeaDeviceServiceComponent.StartService();
            }
        }
    }
}