using UnityEngine;
using System.Collections;
using System.Linq;
using System.Collections.Generic;

public class HumanEnemyAI : PrimitiveScript {

	private Rigidbody npcRigidbody;
	private HumansController controller;
	private Castdar cast;
	private Animator anim;
	private bool backoff = false;
	private Vector3 oldPlayerPos;
	private Vector3 localPos;
	private string shootingStyle = "not assigned yet";
	private Gun gun;
	private int currentAmmo;
	private float nextFire = 0.0f;
	private bool attackingHuman = false;
	private bool dead = false;
	private bool highCOT;

	private CharacterInstance characterInstance;

	// wander variables -----
	// number of seconds between each wander direction recalculation
	public float directionChangeInterval = 1;
	// largest alteration in angle from last recalculation
	public float maxHeadingChange = 30;
	// agents heading
	float heading;
	Vector3 targetRotation;	
	// -----

	public float aggressiveness = 0.0f; // how aggressive the NPC's play style is.
	public float COT = 0.0f; // complexity of tactics.
	public float IPD = 5.0f; // Interpersonal distance.
	public float reactionTime = 100.0f; // higher reactionTime, longer time before updating target.
	public int ammoPerClip = 30;

	// Use this for initialization
	void Start () {
		// Find the server the game is running on
		findNetworkServer ();

		anim = GetComponent<Animator> ();
		npcRigidbody = GetComponent<Rigidbody>();
		controller = transform.GetComponent<HumansController>();

		characterInstance = controller.instance;
		aggressiveness = (float)SettingParser.getSetting(characterInstance, "Aggressiveness");
		COT = (float)SettingParser.getSetting(characterInstance, "COT");
		IPD = (float)SettingParser.getSetting(characterInstance, "IPD");
		reactionTime = (float)SettingParser.getSetting(characterInstance, "Reaction Time");
		ammoPerClip = (int)SettingParser.getSetting(characterInstance, "Ammo Per Clip");

		// get castDar for sight and set the variables.
		cast = GetComponentInChildren<Castdar> ();
		cast.visionAngle = (int)SettingParser.getSetting(characterInstance, "Vision Angle");
		cast.radarRange = (float)SettingParser.getSetting (characterInstance, "Radar Range");
		cast.refreshObjectScan = (float)SettingParser.getSetting (characterInstance, "Refresh Object Scan");
		cast.refreshWallScan = (float)SettingParser.getSetting (characterInstance, "Refresh Wall Scan");

		gun = GetComponentInChildren<Gun>();
		gun.OnBulletHit += OnBulletHit;
		currentAmmo = ammoPerClip;

		// gen COT behaviour choice
		float cotChecker = Random.Range (0, 100);
		//Debug.Log (cotChecker);
		highCOT = (cotChecker < COT);
	}


	void Awake ()
	{
		// initialise wander functionality.
		StartCoroutine(NewHeading());
	}
	
	// Update is called once per frame
	void Update () {

		// -----------Death and respawn code--------------
		if(controller.getHealth() <= 0.0f && dead == false){
			dead = true;
			onDeath();
			Invoke ("respawn", 5.0f);
		}
		//------------------------------------------------
		// If the agent is not dead
		if (!anim.GetBool ("Dead")) {

			// Not attacking someone as of yet.
			if (attackingHuman == false) { 

				// I will react when I do see someone.
				if (reactionDiceRoll ()) {

					// Do I see someone? yes, then...
					if (canISeeAHumanBool ()) {

						// Find and persue the target.
						attackingHuman = true;
						checkIfFoundPlayer ();

					// Do I see someone, no, then...
					} else {
						attackingHuman = false;

						wander ();
					}
				
				// I will not react fast enough to see someone.
				} else if (!reactionDiceRoll ()) {
					// so carry on my business.
					wander ();
				}

			// found someone with reactions beforehand so keep attacking until cannot see them alive anymore.
			} else if(attackingHuman == true && canISeeAHumanBool ()){ 

				checkIfFoundPlayer ();

			} else if(attackingHuman == true && !canISeeAHumanBool ()){
				attackingHuman = false;

				wander ();
			}
		}
	}

	bool reactionDiceRoll(){
		float reactionChecker = Random.Range (0, reactionTime);
		//Debug.Log (reactionChecker);
		// if it a value that is less than or equal to 1 is generated.
		return (reactionChecker <= 1);
	}

	void persueStateActivate(Vector3 playerCoords, float distBtwnPlayerAndNPC){
		// Go towards player.
		controller.moveForward ();
		// if player is on the right side of the ray cast.
		if (playerCoords.x >= 0) {
			controller.turnRight ();
		} else { // if on left side of the raycast.
			controller.turnLeft ();
		}
	}
	
	void maintainGap(Vector3 playerCoords, float distBtwnPlayerAndNPC){
		controller.moveBackward ();
	}
	
	void resetBackoff()
	{
		backoff = false;
	}
	
	void resetSpeed(){
		anim.SetFloat ("Speed", 0);
	}
	
	void behaviourIPD(Vector3 playerCoords, float distBtwnPlayerAndNPC, System.Collections.Generic.IEnumerable<Castdar.HitObject> playersHit){
		if (distBtwnPlayerAndNPC > IPD) { 
			if(shootingStyle != "defensive"){ // aggressive	
				if(backoff == false){
					if(anim.GetFloat("Speed") < 1)
						anim.SetFloat ("Speed", anim.GetFloat("Speed") + 0.1f);
					
					persueStateActivate (playerCoords, distBtwnPlayerAndNPC);
				}
				else{ // backoff == true
					// Check if player position has changed since backing off
					if(transform.TransformPoint(localPos) != oldPlayerPos){
						Invoke ("resetBackoff", 3f);
					}
					// Stops NPC walking animation continuously.
					Invoke ("resetSpeed", 0.5f);
				}
			}else{ // defensive
				anim.SetFloat ("Speed", 0.0f);
			}
		}
		else { // <-- distBtwnPlayerAndNPC <= IPD)
			backoff = true;
			// getting the position of the player before it moves again.
			oldPlayerPos = transform.TransformPoint(localPos);
			if(backoff == true){
				// begin to back away
				maintainGap (playerCoords, distBtwnPlayerAndNPC);
				if(anim.GetFloat("Speed") > -1){
					CancelInvoke("resetSpeed");
					anim.SetFloat ("Speed", anim.GetFloat("Speed") - 0.1f);
				}
			}
		}
	}
	// ------------------------------------------------------------------------
	
	void wander(){

		if(anim.GetFloat("Speed") < 1)
			anim.SetFloat ("Speed", anim.GetFloat("Speed") + 0.1f);

		transform.eulerAngles = Vector3.Slerp(transform.eulerAngles, targetRotation, Time.deltaTime * directionChangeInterval);
		controller.moveForward ();

	}

	
	/// <summary>
	/// Repeatedly calculates a new direction to move towards.
	/// Use this instead of MonoBehaviour.InvokeRepeating so that the interval can be changed at runtime.
	/// </summary>
	IEnumerator NewHeading ()
	{
		while (true) {
			NewHeadingRoutine();
			yield return new WaitForSeconds(directionChangeInterval);
		}
	}
	
	/// <summary>
	/// Calculates a new direction to move towards.
	/// </summary>
	void NewHeadingRoutine ()
	{
		var floor = Mathf.Clamp(heading - maxHeadingChange, 0, 360);
		var ceil  = Mathf.Clamp(heading + maxHeadingChange, 0, 360);
		heading = Random.Range(floor, ceil);
		targetRotation = new Vector3(0, heading, 0);
	}

	System.Collections.Generic.IEnumerable<Castdar.HitObject> canISeeAHuman(){
		// list of all objects that are hit.
		var objectHit = cast.GetSeenObject();
		// list all objects with the tag Player or objects which parents have tag Player.
		var playersHit = objectHit.Where (x => x.seenOBJ.tag.Equals("Player") || x.seenOBJ.transform.root.tag.Equals("Player") || x.seenOBJ.tag.Equals("NPC") || x.seenOBJ.transform.root.tag.Equals("NPC"));
		// returns a list of all humans it can see at that time.
		return playersHit;
	}

	bool canISeeAHumanBool(){
		// returns a list of all humans it can see at that time.
		return (canISeeAHuman().Count() > 0);
	}


	void checkIfFoundPlayer(){

		//Debug.Log (highCOT);

		// get a list of all the humans that are seen at this iteration.
		System.Collections.Generic.IEnumerable<Castdar.HitObject> playersHit = canISeeAHuman ();

		//------
		// gets vectors from NPC to enemies seen
		Vector3[] playersSeenVectors = new Vector3[playersHit.Count()];
		// gets angle of NPC to enemies seen
		float[] playersSeenAngles = new float[playersSeenVectors.Count()];
		// if this stays at 0 then it has not found anyone directly in front and has yet to make a compromise.
		float distance = 0.0f;

		if (!highCOT) {
			// go for ahead seen enemy
			// first, get product to see if anyone is directly in front of NPC.
			for(int i = 0; i < playersHit.Count(); i++){ 
				// get the angles of each enemy to the NPC
				playersSeenVectors[i] = playersHit.ElementAt(i).seenOBJ.transform.position - transform.position;
				playersSeenAngles[i] = Mathf.Abs(Vector3.Angle(transform.forward, playersSeenVectors[i]));
			}
			//go after opponent who is the closest to being in front of the NPC = minimum angle from NPC.
			//Debug.Log(playersSeenAngles.Min());
			int enemyToAttackIndex = playersSeenAngles.ToList().IndexOf(playersSeenAngles.Min());
			// position of the enemy chosen to attack
			localPos = transform.InverseTransformPoint (playersHit.ElementAt(enemyToAttackIndex).seenOBJ.transform.position);
			// Distance between the NPC and the opponent chosen to attack.
			distance = Vector3.Distance (playersHit.ElementAt(enemyToAttackIndex).seenOBJ.transform.position, transform.position);

		}
		else{ //if (highCOT) {
			//go for closest enemy seen
			// position of the player that has been seen.
			localPos = transform.InverseTransformPoint (playersHit.FirstOrDefault ().seenOBJ.transform.position);
			// Distance between the NPC and the player that has been seen.
			distance = Vector3.Distance (playersHit.FirstOrDefault ().seenOBJ.transform.position, transform.position);
		}
		// -------------------------

		// Set aggressiveness mode.
		if(shootingStyle == "not assigned yet"){
			aggressivenessBehaviour();
		}

		// aggressive = run at enemy, defensive = stop and shoot from distance.
		behaviourIPD (localPos, distance, playersHit);

		// shooting stuff
		// Got ammo? yes, then shoot!
		if(currentAmmo > 0 && anim.GetBool("Reloading") != true){
			StartCoroutine(shoot(gun.delayBetweenShots));
			StopCoroutine("shoot");
		}

		else if(currentAmmo <= 0){ // out of ammo
			anim.SetBool ("Firing", false);
			StartCoroutine("reloadAmmo");
		}
	}
	// ------------------------Shooting helper methods -----------------------------------

	void setShootingStyle (string fireType){
		shootingStyle = fireType;
	}

	void aggressivenessBehaviour(){
		// generate a number between 0 and 100.
		float moodFinder = Random.Range (0.0F, 100.0F);
		//Debug.Log (moodFinder);
		// set the shooting type, more likely to be aggressive shooting with aggressiveness being higher but not gauranteed.
		if (moodFinder < aggressiveness) {
			//do aggression
			setShootingStyle("allOrNothing");
		} else {
			//do conservative
			setShootingStyle("defensive");
		}
	}
	
	IEnumerator shoot(float delay){
		// left click down, not out of ammo and it has been long enough since last shot.
		if (Time.time > nextFire && currentAmmo > 0) { 
			anim.SetBool ("Firing", true);
			nextFire = Time.time + delay; // recalculate next possible shot
			OnBulletHit(gun.Fire()); // shoots a bullet
			currentAmmo -= 1; // deduct one ammo from the player.
			//Debug.Log (currentAmmo);
		}
		// Wait delay amount before attempting to fire a new shot after restarting the coroutine.
		yield return new WaitForSeconds(delay);
	} 
	
	public void OnBulletHit(RaycastHit[] hitData)
	{
		if(hitData.Length == 0)
			return;
		
		RaycastHit hit = hitData[0];
		
		Debug.Log ("Name of hit object: " + hit.transform.name);
		
		// Damage the player or npc that has been hit.
		if (hit.transform.tag.Equals ("Player") || hit.transform.tag.Equals ("NPC")) {
			//Debug.Log(hit.transform.tag);
			// Take health from the opponent LOCALLY
			hit.transform.GetComponent<ILocomotionScript> ().takeHealth (10.0f);
			//Debug.Log(hit.transform.name + ": " + hit.transform.GetComponent<ILocomotionScript> ().getHealth() + " life left.");

			// Update person you hit's health server-wide
			// Here is where the agent that was hit will be stored from the network.
			GameObject agentWhoWasHit;
			// Before you can send who has been hit for their health to be reduced and updated over the network
			// You must get the ID of the character.
			int agentWhoHasBeenHitIdx = network.characters.Where(x => x.Value.Equals(hit.transform.gameObject)).FirstOrDefault().Key;	
			Debug.Log("The ID for the agent that was hit was: " + agentWhoHasBeenHitIdx);
			// Send who was hit to the server and have the server take the health from the agent being hit on all other sessions.
			network.networkView.RPC("takeHealthAndUpdate", RPCMode.Others, agentWhoHasBeenHitIdx);
		}
	}
	
	[RPC]
	public void takeHealthAndUpdate(int whoHasBeenHit){
		network.characters[whoHasBeenHit].GetComponent<ILocomotionScript> ().takeHealth (10.0f);
	}

	public void respawn(){
		// set's the health of the human back to it's max health.
		controller.setHealth (controller.getMaxHealth());
		//Debug.Log ("After revive:" + controller.health);
		
		// Make the player invisible until they respawn
		GetComponent<Renderer>().gameObject.SetActive (false);
		anim.SetBool ("Dead", false);
		dead = false;
		transform.position = new Vector3(Random.Range(200,300), -0.02072453f , Random.Range(200,300));
		// Move forward (or any movement with updatePosition() in it) to fire the network update.
		controller.moveForward ();
		// Make the agent visible again.
		GetComponent<Renderer>().gameObject.SetActive(true);
	}
	
	IEnumerator reloadAmmo()
	{
		anim.SetBool ("Reloading", true);
		//Debug.Log ("Reloading!");
		currentAmmo = ammoPerClip;
		// wait for a bit for the animation to finish before resetting the bool to false.
		yield return new WaitForSeconds(2.6f);
		anim.SetBool ("Reloading", false);
	} 


	public void onDeath(){
		anim.SetTrigger("Dying");
		anim.SetBool ("Dead", true);
	}

	// ------------------------End of Shooting helper methods -----------------------------------

}