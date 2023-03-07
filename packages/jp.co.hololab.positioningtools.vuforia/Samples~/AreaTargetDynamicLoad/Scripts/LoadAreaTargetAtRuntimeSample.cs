using HoloLab.PositioningTools.CoordinateSystem;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace HoloLab.PositioningTools.Vuforia.Samples
{
    public class LoadAreaTargetAtRuntimeSample : MonoBehaviour
    {
        [SerializeField]
        private GameObject areaTargetRootObjectPrefab = null;

        private string AreaTargetDataRootPath
        {
            get => Path.Combine(Application.persistentDataPath, "AreaTargetData");
        }

        private void Start()
        {
            var spaceBinderWithVuforiaAreaTarget = GameObject.FindObjectOfType<SpaceBinderWithVuforiaAreaTarget>();

            if (!Directory.Exists(AreaTargetDataRootPath))
            {
                Directory.CreateDirectory(AreaTargetDataRootPath);
            }

            Debug.Log($"Area target data root folder: {AreaTargetDataRootPath}");

            var mapFiles = Directory.EnumerateFiles(AreaTargetDataRootPath, "*.xml", SearchOption.AllDirectories);

            foreach (var mapFile in mapFiles)
            {
                try
                {
                    Debug.Log($"Load map: {mapFile}");
                    spaceBinderWithVuforiaAreaTarget.LoadAreaTarget(mapFile);

                    // Place a cube at map origin
                    var areaTargetRootObject = Instantiate(areaTargetRootObjectPrefab);
                    var spaceOrigin = areaTargetRootObject.AddComponent<SpaceOrigin>();
                    var mapName = Path.GetFileNameWithoutExtension(mapFile);
                    spaceOrigin.Initialize(SpaceOrigin.SpaceTypeVuforiaAreaTarget, mapName);
                }
                catch (Exception e)
                {
                    Debug.LogWarning(e);
                }
            }
        }
    }
}
