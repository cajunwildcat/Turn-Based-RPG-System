using UnityEditor;
using UnityEditor.SceneManagement;

namespace com.cajunwildcat.saveSceneOnPlay.Editor{
[InitializeOnLoad]
    public static class AutoSaveSceneOnPlay {
        static AutoSaveSceneOnPlay() {
            EditorApplication.playModeStateChanged += PlayModeStateChanged;
        }

        private static void PlayModeStateChanged(PlayModeStateChange state) {
            if (state == PlayModeStateChange.ExitingEditMode) {
                EditorSceneManager.SaveOpenScenes();
                AssetDatabase.SaveAssets();
            }
        }
    }
}