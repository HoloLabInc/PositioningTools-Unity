using Immersal;
using Immersal.AR;
using Immersal.REST;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace HoloLab.PositioningTools.Immersal
{
    public class ImmersalMapManager : MonoBehaviour
    {
        private ImmersalSDK immersalSDK;

        private ARSpace arSpace;

        public event Action OnLogin;

        private void Awake()
        {
            immersalSDK = GetComponentInChildren<ImmersalSDK>();
            arSpace = FindObjectOfType<ARSpace>();

            if (arSpace == null)
            {
                var arSpaceObject = new GameObject("ARSpace");
                arSpace = arSpaceObject.AddComponent<ARSpace>();
            }
        }

        public async Task LoginAsync(string username, string password)
        {
            var job = new JobLoginAsync()
            {
                username = username,
                password = password
            };


            job.OnResult += (SDKLoginResult result) =>
            {
                if (result.error == "none")
                {
                    immersalSDK.developerToken = result.token;
                    OnLogin?.Invoke();
                }
            };

            await job.RunJobAsync();
        }

        public async Task<(ARMap ARMap, string Error)> LoadMapAsync(int mapId)
        {
            if (ARSpace.mapIdToMap.TryGetValue(mapId, out var arMap))
            {
                return (arMap, "");
            }

            var completionSource = new TaskCompletionSource<(ARMap ARMap, string Error)>();

            var job = new JobLoadMapBinaryAsync
            {
                id = mapId
            };

            job.OnResult += async (SDKMapResult result) =>
            {
                if (result.metadata.error == "none")
                {
                    var map = await ARSpace.LoadAndInstantiateARMap(arSpace.transform, result);
                    completionSource.TrySetResult((map, ""));
                }
                else
                {
                    completionSource.TrySetResult((null, result.metadata.error));
                }
            };

            job.OnError += (string error) =>
            {
                completionSource.TrySetResult((null, error));
            };

            await job.RunJobAsync();
            return await completionSource.Task;
        }
    }
}
