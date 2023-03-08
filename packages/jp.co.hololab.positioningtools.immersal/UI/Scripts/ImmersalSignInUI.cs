using Immersal;
using System;
using System.Collections;
using System.Collections.Generic;
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

        private const string emailAddressInputFieldKey = "ImmersalSignInUI_emailAddressInputField";
        private const string passwordInputFieldKey = "ImmersalSignInUI_passwordInputField";
        private const string serverUrlInputFieldKey = "ImmersalSignInUI_serverUrlInputField";

        private ImmersalSDK immersalSDK;

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

        private void LoadSettings()
        {
            var emailAddress = PlayerPrefs.GetString(emailAddressInputFieldKey);
            emailAddressInputField.text = emailAddress;

            var password = PlayerPrefs.GetString(passwordInputFieldKey);
            passwordInputField.text = password;

            var serverUrl = PlayerPrefs.GetString(serverUrlInputFieldKey, ImmersalSDK.DefaultServer);
            serverUrlInputField.text = serverUrl;
        }

        private async void Login()
        {
            var emailAddress = emailAddressInputField.text.Trim();
            var password = passwordInputField.text.Trim();
            var serverUrl = serverUrlInputField.text.Trim();

            PlayerPrefs.SetString(emailAddressInputFieldKey, emailAddress);
            PlayerPrefs.SetString(passwordInputFieldKey, password);

            if (serverUrl != ImmersalSDK.DefaultServer)
            {
                immersalSDK.localizationServer = serverUrl;
                PlayerPrefs.SetString(serverUrlInputFieldKey, serverUrl);
            }
            PlayerPrefs.Save();

            signInButtton.interactable = false;
            await immersalMapManager.LoginAsync(emailAddress, password);
            signInButtton.interactable = true;
        }

        private void ImmersalMapManager_OnLogin()
        {
            gameObject.SetActive(false);
        }
    }
}
