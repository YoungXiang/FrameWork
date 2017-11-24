using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using FrameWork;

public class BattleStateMachine : MonoStateMachine 
{
    internal WaveManager waveManager;

	///
	/// initialState is set in Start()
	///
    public override string initialStateName { get { return "BattleIdleState"; } }

	///
	/// Use this for Initialization
	///
    public override void Awake()
	{
        waveManager = new WaveManager();

		// Register all states.
		RegisterState(new BattleIdleState());
		RegisterState(new BattleStarted());
		RegisterState(new BattleStopped());
		RegisterState(new BattlePaused());

		// Use MonoStateMachine.DispatchStateChange("GameObjectName", "StateName"); to change state.
	}
}
