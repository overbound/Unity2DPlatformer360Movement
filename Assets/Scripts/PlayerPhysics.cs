using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPhysics : MonoBehaviour {

	public LayerMask collisionMask;
	public InputController controller;
	public ActionController actionController;
	public float relativeAngle = 0;
	public float previousRelativeAngle = 0;
	public Vector2 velocity;
	public Vector2 positionPrevious;
	public float groundSpeed = 0;
	public FloorMode floorMode = FloorMode.DOWN;
	public BoxCollider2D collider;
	PlayerCollisionInfo collisionInfo;
	public bool landed = false;
	public bool isBraking = false;
	float gravity = 0.0021875f;
	public float direction = 1;
	public float jumpSpeed = 0.065f;
	public float maxGroundSpeed = 0.06f;
	public float maxYVelocity = 0.06f;
	public float slopeFactor = 0.0009765625f;
	public float acceleration = 0.00046875f;
	public float deceleration = 0.00390625f;
	public float friction = 0.00046875f;
	public float fallThreshold = 0.01953125f;
	public float brakeThreshold = 0.03125f;
	public float airDrag = 0.96875f;

	void Awake () {
		QualitySettings.vSyncCount = 0;  // VSync must be disabled to set FPS
		Application.targetFrameRate = 60;
		floorMode = FloorMode.DOWN;
		collider = GetComponent<BoxCollider2D> ();
	}

	void Start () {
		controller = GetComponent<InputController> ();
		collisionInfo = GetComponent<PlayerCollisionInfo> ();
		actionController = GetComponent<ActionController> ();
		positionPrevious = new Vector2 (transform.position.x, transform.position.y);
	}

	public void setPreviousPosition() {
		positionPrevious.y = transform.position.y;
		positionPrevious.x = transform.position.x;
	}

	public void UpdateMovement() {
		collisionInfo.UpdateSensorOrigins (ref floorMode, collider, relativeAngle);

		if (landed) {
			GroundMovement ();
		} else {
			AirMovement ();
		}
		previousRelativeAngle = relativeAngle;
	}

	void CalculateGroundSpeed() {
		if (relativeAngle <= 22.5f || relativeAngle >= 337.5f) {
			groundSpeed = velocity.x;
		} else if (relativeAngle <= 45f || relativeAngle >= 315f) {
			if (Mathf.Abs (velocity.x) > Mathf.Abs (velocity.y)) {
				groundSpeed = velocity.x;
			} else {
				groundSpeed = velocity.y * 0.5f * -Mathf.Sign (Mathf.Sin ((relativeAngle) * Mathf.Deg2Rad));
			}
		} else if (relativeAngle >= 270f || relativeAngle <= 90f) {
			if (Mathf.Abs (velocity.x) > Mathf.Abs (velocity.y)) {
				groundSpeed = velocity.x;
			} else {
				groundSpeed = velocity.y * -Mathf.Sign (Mathf.Sin ((relativeAngle) * Mathf.Deg2Rad));
			}
		}

	}

	void AirMovement() {
		float horizontalSign = (floorMode == FloorMode.DOWN || floorMode == FloorMode.UP) ? -1 : 1;

		velocity.y -= gravity;
		if (Mathf.Abs(velocity.y) > maxYVelocity) {
			velocity.y = maxYVelocity * Mathf.Sign(velocity.y);
		}

		CollisionsHorizontal ();
		if (collisionInfo.currentHorizontalHit) {
			if ((collisionInfo.currentHorizontalHit == collisionInfo.aHorizontalHit && velocity.x < 0)
				|| collisionInfo.currentHorizontalHit == collisionInfo.bHorizontalHit && velocity.x > 0) {
				if (floorMode == FloorMode.DOWN || floorMode == FloorMode.UP) {
					velocity.x = (collisionInfo.currentHorizontalHit.distance - collisionInfo.boundOffset) * Mathf.Sign(velocity.x) * -collisionInfo.downSign;
				} else {
					velocity.y = (collisionInfo.currentHorizontalHit.distance - collisionInfo.boundOffset) * Mathf.Sign(velocity.y)  * -collisionInfo.downSign;
				}
			}
		}

		if(velocity.y <= 0f) {
		//if(velocity.y >= 0f) {
			CollisionsBelow (collisionInfo.directionBottom);

			if (collisionInfo.currentBottomHit) {
				velocity.y = (collisionInfo.currentBottomHit.distance - (collider.size.y * 0.5f) + collisionInfo.minDistance) * collisionInfo.downSign;
				relativeAngle = SetAngle (collisionInfo.currentBottomHit.normal);
				CalculateGroundSpeed();
				landed = true;
				if (actionController.lockControlsWhenLanding) {
					actionController.SetLockedControls(30f);
				}
				actionController.action = ActionController.PlayerActions.RUN;
			}
		}

		CollisionsAbove (-collisionInfo.directionBottom);
		if (collisionInfo.currentTopHit) {
			if (floorMode == FloorMode.DOWN || floorMode == FloorMode.UP) {
				velocity.y = Mathf.Clamp(velocity.y, -Mathf.Infinity, (collisionInfo.currentTopHit.distance - collisionInfo.boundOffset) * -collisionInfo.downSign);
			} else {
				velocity.x = Mathf.Clamp(velocity.x, -Mathf.Infinity, (collisionInfo.currentTopHit.distance - collisionInfo.boundOffset) * -collisionInfo.downSign);
			}
		}

		setPreviousPosition ();
		transform.Translate (velocity);
	}

	void GroundMovement () {
		groundSpeed += slopeFactor * Mathf.Sin (relativeAngle * Mathf.Deg2Rad);

		groundSpeed = Mathf.Round(groundSpeed * 10000f) / 10000f;
		velocity.x = (Mathf.Cos (relativeAngle * Mathf.Deg2Rad) * groundSpeed);
		velocity.x = Mathf.Round(velocity.x * 10000f) / 10000f;
		velocity.y = -(Mathf.Sin (relativeAngle * Mathf.Deg2Rad) * groundSpeed);
		velocity.y = Mathf.Round(velocity.y * 10000f) / 10000f;

		CollisionsHorizontal ();
		if (collisionInfo.currentHorizontalHit) {
			if ((collisionInfo.currentHorizontalHit == collisionInfo.aHorizontalHit && groundSpeed < 0)
				|| collisionInfo.currentHorizontalHit == collisionInfo.bHorizontalHit && groundSpeed > 0) {
				if (floorMode == FloorMode.DOWN || floorMode == FloorMode.UP) {
					velocity.x = (collisionInfo.currentHorizontalHit.distance - collisionInfo.boundOffset) * Mathf.Sign(velocity.x) * -collisionInfo.downSign;
				} else {
					velocity.y = (collisionInfo.currentHorizontalHit.distance - collisionInfo.boundOffset) * Mathf.Sign(velocity.y)  * -collisionInfo.downSign;
				}
			}
		}

		CollisionsBelow (collisionInfo.directionBottom);
		if (collisionInfo.currentBottomHit) {
			if (floorMode == FloorMode.DOWN || floorMode == FloorMode.UP) {
				velocity.y = (collisionInfo.currentBottomHit.distance - (collider.size.y * 0.5f) + collisionInfo.minDistance) * collisionInfo.downSign;
			} else {
				velocity.x = (collisionInfo.currentBottomHit.distance - (collider.size.x * 0.5f) + collisionInfo.minDistance) * collisionInfo.downSign;
			}
			relativeAngle = SetAngle (collisionInfo.currentBottomHit.normal);
		} else {
			actionController.action = ActionController.PlayerActions.FALL;
			Debug.Log ("I'm sorry about your fall");
		}

		if (Mathf.Abs (groundSpeed) < fallThreshold && floorMode != FloorMode.DOWN) {
			if (relativeAngle >= 90f && relativeAngle <= 270f) {
				actionController.action = ActionController.PlayerActions.FALL;
				actionController.lockControlsWhenLanding = true;
				actionController.lockedControls = true;
				Debug.Log ("YA FELL FOO");
			} else {
				actionController.SetLockedControls (30f);
			}
		}
	
		/* TODO Collisions above warp player through ground when moving too fast.*/
		CollisionsAbove (-collisionInfo.directionBottom);
		if (collisionInfo.currentTopHit) {
			if (floorMode == FloorMode.DOWN || floorMode == FloorMode.UP) {
				velocity.y = (collisionInfo.currentTopHit.distance -  collider.size.y * 0.5f) * -collisionInfo.downSign;
			} else {
				velocity.x = (collisionInfo.currentTopHit.distance -  collider.size.y * 0.5f) * -collisionInfo.downSign;
			}
		}

		setPreviousPosition ();
		transform.Translate (velocity);
	}

	void CollisionsAbove(Vector2 directionAbove) {
		float velocityV = (floorMode == FloorMode.DOWN || floorMode == FloorMode.UP) ? velocity.y : velocity.x;
		float length = Mathf.Abs (velocityV) + ((floorMode == FloorMode.DOWN || floorMode == FloorMode.UP) ? collider.size.y : collider.size.x) * 0.5f;

		collisionInfo.aTopHit = Physics2D.Raycast (collisionInfo.aTop, directionAbove, length, collisionMask);
		Debug.DrawRay (collisionInfo.aTop, directionAbove * length, Color.red);
		collisionInfo.bTopHit = Physics2D.Raycast (collisionInfo.bTop, directionAbove, length, collisionMask);
		Debug.DrawRay (collisionInfo.bTop, directionAbove * length, Color.red);

		if (relativeAngle != 0) {

		}
		if (collisionInfo.aTopHit && collisionInfo.bTopHit) {
			collisionInfo.currentTopHit = collisionInfo.aTopHit.distance <= collisionInfo.bTopHit.distance ? collisionInfo.aTopHit : collisionInfo.bTopHit;
		} else if (collisionInfo.aTopHit) {
			collisionInfo.currentTopHit = collisionInfo.aTopHit;
		} else if (collisionInfo.bTopHit) {
			collisionInfo.currentTopHit = collisionInfo.bTopHit;
		} else {
			collisionInfo.currentTopHit = new RaycastHit2D ();
		}
	}

	void CollisionsBelow(Vector2 directionBelow) {
		float velocityV = (floorMode == FloorMode.DOWN || floorMode == FloorMode.UP) ? velocity.y : velocity.x;
		float length = Mathf.Abs(velocityV) + ((floorMode == FloorMode.DOWN || floorMode == FloorMode.UP) ? collider.size.y : collider.size.x) * 0.5f;

		if (Mathf.Sign (velocityV) != collisionInfo.downSign && !landed) {
			length = collisionInfo.boundOffset;
		}

		collisionInfo.aBottomHit = Physics2D.Raycast (collisionInfo.aBottom, directionBelow, length, collisionMask);
		Debug.DrawRay (collisionInfo.aBottom, directionBelow * length, Color.cyan);
		collisionInfo.bBottomHit = Physics2D.Raycast (collisionInfo.bBottom, directionBelow, length, collisionMask);
		Debug.DrawRay (collisionInfo.bBottom, directionBelow * length, Color.cyan);

		if (collisionInfo.aBottomHit && collisionInfo.bBottomHit) {
			collisionInfo.currentBottomHit = collisionInfo.aBottomHit.distance <= collisionInfo.bBottomHit.distance ? collisionInfo.aBottomHit : collisionInfo.bBottomHit;
		} else if (collisionInfo.aBottomHit) {
			collisionInfo.currentBottomHit = collisionInfo.aBottomHit;
		} else if (collisionInfo.bBottomHit) {
			collisionInfo.currentBottomHit = collisionInfo.bBottomHit;
		} else {
			collisionInfo.currentBottomHit = new RaycastHit2D ();
		}
	}

	void CollisionsHorizontal() {
		collisionInfo.aHorizontalHit = new RaycastHit2D ();
		collisionInfo.bHorizontalHit = new RaycastHit2D ();
		float velocityH = (floorMode == FloorMode.DOWN || floorMode == FloorMode.UP) ? velocity.x : velocity.y;
		float length = Mathf.Abs (velocityH) + collisionInfo.boundOffset;

		RaycastHit2D currentHit = Physics2D.Raycast (collisionInfo.horizontal, -collisionInfo.directionRight, length, collisionMask);
		if (currentHit && (!collisionInfo.aHorizontalHit || currentHit.distance < collisionInfo.aHorizontalHit.distance)) {
			collisionInfo.aHorizontalHit = currentHit;
		}
		Debug.DrawRay (collisionInfo.horizontal, -collisionInfo.directionRight * length, Color.green);

		currentHit = Physics2D.Raycast (collisionInfo.horizontal, collisionInfo.directionRight, length, collisionMask);
		if (currentHit && (!collisionInfo.bHorizontalHit || currentHit.distance < collisionInfo.bHorizontalHit.distance)) {
			collisionInfo.bHorizontalHit = currentHit;
		}
		Debug.DrawRay (collisionInfo.horizontal, collisionInfo.directionRight * length, Color.green);

		if (collisionInfo.aHorizontalHit && collisionInfo.bHorizontalHit) {
			collisionInfo.currentHorizontalHit = (Mathf.Sign (velocityH) == -1) ? collisionInfo.aHorizontalHit : collisionInfo.bHorizontalHit;
		} else if (collisionInfo.aHorizontalHit) {
			collisionInfo.currentHorizontalHit = collisionInfo.aHorizontalHit;
		} else if (collisionInfo.bHorizontalHit) {
			collisionInfo.currentHorizontalHit = collisionInfo.bHorizontalHit;
		} else {
			collisionInfo.currentHorizontalHit = new RaycastHit2D ();
		}
	}

	float SetAngle(Vector2 hit) {
		float angle = Vector2.SignedAngle(hit, Vector2.down);
		return Mathf.Repeat (angle-180f, 360f);
	}
}