using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if VUFORIA_PRESENT
using Vuforia;
#endif

#if UNITY_EDITOR
using System.IO;
using UnityEditor;
#endif

namespace HoloLab.PositioningTools.Vuforia
{
    [ExecuteAlways]
    public class VuforiaLicenseKeyLoader : MonoBehaviour
    {
        [SerializeField]
        bool initializeVuforia = true;

        private const string assetLoadPath = "VuforiaLicenseKeySettings";
        private static string AssetFilePathFromProjectRoot => $"Assets/Resources/{assetLoadPath}.asset";

        private void Awake()
        {
            if (Application.isPlaying)
            {
                LoadLicenseKey(initializeVuforia);
            }
            else
            {
                CreateSettingsAsset();
            }
        }

        private static void LoadLicenseKey(bool initializeVuforia)
        {
            var settings = Resources.Load<VuforiaLicenseKeySettings>(assetLoadPath);

            if (settings == null)
            {
                Debug.LogError($"{assetLoadPath} not found");
                return;
            }

            var licenseKey = settings.LicenseKey;
            SetLicenseKey(licenseKey, initializeVuforia);
        }

        private static void SetLicenseKey(string licenseKey, bool initializeVuforia)
        {
#if VUFORIA_PRESENT
            var vuforia = VuforiaConfiguration.Instance.Vuforia;
            vuforia.LicenseKey = licenseKey;

            if (vuforia.DelayedInitialization)
            {
                if (initializeVuforia)
                {
                    VuforiaApplication.Instance.Initialize();
                }
            }
            else
            {
                Debug.LogError("Delayed Initialization should be enabled in VuforiaConfiguration");
            }
#else
            Debug.LogWarning("Vuforia Engine is not installed");
#endif
        }

        private static void CreateSettingsAsset()
        {
#if UNITY_EDITOR
            if (File.Exists(AssetFilePathFromProjectRoot))
            {
                return;
            }

            var asset = ScriptableObject.CreateInstance<VuforiaLicenseKeySettings>();
            AssetDatabase.CreateAsset(asset, AssetFilePathFromProjectRoot);
            AssetDatabase.SaveAssets();
#endif
        }
    }
}
