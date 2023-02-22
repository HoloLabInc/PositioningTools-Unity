using System;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace HoloLab.PositioningTools
{
    public class CardinalDirectionServiceComponent : CardinalDirectionServiceComponentBase
    {
        [SerializeField]
        private bool autoStartService;

        [SerializeField]
        [HideInInspector]
        private Component cardinalDirectionServiceComponent = null;

        [SerializeField]
        [HideInInspector]
        private bool useDifferentServiceForEditor;

        [SerializeField]
        [HideInInspector]
        private Component cardinalDirectionServiceComponentForEditor = null;

        private async void Start()
        {
            if (autoStartService)
            {
                await StartServiceAsync();
            }
        }

        protected override ICardinalDirectionService InitCardinalDirectionService()
        {
#if UNITY_EDITOR
            var component = useDifferentServiceForEditor ? cardinalDirectionServiceComponentForEditor : cardinalDirectionServiceComponent;
#else
            var component = cardinalDirectionServiceComponent;
#endif
            if (component == null)
            {
                Debug.LogError($"CardinalDirectionServiceComponent is null");
                return null;
            }
            return component.GetComponent<ICardinalDirectionService>();
        }

#if UNITY_EDITOR
        [CustomEditor(typeof(CardinalDirectionServiceComponent))]
        public class CardinalDirectionServiceComponentEditor : Editor
        {
            private CardinalDirectionServiceComponent _target;

            private SerializedProperty cardinalDirectionServiceComponentProperty;
            private SerializedProperty useDifferentServiceForEditorProperty;
            private SerializedProperty cardinalDirectionServiceComponentForEditorProperty;

            private void OnEnable()
            {
                _target = target as CardinalDirectionServiceComponent;

                cardinalDirectionServiceComponentProperty
                    = serializedObject.FindProperty(nameof(_target.cardinalDirectionServiceComponent));
                useDifferentServiceForEditorProperty
                    = serializedObject.FindProperty(nameof(_target.useDifferentServiceForEditor));
                cardinalDirectionServiceComponentForEditorProperty
                    = serializedObject.FindProperty(nameof(_target.cardinalDirectionServiceComponentForEditor));
            }

            public override void OnInspectorGUI()
            {
                base.OnInspectorGUI();

                serializedObject.Update();

                EditorGUILayout.PropertyField(cardinalDirectionServiceComponentProperty);

                // If useDifferentServiceForEditor is checked, show cardinalDirectionServiceComponentForEditor.
                EditorGUILayout.PropertyField(useDifferentServiceForEditorProperty);
                if (_target.useDifferentServiceForEditor)
                {
                    EditorGUILayout.PropertyField(cardinalDirectionServiceComponentForEditorProperty);
                }

                serializedObject.ApplyModifiedProperties();
            }
        }
#endif
    }
}