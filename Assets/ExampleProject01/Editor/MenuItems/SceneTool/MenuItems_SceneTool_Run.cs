using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using FrameWork;

[InitializeOnLoad]
public class MenuItems_SceneTool_Run
{
    static MenuItems_SceneTool_Run()
    {
#if UNITY_2017
        EditorApplication.playModeStateChanged += EditorApplication_playModeStateChanged;
#else
        EditorApplication.playmodeStateChanged += EditorApplication_playModeStateChanged;
#endif
    }

    [MenuItem("SceneTool/Run %#r")]
    public static void Run()
    {
        if (!EditorApplication.isPlaying)
        {
            EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
            string sceneName = EditorSceneManager.GetActiveScene().name;
            if (sceneName != "Loading")
            {
                EditorPrefs.SetString("Editor_LastActiveScene", EditorSceneManager.GetActiveScene().path);
                EditorSceneManager.sceneOpened += EditorSceneManager_sceneOpened;                
                //EditorSceneManager.OpenScene("Assets/Res/_Scenes/Loading.unity", OpenSceneMode.Single);                
                EditorSceneManager.OpenScene(EditorBuildSettings.scenes[0].path, OpenSceneMode.Single);
            }
            else
            {
                EditorApplication.isPlaying = true;
            }

            EditorPrefs.SetBool("Editor_StartFromScript", true);
        }

        if (EditorApplication.isPlaying)
        {
            EditorApplication.isPlaying = false;
        }
    }

#if UNITY_2017
    private static void EditorApplication_playModeStateChanged(PlayModeStateChange obj)
    {
        if (obj == PlayModeStateChange.EnteredEditMode)
#else
    private static void EditorApplication_playModeStateChanged()
    {
        if (!EditorApplication.isPlaying)
#endif
        {
            StopRunning();
        }
    }

    private static void StopRunning()
    {
        // stopped
        if (EditorPrefs.GetBool("Editor_StartFromScript", false))
        {
            string scenePath = EditorSceneManager.GetActiveScene().path;
            string lastSaved = EditorPrefs.GetString("Editor_LastActiveScene", "");
            
            if (!string.IsNullOrEmpty(lastSaved) && scenePath != lastSaved)
            {
                EditorSceneManager.OpenScene(lastSaved);
            }

            EditorSceneManager.sceneOpened -= EditorSceneManager_sceneOpened;
            EditorPrefs.SetBool("Editor_StartFromScript", false);
        }
    }

    private static void EditorSceneManager_sceneOpened(UnityEngine.SceneManagement.Scene scene, OpenSceneMode mode)
    {
        EditorApplication.isPlaying = true;
    }
}
