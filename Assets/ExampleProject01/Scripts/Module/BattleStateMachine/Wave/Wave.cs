using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FrameWork;
using Artemis;

public class Wave
{
    public float waveTime;
    WaveConfig waveConfig;
    public List<Entity> entityCreated;

    public Wave(WaveConfig config)
    {
        waveConfig = config;
        waveTime = 0.0f;
    }

    public void Update()
    {
        waveTime += Time.deltaTime;
    }

    public bool IsWaveFinished()
    {
        return false;
    }

    public void Clear()
    {

    }
}
