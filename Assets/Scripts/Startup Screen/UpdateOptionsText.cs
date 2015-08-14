using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UpdateOptionsText : MonoBehaviour 
{
	private Text component;
	
	void Start()
	{
		component = GetComponent<Text>();
	}
	
	void Update()
	{
		string builtString = "";
		
		if(StartButtonListener.charToLoad != null)
			builtString += "NPC prefab: " + StartButtonListener.charToLoad + "\n\n";
		
		if(StartButtonListener.sceneToLoad != null)
			builtString += "Scene to load: " + StartButtonListener.sceneToLoad;
			
		if(builtString == "")
			builtString = "(none)";
			
		component.text = builtString;
	}
}
