using UnityEngine;
using System.Collections;

public class CompanionIdentity : MonoBehaviour {

	// Robot (true) or human (false)
	private bool species;
	// Male (true) or female (false)
	private bool gender;
	// Child (true) or Adult (false)
	private bool age;
	// String to hold the name of the picture
	private string pictureName;
	// String for the companion name
	private string companionName;

	// Use this for initialization
	void Start () {
		species = (Random.value < 0.5)? true : false;
		gender = (Random.value < 0.5)? true : false;
		age = (Random.value < 0.5)? true : false;
		pictureName = (species) ? "r_" : "h_";
		pictureName += (gender) ? "m_" : "f_";
		pictureName += (age) ? "c" : "a";
	}

	public string GetName(){
		if (species)
			return "A-H-Bot 1153";
		else {
			if(gender)
				return "Timmy";
			else
				return "Daisy";
		}
	}

	public string GetPicName(){
		return pictureName;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
