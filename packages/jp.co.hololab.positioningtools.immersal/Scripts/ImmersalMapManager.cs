using Immersal;
using Immersal.AR;
using Immersal.REST;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;

namespace HoloLab.PositioningTools.Immersal
{
    public class ImmersalMapManager : MonoBehaviour
    {
        [SerializeField]
        private bool enableMapCache = true;

        private ImmersalSDK immersalSDK;
        private ARSpace arSpace;

        private SDKMapResult mapResultCache;

        private readonly MapResultSerializer mapResultSerializer = new MapResultSerializer();

        private string MapCacheFilepath
        {
            get
            {
                return Path.Combine(Application.temporaryCachePath, "PositioningTools_ImmersalMapManager_MapCache.json");
            }
        }

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

            if (enableMapCache)
            {
                LoadMapCache();
            }
        }

        private void LoadMapCache()
        {
            try
            {
                if (File.Exists(MapCacheFilepath) == false)
                {
                    return;
                }

                var mapCacheJson = File.ReadAllText(MapCacheFilepath);
                mapResultSerializer.TryDeserialize(mapCacheJson, out mapResultCache);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
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

            // Load from cache
            if (mapId == mapResultCache.metadata.id)
            {
                var map = await ARSpace.LoadAndInstantiateARMap(arSpace.transform, mapResultCache);
                return (map, "");
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

                    // Cache map data
                    if (enableMapCache)
                    {
                        try
                        {
                            var mapResultCache = mapResultSerializer.Serialize(result);
                            File.WriteAllText(mapResultCache, mapResultCache);
                        }
                        catch (Exception e)
                        {
                            Debug.LogException(e);
                        }
                    }

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
