using UnityEngine;
using System.Collections;
using System.Linq;

public class TankTurretAI : MonoBehaviour {

	private GameObject acTarget;

	private TankEnemyController parent;

	private int tankScore;

	void BulletHit(RaycastHit[] hits){
		foreach (RaycastHit hit in hits) {
			GameObject go = hit.transform.gameObject;

			//Debug.Log(go.name);
			Vector3 top = hit.point;
			top.y += 100;
			Debug.DrawLine(hit.point, top, Color.blue);
			if(parent.GetTargets().Contains(go)){
				go.GetComponent<TankController>().takeHealth(1);
				if(go.GetComponent<TankController>().health < 1){
					parent.GetTargets().Remove(go);
					Destroy(go);
					tankScore ++;
				}
			}
		}
	}

	public void setScore(int sc){
		tankScore = sc;
	}
	
	public int getScore(){
		return tankScore;
	}


	// Use this for initialization
	void Start () {
		parent = gameObject.transform.parent.GetComponent<TankEnemyController>();

		gameObject.GetComponentInChildren<Gun> ().OnBulletHit += BulletHit;
		tankScore = 0;
	}
	
	// Update is called once per frame
	void Update () {
		Vision ();
	}


	void Vision(){

		foreach(GameObject target in parent.GetTargets()){


			Vector3 forward = transform.forward;
			Vector3 tPosition = target.transform.position;
			Vector3 position = transform.position;
			// set the ys to 0 as these mess with the calculations
			forward.y = tPosition.y = position.y = 0;
			forward.z = -forward.z;
			//float angleDot = Vector3.Dot (forward.normalized, (tPosition - position).normalized); 
			float angle = Vector3.Angle (forward, tPosition - position) - 90;
			float distance = Vector3.Distance (forward, tPosition - position);


			int visionAngle = parent.GetVisionAngle();
			int visionDistance = parent.GetVisionDistance();

			if (angle < visionAngle && distance < visionDistance) {
//				target.GetComponent<Renderer> ().material.color = Color.red;
				float turretAngle = gameObject.GetComponent<TankTurretController>().GetTurretAngle(tPosition);
				//Debug.Log (turretAngle);
				if(turretAngle < 61 && turretAngle > 50){
					float lor = gameObject.GetComponent<TankTurretController>().AngleDir(
						transform.forward, tPosition - position, transform.up
						);
					if(lor > 0){
						gameObject.GetComponent<TankTurretController>().TurnTurretLeft();
					} else {
						gameObject.GetComponent<TankTurretController>().TurnTurretRight();
					}
					Debug.Log (turretAngle);
				} else {

					Vector3 pos =target.transform.position;


					transform.FindChild("Gun").transform.LookAt(pos);
					//Debug.DrawRay(tip.position, -tip.up * 300, Color.red);
					gameObject.GetComponentInChildren<Gun>().StartFiring();

				}
				acTarget = target;
				parent.UnderAttack(true);
			} else {
				parent.UnderAttack(false);
				gameObject.GetComponentInChildren<Gun>().StopFiring();
//				target.GetComponent<Renderer> ().material.color = Color.green;
			}
		}
	}

	void Fire(){

	}
}
