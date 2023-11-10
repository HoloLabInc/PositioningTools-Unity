using HoloLab.PositioningTools.CoordinateSystem;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoordinateBinderWithSceneAnchors : MonoBehaviour
{
    private CoordinateManager coordinateManager;
    private OVRSceneManager ovrSceneManager;

    private const string spaceType = "SceneAnchor";

    private void Start()
    {
        coordinateManager = CoordinateManager.Instance;

        var ovrSceneManager = FindObjectOfType<OVRSceneManager>();
        if (ovrSceneManager == null)
        {
            Debug.LogWarning($"{nameof(OVRSceneManager)} not found in scene.");
            return;
        }

        ovrSceneManager.SceneModelLoadedSuccessfully += OVRSceneManager_SceneModelLoadedSuccessfully;
    }

    private void OnDestroy()
    {
        ovrSceneManager.SceneModelLoadedSuccessfully -= OVRSceneManager_SceneModelLoadedSuccessfully;
    }

    private void OVRSceneManager_SceneModelLoadedSuccessfully()
    {
        var sceneRooms = FindObjectsOfType<OVRSceneRoom>();
        foreach (var sceneRoom in sceneRooms)
        {
            var sceneAnchors = sceneRoom.GetComponentsInChildren<OVRSceneAnchor>();
            foreach (var sceneAnchor in sceneAnchors)
            {
                if (sceneAnchor.IsTracked == false)
                {
                    continue;
                }

                var spaceId = sceneAnchor.Uuid.ToString();
                var spaceBinding = new SpaceBinding(sceneAnchor.transform, spaceType, spaceId);
                coordinateManager.BindSpace(spaceBinding);
            }
        }
    }
}
