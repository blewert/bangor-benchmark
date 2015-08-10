using UnityEngine;
using System.Collections;

public class NPCcontroler : IFlyableNPC 
{
	
	////////////////////////////
	// Velocity Vairables
	public float maxSpeed = 7f; // The Max speed in X or Z axis
	public float acceleration = 0.05f; // The acceleration in any axis
	public float rotationSpeed = 1.5f; // The rotation speed arround the Y axis
	public float drag = 0.1f; // The drag force

	/* Private */
	private float currentSpeed = 0; // Current speed in the Z axis
	private float strafeCurrentSpeed = 0; // Current speed in the X axis
	private float heightCurrentSpeed = 0; // Current speed in the Y axis
	private bool dragActivator = false; // Apply drag force on Z
	private bool strafeDragActivator = false; // Apply drag force on X
	private bool heightDragActivator = false; // Apply drag force on Y
	private bool moving = false;
	
	////////////////////////////
	// Steering Behaviour Vairables
	public float safeDistance = 20f; // The safe distance for a flee function

	/* Private */
	private Transform theTarget; // The target used for seek and flee

	////////////////////////////
	// Hover Vairables
	public bool hoverWobbleEnabled = true;
	public float hoverHeight = 5f; // The height the agent should hover at
	public float hoverDrift = 1f; // The amount the agent should drife during hover
	public float driftResponce = 0.6f; // How often we change the hover drift position
	public float hoverDriftSpeed = 0.1f; // How fast the drone drifts
	public float minHoverHeight = 0.3f;

	/* Private */
	public float hoverAdjust; // The speed at which we should change hover height
	private float hoverHeightModifier; // The vertical modifier for the hover wobble function
	private float fastAdjust = 2f; // Change Height quickly if we are at the wrong height
	private float slowAdjust = 0f; // Dont change (or change slowly) if we are within the error range, let hoverWobble function take over
	private float heightHoverPos = 0; // The vertical height including the hover wobble
	private Vector3 hoverDriftPos; // The transform that we drift towards during hoverwobble
	private Vector3 hoverPos; // The position being hovered arround

	////////////////////////////
	// The Model - Animation Vairables
	private Transform model;
	private float turnCurrentTilt;
	private bool turnTilt;

	////////////////////////////
	// Debug Vairables


	// Use this for initialization
	void Start () {

		// Set the model
		model = GameObject.Find("model").transform;

		// Set initial hover position
		hoverPos = transform.position;

		StartCoroutine ("HoverWobble");

		// Quickly move to the correct hover height
		hoverAdjust = fastAdjust; 
	}
	
	// Update is called once per frame
	void FixedUpdate () {

		// Is the drone hovering at the correct height
		CheckHeight ();

		transform.Translate (new Vector3 (strafeCurrentSpeed, heightCurrentSpeed, currentSpeed) * Time.deltaTime);

		// Tilt the model based on movement
		float sideTilt = (turnCurrentTilt * 0.7f) + (strafeCurrentSpeed * 0.5f);
		model.localRotation = Quaternion.Euler (currentSpeed * 4f, 0, (sideTilt * 3f) * -1);

		// Apply Drag if not moving and update hoverWobble
		applyDrag ();

		// Kill the rigedbody velocity... otherwise the kinematic animation doesnt work;
		Rigidbody rigidbody = GetComponent<Rigidbody>();
		
		if(rigidbody == null)
			return;
		
		GetComponent<Rigidbody>().velocity = new Vector3(0, 0, 0);
		GetComponent<Rigidbody>().angularVelocity = new Vector3(0, 0, 0);
		GetComponent<Rigidbody>().freezeRotation = true;

	}

	/////////////////////////////////////////////
	/// --Turning Functions-- (PUBLIC)
	/// Rotates the agent arround the Y axis
	/////////////////////////////////////////////
	
	// TurnLeft
	public override void turnLeft()
	{
		//Quaternion rotateComponent = Quaternion.Euler (Vector3.up * rotationSpeed);
		Quaternion rotateComponent = Quaternion.Euler (Vector3.up * -(rotationSpeed * Mathf.Sign (currentSpeed))); 
		transform.rotation *= rotateComponent;

		// Tilt the model
		if (turnCurrentTilt > 0) // We are traveling the other direction, add drag
			turnCurrentTilt -= drag;
	
		turnCurrentTilt -= acceleration;
		turnCurrentTilt = Mathf.Clamp (turnCurrentTilt, -5, 0);
		
		turnTilt = false;
	}
	
	// TurnRight
	public override void turnRight() 
	{
		//Debug.Log (currentSpeed);
		
		//Quaternion rotateComponent = Quaternion.Euler (Vector3.up * rotationSpeed);
		Quaternion rotateComponent = Quaternion.Euler (Vector3.up * (rotationSpeed * Mathf.Sign (currentSpeed))); 
		transform.rotation *= rotateComponent;

		// Tilt the model
		if (turnCurrentTilt > 0) 
			turnCurrentTilt += drag;
	
			turnCurrentTilt += acceleration; 
		turnCurrentTilt = Mathf.Clamp (turnCurrentTilt, 0, 5);

		turnTilt = false;
	}

	/////////////////////////////////////////////
	/// Movement Functions (PUBLIC)
	/// Allows the drone to move along one axis (x or z)
	/////////////////////////////////////////////

	// Forward
	public override void moveForward()
	{
		accelerate ();
		moving = true; 
		dragActivator = false;
	}
	
	public override void moveBackward() 
	{
		decelerate ();
		moving = true;
		dragActivator = false;
	}
	
	public override void strafeRight() 
	{
		strafeRightAccelerate ();
		moving = true;
		strafeDragActivator = false;
	}
	
	public override void strafeLeft() 
	{
		strafeLeftAccelerate ();
		moving = true;
		strafeDragActivator = false;
	}
	
	public override void ascend() {
		hoverHeight += acceleration;
	}
	
	public override void descend() {
		hoverHeight -= acceleration;
	}


	/////////////////////////////////////////////
	/// Power Functions (PRIVATE)
	/// Allows the drone to accelerate along one axis (x or z)
	/////////////////////////////////////////////

	// Forward Z
	void accelerate () 
	{	
		if (currentSpeed < 0) // We are traveling the other direction, add drag
			currentSpeed += drag;
	
			currentSpeed += acceleration;
		currentSpeed = Mathf.Clamp (currentSpeed, -maxSpeed, maxSpeed);
	}

	// Backward Z
	void decelerate () 
	{
		if (currentSpeed > 0) // We are traveling the other direction, add drag
			currentSpeed -= drag;
	
		currentSpeed -= acceleration; 
		currentSpeed = Mathf.Clamp (currentSpeed, -maxSpeed, maxSpeed);
	}

	// Up Y
	void heightAccelerate ()
	 {
		if (heightCurrentSpeed < 0) // We are traveling the other direction, add drag
			heightCurrentSpeed += drag;
		heightCurrentSpeed += acceleration;
		heightCurrentSpeed = Mathf.Clamp (heightCurrentSpeed, -maxSpeed, maxSpeed);
		heightDragActivator = false;
		moving = true;
	}
	
	// Down Y
	void heightDecelerate () {
		if (heightCurrentSpeed > 0) // We are traveling the other direction, add drag
			heightCurrentSpeed -= drag;
		heightCurrentSpeed -= acceleration; 
		heightCurrentSpeed = Mathf.Clamp (heightCurrentSpeed, -maxSpeed, maxSpeed);
		heightDragActivator = false;
		moving = true;
	}

	// Right X
	void strafeRightAccelerate () {
		if (strafeCurrentSpeed < 0) // We are traveling the other direction, add drag
			strafeCurrentSpeed += drag;
		strafeCurrentSpeed += acceleration;
		strafeCurrentSpeed = Mathf.Clamp (strafeCurrentSpeed, -maxSpeed, maxSpeed);
	}

	// Left X
	void strafeLeftAccelerate () {
		if (strafeCurrentSpeed > 0) // We are traveling the other direction, add drag
			strafeCurrentSpeed -= drag;
		strafeCurrentSpeed -= acceleration; 
		strafeCurrentSpeed = Mathf.Clamp (strafeCurrentSpeed, -maxSpeed, maxSpeed);
	}

	/////////////////////////////////////////////
	/// Drag Functions (PRIVATE)
	/// Applys Drag and sets hoverWobble when power not being applied
	/////////////////////////////////////////////

	//bool oldMoving = true;
	
	void applyDrag() {

		// Check if the agent is currently hovering
		// if so, apply some hoverWobble
		if (!moving) {
			HoverDrift ();
		}
		
		//oldMoving = moving;
		
		// Apply the drag along the Z axis (forwards and backwards)
		if (dragActivator) {
			float speedError = FindDifference (currentSpeed, 0);
			if (speedError > 0.01) {
				if (currentSpeed > 0) {
					currentSpeed -= drag;
				}
				if (currentSpeed < 0) {
					currentSpeed += drag;
				}
			} 
		} 
		
		// Apply the drag along the X axis (strafe)
		if (strafeDragActivator) {
			float speedError = FindDifference (strafeCurrentSpeed, 0);
			if (speedError > 0.01) {
				if (strafeCurrentSpeed > 0) {
					strafeCurrentSpeed -= drag;
				}
				if (strafeCurrentSpeed < 0) {
					strafeCurrentSpeed += drag;
				}
			} 
		}
		
		// Apply the drag along the Y axis (height)
		if (heightDragActivator) {
			float speedError = FindDifference (heightCurrentSpeed, 0);
			if (speedError > 0.01) {
				if (heightCurrentSpeed > 0) {
					heightCurrentSpeed -= drag;
				}
				if (heightCurrentSpeed < 0) {
					heightCurrentSpeed += drag;
				}
			} 
		}

		// Reset the turning tilt
		if (turnTilt) {
			float tiltError = FindDifference (turnCurrentTilt, 0);
			if (tiltError > 0.01) {
				if (turnCurrentTilt > 0) {
					turnCurrentTilt -= drag;
				}
				if (turnCurrentTilt < 0) {
					turnCurrentTilt += drag;
				}
			} 
		}

		
		// If we are moving in the Z or X update the hover pos
		// position, so we wobble in the correct location when 
		// we stop moving - applies a spring to pull the agent back
		// to where it stopped moving.
		if (moving) {
			// Transform the game object
			hoverPos = transform.position;
		}
		
		// Switch the Drag Activator On, we will switch off in the 
		// movement functions
		dragActivator = true;
		strafeDragActivator = true;
		heightDragActivator = true;
		turnTilt = true;
		moving = false;
	}

	/////////////////////////////////////////////
	/// --Steering Behaviour Functions-- (PUBLIC)
	/// Currently only Seek and Flee included in the script
	/// steering behaviours MUST be implemented with the same
	/// turning and velocity limits as the manual controls
	/// to avoid any bias. 
	/// Further behaviours to be inluded in updated releases
	/////////////////////////////////////////////

	public void SetTarget (Transform inTarget) {
		theTarget = inTarget;
	}

	// Seek
	public void Seek () {

		dragActivator = false;

		// Set the ideal vector
		Vector3 direction = theTarget.position - transform.position;

		Vector3 relativePoint = transform.InverseTransformPoint (theTarget.position);

		if (relativePoint.x < 0)
			turnLeft ();
			
		if (relativePoint.x > 0)
			turnRight ();

		// Evaluate Distance
		float distance = Vector2.Distance (new Vector2 (transform.position.x, transform.position.z), new Vector2 (theTarget.position.x, theTarget.position.z));

		// Speed up if the distance is greater than the maximum velocity
		if (currentSpeed < distance)
			accelerate ();
		if (currentSpeed > distance)
			decelerate ();

		// Move if we are not there already. 
		if(direction.magnitude > 0.001f){
			Vector3 moveVector = direction.normalized * currentSpeed * Time.deltaTime;
		}
	}
	
	// Flee
	public void Flee () {
		
		dragActivator = false;

		// Set the ideal vector
		Vector3 direction = theTarget.position - transform.position;
		
		Vector3 relativePoint = transform.InverseTransformPoint (theTarget.position);
		
		if (relativePoint.x > 0)
			turnLeft ();
		if (relativePoint.x < 0)
			turnRight ();

		float distance = Vector2.Distance (new Vector2 (transform.position.x, transform.position.z), new Vector2 (theTarget.position.x, theTarget.position.z));

		if (direction.magnitude < safeDistance) {
			if (currentSpeed < distance)
				accelerate ();
			if (currentSpeed > distance)
				decelerate ();
		}

	}

	/////////////////////////////////////////////
	/// ---Hover Wobble functions-- (PRIVATE)
	/// When the drone is stationary (hovering), if it stays perfectly still
 	/// it looks unrealistic, however, we dont want it to move away from its 
	/// stationary position. HoverWobble simulates the micro-corrections a 
	/// gyroscope/accelerometer would make to maintain a geo-stationary position.
	/// 
	/// To switch off the hover wobble 
	/// hoverDrift = 0
	/////////////////////////////////////////////

	// Simulates Positional Correction
	void HoverDrift() 
	{
		if(hoverWobbleEnabled)
			transform.position = Vector3.Lerp (transform.position, hoverDriftPos, Time.deltaTime * hoverDriftSpeed);
	}

	// Creates new hover wobble gremlins 
	IEnumerator HoverWobble() 
	{
		while (true) 
		{
			float xDrift = Random.Range(hoverDrift * -1, hoverDrift);
			float zDrift = Random.Range(hoverDrift * -1, hoverDrift);
			float yDrift = Random.Range((hoverDrift/2) * -1, (hoverDrift/2));

			hoverDriftPos = new Vector3(hoverPos.x + xDrift, hoverPos.y + yDrift, hoverPos.z + zDrift);

			yield return new WaitForSeconds (driftResponce);
		}
	}

	/////////////////////////////////////////////
	/// --Check Height Function-- (PRIVATE)
	/// The CheckHeight function ensures that the 
	/// agent allways hovers above the ground 
	/// and adds some vertical hover wobble if required
	/////////////////////////////////////////////
	void CheckHeight() {

		// The distance to the ground from the drone
		float distanceToGround;

		// A vairable to hold the raycast data
		RaycastHit hit;

		if (hoverHeight < minHoverHeight)
						hoverHeight = minHoverHeight;
		
		//Debug.DrawLine (transform.position, transform.position + Vector3.down * 150f);
		
		// Cast a ray to the ground - check the height
		// If we want the drone to hover higher than 150 units
		// we need to increase the cast length.. 
		
		Vector3 start = transform.position;
		Vector3 end = transform.position + (Vector3.down * 150f);
		
		Ray ray = new Ray(transform.position,  end - start);
		
		Debug.DrawLine (ray.origin, ray.direction * 150f);
		
		//Physics.Raycast (transform.position, transform.position + Vector3.down, out hit, 150f) 
		if(Physics.Raycast (ray, out hit, 150f))
		//if( Physics.Raycast(transform.position, Vector3.down, out hit, 150.0F))
			distanceToGround = hit.distance;
		else {
			distanceToGround = 150;
		}

		// The hoverError is the difference between the ideal hover height
		// and the actual hover height
		float hoverError = FindDifference (distanceToGround, hoverHeight);

		if (distanceToGround < hoverHeight) {
			if (hoverError > 0.2) {
				heightAccelerate ();
			}
		}
		if (distanceToGround > hoverHeight) {
			if (hoverError > 0.2) {
				heightDecelerate ();
			}
		}
	}
	
	/////////////////////////////////////////////
	/// --Helper Functions-- (PRIVATE)
	/////////////////////////////////////////////
	float FindDifference(float v1, float v2)
	{
		return Mathf.Abs(v1 - v2);
	}
	

	void getKeyboardInput() {
		// Turn Left with left arrow key
		/*if (Input.GetKey ("left")) {
			turnLeft();
		}
		// Turm Right with right arrow key
		if (Input.GetKey ("right")) {
			turnRight();
		}
		// Move Forward with up arrow key
		if (Input.GetKey ("up")) {
			moveForward();
		}
		// Move Backwards with down arrow key
		if (Input.GetKey ("down")) {
			moveBackward();
		}
		// Strafe Right with X key
		if (Input.GetKey ("x")) {
			strafeRight();
		}
		// Strafe LEft with Z key
		if (Input.GetKey ("z")) {
			strafeLeft();
		}
		// Strafe Right with X key
		if (Input.GetKey ("q")) {
			ascend();
		}
		// Strafe LEft with Z key
		if (Input.GetKey ("a")) {
			descend();
		}*/
	}

}
