using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace HoloLab.PositioningTools.CoordinateSystem
{
    public class SpaceOrigin : MonoBehaviour
    {
#pragma warning disable CS0414
        [SerializeField]
        private bool customSpace = false;
#pragma warning restore CS0414

        [SerializeField]
        private string spaceType = "";

        /// <summary>
        /// The name of the system used for alignment (e.g. Marker, Immersal)
        /// 
        /// If this is an empty string, the alignment method is not specified.
        /// </summary>
        public string SpaceType => spaceType;

        [SerializeField]
        private string spaceId;

        public string SpaceId => spaceId;

        private Transform bindingTransform;

        public const string SpaceTypeAnySpace = "";
        public const string SpaceTypeMarker = "Marker";
        public const string SpaceTypeImmersal = "Immersal";
        public const string SpaceTypeVuforiaAreaTarget = "VuforiaAreaTarget";

        public void Initialize(string spaceType, string spaceId)
        {
            this.spaceType = spaceType;
            this.spaceId = spaceId;
        }

        private CoordinateManager spaceCoordinateManager;

        private void Start()
        {
            spaceCoordinateManager = CoordinateManager.Instance;
            spaceCoordinateManager.OnSpaceBound += OnSpaceBound;
            spaceCoordinateManager.OnSpaceLost += OnSpaceLost;
            gameObject.SetActive(false);

            var spaceBindingList = spaceCoordinateManager.SpaceBindingList;

            foreach (var spaceBinding in spaceBindingList)
            {
                OnSpaceBound(spaceBinding);
            }
        }

        private void Update()
        {
            if (bindingTransform != null)
            {
                transform.SetPositionAndRotation(bindingTransform.position, bindingTransform.rotation);
            }
        }

        private void OnDestroy()
        {
            spaceCoordinateManager.OnSpaceBound -= OnSpaceBound;
        }

        public void BindSpace(SpaceBinding spaceBinding)
        {
            if (BindingIsValid(spaceBinding))
            {
                // Transform is specified
                if (spaceBinding.Transform != null)
                {
                    bindingTransform = spaceBinding.Transform;
                    return;
                }

                // Pose is specified
                var pose = spaceBinding.Pose;
                if (pose.HasValue)
                {
                    transform.SetPositionAndRotation(pose.Value.position, pose.Value.rotation);
                }
            }
        }

        private bool BindingIsValid(SpaceBinding spaceBinding)
        {
            if (string.IsNullOrEmpty(spaceType) || string.Compare(spaceBinding.SpaceType, spaceType, true) == 0)
            {
                if (spaceBinding.SpaceId == SpaceId)
                {
                    return true;
                }
            }
            return false;
        }

        private void OnSpaceBound(SpaceBinding spaceBinding)
        {
            if (BindingIsValid(spaceBinding))
            {
                BindSpace(spaceBinding);
                gameObject.SetActive(true);
            }
        }

        private void OnSpaceLost(SpaceBinding spaceBinding)
        {
            if (BindingIsValid(spaceBinding))
            {
                bindingTransform = null;
                gameObject.SetActive(false);
            }
        }

#if UNITY_EDITOR
        [CustomEditor(typeof(SpaceOrigin))]
        protected class SpaceOriginEditor : Editor
        {
            private SerializedProperty spaceTypeProperty;
            private SerializedProperty customSpaceProperty;
            private SerializedProperty spaceIdProperty;

            protected static string[] spaceTypeOptions = new string[]
            {
                "AnySpace",
                "Marker",
                "Immersal",
                "VuforiaAreaTarget",
                "Custom"
            };

            private void OnEnable()
            {
                spaceTypeProperty = serializedObject.FindProperty(nameof(spaceType));
                customSpaceProperty = serializedObject.FindProperty(nameof(customSpace));
                spaceIdProperty = serializedObject.FindProperty(nameof(spaceId));
            }

            public override void OnInspectorGUI()
            {
                // base.OnInspectorGUI();

                serializedObject.Update();

                // Show dropdown to select SpaceType
                var previousSpaceTypeIndex = GetSpaceTypeOptionIndex(spaceTypeProperty.stringValue, customSpaceProperty.boolValue);
                var spaceTypeIndex = EditorGUILayout.Popup("Space Type", previousSpaceTypeIndex, spaceTypeOptions);

                var customSpaceTypeSelected = spaceTypeIndex == spaceTypeOptions.Length - 1;
                if (customSpaceTypeSelected)
                {
                    customSpaceProperty.boolValue = true;

                    // If "Custom" is selected newly, set "SpaceType" to ""
                    if (spaceTypeIndex != previousSpaceTypeIndex)
                    {
                        spaceTypeProperty.stringValue = "";
                    }
                }
                else
                {
                    customSpaceProperty.boolValue = false;
                }

                var spaceType = GetSpaceTypeString(spaceTypeIndex);
                if (spaceType != null)
                {
                    spaceTypeProperty.stringValue = spaceType;
                }

                // Show custom space type input field
                if (customSpaceProperty.boolValue)
                {
                    EditorGUILayout.PropertyField(spaceTypeProperty, new GUIContent("Custom Space Type"));
                }

                // Show space id input field
                EditorGUILayout.PropertyField(spaceIdProperty);

                serializedObject.ApplyModifiedProperties();
            }

            private static string GetSpaceTypeString(int optionIndex)
            {
                var selectedSpaceName = spaceTypeOptions[optionIndex];
                switch (selectedSpaceName)
                {
                    case "AnySpace":
                        return "";
                    case "Custom":
                        return null;
                    default:
                        return selectedSpaceName;
                }
            }

            private static int GetSpaceTypeOptionIndex(string currentSpaceType, bool isCustomSpace)
            {
                if (isCustomSpace)
                {
                    return spaceTypeOptions.Length - 1;
                }

                // AnySpace
                if (string.IsNullOrEmpty(currentSpaceType))
                {
                    return 0;
                }

                for (var i = 1; i < spaceTypeOptions.Length - 1; i++)
                {
                    if (currentSpaceType == spaceTypeOptions[i])
                    {
                        return i;
                    }
                }

                // Custom
                return spaceTypeOptions.Length - 1;
            }
        }
#endif
    }
}
