using UnityEngine;
using System.Collections;
using System.Linq;

/// <summary>
/// Creates wandering behaviour for a CharacterController.
/// </summary>
using System.Collections.Generic;


[RequireComponent(typeof(CharacterController))]
public class TankEnemyController : MonoBehaviour
{
	public float speed = 5;
	public float directionChangeInterval = 0.5f;
	public float maxHeadingChange = 30;
	public List<GameObject> targets = new List<GameObject>();
	public int visionAngle = 30;
	public int visionDistance = 20;
	private bool attack = false;
	private bool turnAway = false;
	private GameObject acTarget;
	private Vector3 originPoint;


	CharacterController controller;
	float heading;
	Vector3 targetRotation;
	
	void Awake ()
	{
		controller = GetComponent<CharacterController>();
		
		// Set random initial rotation
		//heading = Random.Range(0, 360);
		//transform.eulerAngles = new Vector3(0, heading, 0);
		heading = transform.eulerAngles.y;
		StartCoroutine(NewHeading());
		originPoint = transform.position;
	}
	
	void Update ()
	{
		if (!attack && !turnAway) {
//			if(targetRotation.y < 0)
//				targetRotation.y = 360 - (-targetRotation.y);
			Vector3 vector = Quaternion.Euler (targetRotation) * transform.forward;
			transform.eulerAngles = Vector3.Lerp (transform.eulerAngles, targetRotation, Time.deltaTime);
			var forward = transform.TransformDirection (Vector3.forward);
			controller.SimpleMove (forward * speed);
		} else if (turnAway) {
			GetComponent<TankController>().turnRight();
			Vector3 thisFor = transform.forward;
			thisFor.y = 0;
			Vector3 op = originPoint - transform.position;
			op.y = 0;
			if(Vector3.Angle(thisFor, op) < 15){

				turnAway = false;
				heading = transform.eulerAngles.y;
			}
		}
	}
	
	/// <summary>
	/// Repeatedly calculates a new direction to move towards.
	/// Use this instead of MonoBehaviour.InvokeRepeating so that the interval can be changed at runtime.
	/// </summary>
	IEnumerator NewHeading ()
	{
		while (true) {
			NewHeadingRoutine();
			yield return new WaitForSeconds(directionChangeInterval);
		}
	}
	
	/// <summary>
	/// Calculates a new direction to move towards.
	/// </summary>
	void NewHeadingRoutine ()
	{
		bool wait = true;

		// reset direction change interval
		directionChangeInterval = 0.5f;
		var oldHead = heading;
		var floor = Mathf.Clamp(heading - maxHeadingChange, 0, 360);
		var ceil  = Mathf.Clamp(heading + maxHeadingChange, 0, 360);
		heading = Random.Range(floor, ceil);
		int dist = 30;
		Vector3 dir = transform.forward * dist;
		Vector3 pos = transform.TransformPoint (transform.position + dir);
		Debug.DrawLine (transform.position, transform.position + dir, Color.blue, 2);
		if (Terrain.activeTerrain.SampleHeight (transform.position + dir) < 10) {

//			GameObject obs = GameObject.Find("Observer").GetComponent<TankSpawner>().originPoint;
//
//			Vector3 o = new Vector3(0,obs.transform.position.y,0);
//			Vector3 d = new Vector3(0,transform.position.y,0);
//
//			Vector3 _dir = (o - d);
//
//			Debug.DrawLine(obs.transform.position, transform.position, Color.red, 2);
//
//			Vector3 referenceRight= Vector3.Cross(Vector3.up, transform.forward);
//
//			float angle = Vector3.Angle(_dir, transform.forward);
//
//			float sign = Mathf.Sign(Vector3.Dot(_dir.normalized, referenceRight.normalized));
//
//
//
//			heading = sign * angle;
//
//			Debug.Log (angle);

			// give the NPC time to get way from the edge

//			heading += oldHead + 90;
//			Debug.DrawLine (transform.position, transform.position + dir, Color.blue, 2);
//			directionChangeInterval = 2;
			turnAway = true;
		}

		targetRotation = new Vector3(0, heading, 0);
	}

	public void UnderAttack (bool att){
		this.attack = att;
	}

	void Fire(){
		// fire projectiles at the enemy.
	}

	public int GetVisionDistance(){
		return visionDistance;
	}

	public int GetVisionAngle(){
		return visionAngle;
	}

	public List<GameObject> GetTargets(){
		return targets.Where(x => x!=null).ToList();
	}
}