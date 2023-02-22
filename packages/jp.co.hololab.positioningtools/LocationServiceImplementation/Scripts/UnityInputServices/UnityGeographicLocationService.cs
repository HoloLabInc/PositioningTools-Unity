using System;
using System.Threading.Tasks;
using UnityEngine;

namespace HoloLab.PositioningTools.LocationServiceImplementation
{
    /// <summary>
    /// A class that obtain the device geographic location using Unity Input.location.
    /// </summary>
    public class UnityGeographicLocationService : IGeographicLocationService
    {
        private LocationInfo? latestData;
        private bool isRunning;

        private const int loopIntervalMilliseconds = 1000;

        public bool IsEnabled => Input.location.isEnabledByUser;

        public event Action<GeographicLocation> OnLocationUpdated;


        public async void StartService()
        {
            await StartServiceAsync();
        }

        public async Task<(bool ok, Exception exception)> StartServiceAsync()
        {
            Input.location.Start();

            int maxWait = 20;
            while (Input.location.status == LocationServiceStatus.Initializing)
            {
                await Task.Delay(1000);
                maxWait--;
                if (maxWait == 0)
                {
                    return (false, new Exception("Initialization timed out"));
                }
            }

            // TODO: On the first launch, a permission request screen will be displayed and the value cannot be obtained.

            if (Input.location.status == LocationServiceStatus.Failed)
            {
                return (false, new Exception("LocationService Failed"));
            }

            if (!isRunning)
            {
                isRunning = true;
                var getLocationLoop = new Task(() =>
                {
                    _ = GetLocationLoop();
                });
                getLocationLoop.RunSynchronously();
            }

            return (true, null);
        }

        public async void StopService()
        {
            await StopServiceAsync();
        }

        public Task<(bool ok, Exception exception)> StopServiceAsync()
        {
            isRunning = false;
            Input.location.Stop();
            return Task.FromResult<(bool, Exception)>((true, null));
        }


        private async Task GetLocationLoop()
        {
            while (isRunning)
            {
                var locationInfo = Input.location.lastData;

                if (locationInfo.timestamp > 0 && (latestData == null || locationInfo.timestamp > latestData.Value.timestamp))
                {
                    latestData = locationInfo;

                    var dateTimeOffset = TimeStampConverter.TimeStampToDateTimeOffset(locationInfo.timestamp);
                    var position = new GeographicLocation(locationInfo.latitude, locationInfo.longitude, locationInfo.altitude, dateTimeOffset);
                    this.OnLocationUpdated?.Invoke(position);
                }

                await Task.Delay(loopIntervalMilliseconds);
            }
        }
    }
}