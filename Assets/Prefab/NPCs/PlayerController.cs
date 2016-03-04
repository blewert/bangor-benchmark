using UnityEngine;
using System.Collections;
using UnityEngine.UI;//Needed for access to UI variable and class methods
using System.Linq; // allows Where clause

public class PlayerController : PrimitiveScript {
	
	public int ammoPerClip = 30;
	private Vector3 movement;
	private Animator anim;
	private Rigidbody playerRigidbody;
	private float animSpeed = 1.5f;
	private HumansController controller;
	private Gun gun;
	private int currentAmmo;
	private float nextFire = 0.0f;
	private bool dead = false;

	void Awake(){
		Cursor.visible = false;

		anim = GetComponent<Animator> ();
		playerRigidbody = GetComponent<Rigidbody>();
		controller = transform.GetComponent<HumansController>();
	}

	void Start () 

	{	
		// Find the server the game is running on
		findNetworkServer ();

		gun = GetComponentInChildren<Gun>();

		gun.gunType = Gun.GunType.SINGLE_SHOT; // Do not change this confirms that the shooting works.

		gun.OnBulletHit += OnBulletHit;

		currentAmmo = ammoPerClip;

		attachLocomotionScripts ();

	}

	// In attachLocomotionScripts have:
	public void attachLocomotionScripts()
	{
		// This will get all players inc npcs
		var objs = GameObject.FindGameObjectsWithTag ("Player");
		//Run through every local NPC and attach the locomotion script so we can
		//do shit with them (this is on the player's machine)
		foreach (var npc in objs) {
			npc.AddComponent<HumansController> ();
		}
		
	}
	
	void FixedUpdate(){	

		// Death stuff
		if(controller.getHealth() <= 0.0f && dead == false){
			dead = true;
			onDeath();
			Invoke ("respawn", 5.0f);
		}

		float v = Input.GetAxis("Vertical");				// setup v variables as our vertical input axis
		anim.SetFloat("Speed", v);							// set our animator's float parameter 'Speed' equal to the vertical input axis				

		// send all clients the speed param for the animations for this agent
		int agentToUpdateIdx = network.characters.Where(x => x.Value.Equals(transform.gameObject)).FirstOrDefault().Key;	
		// Send which agent's speed to update and the speed to update it with.
		network.networkView.RPC("updateSpeed", RPCMode.Others, agentToUpdateIdx, anim.GetFloat("Speed"));

		if (!anim.GetBool ("Dead")) {
			Move ();
			Turning ();
		}
	}
	
	void Move(){
		if (Input.GetKey (KeyCode.UpArrow)) {
			controller.moveForward ();
		}

		if (Input.GetKey (KeyCode.DownArrow)) {
			controller.moveBackward ();
		}
	}

	void Turning(){

		/*if(Input.GetAxis("Mouse X")<0){
			//Code for action on mouse moving left
			controller.turnLeft ();
		}
		if(Input.GetAxis("Mouse X")>0){
			//Code for action on mouse moving right
			controller.turnRight();
		}*/
		float sensitivityOfX = 2F;
		float dx = Input.GetAxis("Mouse X");
		dx = dx * Mathf.Abs(dx); // squares mouse x while keeping the polarity
		controller.humanTurn (dx, sensitivityOfX);
	}

	void OnGUI(){
		int screenWidth = Screen.width;
		// Creates a Crosshair for the player.
		GUI.Box(new Rect(Screen.width/2,Screen.height/2, 10, 10), "");
		// set colour to black
		Color colour = GUI.color;
		colour.r = 0.0f; colour.g = 0.0f; colour.b = 0.0f; GUI.color = colour;

		// Offset from the middle of the screen so it is in middle.
		int offset = -50;

		// health UI
		if (transform.GetComponent<ILocomotionScript> ().getHealth () > -0.01) {
			GUI.Label (new Rect (offset + screenWidth/2, 10, 100, 20), "Health: ");
			GUI.Label (new Rect (offset +(screenWidth/2) + 50, 10, 100, 20), 
			          transform.GetComponent<ILocomotionScript> ().getHealth ().ToString ());
		} else { // if they are shooting still when already dead
			GUI.Label (new Rect (offset +screenWidth/2, 10, 100, 20),"Health: ");
			GUI.Label (new Rect (offset +(screenWidth/2) + 50, 10, 100, 20), "0");
		}

		// ammo UI
		GUI.Label (new Rect ( offset +(screenWidth/2) + 100, 10, 100, 20),"Current Ammo: ");
		GUI.Label(new Rect( offset +(screenWidth/2) + 200, 10, 100, 20), currentAmmo.ToString());
	}

	// -------shooting stuff ( until next hypen line )

	// Update is called once per frame
	void Update ()
	{
		if(Input.GetKeyDown(KeyCode.RightControl)) { // reload button pressed.
			if (currentAmmo != ammoPerClip) {
				anim.SetBool ("Firing", false);

				// send all clients the animation param for the animations for this agent
				int agentToUpdateIdx = network.characters.Where(x => x.Value.Equals(transform.gameObject)).FirstOrDefault().Key;	
				// Send which agent's to update and the animation parameter to update.
				network.networkView.RPC("updateBool", RPCMode.Others, agentToUpdateIdx, "Firing", anim.GetBool("Firing"));

				StartCoroutine("reloadAmmo");
			}
		}

		if (!anim.GetBool ("Dead") && Input.GetMouseButton (0)) { // left click down
			// Makes it so that it only does the coroutine once before stopping. (Prevents firing all clip at once!)
			if(anim.GetBool("Reloading") != true){
				StartCoroutine(shoot(gun.delayBetweenShots));
				StopCoroutine("shoot");
			}
		}
		else if (!Input.GetMouseButton (0)){ // left click up
			anim.SetBool ("Firing", false);

			// send all clients the animation param for the animations for this agent
			int agentToUpdateIdx = network.characters.Where(x => x.Value.Equals(transform.gameObject)).FirstOrDefault().Key;	
			// Send which agent's to update and the animation parameter to update.
			network.networkView.RPC("updateBool", RPCMode.Others, agentToUpdateIdx, "Firing", anim.GetBool("Firing"));
		}
	}

	IEnumerator shoot(float delay){

		// left click down, not out of ammo and it has been long enough since last shot.
		if (Time.time > nextFire && currentAmmo > 0) { 
			anim.SetBool ("Firing", true);

			// send all clients the animation param for the animations for this agent
			int agentToUpdateIdx = network.characters.Where(x => x.Value.Equals(transform.gameObject)).FirstOrDefault().Key;	
			// Send which agent's to update and the animation parameter to update.
			network.networkView.RPC("updateBool", RPCMode.Others, agentToUpdateIdx, "Firing", anim.GetBool("Firing"));

			nextFire = Time.time + delay; // recalculate next possible shot
			OnBulletHit(gun.Fire()); // shoots a bullet
			currentAmmo -= 1; // deduct one ammo from the player.
			//Debug.Log (currentAmmo);
		}
		else if(currentAmmo <= 0){ // out of ammo
			anim.SetBool ("Firing", false);

			// send all clients the animation param for the animations for this agent
			int agentToUpdateIdx = network.characters.Where(x => x.Value.Equals(transform.gameObject)).FirstOrDefault().Key;	
			// Send which agent's to update and the animation parameter to update.
			network.networkView.RPC("updateBool", RPCMode.Others, agentToUpdateIdx, "Firing", anim.GetBool("Firing"));

			StartCoroutine("reloadAmmo");
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
			hit.transform.GetComponent<ILocomotionScript> ().takeHealth (25.0f);
			Debug.Log(hit.transform.name + ": " + hit.transform.GetComponent<ILocomotionScript> ().getHealth() + " life left.");

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

	public void respawn(){
		// set's the health of the human back to it's max health.
		controller.setHealth (controller.getMaxHealth());
		Debug.Log ("After revive:" + controller.health);

		// send all clients the speed param for the animations for this agent
		int agentToUpdateIdx = network.characters.Where(x => x.Value.Equals(transform.gameObject)).FirstOrDefault().Key;

		// Make the player invisible until they respawn
		GetComponent<Renderer>().gameObject.SetActive (false);
		network.networkView.RPC("networkWideShowOrHideAgent", RPCMode.Others, agentToUpdateIdx, false);
		anim.SetBool ("Dead", false);

		// Send which agent's to update and the animation parameter to update. (GLOBALLY)
		network.networkView.RPC("updateBool", RPCMode.Others, agentToUpdateIdx, "Dead", anim.GetBool("Dead"));

		dead = false;
		transform.position = new Vector3(Random.Range(200,300), -0.02072453f , Random.Range(200,300));
		// Move forward (or any movement with updatePosition() in it) to fire the network update.
		controller.moveForward ();
		// Make the agent visible again.
		GetComponent<Renderer>().gameObject.SetActive(true);
		network.networkView.RPC("networkWideShowOrHideAgent", RPCMode.Others, agentToUpdateIdx, true);

		// reset ammo to full
		currentAmmo = ammoPerClip;

	}

	public void onDeath(){
		anim.SetTrigger("Dying");

		// send all clients the speed param for the animations for this agent
		int agentToUpdateIdx = network.characters.Where(x => x.Value.Equals(transform.gameObject)).FirstOrDefault().Key;

		// Send which agent's to update and the animation parameter to update.
		network.networkView.RPC("setTheTrigger", RPCMode.Others, agentToUpdateIdx, "Dying");

		anim.SetBool ("Dead", true);

		// Send which agent's to update and the animation parameter to update. (GLOBALLY)
		network.networkView.RPC("updateBool", RPCMode.Others, agentToUpdateIdx, "Dead", anim.GetBool("Dead"));
	}

	IEnumerator reloadAmmo()
	{
		anim.SetBool ("Reloading", true);

		// send all clients the param for the animations for this agent
		int agentToUpdateIdx = network.characters.Where(x => x.Value.Equals(transform.gameObject)).FirstOrDefault().Key;	
		// Send which agent's to update and the animation parameter to update.
		network.networkView.RPC("updateBool", RPCMode.Others, agentToUpdateIdx, "Reloading", anim.GetBool("Reloading"));

		Debug.Log ("Reloading!");
		currentAmmo = ammoPerClip;
		// wait for a bit for the animation to finish before resetting the bool to false.
		yield return new WaitForSeconds(2.6f);
		anim.SetBool ("Reloading", false);

		// Send which agent's to update and the animation parameter to update.
		network.networkView.RPC("updateBool", RPCMode.Others, agentToUpdateIdx, "Reloading", anim.GetBool("Reloading"));
	} 
	//--------------------------------------------------------------

}
