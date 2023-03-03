using System;
using UnityEngine;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;

#endif

namespace HoloLab.PositioningTools.CoordinateSystem
{
    public class SpaceBinder : MonoBehaviour
    {
        private enum RuntimeType
        {
            None = 0,
            Editor = 1,
            Player = 2,
            EditorAndPlayer = 3,
        }

#pragma warning disable CS0414
        [SerializeField]
        private bool customSpace = false;
#pragma warning restore CS0414

        [SerializeField]
        private string spaceType = "";

        [SerializeField]
        private string spaceId;

        [SerializeField]
        private RuntimeType runtimeType = RuntimeType.Editor;

        private void Start()
        {
            if (IsBindingValid())
            {
                var spaceBinding = new SpaceBinding(transform, spaceType, spaceId);
                var manager = CoordinateManager.Instance;
                manager.BindSpace(spaceBinding);
            }
        }

        private bool IsBindingValid()
        {
#if UNITY_EDITOR
            switch (runtimeType)
            {
                case RuntimeType.Editor:
                case RuntimeType.EditorAndPlayer:
                    return true;
                default:
                    return false;
            }
#else
            switch (runtimeType)
            {
                case RuntimeType.Player:
                case RuntimeType.EditorAndPlayer:
                    return true;
                default:
                    return false;
            }
#endif
        }

#if UNITY_EDITOR
        [CustomEditor(typeof(SpaceBinder))]
        protected class SpaceBinderEditor : Editor
        {
            private SerializedProperty spaceTypeProperty;
            private SerializedProperty customSpaceProperty;
            private SerializedProperty spaceIdProperty;
            private SerializedProperty runtimeTypeProperty;

            protected static string[] spaceTypeOptions = new string[]
            {
                "Marker",
                "Immersal",
                "VuforiaAreaTarget",
                "Custom"
            };

            protected static string[] runtimeTypeOptions = new string[]
            {
                "None",
                "Editor",
                "Player",
                "Editor and Player"
            };

            private void OnEnable()
            {
                spaceTypeProperty = serializedObject.FindProperty(nameof(spaceType));
                customSpaceProperty = serializedObject.FindProperty(nameof(customSpace));
                spaceIdProperty = serializedObject.FindProperty(nameof(spaceId));
                runtimeTypeProperty = serializedObject.FindProperty(nameof(runtimeType));
            }

            public override void OnInspectorGUI()
            {
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

                // Show runtime type select box
                runtimeTypeProperty.enumValueIndex = EditorGUILayout.Popup("Runtime Type", runtimeTypeProperty.enumValueIndex, runtimeTypeOptions);

                serializedObject.ApplyModifiedProperties();
            }

            private static string GetSpaceTypeString(int optionIndex)
            {
                var selectedSpaceName = spaceTypeOptions[optionIndex];
                switch (selectedSpaceName)
                {
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

                for (var i = 0; i < spaceTypeOptions.Length - 1; i++)
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