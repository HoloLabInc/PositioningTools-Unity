namespace HoloLab.PositioningTools.LocationServiceImplementation
{
    public class UnityCompassServiceComponent : CardinalDirectionServiceComponentBase
    {
        protected override ICardinalDirectionService InitCardinalDirectionService()
        {
            return new UnityCompassService();
        }
    }
}