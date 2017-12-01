using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FrameWork;
using System;

/// <summary>
/// A simple AI brain should make simple decisions and take less time to compute the conditions.
/// </summary>
public class AISimpleBrain : AIBrain
{
    protected override int FindNextBehaviour()
    {
        if (cur >= 0)
        {
            if (behaviours[cur].isRunning)
            {

            }
        }

        return 0;
    }
}
