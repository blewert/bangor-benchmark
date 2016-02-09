using UnityEngine;
using System.Collections;

public class TankTurretController : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void TurnTurretLeft()
	{
		transform.Rotate(new Vector3(0,1,0));
	}
	
	
	public void TurnTurretLeft(float change)
	{
		transform.Rotate(new Vector3(0,change,0));
	}
	
	public void TurnTurretRight()
	{
		transform.Rotate(new Vector3(0,-1f,0));
	}
	
	public void TurnTurretRight(float change)
	{
		transform.Rotate(new Vector3(0,change,0));
	}
	public float GetTurretAngle(){
		return 0.0f;
	}
	public float GetTurretAngle(Vector3 other){
		Vector3 turretForward = transform.forward;
		turretForward.y = 0;
		float turretAngle = Vector3.Angle (turretForward, other - transform.position);
		return turretAngle;
	}
	public float AngleDir(Vector3 fwd, Vector3 targetDir, Vector3 up) {
		Vector3 perp = Vector3.Cross (fwd, targetDir);
		float dir = Vector3.Dot (perp, up);
		
		if (dir > 0f) {
			return 1f;
		} else if (dir < 0f) {
			return -1f;
		} else {
			return 0f;
		}
	}
}
