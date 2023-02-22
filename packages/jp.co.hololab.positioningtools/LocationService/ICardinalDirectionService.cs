using System;
using System.Threading.Tasks;

namespace HoloLab.PositioningTools
{
    public interface ICardinalDirectionService
    {
        event Action<CardinalDirection> OnDirectionUpdated;

        void StartService();
        Task<(bool ok, Exception exception)> StartServiceAsync();
        void StopService();
        Task<(bool ok, Exception exception)> StopServiceAsync();
    }
}