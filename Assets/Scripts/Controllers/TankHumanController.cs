using UnityEngine;
using System.Collections;

public class TankHumanController : MonoBehaviour {

	public ITankNPC npc;
	int i = 0;
	
	// Use this for initialization
	void Start () 
	{
		npc = GetComponent<ITankNPC>();
	}
	
	// Update is called once per frame
	void Update () {

		if (Input.GetKey (KeyCode.LeftArrow)) {
			npc.turnLeft();
		} else if (Input.GetKey (KeyCode.RightArrow)) {
			npc.turnRight();
		}
		if (Input.GetKey (KeyCode.UpArrow)) {
			npc.moveForward();
		} else if (Input.GetKey (KeyCode.DownArrow)) {
			npc.moveBackward();
		}
		if (Input.GetKey (KeyCode.A)) {
			npc.TurnTurretLeft ();
		} else if (Input.GetKey (KeyCode.D)) {
			npc.TurnTurretRight ();
		}
//		if (Input.GetKey (KeyCode.X)){
//			Debug.Log ("pressed " + i);
//			var rigidBody = npc.gameObject.AddComponent<Rigidbody>();
//			rigidBody.AddExplosionForce(1000.0f, Vector3.zero, 1000.0f, 1000.0f, ForceMode.Impulse);
//			i++;
//		}
	}
}
