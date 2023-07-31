using System;
using UnityEngine;
using System.Collections.Generic;

namespace HoloLab.PositioningTools.CoordinateSystem
{
    public class BinderComponentBase : MonoBehaviour
    {
        protected enum RuntimeType
        {
            None = 0,
            Editor = 1,
            Player = 2,
            EditorAndPlayer = 3,
        }

        [SerializeField]
        protected RuntimeType runtimeType = RuntimeType.Editor;

        protected bool IsBindingValid()
        {
#if UNITY_EDITOR
            switch (runtimeType)
            {
                case RuntimeType.Editor:
                case RuntimeType.EditorAndPlayer:
                    return true;
                default:
                    return false;
            }
#else
            switch (runtimeType)
            {
                case RuntimeType.Player:
                case RuntimeType.EditorAndPlayer:
                    return true;
                default:
                    return false;
            }
#endif
        }
    }
}