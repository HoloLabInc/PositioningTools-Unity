using System;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace HoloLab.PositioningTools.LocationServiceImplementation
{
    /// <summary>
    /// A class that obtain the device orientation using Unity Input.compass.
    /// </summary>
    public class UnityCompassService : ICardinalDirectionService
    {
        private double latestTimestamp;

        private bool isRunning;

        private const int loopIntervalMilliseconds = 100;

        public event Action<CardinalDirection> OnDirectionUpdated;

        public async void StartService()
        {
            await StartServiceAsync();
        }

        public Task<(bool ok, Exception exception)> StartServiceAsync()
        {
            Input.compass.enabled = true;

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
            isRunning = false;
            Input.compass.enabled = false;
            return Task.FromResult<(bool, Exception)>((true, null));
        }

        private async Task GetCompassLoop()
        {
            while (isRunning)
            {
                var timestamp = Input.compass.timestamp;
                var trueHeading = Input.compass.trueHeading;

                if (timestamp > 0 && timestamp > latestTimestamp)
                {
                    latestTimestamp = timestamp;

                    // The timestamp obtained on Android is strange, so for now, the DateTimeOffset.Now is used instead.
                    var dateTimeOffset = DateTimeOffset.Now;
                    // var dateTimeOffset = TimeStampConverter.TimeStampToDateTimeOffset(timestamp);

                    var data = new CardinalDirection(trueHeading, dateTimeOffset);
                    OnDirectionUpdated?.Invoke(data);
                }

                await Task.Delay(loopIntervalMilliseconds);
            }
        }
    }
}