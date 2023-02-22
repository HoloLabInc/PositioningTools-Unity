using UnityEngine;

namespace HoloLab.PositioningTools.LocationServiceImplementation
{
    public class DummyDataCardinalDirectionServiceComponent : CardinalDirectionServiceComponentBase
    {
        [SerializeField]
        private float headingDegrees;

        protected override ICardinalDirectionService InitCardinalDirectionService()
        {
            return new DummyDataCardinalDirectionService(headingDegrees);
        }
    }
}