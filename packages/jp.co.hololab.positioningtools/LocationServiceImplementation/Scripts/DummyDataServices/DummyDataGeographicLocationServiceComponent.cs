using UnityEngine;

namespace HoloLab.PositioningTools.LocationServiceImplementation
{
    public class DummyDataGeographicLocationServiceComponent : GeographicLocationServiceComponentBase
    {
        [SerializeField]
        private double latitude;

        [SerializeField]
        private double longitude;

        [SerializeField]
        private double height;

        protected override IGeographicLocationService InitGeographicLocationService()
        {
            return new DummyDataGeographicLocationService(latitude, longitude, height);
        }
    }
}