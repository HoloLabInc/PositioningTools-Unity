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
    }
}