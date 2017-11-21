using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FrameWork;

public class Main : MonoBehaviour
{
    void Awake()
    {
        DontDestroyOnLoad(gameObject);
        main();
    }

    void Update()
    {
        KeyCodeManager.Instance.Update();
    }
    
    void OnEnable()
    {
        Application.logMessageReceived += HandleLog;
    }

    void OnDisable()
    {
        Application.logMessageReceived -= HandleLog;
    }

    void OnDestroy()
    {
        PlayerPrefs.Save();
    }

    void HandleLog(string logString, string stackTrace, LogType type)
    {
        if (type == LogType.Error || type == LogType.Exception)
        {
            Debug.Log("[Fatal Error Detected]");
            Debug.LogFormat("{0}\n{1}", logString, stackTrace);
#if !ENABLE_ERROR_PAUSE
            //UPEventReporter.Report("DebugError", logString, stackTrace);
#else
            //MessageManager.Instance.SendMessage(new MessageString(null, "DebugError", string.Format("{0}\n{1}",  logString, stackTrace)));
#endif
        }
    }

    // main
    void main()
    {
        Debug.Log(System.Environment.Version);

        KeyCodeManager.Instance.PushKeyCodeEvent(KeyCode.Escape, () =>
        {
            Application.Quit();
        });

        /// Do Initialization here        
        Coroutiner.Create();

        // Load ConfigDatas
        ConfDataLoader.Instance.Add(new SerializedData<CharacterConfigData>());
        ConfDataLoader.Instance.Load();

        // Init EntityWorld
        EntityWorldRegistry.Instance.Initialize();

        // Register Scenes
        SceneManager.Instance.RegisterScene<MainScene>("Main", "Assets/ExampleProject01/Res/_Scenes/MainScene.unity");

        // Load Scene
        SceneManager.Instance.LoadScene("Main");
    }
}
