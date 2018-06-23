using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCollisionInfo : MonoBehaviour {

	PlayerPhysics playerPhysics;
	public Vector2 aTop, bTop, aBottom, bBottom, horizontal, directionBottom, directionRight;
	public RaycastHit2D aBottomHit, bBottomHit, aTopHit, bTopHit, aHorizontalHit, bHorizontalHit, currentBottomHit, currentTopHit, currentHorizontalHit;
	public float verticalDirectionSign;
	public float boundOffset = 0.0625f;
	public float verticalOffset = 0.0225f;
	public float downSign = -1;
	public float minDistance;


	public float floor = 1f;

	void Start() {
		playerPhysics = GetComponent<PlayerPhysics> ();
	}

	public void UpdateSensorOrigins(ref FloorMode floorMode, BoxCollider2D collider, float angle) {
		Bounds bounds = collider.bounds;
		bounds.Expand (boundOffset * -2);

		if (playerPhysics.landed) {
			float floorModeRaw = (Mathf.Round ((angle / 90f)) % 4f);
			Debug.Log ("Floor Mode" + floorModeRaw);
			floorMode = (FloorMode)(Mathf.Round ((angle / 90f)) % 4f);
		}

		switch (floorMode) {
		case (FloorMode.DOWN):
			collider.size = new Vector2 (0.125f, 0.2f);
			minDistance = collider.bounds.size.y / 8;
			aBottom = new Vector2 (collider.bounds.min.x + verticalOffset, collider.bounds.center.y);
			bBottom = new Vector2 (collider.bounds.max.x - verticalOffset, collider.bounds.center.y);

			aTop = new Vector2 (collider.bounds.min.x + verticalOffset, collider.bounds.min.y + .15f);
			bTop = new Vector2 (collider.bounds.max.x - verticalOffset, collider.bounds.min.y + .15f);

			horizontal = new Vector2 (collider.bounds.center.x, collider.bounds.min.y + .15f);
			//bHorizontal = new Vector2 (collider.bounds.center.x, collider.bounds.min.y + .0005f);

			directionBottom = Vector2.down;
			directionRight = Vector2.right;
			downSign = -1;
			return;
		case (FloorMode.LEFT):
			collider.size = new Vector2 (0.2f, 0.125f);
			minDistance = collider.bounds.size.x / 8;
			aBottom = new Vector2 (collider.bounds.center.x, collider.bounds.max.y - verticalOffset);
			bBottom = new Vector2 (collider.bounds.center.x, collider.bounds.min.y + verticalOffset);

			aTop = new Vector2 (collider.bounds.min.x + .15f, collider.bounds.max.y - verticalOffset);
			bTop = new Vector2 (collider.bounds.min.x + .15f, collider.bounds.min.y + verticalOffset);
		
			horizontal = new Vector2 (collider.bounds.min.x + .15f, collider.bounds.center.y);
			//bHorizontal = new Vector2 (collider.bounds.min.x + .0005f, collider.bounds.center.y);

			directionBottom = Vector2.left;
			directionRight = Vector2.down;
			downSign = -1;
			return;

		case (FloorMode.UP):
			collider.size = new Vector2 (0.125f, 0.2f);
			minDistance = collider.bounds.size.y / 8;
			aBottom = new Vector2 (collider.bounds.max.x - verticalOffset, collider.bounds.center.y);
			bBottom = new Vector2 (collider.bounds.min.x + verticalOffset, collider.bounds.center.y);

			aTop = new Vector2 (collider.bounds.min.x + verticalOffset, collider.bounds.max.y - .15f);
			bTop = new Vector2 (collider.bounds.max.x - verticalOffset, collider.bounds.max.y - .15f);

			horizontal = new Vector2 (collider.bounds.center.x, collider.bounds.max.y - .15f);
			//bHorizontal = new Vector2 (collider.bounds.center.x, collider.bounds.max.y - .005f);

			directionBottom = Vector2.up;
			directionRight = Vector2.left;
			downSign = 1;
			return;

		case (FloorMode.RIGHT):
			collider.size = new Vector2 (0.2f, 0.125f);
			minDistance = collider.bounds.size.x / 8;
			aBottom = new Vector2 (collider.bounds.center.x, collider.bounds.min.y + verticalOffset);
			bBottom = new Vector2 (collider.bounds.center.x, collider.bounds.max.y - verticalOffset);

			aTop = new Vector2 (collider.bounds.max.x - .15f, collider.bounds.min.y + verticalOffset);
			bTop = new Vector2 (collider.bounds.max.x - .15f, collider.bounds.max.y - verticalOffset);

			horizontal = new Vector2 (collider.bounds.max.x - .15f, collider.bounds.center.y);
			//bHorizontal = new Vector2 (collider.bounds.max.x - .0005f, collider.bounds.center.y);

				
			directionBottom = Vector2.right;
			directionRight = Vector2.up;
			downSign = 1;
			return;
		}
	}
}

public enum FloorMode {
	DOWN = 0, LEFT = 1 , UP = 2, RIGHT = 3
}