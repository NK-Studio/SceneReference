using System;

#if UNITY_EDITOR
using UnityEditor;
#if UNITY_2021_3 || ODIN_INSPECTOR
using UnityEditor.SceneManagement;
#endif
#endif

namespace UnityEngine
{
    /// <summary>
    /// 런타임에서 사용할 수 있도록 씬 에셋에 대한 참조를 제공합니다.
    /// </summary>
    [Serializable]
    public class SceneReference : ISerializationCallbackReceiver
    {
#if UNITY_EDITOR
        // 에디터에서 사용되는 자산에 대한 참조. Player 빌드는 SceneAsset을 인식하지 못합니다.
        // 씬 경로를 업데이트하는 데 사용됩니다.
        [SerializeField]
        private SceneAsset sceneAsset;
#endif

        // Player 빌드는 여기에 저장된 경로를 사용할 것입니다. 에디터나 빌드 도중에 업데이트해야 합니다.
        // 씬이 삭제되면 경로는 그대로 유지됩니다.
        [SerializeField]
        private string path = string.Empty;

#if UNITY_2021_3 || ODIN_INSPECTOR
#if UNITY_EDITOR
#pragma warning disable 0414 // Never used warning - will be used by SerializedProperty.
        // Used to dirtify the data when needed upon displaying in the inspector.
        // Otherwise the user will never get the changes to save (unless he changes any other field of the object / scene).
        [SerializeField]
        private bool isDirty;
#pragma warning restore 0414
#endif
#else
#endif

        /// <summary>
        /// <see cref="UnityEngine.SceneManagement.SceneManager"/> API에서 사용할 씬 경로를 반환합니다.
        /// 에디터 내에서는 이 경로가 항상 최신 상태로 유지됩니다(씬이 이동되거나 이름이 변경된 경우).
        /// 참조하는 씬 자산이 삭제되면 경로는 그대로 유지됩니다.
        /// </summary>
        public string Path
        {
            get
            {
#if UNITY_EDITOR
                AutoUpdateReference();
#endif

                return path;
            }

            set
            {
                path = value;

#if UNITY_EDITOR
                if (string.IsNullOrEmpty(path))
                {
                    sceneAsset = null;
                    return;
                }

                sceneAsset = AssetDatabase.LoadAssetAtPath<SceneAsset>(path);
                if (sceneAsset == null)
                {
                    Debug.LogError(
                        Application.systemLanguage == SystemLanguage.Korean
                            ? $"SceneReference에 {nameof(SceneAsset)}를 설정했지만, 해당 경로에 씬이 없습니다."
                            : $"Setting {nameof(SceneAsset)} to {value}, but no scene could be located there.");
                }
#endif
            }
        }

        /// <summary>
        /// 확장자 없는 씬의 이름을 반환합니다.
        /// </summary>
        public string Name => System.IO.Path.GetFileNameWithoutExtension(Path);

        public bool IsEmpty => string.IsNullOrEmpty(Path);

        public SceneReference()
        {
        }

        public SceneReference(string assetPath)
        {
            Path = assetPath;
        }

        public SceneReference(SceneReference other)
        {
            path = other.path;

#if UNITY_EDITOR
            sceneAsset = other.sceneAsset;

            AutoUpdateReference();
#endif
        }

        public SceneReference Clone() => new SceneReference(this);

        public override string ToString()
        {
            return path;
        }

        [Obsolete("에디터에 필요하며, 런타임 코드에서는 사용하지 마십시오!", true)]
        public void OnBeforeSerialize()
        {
#if UNITY_EDITOR
            AutoUpdateReference();
#endif
        }

        [Obsolete("에디터에 필요하며, 런타임 코드에서는 사용하지 마십시오!", true)]
        public void OnAfterDeserialize()
        {
#if UNITY_EDITOR
            // OnAfterDeserialize는 직렬화 스레드에서 호출되므로 Unity API에 접근할 수 없습니다.
            // 다음 업데이트 프레임을 기다려서 처리합니다.
            EditorApplication.update += OnAfterDeserializeHandler;
#endif
        }

#if UNITY_EDITOR
        private void OnAfterDeserializeHandler()
        {
            EditorApplication.update -= OnAfterDeserializeHandler;
            AutoUpdateReference();
        }

        private void AutoUpdateReference()
        {
            if (sceneAsset == null)
            {
                 if (!string.IsNullOrEmpty(path))
                     path = string.Empty;
            }
            else
            {
                string foundPath = AssetDatabase.GetAssetPath(sceneAsset);
                if (string.IsNullOrEmpty(foundPath))
                    return;

#if UNITY_2021_3 || ODIN_INSPECTOR
                if (path != foundPath)
                {
                    path = foundPath;
                    isDirty = true;

                    if (!Application.isPlaying)
                    {
                        // NOTE: This doesn't work for scriptable objects, hence the m_IsDirty.
                        EditorSceneManager.MarkAllScenesDirty();
                    }
                }
#else
                if (path != foundPath)
                    path = foundPath;
#endif
            }
        }
#endif
    }
}
