using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FrameWork;


public class MainScene : UnityScene
{
    public override void OnLoaded()
    {
        //EntityWorldRegistry.Instance.entityWorld.CreateEntityFromTemplate("Character", 0);
        //EntityWorldRegistry.Instance.entityWorld.CreateEntityFromTemplate("Character", 1);

        UIManager.Instance.Show("UITopBar", "UITopBarIdle");
        UIManager.Instance.Show("UIBattle", "UIBattleIdle");
    }
}
