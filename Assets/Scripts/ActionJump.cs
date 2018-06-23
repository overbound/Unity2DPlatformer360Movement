using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionJump : ActionInterface {

	public void BeginJump() {
		physics.landed = false;
		if (physics.relativeAngle < 5 || physics.relativeAngle > 355) {
			physics.relativeAngle = 0f;
		}
		physics.velocity.x -= -(physics.jumpSpeed * Mathf.Sin(physics.relativeAngle * Mathf.Deg2Rad));
		physics.velocity.y -= -(physics.jumpSpeed * Mathf.Cos(physics.relativeAngle * Mathf.Deg2Rad));
		physics.relativeAngle = 0f;
		physics.floorMode = FloorMode.DOWN;
	}

	public void BeginFall() {
		physics.landed = false;
		physics.relativeAngle = 0f;
		physics.floorMode = FloorMode.DOWN;
		physics.groundSpeed = 0;
	}
	
	// Update is called once per frame
	public override void Action () {
		float minJump = 0.03125f;
		if (controller.button1.isReleased && actionController.action == ActionController.PlayerActions.JUMP) {
			//float minJump = physics.jumpSpeed * 0.615f;

			if (physics.velocity.y > minJump) {
				//physics.velocity.x = -(minJump * Mathf.Sin (physics.relativeAngle * Mathf.Deg2Rad));
				physics.velocity.y = minJump;
				actionController.action = ActionController.PlayerActions.FALL;
			}
		}

		if (controller.xAxis1.direction != 0f && !actionController.lockedControls) {
			physics.velocity.x += (0.000732421875f) * controller.xAxis1.direction;
		}

		if (physics.velocity.y < 0f) {
			actionController.action = ActionController.PlayerActions.FALL;
		}


		if (physics.velocity.y > 0f && physics.velocity.y < minJump) {
			if (Mathf.Abs (physics.velocity.x) >= 0.0009765625f) {
				physics.velocity.x = physics.velocity.x * physics.airDrag;
			}
		}
			
		if (Mathf.Abs (physics.velocity.x) > physics.maxGroundSpeed) {
			physics.velocity.x = physics.maxGroundSpeed * Mathf.Sign(physics.velocity.x);
		}

		physics.UpdateMovement();
	}
}
