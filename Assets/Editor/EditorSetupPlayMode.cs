#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

[InitializeOnLoad]
public class EditorSetupPlayMode : MonoBehaviour
{
    // need to use PlayerPrefs because the script gets recreated on entering play mode
    const string CURRENT_SCENE_INDEX_KEY = "EDITOR_EditorSetupPlayMode_currentSceneIndex";
    const string CURRENT_SCENE_PATH_KEY = "EDITOR_EditorSetupPlayMode_currentSceneAssetPath";

    const int BASE_LEVEL_SCENE_INDEX = 0;

    static EditorSetupPlayMode()
    {
        // Set the BaseLevelScene as the default play scene
        EditorApplication.update += SetDefaultPlayModeScene; // defferes the calls one frame

        // Subscribe to the play mode state change event to load the current scene additionally
        EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
    }

    private static void SetDefaultPlayModeScene()
    {
        EditorApplication.update -= SetDefaultPlayModeScene;

        var pathOfFirstScene = EditorBuildSettings.scenes[BASE_LEVEL_SCENE_INDEX].path;
        var sceneAsset = AssetDatabase.LoadAssetAtPath<SceneAsset>(pathOfFirstScene);
        EditorSceneManager.playModeStartScene = sceneAsset;
    }

    private static void OnPlayModeStateChanged(PlayModeStateChange state)
    {
        int currentSceneIndex;
        switch (state)
        {
            case PlayModeStateChange.ExitingEditMode:
                var activeScene = EditorSceneManager.GetActiveScene();

                currentSceneIndex = activeScene.buildIndex;
                PlayerPrefs.SetInt(CURRENT_SCENE_INDEX_KEY, currentSceneIndex);

                if (currentSceneIndex == -1)
                {
                    var assetPath = activeScene.path;
                    PlayerPrefs.SetString(CURRENT_SCENE_PATH_KEY, assetPath);
                }

                EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
                break;

            case PlayModeStateChange.EnteredPlayMode:
                currentSceneIndex = PlayerPrefs.GetInt(CURRENT_SCENE_INDEX_KEY, -1);

                if (currentSceneIndex == -1)
                {
                    var assetPath = PlayerPrefs.GetString(CURRENT_SCENE_PATH_KEY, "");
                    if (assetPath == "") break;

                    EditorSceneManager.LoadSceneAsyncInPlayMode(assetPath, new LoadSceneParameters
                    {
                        loadSceneMode = LoadSceneMode.Additive,
                    });
                }
                else if (currentSceneIndex != BASE_LEVEL_SCENE_INDEX) // don't add the BaseLevelScene with index 0
                {
                    SceneManager.LoadScene(currentSceneIndex, LoadSceneMode.Additive);
                }
                break;
        }
    }
}
#endif