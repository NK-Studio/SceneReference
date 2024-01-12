using UnityEngine;
using UnityEditor.UIElements;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

namespace UnityEditor
{
    [CustomPropertyDrawer(typeof(SceneReference))]
    class SceneReferencePropertyDrawer : PropertyDrawer
    {
        private SerializedProperty _isDirtyProperty;
        private SerializedProperty _sceneAssetProperty;
        private VisualElement _root;
        private PropertyField _sceneAssetField;
        private HelpBox _helpBox;

        private void FindProperty(SerializedProperty property)
        {
            _sceneAssetProperty = property.FindPropertyRelative("sceneAsset");
        }

        private void InitializeRoot()
        {
            _root = new VisualElement();
            _sceneAssetField = new PropertyField(_sceneAssetProperty);

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
            InitializeRoot();

            _root.schedule.Execute(() => {

                bool hadReference = _sceneAssetProperty.objectReferenceValue != null;
                
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
            }).Every(100);

            return _root;
        }
        
        private bool IsValidSceneAsset(SceneAsset asset)
        {
            if (asset == null) return false;
            return SceneUtility.GetBuildIndexByScenePath(AssetDatabase.GetAssetPath(asset)) != -1;
        }
    }
}