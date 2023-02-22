using System;

namespace HoloLab.PositioningTools.LocationServiceImplementation
{
    /// <summary>
    /// This is a dummy class that implements ICardinalDirectionService.
    /// This is used for debugging in UnityEditor.
    /// </summary>
    public class DummyDataCardinalDirectionService : DummyDataServiceBase, ICardinalDirectionService
    {
        public event Action<CardinalDirection> OnDirectionUpdated;

        protected override int LoopIntervalMilliseconds { get; } = 100;

        private readonly float headingDegrees;

        public DummyDataCardinalDirectionService() : this(0)
        {
        }

        public DummyDataCardinalDirectionService(float headingDegrees)
        {
            this.headingDegrees = headingDegrees;
        }

        protected override void NotifyData()
        {
            var now = DateTimeOffset.Now;
            var data = new CardinalDirection(headingDegrees, now);
            OnDirectionUpdated?.Invoke(data);
        }
    }
}