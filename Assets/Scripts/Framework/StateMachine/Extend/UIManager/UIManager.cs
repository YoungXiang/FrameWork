using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using FrameWork;
using System.Linq;

public class UILayer
{
    public const int Base = 0;
    /// <summary>
    /// Performant UI design requires a balance between minimizing the cost of rebuilds 
    /// and minimizing wasted draw calls
    /// So, it's important not to use too many layers (canvas) in order to balance draw calls with re-batches.
    /// </summary>
    public const int TopMost = 20;
}

public class UIManager : SingleBehaviour<UIManager>
{
    internal const string PrefabUrl = "Assets/Res/UI/Prefabs/{0}.prefab";
    
    #region Engine Callbacks
    void Awake()
    {
        gameObject.layer = LayerMask.NameToLayer("UI");
        GameObject eventSystemGo = GameObject.Find("EventSystem");
        if (eventSystemGo != null)
        {
            eventSystem = eventSystemGo.GetComponent<EventSystem>();
        }
        else
        {
            eventSystemGo = new GameObject("EventSystem");
            eventSystem = eventSystemGo.AddComponent<EventSystem>();
            eventSystemGo.AddComponent<StandaloneInputModule>();
        }
        eventSystemGo.transform.SetParent(transform);

        SetupBuiltingLayers();

        // NOTE: [Bug], when UnityEditor.MaterialEditor:OnDisable(), the sceneUnloaded delegate was triggered.
#if !UNITY_EDITOR
        UnityEngine.SceneManagement.SceneManager.sceneLoaded += OnSceneLoaded;
        UnityEngine.SceneManagement.SceneManager.sceneUnloaded += OnSceneUnloaded;
#endif
    }

    public override void OnDestroy()
    {
#if !UNITY_EDITOR
        UnityEngine.SceneManagement.SceneManager.sceneLoaded -= OnSceneLoaded;
        UnityEngine.SceneManagement.SceneManager.sceneUnloaded -= OnSceneUnloaded;
#endif
        base.OnDestroy();
    }

    void OnSceneLoaded(UnityEngine.SceneManagement.Scene scene, UnityEngine.SceneManagement.LoadSceneMode mode)
    {

    }

    void OnSceneUnloaded(UnityEngine.SceneManagement.Scene scene)
    {
        for (int i = 0; i < unloadWhenSceneUnloadList.Count; i++)
        {
            Unload(unloadWhenSceneUnloadList[i]);
        }

        unloadWhenSceneUnloadList.Clear();

        // check if ui canvas should be disabled
        foreach (KeyValuePair<int, GameObject> layer in uiLayers)
        {
            if (layer.Value.transform.childCount <= 0)
            {
                layer.Value.SetActive(false);
            }
        }
    }

#if UNITY_EDITOR
    float cachedResolutionX;
    float cachedResolutionY;
    void OnValidate()
    {
        if (cachedResolutionX != ResolutionWidth || cachedResolutionY != ResolutionHeight)
        {
            // change resolution
        }
    }
#endif
    #endregion

    #region Private Methods
    void SetupBuiltingLayers()
    {
    }
    
    void NewLayer(int layerLevel, string layerName)
    {
        GameObject layer = new GameObject(layerName);
        layer.transform.SetParent(transform, false);
        layer.layer = LayerMask.NameToLayer("UI");

        Canvas canvas = layer.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.pixelPerfect = true;
        
        CanvasScaler scaler = layer.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(ResolutionWidth, ResolutionHeight);
        scaler.matchWidthOrHeight = 1.0f;   // match height; 0 : match width
        scaler.referencePixelsPerUnit = 1;

        GraphicRaycaster ray = layer.AddComponent<GraphicRaycaster>();
        ray.ignoreReversedGraphics = true;

        // this part is tricky... must activate first and then set the sortingOrder.
        layer.SetActive(true);
        canvas.sortingOrder = layerLevel;

        uiLayers.Add(layerLevel, layer);
    }

    void SortSiblingUnderLayer(int layer)
    {
        int childCount = uiLayers[layer].transform.childCount;
        List<string> uiStateNames = new List<string>(childCount);
        for (int i = 0; i < childCount; i++)
        {
            uiStateNames.Add(uiLayers[layer].transform.GetChild(i).name);
        }

        // return 1 if a > b;
        //       -1 if a < b;
        uiStateNames.Sort((string a, string b) =>
        {
            return uiStates[a].sortingOrder - uiStates[b].sortingOrder;
        });

        for (int i = 0; i < childCount; i++)
        {
            uiStates[uiStateNames[i]].transform.SetSiblingIndex(childCount - i);
        }
    }
    #endregion

    #region Public API
    /// <summary>
    /// Instantiate ui prefab and show
    /// The main purpose of UIManager is to load the prefab.
    /// </summary>
    /// <param name="uiStateName">uiPrefab name should be the same as uiState name</param>
    public void Show(string uiStateName, string showState)
    {
        if (!uiStates.ContainsKey(uiStateName))
        {
            GameObject go = AssetManager.LoadPrefab(string.Format(PrefabUrl, uiStateName));
            // ensures that go has the same name as uiStateName
            go.name = uiStateName;
            UIStateMachine uiState = go.GetComponent<UIStateMachine>();
            if (uiState == null)
            {
                Debug.LogErrorFormat("{0} is not an UI State.", uiStateName);
                return;
            }
            uiStates.Add(uiStateName, uiState);
        }

        int layer = uiStates[uiStateName].layer;
        if (layer > UILayer.TopMost)
        {
            // error:
            Debug.LogErrorFormat("[Error]:Layer {0} is bigger than limit : {1}.", layer, UILayer.TopMost);
            return;
        }

        if (!uiLayers.ContainsKey(layer))
        {
            NewLayer(layer, string.Format("CustomLayer{0}", layer));
        }
        uiLayers[layer].SetActive(true);

        if (uiStates[uiStateName].unloadWhenSceneUnload)
        {
            unloadWhenSceneUnloadList.Add(uiStateName);
        }

        uiStates[uiStateName].transform.SetParent(uiLayers[layer].transform, false);
        if (uiStates[uiStateName].bringToFirst)
            uiStates[uiStateName].transform.SetAsLastSibling();
        else
        {
            uiStates[uiStateName].transform.SetSiblingIndex(uiStates[uiStateName].sortingOrder);
            if (uiLayers[layer].transform.childCount > 1) SortSiblingUnderLayer(layer);
        }

        if (!string.IsNullOrEmpty(showState))
            uiStates[uiStateName].ChangeState(showState);
    }

    public void Hide(string uiStateName, string hideState)
    {
        if (!uiStates.ContainsKey(uiStateName))
        {
            Debug.LogWarningFormat("{0} is not SHOWING.", uiStateName);
            return;
        }

        if (!string.IsNullOrEmpty(hideState))
            uiStates[uiStateName].ChangeState(hideState);
    }

    public void Unload(string uiStateName)
    {
        LogUtil.LogColor(LogUtil.Color.green, "UI Unload {0}", uiStateName);
        if (uiStates.ContainsKey(uiStateName))
        {
            Destroy(uiStates[uiStateName].gameObject);
            uiStates.Remove(uiStateName);
        }
    }

    public void SetActive(bool active)
    {
        gameObject.SetActive(active);
    }

    public void SetUIActive(string uiStateName, bool active)
    {
        if (uiStates.ContainsKey(uiStateName))
        {
            uiStates[uiStateName].gameObject.SetActive(active);
        }
    }

    public void SetLayerActive(int layer, bool active)
    {
        if (uiLayers.ContainsKey(layer))
        {
            uiLayers[layer].SetActive(active);
        }
    }
    #endregion

    /////////////////////////////////////////////////////////////////////////
    #region Public variables
    [Tooltip("Canvas Scaler - Reference Resolution (X)")]
    public float ResolutionWidth = 1536.0f;
    [Tooltip("Canvas Scaler - Reference Resolution (Y)")]
    public float ResolutionHeight = 2048.0f;

    [HideInInspector]
    public EventSystem eventSystem;
#endregion

    #region Private variables
    Dictionary<string, UIStateMachine> uiStates = new Dictionary<string, UIStateMachine>();
    List<string> unloadWhenSceneUnloadList = new List<string>();

    Dictionary<int, GameObject> uiLayers = new Dictionary<int, GameObject>();
    #endregion
}
