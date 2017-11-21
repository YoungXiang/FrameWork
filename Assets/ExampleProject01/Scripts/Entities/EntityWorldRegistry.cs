using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Artemis;
using FrameWork;

// Responsible for Initializing the EntityWolrd.
public class EntityWorldRegistry : SingleBehaviour<EntityWorldRegistry>
{
    [System.NonSerialized]
    public EntityWorld entityWorld;

    [System.NonSerialized]
    public CharacterConfigData charConfigs;
    	
	// Update is called once per frame
	void Update ()
    {
        entityWorld.Update();
	}

    public void Initialize()
    {
        entityWorld = new EntityWorld(false, true, true);
        charConfigs = ConfDataLoader.Instance.GetData<CharacterConfigData>();
    }
}
