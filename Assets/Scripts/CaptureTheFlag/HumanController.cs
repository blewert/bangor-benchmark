using UnityEngine;
using System.Linq;
using System.Collections;

public class HumanController : MonoBehaviour 
{
	private enum Team
	{
		RED, BLUE 
	}
	
	private Team myTeam;
	
	private INPC npc; 
	private Gun gun;
	
	private bool hasFlag = false;
	
	public GameObject enemyFlag;
	public GameObject myFlag;
	
	public Vector3 enemyBasePos;
	public Vector3 myBasePos;
	
	private float FLAG_PICKUP_RADIUS = 5.0f;
	
	public float smoothing = 5f;
	public float zoffsetDistance = 5.4f;
	public float yoffsetDistance = 2.1f;
	
	// Use this for initialization
	void Start () 
	{
		npc = GetComponent<INPC>();
		gun = GetComponentInChildren<Gun>();
		
		if(npc.gameObject.tag == "Team1")
			myTeam = Team.RED;
		else
			myTeam = Team.BLUE;
		
		GameObject redFlag = GameObject.FindGameObjectWithTag("Red Flag");
		GameObject blueFlag = GameObject.FindGameObjectWithTag("Blue Flag");
		
		GameObject redBase = GameObject.Find("Red Base");
		GameObject blueBase = GameObject.Find("Blue Base");
		
		if(myTeam == Team.RED)
		{
			enemyFlag = blueFlag;
			enemyBasePos = blueFlag.transform.position;
			
			myFlag = redFlag;
			myBasePos = redFlag.transform.position;
		}
		else
		{
			enemyFlag = redFlag;
			enemyBasePos = redFlag.transform.position;
			
			myFlag = blueFlag;
			myBasePos = blueFlag.transform.position;
		}
	}
	
	// Update is called once per frame
	void FixedUpdate () 
	{
		if(npc.health < 0f)
		{
			if(hasFlag)
			{
				enemyFlag.transform.SetParent(null);
				enemyFlag.transform.position = enemyBasePos;
			}
			
			//logger.addMessage (getNPCName() + " died!");
			
			hasFlag = false;
			
			npc.health = INPC.MAX_HEALTH;
			
			Vector3 position = myBasePos;
			
			position.x += UnityEngine.Random.Range (-10f, 10f);
			position.z += UnityEngine.Random.Range (-10f, 10f);
			
			Vector3 euler = new Vector3(0, UnityEngine.Random.Range (0, 360), 0); 
			
			npc.transform.rotation = Quaternion.Euler(euler);
			npc.transform.position = position;
		}
		
		if(Vector3.Distance (npc.transform.position, enemyFlag.transform.position) < FLAG_PICKUP_RADIUS)
		{
			//logger.addMessage (getNPCName() + " picked up the flag!");
			hasFlag = true;
			enemyFlag.transform.SetParent (npc.transform);
		}
		
		if(hasFlag && Vector3.Distance (npc.transform.position, myBasePos) < FLAG_PICKUP_RADIUS)
		{
			hasFlag = false;
			enemyFlag.transform.SetParent (null);
			enemyFlag.transform.position = myBasePos;
		}
		
		if(Input.GetAxis("Vertical") > 0)
			npc.moveForward();
			
		else if(Input.GetAxis("Vertical") < 0)
			npc.moveBackward();
			
		if(Input.GetAxis ("Horizontal") < 0)
			npc.turnLeft ();
		
		else if(Input.GetAxis ("Horizontal") > 0)
			npc.turnRight();
			
		if(Input.GetButton("Fire1"))
		{
			RaycastHit[] hitpoints = gun.Fire ();
			
			foreach(var point in hitpoints)
			{
				var pointObject = point.transform.gameObject;
				
				if(pointObject.transform.root == npc.transform.root)
					continue;
				
				if(pointObject.transform.root.gameObject.tag.StartsWith("Team") && pointObject.transform.root.gameObject.tag != npc.gameObject.tag)
				{
					//On a team, but not the player's team			
					GameObject hitEnemy = pointObject.transform.root.gameObject;
					var enemyNPC = hitEnemy.GetComponent<INPC>();
					
					enemyNPC.takeHealth (1f/30f);
					
					break;
				}
			}
			
		}
			
		cameraFollow();
	}
	
	private void cameraFollow()
	{
		Vector3 oldPos = Camera.main.transform.position;
		Vector3 newPos = npc.transform.TransformPoint (Vector3.forward * -zoffsetDistance);
		
		newPos.y += yoffsetDistance;
		
		Camera.main.transform.position = Vector3.Lerp (oldPos, newPos, Time.deltaTime * smoothing);
		Camera.main.transform.LookAt (npc.transform.position);
	}
}
