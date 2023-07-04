using Google.XR.ARCoreExtensions;
using System;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine.XR.ARSubsystems;

namespace HoloLab.PositioningTools.ARCoreExtensions
{
    public class ARCoreSpatialApiService : ICardinalDirectionService, IGeographicLocationService
    {
        private readonly Google.XR.ARCoreExtensions.ARCoreExtensions arCoreExtensions;
        private readonly AREarthManager earthManager;

        private bool isRunning;

        private const int loopIntervalMilliseconds = 100;

        public event Action<CardinalDirection> OnDirectionUpdated;
        public event Action<GeographicLocation> OnLocationUpdated;

        public ARCoreSpatialApiService(Google.XR.ARCoreExtensions.ARCoreExtensions arCoreExtensions, AREarthManager arEarthManager)
        {
            this.arCoreExtensions = arCoreExtensions;
            earthManager = arEarthManager;
        }

        public async void StartService()
        {
            await StartServiceAsync();
        }

        public Task<(bool ok, Exception exception)> StartServiceAsync()
        {
#if UNITY_IOS
            UnityEngine.Input.location.Start();
#endif
            arCoreExtensions.ARCoreExtensionsConfig.GeospatialMode = GeospatialMode.Enabled;

            if (!isRunning)
            {
                isRunning = true;
                var getCompassLoop = new Task(() =>
                {
                    _ = GetCompassLoop();
                });
                getCompassLoop.RunSynchronously();
            }
            return Task.FromResult<(bool, Exception)>((true, null));
        }

        public async void StopService()
        {
            await StopServiceAsync();
        }

        public Task<(bool ok, Exception exception)> StopServiceAsync()
        {
#if UNITY_IOS
            UnityEngine.Input.location.Stop();
#endif
            arCoreExtensions.ARCoreExtensionsConfig.GeospatialMode = GeospatialMode.Disabled;

            isRunning = false;
            return Task.FromResult<(bool, Exception)>((true, null));
        }

        private async Task GetCompassLoop()
        {
            while (isRunning)
            {
                var earthTrackingState = earthManager.EarthTrackingState;
                if (earthTrackingState == TrackingState.Tracking)
                {
                    var pose = earthManager.CameraGeospatialPose;

                    var dateTimeOffset = DateTimeOffset.Now;
                    var heading = (float)pose.Heading;
                    var data = new CardinalDirection(heading, dateTimeOffset);
                    OnDirectionUpdated?.Invoke(data);

                    var geographicLocation = new GeographicLocation(pose.Latitude, pose.Longitude, pose.Altitude, dateTimeOffset);
                    OnLocationUpdated?.Invoke(geographicLocation);
                }

                await Task.Delay(loopIntervalMilliseconds);
            }
        }
    }
}