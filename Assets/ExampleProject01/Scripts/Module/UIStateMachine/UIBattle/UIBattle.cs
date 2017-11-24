using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using FrameWork;

public class UIBattle : UIStateMachine 
{
    public Button battleStart;

	///
	/// initialState is set in Start()
	///
    public override string initialStateName { get { return "UIBattleIdle"; } }

	///
	/// Use this for Initialization
	///
    public override void Awake()
	{
		// Register all states.
		RegisterState(new UIBattleIdle());
		RegisterState(new UIBattleStart());
		RegisterState(new UIBattleFinish());

        // Use MonoStateMachine.DispatchStateChange("GameObjectName", "StateName"); to change state.

        EventListener.Do(() => {
            ChangeState("UIBattleStart");
        }).When(battleStart.onClick);
	}
}
