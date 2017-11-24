using System;
using System.Collections.Generic;
using UnityEngine;
using FrameWork;

// a little bit like share data.
public class GameData : Singleton<GameData>
{
    private DataListener gameData;

    public object this[string key]
    {
        get
        {
            return gameData[key];
        }
        set
        {
            gameData[key] = value;
        }
    }

    public override void Init()
    {
        // game res
        gameData.RegisterAttr("population", 0);
        gameData.RegisterAttr("wood", 0);
        gameData.RegisterAttr("iron", 0);
        gameData.RegisterAttr("diamond", 0);

        // user info
        gameData.RegisterAttr("level", 1);
        gameData.RegisterAttr("waveTotal", 0);

    }

    public void RegisterEvent<T>(string key, Action<T> onValueChanged)
    {
        gameData.RegisterEvent(key, onValueChanged);
    }

    public void UnRegisterEvent<T>(string key, Action<T> onValueChanged)
    {
        gameData.UnRegisterEvent(key, onValueChanged);
    }

    public void Load()
    {
        // load game data;

    }

    public void Save()
    {

    }
}
