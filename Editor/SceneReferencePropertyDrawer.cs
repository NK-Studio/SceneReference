using UnityEngine;
using UnityEngine.SceneManagement;

#if UNITY_2021_3 || ODIN_INSPECTOR || USE_MYBOX

#else
using UnityEditor.UIElements;
using UnityEngine.UIElements;
#endif

namespace UnityEditor
{
    [CustomPropertyDrawer(typeof(SceneReference))]
    class SceneReferencePropertyDrawer : PropertyDrawer
    {
#if UNITY_2021_3 || ODIN_INSPECTOR || USE_MYBOX
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);
            
            SerializedProperty isDirtyProperty = property.FindPropertyRelative("isDirty");

            if (isDirtyProperty.boolValue)
                isDirtyProperty.boolValue = false;

            var sceneAssetProperty = property.FindPropertyRelative("sceneAsset");
            bool hadReference = sceneAssetProperty.objectReferenceValue != null;
            
            var assetFieldPosition = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
            EditorGUI.PropertyField(assetFieldPosition, sceneAssetProperty, new GUIContent("Scene Asset"));

            position.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing; // Move position on for the
            
            string msg = Application.systemLanguage switch {
                SystemLanguage.Korean => "해당 SceneAsset은 빌드 설정에 포함되지 않았습니다.\n제안 : 빌드 세팅에 씬을 추가해주세요.",
                _ => "The SceneAsset is not included in the build settings.\nSuggestion : Add the scene to the build settings."
            };
            
            if (hadReference)
            {
                SceneAsset sceneAsset = sceneAssetProperty.objectReferenceValue as SceneAsset;
                if (sceneAsset != null)
                {
                    if (!IsValidSceneAsset(sceneAsset))
                    {
                        var helpBoxPosition = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight * 2);
                        EditorGUI.HelpBox(helpBoxPosition, msg, MessageType.Error);
                        position.y += EditorGUIUtility.singleLineHeight * 2 + EditorGUIUtility.standardVerticalSpacing; // Adjust position for the next controls
                    }
                }
            }
            
            if (!sceneAssetProperty.objectReferenceValue)
                if (hadReference)
                    property.FindPropertyRelative("path").stringValue = string.Empty;

            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            float propertyHeight = EditorGUIUtility.singleLineHeight;
            SerializedProperty sceneAssetProperty = property.FindPropertyRelative("sceneAsset");
            bool hadReference = sceneAssetProperty.objectReferenceValue != null;

            if (hadReference)
            {
                SceneAsset sceneAsset = sceneAssetProperty.objectReferenceValue as SceneAsset;
                if (sceneAsset != null && !IsValidSceneAsset(sceneAsset))
                {
                    propertyHeight += (EditorGUIUtility.singleLineHeight * 2 + EditorGUIUtility.standardVerticalSpacing);
                }
            }

            return propertyHeight + EditorGUIUtility.standardVerticalSpacing;
        }

#else
        private SerializedProperty _isDirtyProperty;
        private SerializedProperty _sceneAssetProperty;
        private VisualElement _root;
        private PropertyField _sceneAssetField;
        private HelpBox _helpBox;

        private void FindProperty(SerializedProperty property)
        {
            _sceneAssetProperty = property.FindPropertyRelative("sceneAsset");
        }

        private void InitializeRoot(SerializedProperty property)
        {
            _root = new VisualElement();
            _sceneAssetField = new PropertyField(_sceneAssetProperty);
            _sceneAssetField.label = property.name; 
            
            string msg = Application.systemLanguage switch {
                SystemLanguage.Korean => "해당 SceneAsset은 빌드 설정에 포함되지 않았습니다.\n제안 : 빌드 세팅에 씬을 추가해주세요.",
                _ => "The SceneAsset is not included in the build settings.\nSuggestion : Add the scene to the build settings."
            };
            _helpBox = new HelpBox(msg, HelpBoxMessageType.Error);
            _helpBox.style.display = DisplayStyle.None;

            _root.Add(_sceneAssetField);
            _root.Add(_helpBox);
        }

        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            FindProperty(property);
            InitializeRoot(property);

            _root.TrackPropertyValue(_sceneAssetProperty, newValue =>
            {
                bool hadReference = newValue.objectReferenceValue != null;

                if (hadReference)
                {
                    SceneAsset sceneAsset = _sceneAssetProperty.objectReferenceValue as SceneAsset;
                    if (IsValidSceneAsset(sceneAsset))
                        _helpBox.style.display = DisplayStyle.None;
                    else
                        _helpBox.style.display = DisplayStyle.Flex;
                }
                else
                    _helpBox.style.display = DisplayStyle.None;
            });
            
            return _root;
        }
#endif
        private bool IsValidSceneAsset(SceneAsset asset)
        {
            if (asset == null) return false;
            return SceneUtility.GetBuildIndexByScenePath(AssetDatabase.GetAssetPath(asset)) != -1;
        }
    }
}
