using Immersal.REST;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HoloLab.PositioningTools.Immersal
{
    internal class MapResultSerializer
    {
        [Serializable]
        private struct MapResultForSerialize
        {
            public SDKMapResult sdkMapResult;

            public string mapDataAsBase64;
        }

        public string Serialize(SDKMapResult mapResult)
        {
            var resultForSerialize = new MapResultForSerialize()
            {
                sdkMapResult = mapResult,
                mapDataAsBase64 = Convert.ToBase64String(mapResult.mapData)
            };

            resultForSerialize.sdkMapResult.mapData = null;

            return JsonUtility.ToJson(resultForSerialize);
        }

        public bool TryDeserialize(string json, out SDKMapResult mapResult)
        {
            try
            {
                var resultForSerialize = JsonUtility.FromJson<MapResultForSerialize>(json);

                mapResult = resultForSerialize.sdkMapResult;
                mapResult.mapData = Convert.FromBase64String(resultForSerialize.mapDataAsBase64);

                return true;
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                mapResult = default;
                return false;
            }
        }
    }
}
