using System;

namespace HoloLab.PositioningTools.LocationServiceImplementation
{
    /// <summary>
    /// This is a dummy class that implements IGeographicLocationService.
    /// This is used for debugging in UnityEditor.
    /// </summary>
    public class DummyDataGeographicLocationService : DummyDataServiceBase, IGeographicLocationService
    {
        private const double DefaultLatitude = 0;
        private const double DefaultLongitude = 0;
        private const double DefaultHeight = 0;

        private readonly double latitude;
        private readonly double longitude;
        private readonly double height;

        protected override int LoopIntervalMilliseconds { get; } = 1000;

        public event Action<GeographicLocation> OnLocationUpdated;

        public DummyDataGeographicLocationService() : this(DefaultLatitude, DefaultLongitude, DefaultHeight)
        {
        }

        public DummyDataGeographicLocationService(double latitude, double longitude, double height)
        {
            this.latitude = latitude;
            this.longitude = longitude;
            this.height = height;
        }

        protected override void NotifyData()
        {
            var now = DateTimeOffset.Now;
            var data = new GeographicLocation(latitude, longitude, height, now);
            OnLocationUpdated?.Invoke(data);
        }
    }
}