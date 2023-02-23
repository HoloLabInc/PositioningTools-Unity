using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace HoloLab.PositioningTools.Samples
{
    public class GetGeographicPoseSample : MonoBehaviour
    {
        [SerializeField]
        private CardinalDirectionServiceComponent cardinalDirectionService = null;

        [SerializeField]
        private GeographicLocationServiceComponent geographicLocationService = null;

        [SerializeField]
        private Text geographicLocationText = null;

        [SerializeField]
        private Text cardinalDirectionText = null;

        private void Start()
        {
            geographicLocationService.OnLocationUpdated += GeographicLocationService_OnLocationUpdated;
            cardinalDirectionService.OnDirectionUpdated += CardinalDirectionService_OnDirectionUpdated;
        }

        private void GeographicLocationService_OnLocationUpdated(GeographicLocation geographicLocation)
        {
            Debug.Log(geographicLocation);
            geographicLocationText.text = $"{geographicLocation}\n{geographicLocation.DateTimeOffset}";
        }

        private void CardinalDirectionService_OnDirectionUpdated(CardinalDirection cardinalDirection)
        {
            Debug.Log(cardinalDirection);
            cardinalDirectionText.text = $"{cardinalDirection}\n{cardinalDirection.DateTimeOffset}";
        }
    }
}