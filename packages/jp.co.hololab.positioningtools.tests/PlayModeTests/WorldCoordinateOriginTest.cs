using System.Collections;
using System.Collections.Generic;
using HoloLab.PositioningTools.CoordinateSystem;
using HoloLab.PositioningTools.GeographicCoordinate;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.TestTools.Utils;

namespace Tests
{
    public class WorldCoordinateOriginTest
    {
        [Test]
        public void GetUnityPoseWithBoundPoint_GeodeticPoseAndTargetPositionHaveTheSamePositionAndRotation_TheResultMatchesBoundPoseInUnity()
        {
            var testCases = new List<Pose>()
            {
                new Pose(Vector3.zero, Quaternion.identity),
                new Pose(new Vector3(1, 2, 3), Quaternion.Euler(1, 2, 3))
            };

            foreach (var testCase in testCases)
            {
                var boundPoseInUnity = testCase;
                var targetPosition = new GeodeticPosition(1, 2, 0);
                var geodeticPose = new GeodeticPose(targetPosition, Quaternion.identity);
                var pose = WorldCoordinateOrigin.GetUnityPoseWithBoundPoint(targetPosition, geodeticPose, boundPoseInUnity);

                Assert.That(pose.position, Is.EqualTo(boundPoseInUnity.position));
                Assert.That(pose.rotation, Is.EqualTo(boundPoseInUnity.rotation));
            }
        }

        [Test]
        public void GetUnityPoseWithBoundPoint_GeodeticPoseAndTargetPositionHaveTheSamePositionAndBoundPoseHasNoRotation_TheResultMatchesBoundPoseInUnity()
        {
            var testCases = new List<(Vector3 BoundPosition, Quaternion GeodeticRoration)>()
            {
                (Vector3.zero, Quaternion.identity),
                (new Vector3(1, 2, 3), Quaternion.Euler(10, 20, 30))
            };

            foreach (var testCase in testCases)
            {
                var boundPoseInUnity = new Pose(testCase.BoundPosition, Quaternion.identity);
                var targetPosition = new GeodeticPosition(1, 2, 0);
                var geodeticPose = new GeodeticPose(targetPosition, testCase.GeodeticRoration);
                var pose = WorldCoordinateOrigin.GetUnityPoseWithBoundPoint(targetPosition, geodeticPose, boundPoseInUnity);

                Assert.That(pose.position, Is.EqualTo(boundPoseInUnity.position).Using(Vector3EqualityComparer.Instance));
                Assert.That(pose.rotation, Is.EqualTo(Quaternion.Inverse(geodeticPose.EnuRotation)).Using(QuaternionEqualityComparer.Instance));
            }
        }

        [Test]
        public void GetUnityPoseWithBoundPoint_BoundPoseInUnityAndGeodeticPoseHaveTheSameRotation_NoRotation()
        {
            var testCases = new List<(Vector3 BoundPosition, Quaternion GeodeticRoration)>()
            {
                (Vector3.zero, Quaternion.identity),
                (new Vector3(1, 2, 3), Quaternion.Euler(10, 20, 30))
            };

            foreach (var testCase in testCases)
            {
                var boundPoseInUnity = new Pose(testCase.BoundPosition, testCase.GeodeticRoration);
                var targetPosition = new GeodeticPosition(1, 2, 0);
                var geodeticPose = new GeodeticPose(targetPosition, testCase.GeodeticRoration);
                var pose = WorldCoordinateOrigin.GetUnityPoseWithBoundPoint(targetPosition, geodeticPose, boundPoseInUnity);

                Assert.That(pose.position, Is.EqualTo(boundPoseInUnity.position).Using(Vector3EqualityComparer.Instance));
                Assert.That(pose.rotation, Is.EqualTo(Quaternion.identity).Using(QuaternionEqualityComparer.Instance));
            }
        }

        [Test]
        public void ConvertGeodeticPoseToUnityPoseAndConvertUnityPoseToGeodeticPose_GeodeticPoseIsEuqual()
        {
            var testCases = new List<(Vector3 BoundPosition, Quaternion BoundRotation)>()
            {
                (Vector3.zero, Quaternion.identity),
                (new Vector3(1, 2, 3), Quaternion.Euler(10, 20, 30))
            };

            foreach (var testCase in testCases)
            {
                var boundPoseInUnity = new Pose(testCase.BoundPosition, testCase.BoundRotation);

                var geodeticPose = new GeodeticPose(new GeodeticPosition(0.1, 0.2, 0.3), Quaternion.Euler(5, 6, 7));

                var targetPosition = new GeodeticPosition(0.1001, 0.2001, 0.5);
                var targetRotation = Quaternion.Euler(1, 2, 3);
                var targetPose = new GeodeticPose(targetPosition, targetRotation);

                var poseInUnity = WorldCoordinateOrigin.GetUnityPoseWithBoundPoint(targetPose, geodeticPose, boundPoseInUnity);
                var convertedGeodeticPose = WorldCoordinateOrigin.GetGeodeticPoseWithBoundPoint(poseInUnity, geodeticPose, boundPoseInUnity);

                var epsilon = 0.000000001;
                Assert.That(convertedGeodeticPose.GeodeticPosition.Latitude, Is.EqualTo(targetPose.GeodeticPosition.Latitude).Within(epsilon));
                Assert.That(convertedGeodeticPose.GeodeticPosition.Longitude, Is.EqualTo(targetPose.GeodeticPosition.Longitude).Within(epsilon));

                var heightEpsilon = 0.0001;
                Assert.That(convertedGeodeticPose.GeodeticPosition.EllipsoidalHeight, Is.EqualTo(targetPose.GeodeticPosition.EllipsoidalHeight).Within(heightEpsilon));
            }
        }

        [Test]
        public void EnuRotationIsSetInTransformMode_GeodeticPositionUpdated()
        {
            var coordinateManagerGameObject = new GameObject();
            coordinateManagerGameObject.AddComponent<CoordinateManager>();

            var binderGameObject = new GameObject();
            var binder = binderGameObject.AddComponent<WorldCoordinateBinder>();
            binder.GeodeticPosition = new GeodeticPosition(0.1, 0.2, 0);

            var go = new GameObject();
            var origin = go.AddComponent<WorldCoordinateOrigin>();
            origin.PositionSettingMode = WorldCoordinateOrigin.PositionSettingModeType.Transform;
            var height = 2;
            origin.transform.position = height * Vector3.up;

            origin.EnuRotation = Quaternion.AngleAxis(10, Vector3.right);

            Assert.That(origin.GeodeticPosition.EllipsoidalHeight, Is.EqualTo(height));

            GameObject.Destroy(go);
            GameObject.Destroy(binderGameObject);
            GameObject.Destroy(coordinateManagerGameObject);
        }
    }
}