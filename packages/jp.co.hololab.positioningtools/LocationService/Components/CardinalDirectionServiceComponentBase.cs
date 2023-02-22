using System;
using System.Threading.Tasks;
using UnityEngine;

namespace HoloLab.PositioningTools
{
    public abstract class CardinalDirectionServiceComponentBase : MonoBehaviour, ICardinalDirectionService
    {
        private ICardinalDirectionService cardinalDirectionService;

        public event Action<CardinalDirection> OnDirectionUpdated;

        protected virtual void Awake()
        {
            cardinalDirectionService = InitCardinalDirectionService();
            cardinalDirectionService.OnDirectionUpdated += x => OnDirectionUpdated?.Invoke(x);
        }

        protected virtual void OnDestroy()
        {
            cardinalDirectionService.StopService();
        }

        protected abstract ICardinalDirectionService InitCardinalDirectionService();

        public void StartService()
        {
            cardinalDirectionService.StartService();
        }

        public Task<(bool ok, Exception exception)> StartServiceAsync()
        {
            return cardinalDirectionService.StartServiceAsync();
        }

        public void StopService()
        {
            cardinalDirectionService.StopService();
        }

        public Task<(bool ok, Exception exception)> StopServiceAsync()
        {
            return cardinalDirectionService.StopServiceAsync();
        }
    }
}