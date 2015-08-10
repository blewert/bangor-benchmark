using UnityEngine;
using System.Collections;

public class lightSwitch : MonoBehaviour {
	private GameObject Player;
	private int randomRange;
	//bool lights;
	// Use this for initialization
	void Start () {
		Player = GameObject.FindGameObjectWithTag ("Player");
		if (Random.Range (0, 4) < 2) {
			transform.GetComponent<Light>().enabled = true;
		} else {
			transform.GetComponent<Light>().enabled = false;
		}

		randomRange = Random.Range (40, 250);

	}
	
	// Update is called once per frame
	void Update () {
		//float distance = Vector3.Distance (transform.position, Player.transform.position);
		//if (distance < 30f) {
			
		if (Random.Range (0, randomRange) < 1) {
			if (transform.GetComponent<Light>().enabled) {
				transform.GetComponent<Light>().enabled = false;
			} else {
				transform.GetComponent<Light>().enabled = true;
			}
		}

		//transform.light.enabled = false;
		//}
	}
}
