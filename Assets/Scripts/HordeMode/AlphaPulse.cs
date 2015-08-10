using UnityEngine;
using System.Collections;

public class AlphaPulse : MonoBehaviour {
	
	float duration; // = Random.Range (1f, 3f);
	float alpha = 0f;

	// Use this for initialization
	void Start () {
		duration = Random.Range (6f, 30f);
	}
	
	// Update is called once per frame
	void Update () {

			
		float lerp = Mathf.PingPong(Time.time, duration) / duration;
		alpha = Mathf.Lerp(0.0f, 0.5f, lerp);
		
		Color color = gameObject.GetComponent<Renderer>().material.GetColor("_TintColor");
		color.a = alpha;
			
		gameObject.GetComponent<Renderer>().material.SetColor("_TintColor", color);

	}
}
