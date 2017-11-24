using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using FrameWork;

public class UITopBar : UIStateMachine 
{
	///
	/// initialState is set in Start()
	///
    public override string initialStateName { get { return "UITopBarIdle"; } }

	///
	/// Use this for Initialization
	///
    public override void Awake()
	{
		// Register all states.
		RegisterState(new UITopBarIdle());

		// Use MonoStateMachine.DispatchStateChange("GameObjectName", "StateName"); to change state.
	}
}
