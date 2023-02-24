using HoloLab.PositioningTools.GeographicCoordinate;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Random = UnityEngine.Random;

namespace HoloLab.PositioningTools.CoordinateSystem.Tests
{
    public class TestBinderWithSubReferencePoint : MonoBehaviour
    {
        [SerializeField]
        private GeodeticPositionForInspector geodeticPosition;

        private async void Awake()
        {
            while (Application.isPlaying)
            {
                await Task.Delay(5000);
                BindTest();
            }
        }

        private void BindTest()
        {
            var referencePose = new Pose(Vector3.zero, Quaternion.identity);
            var referencePointGeodeticPosition = geodeticPosition.ToGeodeticPosition();
            var referencePointGeodeticPose = new GeodeticPose(referencePointGeodeticPosition, Quaternion.identity);
            var referencePointBinding = new WorldBinding(referencePose, referencePointGeodeticPose);

            var randomVector2 = Random.insideUnitCircle;
            var subReferencePosition = new Vector3(randomVector2.x, 0, randomVector2.y);
            var subReferencePose = new Pose(subReferencePosition, Quaternion.identity);

            var subReferencePointGeodeticPosition = EnuToGeodeticPosition(subReferencePosition);
            var subReferencePointGeodeticPose = new GeodeticPose(subReferencePointGeodeticPosition, Quaternion.identity);
            var subReferencePointBinding = new WorldBinding(subReferencePose, subReferencePointGeodeticPose);

            Bind(referencePointBinding, subReferencePointBinding);
        }

        private GeodeticPosition EnuToGeodeticPosition(Vector3 position)
        {
            var location = GeographicCoordinateConversion.EnuToGeodetic(
                new EnuPosition(position.x, position.z, position.y),
                geodeticPosition.ToGeodeticPosition());

            return location;
        }

        private void Bind(
            WorldBinding referencePointBinding,
            WorldBinding subReferencePointBinding
        )
        {
            var spaceCoordinateManager = CoordinateManager.Instance;

            var binding = WorldCoordinateBinderWithLocationService.CalcWorldBindingWithSubReferencePoint(
                referencePointBinding, subReferencePointBinding);
            spaceCoordinateManager.BindCoordinates(binding);
        }
    }
}