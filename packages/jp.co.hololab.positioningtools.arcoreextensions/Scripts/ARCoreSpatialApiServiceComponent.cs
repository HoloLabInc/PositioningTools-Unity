using Google.XR.ARCoreExtensions;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace HoloLab.PositioningTools.ARCoreExtensions
{
    public class ARCoreSpatialApiServiceComponent : MonoBehaviour, ICardinalDirectionService, IGeographicLocationService
    {
        [SerializeField]
        private AREarthManager arEarthManager = null;

        private ARCoreSpatialApiService arCoreSpatialApiService;

        public event Action<CardinalDirection> OnDirectionUpdated;
        public event Action<GeographicLocation> OnLocationUpdated;

        private void Awake()
        {
            arCoreSpatialApiService = new ARCoreSpatialApiService(arEarthManager);
            arCoreSpatialApiService.OnDirectionUpdated += x => OnDirectionUpdated?.Invoke(x);
            arCoreSpatialApiService.OnLocationUpdated += x => OnLocationUpdated?.Invoke(x);
        }

        private void OnDestroy()
        {
            arCoreSpatialApiService.StopService();
        }

        public void StartService()
        {
            arCoreSpatialApiService.StartService();
        }

        public Task<(bool ok, Exception exception)> StartServiceAsync()
        {
            return arCoreSpatialApiService.StartServiceAsync();
        }

        public void StopService()
        {
            arCoreSpatialApiService.StopService();
        }

        public Task<(bool ok, Exception exception)> StopServiceAsync()
        {
            return arCoreSpatialApiService.StopServiceAsync();
        }
    }
}