using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace HoloLab.PositioningTools.Nmea
{
    public class NmeaDeviceServiceComponent : MonoBehaviour, ICardinalDirectionService, IGeographicLocationService
    {
        [SerializeField]
        private bool debugLogReceivedMessage;

        [SerializeField]
        private TextAsset nmeaFileForEditorSimulation;

        private NmeaDeviceService nmeaDeviceService;

        private void Awake()
        {
            string nmeaFilePath = null;
            if (nmeaFileForEditorSimulation != null)
            {
#if UNITY_EDITOR
                nmeaFilePath = AssetDatabase.GetAssetPath(nmeaFileForEditorSimulation);
#endif
            }

            nmeaDeviceService = new NmeaDeviceService(SynchronizationContext.Current)
            {
                DebugLogReceivedMessage = debugLogReceivedMessage,
                NmeaFilePathForSimulation = nmeaFilePath
            };
            nmeaDeviceService.OnDirectionUpdated += x => OnDirectionUpdated?.Invoke(x);
            nmeaDeviceService.OnLocationUpdated += x => OnLocationUpdated?.Invoke(x);
        }

        private void OnDestroy()
        {
            nmeaDeviceService.StopService();
        }

        public event Action<CardinalDirection> OnDirectionUpdated;
        public event Action<GeographicLocation> OnLocationUpdated;

        public void StartService()
        {
            nmeaDeviceService.StartService();
        }

        public Task<(bool ok, Exception exception)> StartServiceAsync()
        {
            return nmeaDeviceService.StartServiceAsync();
        }

        public void StopService()
        {
            nmeaDeviceService.StopService();
        }

        public Task<(bool ok, Exception exception)> StopServiceAsync()
        {
            return nmeaDeviceService.StopServiceAsync();
        }

        public IReadOnlyList<NmeaDeviceInfo> GetDeviceList()
        {
            return nmeaDeviceService.GetDeviceList();
        }

        public void SelectDevice(NmeaDeviceInfo deviceInfo)
        {
            nmeaDeviceService.SelectDevice(deviceInfo);
        }
    }
}