using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Data : MonoBehaviour {

	public struct GameEvent {
		public float time;
		public Vector3 position;
		public string type;

		public GameEvent(float t, Vector3 p, string ty){
			time = t;
			position = p;
			type = ty;
		}
	}

	public struct CharacterPosition {
		public float time;
		public Vector3 position;
		public string id;
		
		public CharacterPosition(float t, Vector3 p, string i){
			time = t;
			position = p;
			id = i;
		}
	}

	private List<GameEvent> events;

	private List<CharacterPosition> positions;

	void AddEvent(Vector3 pos, string comment){
		GameEvent ev = new GameEvent(Time.realtimeSinceStartup, pos, comment);
		events.Add (ev);
	}


	void AddPosition(Vector3 pos, string id){
		CharacterPosition p = new CharacterPosition(Time.realtimeSinceStartup, pos, id);
		positions.Add (p);
	}


	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
