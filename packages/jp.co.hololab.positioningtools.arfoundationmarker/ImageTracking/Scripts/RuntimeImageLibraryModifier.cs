using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

namespace HoloLab.PositioningTools.ARFoundationMarker
{
    [Serializable]
    public class ReferenceImageAddedAtRuntime
    {
        public Texture2D Texture;
        public string Name;
    }

    [RequireComponent(typeof(ARTrackedImageManager))]
    public class RuntimeImageLibraryModifier : MonoBehaviour
    {
        [SerializeField]
        private List<ReferenceImageAddedAtRuntime> images = new List<ReferenceImageAddedAtRuntime>();

        private ARTrackedImageManager trackedImageManager;
        private MutableRuntimeReferenceImageLibrary mutableImageLibrary;

        private async void Start()
        {
            trackedImageManager = GetComponent<ARTrackedImageManager>();
            InitializeMutalbeImageLibrary();
            foreach (var image in images)
            {
                await AddImageAsync(image);
            }
        }

        public async Task AddImageAsync(ReferenceImageAddedAtRuntime image)
        {
            if (mutableImageLibrary == null)
            {
                Debug.LogWarning("MutableRuntimeReferenceImageLibrary is not supported in this platform");
                return;
            }

#if UNITY_IOS || UNITY_VISIONOS
            float? widthInMeters = 1f;
#else
            float? widthInMeters = null;
#endif

            while (true)
            {
                var jobState = mutableImageLibrary.ScheduleAddImageWithValidationJob(image.Texture, image.Name, widthInMeters);

                while (true)
                {
                    if (jobState.jobHandle.IsCompleted)
                    {
                        break;
                    }

                    await Task.Delay(100);
                    continue;
                }

                if (jobState.status == AddReferenceImageJobStatus.Success)
                {
                    return;
                }

                // Wait and retry job
                await Task.Delay(1000);
            }
        }

        private void InitializeMutalbeImageLibrary()
        {
            var mutableImageLibrarySupported = true;
            var referenceLibrary = trackedImageManager.referenceLibrary;
            if (referenceLibrary is XRReferenceImageLibrary xrReferenceImageLibrary)
            {
                try
                {
                    var runtimeLibrary = trackedImageManager.CreateRuntimeLibrary(xrReferenceImageLibrary);
                    if (runtimeLibrary is MutableRuntimeReferenceImageLibrary mutableRuntimeReferenceImageLibrary)
                    {
                        mutableImageLibrary = mutableRuntimeReferenceImageLibrary;
                    }
                    else
                    {
                        mutableImageLibrarySupported = false;
                    }
                }
                catch (Exception e)
                {
                    Debug.LogWarning(e);
                    return;
                }
            }
            else if (referenceLibrary is MutableRuntimeReferenceImageLibrary mutableRuntimeReferenceImageLibrary)
            {
                mutableImageLibrary = mutableRuntimeReferenceImageLibrary;
            }
            else
            {
                mutableImageLibrarySupported = false;
            }

            if (mutableImageLibrarySupported == false)
            {
                Debug.LogWarning("MutableRuntimeReferenceImageLibrary is not supported in this platform");
            }
        }
    }
}
