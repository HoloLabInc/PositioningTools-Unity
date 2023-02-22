using System;
using System.Threading.Tasks;

namespace HoloLab.PositioningTools.LocationServiceImplementation
{
    public abstract class DummyDataServiceBase
    {
        protected bool isRunning;
        protected abstract int LoopIntervalMilliseconds { get; }

        public async void StartService()
        {
            await StartServiceAsync();
        }

        public Task<(bool ok, Exception exception)> StartServiceAsync()
        {
            if (!isRunning)
            {
                isRunning = true;
                var updateLoop = new Task(() =>
                {
                    _ = UpdateLoop();
                });
                updateLoop.RunSynchronously();
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
            return Task.FromResult<(bool, Exception)>((true, null));
        }

        protected abstract void NotifyData();

        private async Task UpdateLoop()
        {
            while (isRunning)
            {
                NotifyData();
                await Task.Delay(LoopIntervalMilliseconds);
            }
        }
    }
}