using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HoloLab.PositioningTools.Immersal
{
    public class ImmersalUI : MonoBehaviour
    {
        [SerializeField]
        private ImmersalMapManager immersalMapManager = null;

        [SerializeField]
        private ImmersalSignInUI immersalSignInUI = null;

        [SerializeField]
        private ImmersalLoadMapUI immersalLoadMapUI = null;

        private void Awake()
        {
            immersalSignInUI.Initialize(immersalMapManager);
            immersalLoadMapUI.Initialize(immersalMapManager);
        }

        private void Start()
        {
            immersalMapManager.OnLogin += ImmersalMapManager_OnLogin;
        }

        private void ImmersalMapManager_OnLogin()
        {
            immersalSignInUI.gameObject.SetActive(false);
            immersalLoadMapUI.gameObject.SetActive(true);
        }
    }
}
