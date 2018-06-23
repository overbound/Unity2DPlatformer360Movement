using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionController : MonoBehaviour {

	public enum PlayerActions
	{
		JUMP, RUN, FALL
	}

	public bool lockedControls = false;
	public bool  lockControlsWhenLanding = false;
	public float lockedControlsTimer = 30;

	public PlayerActions action;
	public PlayerActions previousAction;

	ActionJump actionJump;
	ActionRun actionRun;

	// Use this for initialization;
	void Start () {
		actionJump = GetComponent<ActionJump> ();
		actionRun = GetComponent<ActionRun> ();
	}
		
	void Update () {
		//continue
		if (previousAction == action) {
			switch (action) {
			case PlayerActions.JUMP:
				actionJump.Action ();
				Debug.Log ("jump action");
				break;
			case PlayerActions.FALL:
				actionJump.Action ();
				Debug.Log ("fall action");
				break;
			case PlayerActions.RUN:
				actionRun.Action();
				break;
			}
		}

		// start
		if (previousAction != action) {
			previousAction = action;
			switch (action) {
			case PlayerActions.JUMP:
				actionJump.BeginJump ();
				break;
			case PlayerActions.FALL:
				actionJump.BeginFall ();
				break;
			case PlayerActions.RUN:
				actionRun.BeginAction();
				break;
			}

		}

		if (lockedControlsTimer > 0f) {
			lockedControlsTimer -= 1f;
		} else {
			if (!lockControlsWhenLanding /*!inputController.xAxis1.isHeld*/) {
				lockedControls = false;
			}
		}
	}

	public void SetLockedControls(float timer){
		lockedControlsTimer = timer;
		lockedControls = true;
		lockControlsWhenLanding = false;
	}
}
