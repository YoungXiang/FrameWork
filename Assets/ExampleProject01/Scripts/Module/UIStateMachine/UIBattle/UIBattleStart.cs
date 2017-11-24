using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using FrameWork;

public class UIBattleStart : BaseState<UIBattle>
{
	public override string name { get { return "UIBattleStart"; } }

	public override void OnEnter()
    {
        stateMachine.battleStart.gameObject.SetActive(false);

        // move camera to left
        MonoStateMachine.DispatchStateChange("CameraRoot", "CameraLeft");
        // start battle
        MonoStateMachine.DispatchStateChange("BattleStateMachine", "BattleStarted");
    }

	public override void OnUpdate()
	{
	}

	public override void OnExit()
	{
	}
}
