using System;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Serialization;

namespace HoloLab.PositioningTools.CoordinateSystem
{
    public class BinderComponentBase : MonoBehaviour
    {
        public enum RuntimeModeType
        {
            None = 0,
            Editor = 1,
            Player = 2,
            EditorAndPlayer = 3,
        }

        [SerializeField]
        [FormerlySerializedAs("runtimeType")]
        protected RuntimeModeType runtimeMode = RuntimeModeType.EditorAndPlayer;

        public RuntimeModeType RuntimeMode
        {
            get
            {
                return runtimeMode;
            }
            set
            {
                runtimeMode = value;
            }
        }

        protected bool IsBindingValid()
        {
#if UNITY_EDITOR
            switch (runtimeMode)
            {
                case RuntimeModeType.Editor:
                case RuntimeModeType.EditorAndPlayer:
                    return true;
                default:
                    return false;
            }
#else
            switch (runtimeMode)
            {
                case RuntimeModeType.Player:
                case RuntimeModeType.EditorAndPlayer:
                    return true;
                default:
                    return false;
            }
#endif
        }
    }
}