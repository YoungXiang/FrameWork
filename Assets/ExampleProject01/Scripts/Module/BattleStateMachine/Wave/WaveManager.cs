using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FrameWork;


public class WaveManager
{
    WaveConfigData waveConfigData;
    public Wave currentWave;
    
    public void LoadNextWave()
    {
        waveConfigData = ConfDataLoader.Instance.GetData<WaveConfigData>();

        int level = (int)GameData.Instance["level"];

        currentWave = new Wave(waveConfigData[level]);
    }

    public void Update()
    {
        currentWave.Update();
        if (currentWave.IsWaveFinished())
        {
            currentWave.Clear();

        }
    }

    public void GenerateEnemy(int id)
    {
        CharacterConfig charInfo = EntityWorldRegistry.Instance.charConfigs[id];
        EntityWorldRegistry.Instance.entityWorld.CreateEntityFromTemplate(charInfo.template, charInfo);
    }
}
