using Immersal.AR;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace HoloLab.PositioningTools.Immersal.UI
{
    public class ImmersalLoadMapUI : MonoBehaviour
    {
        [SerializeField]
        private TMP_InputField loadMapInputField = null;

        [SerializeField]
        private Button loadMapButtton = null;

        [SerializeField]
        private TMP_Text statusText = null;

        private ImmersalMapManager immersalMapManager;

        private const string loadMapInputFieldKey = "ImmersalLoadMapUI_loadMapInputField";

        public void Initialize(ImmersalMapManager immersalMapManager)
        {
            this.immersalMapManager = immersalMapManager;
        }

        private void Awake()
        {
            loadMapButtton.onClick.AddListener(LoadMap);
            var mapIdText = PlayerPrefs.GetString(loadMapInputFieldKey);
            loadMapInputField.text = mapIdText;
        }

        private async void LoadMap()
        {
            var mapIdText = loadMapInputField.text;

            if (int.TryParse(mapIdText, out var mapId))
            {
                PlayerPrefs.SetString(loadMapInputFieldKey, mapIdText);
                PlayerPrefs.Save();

                loadMapButtton.interactable = false;
                var (arMap, error) = await immersalMapManager.LoadMapAsync(mapId);
                loadMapButtton.interactable = true;

                if (arMap == null)
                {
                    ShowMessage($"Failed to load map {mapId}\n{error}");
                }
                else
                {
                    ShowMessage($"Map {mapId} has been loaded");

                    if (arMap.OnFirstLocalization == null)
                    {
                        arMap.OnFirstLocalization = new MapLocalizedEvent();
                    }
                    arMap.OnFirstLocalization.AddListener(ARMap_OnFirstLocalization);
                }
            }
        }

        private void ARMap_OnFirstLocalization(int mapId)
        {
            ShowMessage($"On first localization with map {mapId}");
        }

        private void ShowMessage(string message)
        {
            if (statusText != null)
            {
                statusText.text = message;
            }
        }
    }
}
