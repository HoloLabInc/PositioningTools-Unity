using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HoloLab.PositioningTools.Immersal.UI
{
    public class ImmersalUI : MonoBehaviour
    {
        [SerializeField]
        private ImmersalMapManager immersalMapManager = null;

        [SerializeField]
        private ImmersalSignInUI immersalSignInUI = null;

        [SerializeField]
        private ImmersalSignOutUI immersalSignOutUI = null;

        [SerializeField]
        private ImmersalLoadMapUI immersalLoadMapUI = null;

        private void Awake()
        {
            immersalSignInUI.Initialize(immersalMapManager);
            immersalSignOutUI.Initialize(immersalMapManager);
            immersalLoadMapUI.Initialize(immersalMapManager);

            immersalMapManager.OnLogin += ImmersalMapManager_OnLogin;
            immersalMapManager.OnLogout += ImmersalMapManager_OnLogout;
        }

        private void ImmersalMapManager_OnLogin()
        {
            immersalSignInUI.gameObject.SetActive(false);
            immersalSignOutUI.gameObject.SetActive(true);
            immersalLoadMapUI.gameObject.SetActive(true);
        }

        private void ImmersalMapManager_OnLogout()
        {
            immersalSignInUI.gameObject.SetActive(true);
            immersalSignOutUI.gameObject.SetActive(false);
            immersalLoadMapUI.gameObject.SetActive(false);
        }
    }
}
