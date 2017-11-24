using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using FrameWork;

public class CameraStateMachine : MonoStateMachine 
{
    public Camera mainCamera;
    public Transform left;
    public Transform right;
	///
	/// initialState is set in Start()
	///
    public override string initialStateName { get { return "CameraRight"; } }

	///
	/// Use this for Initialization
	///
    public override void Awake()
	{
		// Register all states.
		RegisterState(new CameraLeft());
		RegisterState(new CameraRight());

		// Use MonoStateMachine.DispatchStateChange("GameObjectName", "StateName"); to change state.
	}
}
