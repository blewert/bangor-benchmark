using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SliderListener : MonoBehaviour 
{
	public Text textToChange;

	public void OnSliderChanged(float value)
	{
		textToChange.text = value.ToString ();		
		StartButtonListener.numberOfPlayers = (int)value;
	}
}
