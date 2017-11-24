using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using FrameWork;

public class BattleStarted : BaseState<BattleStateMachine>
{
	public override string name { get { return "BattleStarted"; } }

	public override void OnEnter()
	{

	}

	public override void OnUpdate()
	{
        stateMachine.waveManager.Update();
	}

	public override void OnExit()
	{
	}
}
