using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionRun : ActionInterface {



	public override void BeginAction() {

	}

	public override void Action () {
		if (controller.xAxis1.direction == -1 && !actionController.lockedControls) {
			physics.direction = 1;
			if (physics.groundSpeed > 0) {
				if (physics.groundSpeed >= physics.brakeThreshold) {
					if (!physics.isBraking) {
						physics.isBraking = true;
					}
				}
				physics.groundSpeed -= physics.deceleration;

				if (physics.groundSpeed <= 0) {
					physics.groundSpeed = -physics.deceleration;
				}
			} else {
				if (physics.groundSpeed > -physics.maxGroundSpeed) {
					physics.groundSpeed -= physics.acceleration;

					if (physics.groundSpeed < -physics.maxGroundSpeed) {
						physics.groundSpeed = -physics.maxGroundSpeed;
					}

					physics.isBraking = false;
				}
			}
		} else if (controller.xAxis1.direction == 1 && !actionController.lockedControls) {
			physics.direction = -1;
			if (physics.groundSpeed < 0) {
				if (!actionController.lockedControls) {
					if (physics.groundSpeed <= -physics.brakeThreshold) {
						if (!physics.isBraking) {
							physics.isBraking = true;
						}
					}
					physics.groundSpeed += physics.deceleration;

					if (physics.groundSpeed >= 0) {
						physics.groundSpeed = physics.deceleration;
					}
				}
			} else {
				if (physics.groundSpeed < physics.maxGroundSpeed) {
					physics.groundSpeed += physics.acceleration;

					if (physics.groundSpeed > physics.maxGroundSpeed) {
						physics.groundSpeed = physics.maxGroundSpeed;
					}

					physics.isBraking = false;
				}
			}
		} else {
			physics.groundSpeed -= Mathf.Min(Mathf.Abs(physics.groundSpeed),physics.friction)*Mathf.Sign(physics.groundSpeed);
			if (physics.groundSpeed == 0) {
				physics.isBraking = false;
			}
		}




		/*
		if (controller.xAxis1.direction == -1 && !actionController.lockedControls) {
			if (physics.groundSpeed > 0) {
				physics.groundSpeed -= physics.deceleration;
			} else if(physics.groundSpeed > -physics.maxGroundSpeed) {
				physics.groundSpeed = Mathf.Max(physics.groundSpeed-physics.acceleration,-physics.maxGroundSpeed);
			}
		} else if (controller.xAxis1.direction == 1 && !actionController.lockedControls) {
			if (physics.groundSpeed < 0) {
				physics.groundSpeed += physics.deceleration;
			} else if(physics.groundSpeed < physics.maxGroundSpeed) {
				physics.groundSpeed = Mathf.Min(physics.groundSpeed+physics.acceleration,physics.maxGroundSpeed);
			}
		} else if(!actionController.lockedControls) {
			physics.groundSpeed -= Mathf.Min (Mathf.Abs (physics.groundSpeed), physics.friction) * Mathf.Sign (physics.groundSpeed);
		}

		if (controller.xAxis1.direction != 0) {
			physics.direction = controller.xAxis1.direction;
		}*/

		/*
		if (physics.groundSpeed == 0 && !actionController.lockedControls) {
			if (controller.xAxis1.direction != 0) {
				physics.direction = Mathf.Sign (controller.xAxis1.direction);
				physics.groundSpeed += physics.acceleration * physics.direction;
			}
		} else {
			// this logic will not work on left motion possibly right too
			if (controller.xAxis1.direction == physics.direction && !actionController.lockedControls) {
				physics.direction = Mathf.Sign (controller.xAxis1.direction);
				physics.groundSpeed += physics.acceleration * physics.direction;
			} else if (controller.xAxis1.direction == -physics.direction && !actionController.lockedControls) {
				physics.direction = Mathf.Sign (controller.xAxis1.direction);
				physics.groundSpeed += physics.deceleration * physics.direction;
			} else if (controller.xAxis1.direction == 0 && !actionController.lockedControls) {
				float newGroundSpeed = physics.groundSpeed - physics.friction * Mathf.Sign (physics.groundSpeed);
				if (Mathf.Abs (newGroundSpeed) == Mathf.Sign (physics.groundSpeed)) {
					physics.groundSpeed = newGroundSpeed;
				} else {
					physics.groundSpeed = 0;
				}
			}
		}

		if (Mathf.Abs (physics.groundSpeed) > physics.maxGroundSpeed) {
			physics.groundSpeed = physics.maxGroundSpeed * Mathf.Sign(physics.groundSpeed);
		}
		*/
		/*
		if (controller.xAxis1.direction != 0f && !actionController.lockedControls) {
			if (controller.xAxis1.direction.Equals (physics.direction)) {
				physics.direction = Mathf.Sign (controller.xAxis1.direction);
				physics.groundSpeed += physics.acceleration * physics.direction;
			} else {
				physics.direction = Mathf.Sign (controller.xAxis1.direction);
				physics.groundSpeed += physics.deceleration * physics.direction;
			}
		} else if (actionController.lockedControls) {
			physics.groundSpeed -= physics.friction * Mathf.Sign (physics.groundSpeed);
			if (actionController.lockedControlsTimer == 0f) {
				actionController.lockedControls = false;
			}
		} else {
			if (Mathf.Abs (physics.groundSpeed) >= physics.friction) {
				physics.groundSpeed -= physics.friction * Mathf.Sign (physics.groundSpeed);
			} else {
				physics.groundSpeed = 0f;
			}
		}

		if (Mathf.Abs (physics.groundSpeed) > physics.maxGroundSpeed) {
			physics.groundSpeed = physics.maxGroundSpeed * physics.direction;
		}

		if (Mathf.Abs (physics.groundSpeed) == 0) {
			ChangeAnimationState (PlayerAnimations.IDLE);
		} else {
			ChangeAnimationState (PlayerAnimations.RUN);
		}*/


		physics.UpdateMovement();

		if (controller.button1.isPressed) {
			actionController.action = ActionController.PlayerActions.JUMP;
		}
	}
}
