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

    // Grid world
    [System.NonSerialized]
    public World gridWorld;

    #region Global data configs
    [System.NonSerialized]
    public CharacterConfigData charConfigs;
    [System.NonSerialized]
    public WeaponConfigData weaponConfigs;
    #endregion

    // Update is called once per frame
    void Update ()
    {
        entityWorld.Update();
	}

    public void Initialize()
    {
        entityWorld = new EntityWorld(false, true, true);
        charConfigs = ConfDataLoader.Instance.GetData<CharacterConfigData>();
        weaponConfigs = ConfDataLoader.Instance.GetData<WeaponConfigData>();
    }
}
