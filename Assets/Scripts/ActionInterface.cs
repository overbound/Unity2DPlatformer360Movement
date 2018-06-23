using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionInterface : MonoBehaviour {

	protected ActionController actionController;
	protected InputController controller;
	protected PlayerPhysics physics;
	protected Animator animator;

	public enum PlayerAnimations {
		IDLE = 0, RUN = 1, JUMP = 2
	}

	void Start () {
		actionController = GetComponent<ActionController> ();
		controller = GetComponent<InputController> ();
		physics = GetComponent<PlayerPhysics>();
		animator = GetComponent<Animator> ();
	}
	public virtual void BeginAction(){
	}
	public virtual void Action (){
	}

	protected void ChangeAnimationState(PlayerAnimations animationValue) {
		int animVale = (int) animationValue;
		animator.SetInteger("AnimState", animVale);
	}
}
