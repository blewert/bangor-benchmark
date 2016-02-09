using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TankTurretHumanController : MonoBehaviour {
	private TankTurretController turret;
	public List<GameObject> targets = new List<GameObject>();
	// Use this for initialization

	private int tankScore;

	void BulletHit(RaycastHit[] hits){
		foreach (RaycastHit hit in hits) {
			GameObject go = hit.transform.gameObject;
			
			//Debug.Log(go.name);
			Vector3 top = hit.point;
			top.y += 100;
			Debug.DrawLine(hit.point, top, Color.blue);
			Debug.Log(go.name);
			if(targets.Contains(go)){
				Debug.Log("hit");
				go.GetComponent<TankController>().takeHealth(1);
				if(go.GetComponent<TankController>().health < 1){
					targets.Remove(go);
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

	void Start () {
		turret = gameObject.transform.GetComponent<TankTurretController>();
		gameObject.GetComponentInChildren<Gun> ().OnBulletHit += BulletHit;
		tankScore = 0;
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKey (KeyCode.D)) {
			turret.TurnTurretLeft();
		} else if (Input.GetKey (KeyCode.A)) {
			turret.TurnTurretRight();
		}
		if (Input.GetKeyDown (KeyCode.S)) {
			gameObject.GetComponentInChildren<Gun>().StartFiring();
		}
		if (Input.GetKeyUp (KeyCode.S)) {
			gameObject.GetComponentInChildren<Gun>().StopFiring();
		}

	}
}
