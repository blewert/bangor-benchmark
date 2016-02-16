using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour {

	private Vector3 movement;
	private Animator anim;
	private Rigidbody playerRigidbody;
	private float animSpeed = 1.5f;
	private HumansController controller;
	private Gun gun;
	private int currentAmmo;
	private float nextFire = 0.0f;
	private bool dead = false;

	public int ammoPerClip = 30;

	void Awake(){
		anim = GetComponent<Animator> ();
		playerRigidbody = GetComponent<Rigidbody>();
		controller = transform.GetComponent<HumansController>();
	}

	void Start () 
	{
		gun = GetComponentInChildren<Gun>();

		gun.gunType = Gun.GunType.SINGLE_SHOT; // Do not change this confirms that the shooting works.

		gun.OnBulletHit += OnBulletHit;

		currentAmmo = ammoPerClip;
	}
	
	void FixedUpdate(){	

		// Death stuff
		if(controller.getHealth() <= 0.0f && dead == false){
			dead = true;
			onDeath();
			Invoke ("respawn", 5.0f);
		}


		float h = Input.GetAxis("Horizontal");				// setup h variable as our horizontal input axis
		float v = Input.GetAxis("Vertical");				// setup v variables as our vertical input axis
		anim.SetFloat("Speed", v);							// set our animator's float parameter 'Speed' equal to the vertical input axis				
		//anim.SetFloat("Direction", h); 						// set our animator's float parameter 'Direction' equal to the horizontal input axis		
		anim.speed = animSpeed;								// set the speed of our animator to the public variable 'animSpeed'

		if (!anim.GetBool ("Dead")) {
			Move ();
			Turning ();
		}
	}
	
	void Move(){
		if (Input.GetKey (KeyCode.W)) {
			controller.moveForward ();
		}
		if (Input.GetKey (KeyCode.S)) {
			controller.moveBackward ();
		}
	}

	void Turning(){

		if(Input.GetAxis("Mouse X")<0){
			//Code for action on mouse moving left
			controller.turnLeft ();
		}
		if(Input.GetAxis("Mouse X")>0){
			//Code for action on mouse moving right
			controller.turnRight();
		}

		//anim.SetFloat("Direction", Input.GetAxis("Mouse X"));

		/**if(Input.GetKey(KeyCode.A))
			controller.turnLeft ();
		if(Input.GetKey(KeyCode.D))
			controller.turnRight();*/
	}

	void OnGUI(){
		// Creates a Crosshair for the player.
		GUI.Box(new Rect(Screen.width/2,Screen.height/2, 10, 10), "");
	}

	// -------shooting stuff ( until next hypen line )

	// Update is called once per frame
	void Update () 
	{
		if(Input.GetKeyDown(KeyCode.R)) { // reload button pressed.
			if (currentAmmo != ammoPerClip) {
				anim.SetBool ("Firing", false);
				StartCoroutine("reloadAmmo");
			}
		}

		if (Input.GetMouseButton (0)) { // left click down

			// Makes it so that it only does the coroutine once before stopping. (Prevents firing all clip at once!)
			if(anim.GetBool("Reloading") != true){
				StartCoroutine(shoot(gun.delayBetweenShots));
				StopCoroutine("shoot");
			}
		}
		else if (!Input.GetMouseButton (0)){ // left click up
			anim.SetBool ("Firing", false);
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
		else if(currentAmmo <= 0){ // out of ammo
			anim.SetBool ("Firing", false);
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
			hit.transform.GetComponent<ILocomotionScript> ().takeHealth (10.0f);
			Debug.Log(hit.transform.name + ": " + hit.transform.GetComponent<ILocomotionScript> ().getHealth() + " life left.");
		}
	}

	public void respawn(){
		// set's the health of the human back to it's max health.
		controller.setHealth (controller.getMaxHealth());
		Debug.Log ("After revive:" + controller.health);

		// Make the player invisible until they respawn
		GetComponent<Renderer>().gameObject.SetActive (false);
		anim.SetBool ("Dead", false);
		dead = false;
		transform.position = new Vector3(Random.Range(-22,22), -0.02000004f , Random.Range(-22,22));
		GetComponent<Renderer>().gameObject.SetActive(true);
	}

	public void onDeath(){
		anim.SetTrigger("Dying");
		anim.SetBool ("Dead", true);
	}

	IEnumerator reloadAmmo()
	{
		anim.SetBool ("Reloading", true);
		Debug.Log ("Reloading!");
		currentAmmo = ammoPerClip;
		// wait for a bit for the animation to finish before resetting the bool to false.
		yield return new WaitForSeconds(2.6f);
		anim.SetBool ("Reloading", false);
	} 
	//--------------------------------------------------------------

}
