using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HoloLab.PositioningTools
{
    public interface IGeographicLocationService
    {
        event Action<GeographicLocation> OnLocationUpdated;

        void StartService();
        Task<(bool ok, Exception exception)> StartServiceAsync();
        void StopService();
        Task<(bool ok, Exception exception)> StopServiceAsync();
    }
}