using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FrameWork;

public struct SkillArg
{
    public int targetId;
    public Vector3 targetPos;
    public Vector3 targetDir;
}

// base skill module
// mostly responsible for vfx and sound
public class SkillBase
{
    public bool isRunning;

    public SkillBase(WeaponConfig wc)
    {

    }

    // skill start
    public virtual void Use()
    {
        
    }

    public virtual void Update()
    {

    }

    // skill finish
    public virtual void Finish()
    {

    }
}
