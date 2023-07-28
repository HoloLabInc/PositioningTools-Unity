﻿using UnityEngine;

namespace HoloLab.PositioningTools.CoordinateSystem
{
    /// <summary>
    /// Binding between Unity coordinates and geographic coordinates
    /// </summary>
    public class WorldBinding
    {
        /// <summary>
        /// World binding pose in Unity coordinate
        ///
        /// If Pose is null, please use Transform value.
        /// </summary>
        public Pose? Pose { get; }

        /// <summary>
        /// World binding transform in Unity coordinate
        ///
        /// If Transform is null, please use Pose value.
        /// </summary>
        public Transform Transform { get; }

        public GeodeticPose GeodeticPose { get; }

        public WorldBinding(Pose pose, GeodeticPose geodeticPose)
        {
            Pose = pose;
            GeodeticPose = geodeticPose;
        }
    }
}