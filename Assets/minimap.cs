using UnityEngine;
using System.Collections;

public class minimap : MonoBehaviour {

	// Use this for initialization
	void Start () {
		transform.position += new Vector3 (Screen.width / 2 - 106, Screen.height / 2 - 120, 0);
		GameObject.Find("minimapbg").transform.position += new Vector3 (Screen.width / 2 - 110, Screen.height / 2 - 120, 0);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
