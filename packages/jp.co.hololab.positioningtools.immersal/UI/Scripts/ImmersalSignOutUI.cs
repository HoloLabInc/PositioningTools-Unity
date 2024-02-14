using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace HoloLab.PositioningTools.Immersal.UI
{
    public class ImmersalSignOutUI : MonoBehaviour
    {
        [SerializeField]
        private Button signOutButtton = null;

        private ImmersalMapManager immersalMapManager;

        public void Initialize(ImmersalMapManager immersalMapManager)
        {
            this.immersalMapManager = immersalMapManager;
        }

        private void Start()
        {
            signOutButtton.onClick.AddListener(Logout);
        }

        private void Logout()
        {
            immersalMapManager.Logout();
        }
    }
}
