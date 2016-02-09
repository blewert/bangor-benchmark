using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;

public class TankCompanionController : MonoBehaviour {
	private float health;
	private bool called = false;
	// Use this for initialization
	void Start () {
		CheckHealth ();
	}
	
	// Update is called once per frame
	void Update () {
		if (health != gameObject.GetComponentInChildren<TankController> ().health) {
			CallForHelp ();
		}
		CheckHealth ();
	}
	// update health variable so we can check it's value on the next update.
	private void CheckHealth(){
		health = gameObject.GetComponentInChildren<TankController> ().health;
	}
	// issue help commands to the player
	private void CallForHelp(){
		StopCoroutine("Countdown");
		if (!called) {
			GameObject.Find ("hud_msg").transform.position += new Vector3 (0, -50, 0);
			called = true;
		}
		// let the data saver know that the companion has called for help.
		GameObject.Find ("Observer").GetComponent<TankSpawner> ().GetHelp ();
		StartCoroutine (Countdown(2, Reset));
	}

	private void Reset(){
		if (called) {
			GameObject.Find ("hud_msg").transform.position += new Vector3 (0, 50, 0);
			called = false;
		}
	}

	private IEnumerator Countdown(int time,  Action f){
		while(time>0){
			time--;
			yield return new WaitForSeconds(1);
		}
		f ();
		Debug.Log("Countdown Complete!");
	}
}



