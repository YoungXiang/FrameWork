using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using FrameWork;

public class CameraLeft : BaseState<CameraStateMachine>
{
	public override string name { get { return "CameraLeft"; } }

	public override void OnEnter()
    {
        stateMachine.mainCamera.transform.position = stateMachine.left.position;
    }

	public override void OnUpdate()
	{
	}

	public override void OnExit()
	{
	}
}
