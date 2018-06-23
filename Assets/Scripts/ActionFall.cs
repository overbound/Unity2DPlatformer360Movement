using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionFall : ActionInterface {
	
	public void BeginAction() {

	}
	
	// Update is called once per frame
	public void Action () {
		//physics.speed = controller.xAxis1.direction * 0.1f;
		//physics.direction = Mathf.Sign (physics.speed);
		physics.UpdateMovement();
	}
}
