using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class CharacterButtonListener : MonoBehaviour 
{
	public void OnButtonClick(Button b, Mode mode)
	{
		string name = b.GetComponentInChildren<Text>().text;
		
		var path = mode.supportedCharacters.Find (x => name == x.name).path;
		
		StartButtonListener.charToLoad = path;
	}
}
