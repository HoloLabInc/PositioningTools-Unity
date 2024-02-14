using Immersal;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace HoloLab.PositioningTools.Immersal.UI
{
    public class ImmersalSignInUI : MonoBehaviour
    {
        [SerializeField]
        private TMP_InputField emailAddressInputField = null;

        [SerializeField]
        private TMP_InputField passwordInputField = null;

        [SerializeField]
        private TMP_InputField serverUrlInputField = null;

        [SerializeField]
        private Button signInButtton = null;

        private ImmersalMapManager immersalMapManager;
        private ImmersalSDK immersalSDK;

        private const string serverUrlInputFieldKey = "ImmersalSignInUI_serverUrlInputField";

        public string EmailAddress
        {
            get => emailAddressInputField.text;
            set => emailAddressInputField.text = value;
        }

        public string Password
        {
            get => passwordInputField.text;
            set => passwordInputField.text = value;
        }

        public string ServerUrl
        {
            get => serverUrlInputField.text;
            set => serverUrlInputField.text = value;
        }

        public void Initialize(ImmersalMapManager immersalMapManager)
        {
            this.immersalMapManager = immersalMapManager;
        }

        private void Start()
        {
            immersalSDK = ImmersalSDK.Instance;

            LoadSettings();
            signInButtton.onClick.AddListener(Login);

            immersalMapManager.OnLogin += ImmersalMapManager_OnLogin;
        }

        public async Task LoginAsync()
        {
            var emailAddress = emailAddressInputField.text.Trim();
            var password = passwordInputField.text.Trim();
            var serverUrl = serverUrlInputField.text.Trim();

            if (serverUrl != immersalSDK.defaultServerURL)
            {
                immersalSDK.localizationServer = serverUrl;
                PlayerPrefs.SetString(serverUrlInputFieldKey, serverUrl);
            }
            PlayerPrefs.Save();

            signInButtton.interactable = false;
            await immersalMapManager.LoginAsync(emailAddress, password);
            signInButtton.interactable = true;
        }

        private void LoadSettings()
        {
            var serverUrl = PlayerPrefs.GetString(serverUrlInputFieldKey, immersalSDK.defaultServerURL);
            serverUrlInputField.text = serverUrl;
        }

        private async void Login()
        {
            await LoginAsync();
        }

        private void ImmersalMapManager_OnLogin()
        {
            gameObject.SetActive(false);
        }
    }
}
