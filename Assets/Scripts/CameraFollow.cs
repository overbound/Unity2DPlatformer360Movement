using UnityEngine;
using System.Collections;

public class CameraFollow : MonoBehaviour {

	public PlayerPhysics target;
	public Vector2 focusAreaSize;
	public float verticalOffset;
	public float smoothTime;
	public Vector3 velocity;
	Vector3 previousPosition;

	FocusArea focusArea;

	void Start() {
		focusArea = new FocusArea (target.collider.bounds, focusAreaSize, transform.position.z);
	}

	void LateUpdate() {
		focusArea.Update (target.collider.bounds);
		Vector3 focusPosition = focusArea.center + Vector3.up * verticalOffset;

		if (transform.position.x - 1 < focusArea.left || transform.position.x + 1 > focusArea.right) {
			focusArea.RestXFocus (target.collider.bounds, focusAreaSize);
		}

		previousPosition = transform.position;
		transform.position =  Vector3.SmoothDamp(transform.position, focusPosition, ref velocity, smoothTime);
		//transform.position = Vector3.Lerp (transform.position, focusPosition, smoothTime * Time.deltaTime);
	}

	void OnDrawGizmos() {
		Gizmos.color = new Color (1, 0, 1, .2f);
		Gizmos.DrawCube (focusArea.center, focusAreaSize);
	}

	struct FocusArea {
		public Vector3 center;
		public float left, right;
		public float top, bottom;
		public Vector2 velocity;

		public FocusArea(Bounds targetBounds, Vector2 size, float cameraOffset) {
			left = targetBounds.center.x - size.x/2;
			right = targetBounds.center.x + size.x/2;
			bottom = targetBounds.min.y;
			top = targetBounds.min.y + size.y;
			velocity = Vector3.zero;
			center = new Vector3((left+right)/2, (top+bottom)/2, cameraOffset);
		}

		public void RestXFocus(Bounds targetBounds, Vector2 size) {
			left = targetBounds.center.x - size.x/2;
			right = targetBounds.center.x + size.x/2;
			center = new Vector3((left+right)/2, center.y, center.z);
		}

		public void Update(Bounds targetBounds) {
			float shiftX = 0;
			if (targetBounds.min.x < left) {
				shiftX = targetBounds.min.x - left;
			} else if (targetBounds.max.x > right) {
				shiftX = targetBounds.max.x - right;
			}

			left += shiftX;
			right += shiftX;

			float shiftY = 0;
			if (targetBounds.min.y < bottom) {
				shiftY = targetBounds.min.y - bottom;
			} else if (targetBounds.max.y > top) {
				shiftY = targetBounds.max.y - top;
			}

			top += shiftY;
			bottom += shiftY;
			center = new Vector3((left+right)/2, (top+bottom)/2, center.z);
		}
	}

}

