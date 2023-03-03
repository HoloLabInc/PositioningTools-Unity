using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace HoloLab.PositioningTools
{
    public class GeographicLocationServiceComponent : GeographicLocationServiceComponentBase
    {
        [SerializeField]
        private bool autoStartService;

        [SerializeField]
        [HideInInspector]
        private Component geographicLocationServiceComponent = null;

        [SerializeField]
        [HideInInspector]
        [Tooltip("Unity Editor での実行時に別の Service を利用する")]
        private bool useDifferentServiceForEditor;

#pragma warning disable CS0414
        [SerializeField]
        [HideInInspector]
        private Component geographicLocationServiceComponentForEditor = null;
#pragma warning restore CS0414

        private async void Start()
        {
            if (autoStartService)
            {
                await StartServiceAsync();
            }
        }

        protected override IGeographicLocationService InitGeographicLocationService()
        {
#if UNITY_EDITOR
            var component = useDifferentServiceForEditor ? geographicLocationServiceComponentForEditor : geographicLocationServiceComponent;
#else
            var component = geographicLocationServiceComponent;
#endif
            if (component == null)
            {
                Debug.LogError($"GeographicLocationServiceComponent is null");
                return null;
            }
            return component.GetComponent<IGeographicLocationService>();
        }

#if UNITY_EDITOR
        [CustomEditor(typeof(GeographicLocationServiceComponent))]
        public class GeographicLocationServiceComponentEditor : Editor
        {
            private GeographicLocationServiceComponent _target;

            private SerializedProperty geographicLocationServiceComponentProperty;
            private SerializedProperty useDifferentServiceForEditorProperty;
            private SerializedProperty geographicLocationServiceComponentForEditorProperty;

            private void OnEnable()
            {
                _target = target as GeographicLocationServiceComponent;

                geographicLocationServiceComponentProperty
                    = serializedObject.FindProperty(nameof(_target.geographicLocationServiceComponent));
                useDifferentServiceForEditorProperty
                    = serializedObject.FindProperty(nameof(_target.useDifferentServiceForEditor));
                geographicLocationServiceComponentForEditorProperty
                    = serializedObject.FindProperty(nameof(_target.geographicLocationServiceComponentForEditor));
            }

            public override void OnInspectorGUI()
            {
                base.OnInspectorGUI();

                serializedObject.Update();

                EditorGUILayout.PropertyField(geographicLocationServiceComponentProperty);

                // If useDifferentServiceForEditor is checked, show geographicLocationServiceForEditor
                EditorGUILayout.PropertyField(useDifferentServiceForEditorProperty);
                if (_target.useDifferentServiceForEditor)
                {
                    EditorGUILayout.PropertyField(geographicLocationServiceComponentForEditorProperty);
                }

                serializedObject.ApplyModifiedProperties();
            }
        }
#endif
    }
}