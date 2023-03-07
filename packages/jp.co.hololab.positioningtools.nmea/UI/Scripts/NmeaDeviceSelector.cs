using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace HoloLab.PositioningTools.Nmea
{
    public class NmeaDeviceSelector : MonoBehaviour
    {
        [SerializeField]
        private NmeaDeviceServiceComponent nmeaDeviceService = null;

        [SerializeField]
        private Dropdown dropdown = null;

        [SerializeField]
        private Button button = null;

        private Text buttonText;

        private bool serviceIsRunning;

        private IReadOnlyList<NmeaDeviceInfo> nmeaDevices;

        private void Start()
        {
            buttonText = button.GetComponentInChildren<Text>();
            RefreshDeviceList();
            button.onClick.AddListener(Button_OnClick);
        }

        private void OnDestroy()
        {
            button.onClick.RemoveListener(Button_OnClick);
        }

        private void RefreshDeviceList()
        {
            nmeaDevices = nmeaDeviceService.GetDeviceList();

            dropdown.ClearOptions();
            foreach (var nmeaDevice in nmeaDevices)
            {
                dropdown.options.Add(new Dropdown.OptionData(nmeaDevice.DisplayName));
            }

            dropdown.RefreshShownValue();
        }

        private async void Button_OnClick()
        {
            if (serviceIsRunning)
            {
                await StopService();
            }
            else
            {
                await StartService();
            }
        }

        private async Task StartService()
        {
            var index = dropdown.value;
            if (0 > index || index >= nmeaDevices.Count)
            {
                return;
            }

            var selectedDevice = nmeaDevices[index];
            nmeaDeviceService.SelectDevice(selectedDevice);

            button.interactable = false;
            await nmeaDeviceService.StartServiceAsync();

            button.interactable = true;
            buttonText.text = "Stop";

            serviceIsRunning = true;
        }

        private async Task StopService()
        {
            button.interactable = false;
            await nmeaDeviceService.StopServiceAsync();

            button.interactable = true;
            buttonText.text = "Start";

            serviceIsRunning = false;
        }
    }
}