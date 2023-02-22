using System;
using System.Threading.Tasks;
using UnityEngine;

namespace HoloLab.PositioningTools
{
    public abstract class GeographicLocationServiceComponentBase : MonoBehaviour, IGeographicLocationService
    {
        private IGeographicLocationService geographicLocationService;

        public event Action<GeographicLocation> OnLocationUpdated;

        protected virtual void Awake()
        {
            geographicLocationService = InitGeographicLocationService();
            geographicLocationService.OnLocationUpdated += x => OnLocationUpdated?.Invoke(x);
        }

        protected virtual void OnDestroy()
        {
            geographicLocationService.StopService();
        }

        protected abstract IGeographicLocationService InitGeographicLocationService();

        public void StartService()
        {
            geographicLocationService.StartService();
        }

        public Task<(bool ok, Exception exception)> StartServiceAsync()
        {
            return geographicLocationService.StartServiceAsync();
        }

        public void StopService()
        {
            geographicLocationService.StopService();
        }

        public Task<(bool ok, Exception exception)> StopServiceAsync()
        {
            return geographicLocationService.StopServiceAsync();
        }
    }
}