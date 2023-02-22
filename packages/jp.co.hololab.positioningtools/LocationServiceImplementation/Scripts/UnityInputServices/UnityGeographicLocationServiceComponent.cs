namespace HoloLab.PositioningTools.LocationServiceImplementation
{
    public class UnityGeographicLocationServiceComponent : GeographicLocationServiceComponentBase
    {
        protected override IGeographicLocationService InitGeographicLocationService()
        {
            return new UnityGeographicLocationService();
        }
    }
}