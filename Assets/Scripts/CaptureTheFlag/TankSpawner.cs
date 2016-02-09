//#define DEBUG_NON_RELEASE

using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System.Collections;
using System;
using UnityStandardAssets.Utility;
using System.Collections.Generic;

public class TankSpawner : MonoBehaviour 
{
	public GameObject originPoint;
	public GameObject playerOrigin;
	public int enemyCount;
	public GameObject tankPrefab;
	public GameObject playerPrefab;
	public float spawnRadius = 100;
	public int yLimit = 14;
	private List<GameObject> tanks = new List<GameObject>();
	private GameObject player;
	private GameObject companion;
	public GameObject HUD;
	public Camera mmc;
	private float dist = 0.0f;
	private int loopCount = 0;
	private int timeStamp;
	private bool needHelp = false;
	private string companionName;
	private string companionPicName;

	public void Start(){
		SpawnTanks ();
		Camera.main.GetComponent<CameraFollow> ().target = player.transform.FindChild("turret").transform;
//		Camera.main.GetComponent<CameraFollow> ().target = tanks[0].transform;
//		Camera.main.GetComponent<CameraFollow> ().target = companion.transform;
		var epochStart = new System.DateTime(1970, 1, 1, 0, 0, 0,System.DateTimeKind.Utc);
		timeStamp = (int)(System.DateTime.UtcNow - epochStart).TotalSeconds;

		float x = -Screen.width / 2 + 30;
		float y = -Screen.height / 2 + 41f;
		Vector3 n = new Vector3 (x, y, 0);
		HUD.transform.FindChild ("LeftHUD").transform.position += n;
		x = Screen.width / 2 - 45.5f;
		n.x = x;
		HUD.transform.FindChild ("RightHUD").transform.position += n;

		n.x = Screen.width/2 - 283;
		n.y = -Screen.height/2;

		HUD.transform.FindChild ("hud_comp").transform.position += n;
		n.x = -Screen.width/2;
		HUD.transform.FindChild ("hud_play").transform.position += n;

		companionName = GameObject.Find ("Observer").GetComponent<CompanionIdentity>().GetName();
		companionPicName = GameObject.Find ("Observer").GetComponent<CompanionIdentity>().GetPicName();

		GameObject.Find("hud_msg").transform.position += new Vector3(0, Screen.height/2 + 42, 0);
		Sprite sp = Resources.Load<Sprite> (companionPicName);
		GameObject.Find ("CompanionProfilePic").GetComponent<Image> ().sprite = sp;

		string head = "GameId, Companion";
		string data = timeStamp + ", " + companionPicName;
		GameObject.Find("Observer").GetComponent<SaveData>().WriteCSV("Game.csv", data, head);


	}

	private void SpawnTanks(){
		;
		player = (GameObject)Instantiate (playerPrefab, RandPointFromObject (20, playerOrigin, 50), Quaternion.identity);
		var lookPos = originPoint.transform.position - player.transform.position;
		lookPos.x = 0;
		lookPos.z = 0;
		player.transform.LookAt(lookPos);
		player.GetComponentInChildren<Gun> ().shotDistance = 100.0f;
		Quaternion rot = UnityEngine.Random.rotation;
		rot.x = 0;
		rot.z = 0;
		companion = (GameObject)Instantiate (tankPrefab, RandPointFromObject (0.2f, player, 30), rot);
		companion.AddComponent<TankCompanionController>();
		companion.GetComponent<INPC> ().health = 40;
		companion.transform.FindChild ("turret").FindChild ("Cube 1").GetComponent<Renderer>().material.color = Color.green;
		companion.transform.FindChild ("turret").FindChild ("Cube 2").GetComponent<Renderer>().material.color = Color.green;
		mmc.gameObject.GetComponent<MiniMapCameraFollow> ().Target = player.transform;

		player.gameObject.GetComponentInChildren<Gun> ().delayBetweenShots = 0.5f;
		companion.gameObject.GetComponentInChildren<Gun> ().delayBetweenShots = 0.5f;

		for (int i = 0; i < enemyCount; i++) {
			rot = UnityEngine.Random.rotation;
			rot.x = 0;
			rot.z = 0;
			Vector3 position = RandPointFromCenter(0);
			GameObject tank = (GameObject) Instantiate(tankPrefab, position, rot);
			//tank.transform.GetComponent<TankEnemyController>().targets.Add(player);
			tank.transform.GetComponent<TankEnemyController>().targets.Add(companion);
			player.transform.GetComponentInChildren<TankTurretHumanController> ().targets.Add (tank);
			companion.transform.GetComponent<TankEnemyController> ().targets.Add (tank);
			tanks.Add(tank);
			tank.gameObject.GetComponentInChildren<Gun> ().delayBetweenShots = 1;
			tank.transform.FindChild ("turret").FindChild ("Cube 1").GetComponent<Renderer>().material.color = Color.red;
			tank.transform.FindChild ("turret").FindChild ("Cube 2").GetComponent<Renderer>().material.color = Color.red;

		}

	}

	private void SpawnMoreTanks(int number){
		for (int i = 0; i < enemyCount; i++) {
			Vector3 position = RandPointFromCenter(0);
			GameObject tank = (GameObject) Instantiate(tankPrefab, position, Quaternion.identity);
			tank.transform.GetComponent<TankEnemyController>().targets.Add(player);
			player.transform.GetComponentInChildren<TankTurretHumanController> ().targets.Add (tank);
			tanks.Add(tank);
			tank.transform.FindChild ("turret").FindChild ("Cube 1").GetComponent<Renderer>().material.color = Color.red;
			tank.transform.FindChild ("turret").FindChild ("Cube 2").GetComponent<Renderer>().material.color = Color.red;
			tank.gameObject.GetComponentInChildren<Gun> ().delayBetweenShots = 1;
		}
		companion.GetComponent<INPC> ().health += 5;
		player.GetComponent<INPC> ().health += 5;
	}

	public void Update(){
		loopCount++;
		Vector3 p = new Vector3 (player.transform.position.x, 0.0f, player.transform.position.z);
		Vector3 c = new Vector3 (companion.transform.position.x, 0.0f, companion.transform.position.z);
		dist += Vector3.Distance (p, c);
		int score = player.GetComponentInChildren<TankTurretHumanController> ().getScore ();
		if (tanks.Where (x => x != null).ToList ().Count () < 4)
			SpawnMoreTanks (10);
		GameObject.Find("playerScore").GetComponent<Text>().text = "Your Score: " + 
			player.GetComponentInChildren<TankTurretHumanController>().getScore();
		GameObject.Find("playerHealth").GetComponent<Text>().text = "Your Health: " + 
			player.GetComponent<INPC> ().health;
		GameObject.Find("companionScore").GetComponent<Text>().text = companionName + " Score: " + 
			companion.GetComponentInChildren<TankTurretAI>().getScore();
		GameObject.Find ("companionHealth").GetComponent<Text> ().text = companionName + " Health: " + 
			companion.GetComponent<INPC> ().health;
//		var angle = Vector3.Angle (player.transform.position, companion.transform.position);
//		Debug.Log (angle);
//
//		Vector3 targetDir = companion.transform.position - player.transform.position;
//		targetDir = targetDir.normalized;
//		
//		float dot = Vector3.Dot(targetDir, player.transform.forward);
//		float angle2 = Mathf.Acos( dot ) * Mathf.Rad2Deg;  

		Vector3 target = companion.transform.position;
		target.y = 0;

		Vector3 source = player.transform.position;
		source.y = 0;




		float angle = Mathf.Atan2(target.z - source.z, target.x - source.x) * Mathf.Rad2Deg;


//		if(angle > 5)
//			GameObject.Find ("arrow").transform.Rotate(new Vector3(0,0,angle2));
	
		GameObject.Find ("arrow").transform.rotation = Quaternion.Euler(0, 0, angle + 270 - player.transform.rotation.eulerAngles.y);
	}

	private Vector3 RandPointFromCenter(float offset){
		while (true) {
			float randAngle = UnityEngine.Random.Range (0, 360);
			float x = originPoint.transform.position.x + offset + Mathf.Cos (randAngle * Mathf.Rad2Deg) 
				* UnityEngine.Random.Range(0,spawnRadius);
			float z = originPoint.transform.position.z + offset + Mathf.Sin (randAngle * Mathf.Rad2Deg) 
				* UnityEngine.Random.Range(0,spawnRadius);
			float y = Terrain.activeTerrain.SampleHeight (new Vector3 (x, 0, z));
			if(y >= yLimit)
				return new Vector3 (x, y + 10, z);
		}
	}

	private Vector3 RandPointFromObject(float offset, GameObject obj, float radius){
		while (true) {
			float randAngle = UnityEngine.Random.Range (0, 360);
			float x = obj.transform.position.x + offset + Mathf.Cos (randAngle * Mathf.Rad2Deg) 
				* UnityEngine.Random.Range(0,radius);
			float z = obj.transform.position.z + offset + Mathf.Sin (randAngle * Mathf.Rad2Deg) 
				* UnityEngine.Random.Range(0,radius);
			float y = Terrain.activeTerrain.SampleHeight (new Vector3 (x, 0, z));
			if(y >= yLimit)
				return new Vector3 (x, y + 10, z);
		}
	}
	public string GetData(){
		string rtn = "";
		rtn += timeStamp + ", ";
		rtn += player.GetComponent<INPC> ().health + ", ";
		rtn += player.GetComponentInChildren<TankTurretHumanController> ().getScore () + ", ";
		rtn += companion.GetComponent<INPC> ().health + ", ";
		rtn += companion.GetComponentInChildren<TankTurretAI> ().getScore () + ", ";
		rtn += dist/(float)loopCount + ", ";
		rtn += player.transform.GetComponentInChildren<TankTurretHumanController> ().targets.Count + ", ";
		rtn += needHelp + "\n";
		dist = 0.0f;
		loopCount = 0;
		needHelp = false;
		return rtn;
	}

	public void GetHelp(){
		needHelp = true;
	}
}
