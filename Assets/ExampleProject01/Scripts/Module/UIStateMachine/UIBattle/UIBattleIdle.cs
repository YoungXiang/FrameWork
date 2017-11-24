using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using FrameWork;

public class UIBattleIdle : BaseState<UIBattle>
{
	public override string name { get { return "UIBattleIdle"; } }

	public override void OnEnter()
    {
        stateMachine.battleStart.gameObject.SetActive(true);
    }

	public override void OnUpdate()
	{
	}

	public override void OnExit()
	{
	}
}
