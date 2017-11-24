using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using FrameWork;

public class CameraRight : BaseState<CameraStateMachine>
{
	public override string name { get { return "CameraRight"; } }

	public override void OnEnter()
	{
        stateMachine.mainCamera.transform.position = stateMachine.right.position;
	}

	public override void OnUpdate()
	{
	}

	public override void OnExit()
	{
	}
}
