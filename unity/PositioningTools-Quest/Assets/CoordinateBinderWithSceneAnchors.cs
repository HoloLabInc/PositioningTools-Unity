using HoloLab.PositioningTools.CoordinateSystem;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoordinateBinderWithSceneAnchors : MonoBehaviour
{
    private CoordinateManager coordinateManager;
    private OVRSceneManager ovrSceneManager;

    private readonly List<GameObject> spaceTransformList = new List<GameObject>();

    private const string spaceType = "SceneAnchor";

    private void Start()
    {
        coordinateManager = CoordinateManager.Instance;

        ovrSceneManager = FindObjectOfType<OVRSceneManager>();
        if (ovrSceneManager == null)
        {
            Debug.LogWarning($"{nameof(OVRSceneManager)} not found in scene.");
            return;
        }

        ovrSceneManager.SceneModelLoadedSuccessfully += OVRSceneManager_SceneModelLoadedSuccessfully;
    }

    private void OnDestroy()
    {
        if (ovrSceneManager != null)
        {
            ovrSceneManager.SceneModelLoadedSuccessfully -= OVRSceneManager_SceneModelLoadedSuccessfully;
        }
    }

    private void OVRSceneManager_SceneModelLoadedSuccessfully()
    {
        foreach (var spaceTransform in spaceTransformList)
        {
            Destroy(spaceTransform);
        }
        spaceTransformList.Clear();

        var sceneRooms = FindObjectsOfType<OVRSceneRoom>();
        foreach (var sceneRoom in sceneRooms)
        {
            var sceneAnchors = sceneRoom.GetComponentsInChildren<OVRSceneAnchor>();
            foreach (var sceneAnchor in sceneAnchors)
            {
                if (sceneAnchor.IsTracked == false)
                {
                    Debug.LogWarning("Not tracked");
                    continue;
                }

                var spaceTransform = new GameObject("SpaceTransform");
                spaceTransform.transform.SetParent(sceneAnchor.transform, false);
                //spaceTransform.transform.localRotation = Quaternion.Euler(270, 90, 90);
                spaceTransform.transform.localRotation = Quaternion.Euler(-90, 180, 0);
                spaceTransformList.Add(spaceTransform);

                var spaceId = sceneAnchor.Uuid.ToString();
                Debug.Log("space id " + spaceId);
                var spaceBinding = new SpaceBinding(spaceTransform.transform, spaceType, spaceId);
                coordinateManager.BindSpace(spaceBinding);
            }
        }
    }
}
