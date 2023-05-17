#if UNITY_EDITOR
using UnityEditor;

namespace UnityEngine
{
    [CustomPropertyDrawer(typeof(SceneReference)), CanEditMultipleObjects]
    internal class SceneReferencePropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            SerializedProperty isDirtyProperty = property.FindPropertyRelative("m_IsDirty");

            if (isDirtyProperty.boolValue)
                isDirtyProperty.boolValue = false;

            EditorGUI.BeginProperty(position, label, property);
            Rect fieldRect = EditorGUI.PrefixLabel(position, label);

            var sceneAssetProperty = property.FindPropertyRelative("m_SceneAsset");
            bool hadReference = sceneAssetProperty.objectReferenceValue != null;

            EditorGUI.PropertyField(fieldRect, sceneAssetProperty, new GUIContent());
            
            if (!sceneAssetProperty.objectReferenceValue)
                if (hadReference)
                    property.FindPropertyRelative("m_ScenePath").stringValue = string.Empty;

            EditorGUI.EndProperty();
        }
    }
}
#endif