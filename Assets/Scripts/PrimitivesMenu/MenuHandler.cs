using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Menu handler class for the start-up primitives/instances selection menu. Performs UI interaction and provides events/callbacks 
/// to be hooked by the start-up script.
/// </summary>
using System;


public class MenuHandler : MonoBehaviour
{
	[Header("Menu header labels")]
	public RectTransform primitivesMenuLabel;
	public RectTransform instancesMenuLabel;
	
	[Header("UI component settings")]
	public RectTransform settingsPanelText;
	public RectTransform errorText;
	public Button startButton;
	
	[Header("Navigation buttons")]
	public Button leftButton;
	public Button rightButton;
	public Toggle multiplayerToggle;
	
	[Header("Button options")]
	public Button buttonPrefab;
	public float buttonPadding;
	
	//The two types of buttons/columns in the menu
	public enum MenuButtonType
	{
		PRIMITIVES, INSTANCES
	};
	
	//Lists of the primitive and instance buttons for usage by other scripts
	private List<Button> primitiveButtons = new List<Button>();
	private List<Button> instanceButtons = new List<Button>();
			
	//The possible view options
	public enum View
	{
		ENVIRONMENT, GAMEMODE, CHARACTER
	}
	
	//The current "view" and view index - which type of menu is showing? characters, environment, gamemodes?
	private View currentView;
	private int currentViewIndex;
	
	//Event to hook when the view is updated.
	public delegate void ViewUpdateHandler(View view);
	public event ViewUpdateHandler onViewUpdate;

	/// <summary>
	/// Sets the error text for the menu, when something goes wrong.
	/// </summary>
	/// <param name="text">The text to set to.</param>
	public void setErrorText(string text)
	{
		errorText.GetComponentInChildren<Text>().text = text;
	}
	
	/// <summary>
	/// Gets the header label for either the primitives or instances column.
	/// </summary>
	/// <returns>The relevant header label, as an object.</returns>
	/// <param name="buttonType">The type of button grouped in this column (either primitives or instances).</param>
	public RectTransform getHeaderLabel(MenuButtonType buttonType)
	{
		if(buttonType == MenuButtonType.PRIMITIVES)
			return primitivesMenuLabel;
		
		else if(buttonType == MenuButtonType.INSTANCES)
			return instancesMenuLabel;
		
		return null;
	}
	
	/// <summary>
	/// Updates the settings panel, sets to an empty string.
	/// </summary>
	public void updateSettingsPanel()
	{
		settingsPanelText.GetComponentInChildren<Text>().text = "";
	}
	
	/// <summary>
	/// Updates the settings panel.
	/// </summary>
	/// <param name="pairs">The key/value setting pairs to display.</param>
	public void updateSettingsPanel(Instance environment, Instance gamemode, Instance character)
	{
		//Get the text component, set it to an empty string so we dont append
		var textBox = settingsPanelText.GetComponentInChildren<Text>();
		textBox.text = "";
		
		if(environment != null)
		{
			textBox.text += "Selected environment:\n---------------------\n";
			textBox.text += "Name: " + environment.name + "\n";
			
			//Environment settings:
			foreach(var pair in environment.settings)
				textBox.text += (pair.Key + ": " + pair.Value) + "\n";
				
			textBox.text += "\n";
		}
		
		if(gamemode != null)
		{
			textBox.text += "Selected gamemode:\n------------------\n";
			textBox.text += "Name: " + gamemode.name + "\n";
			
			//Gamemode settings:
			foreach(var pair in gamemode.settings)
				textBox.text += (pair.Key + ": " + pair.Value) + "\n";
				
			textBox.text += "\n";
		}
		
		if(character != null)
		{
			textBox.text += "Selected character:\n-------------------\n";
			textBox.text += "Name: " + character.name + "\n";
			
			//Character settings:
			foreach(var pair in character.settings)
				textBox.text += (pair.Key + ": " + pair.Value) + "\n";
				
			textBox.text += "\n";
		}
	}
	
	/// <summary>
	/// Sets up click callbacks for the navigation buttons, from a referring script.
	/// </summary>
	/// <param name="referrer">The script which wishes to hook the navigation buttons click event.</param>
	public void registerNavigationButtonHandlers(Bootup referrer)
	{
		//Left button
		leftButton.onClick.AddListener(delegate
		{
			referrer.onLeftArrowClick();		
		});
		
		//Right button
		rightButton.onClick.AddListener(delegate
		{
			referrer.onRightArrowClick();		
		});
		
		//Start button
		startButton.onClick.AddListener(delegate 
		{
			referrer.onStartButtonClick();
		});
		
		//Toggle button
		multiplayerToggle.onValueChanged.AddListener(delegate(bool value)
		{
			GameObject.Find("NetworkManager").GetComponent<NetworkServer>().isMultiplayer = value;
		});
	}
	
	
	/// <summary>
	/// Adds a number of primitive buttons underneath the correct header. A button is created and it's text is set for every string passed into this function.
	/// </summary>
	/// <param name="strs">The strings for each button.</param>
	/// <param name="referrer">The referring script to hook the each button's callback.</param>
	public void addPrimitivesButtons(IEnumerable<string> strs, Bootup referrer)
	{
		//Just call the array method by converting IEnumerable to an array
		addPrimitivesButtons(strs.ToArray(), referrer);
	}


	/// <summary>
	/// Adds a number of instance buttons underneath the correct header. A button is created and it's text is set for every string passed into this function.
	/// </summary>
	/// <param name="strs">The strings for each button.</param>
	/// <param name="referrer">The referring script to hook the each button's callback.</param>
	public void addInstancesButtons(IEnumerable<string> strs, Bootup referrer)
	{
		//Just call the array method by converting IEnumerable to an array
		addInstancesButtons(strs.ToArray(), referrer);
	}
	
	
	/// <summary>
	/// Adds a number of primitive buttons underneath the correct header. A button is created and it's text is set for every string passed into this function.
	/// </summary>
	/// <param name="strs">The strings for each button.</param>
	/// <param name="referrer">The referring script to hook the each button's callback.</param>
	public void addPrimitivesButtons(string[] buttonTexts, Bootup referrer)
	{				
		//Get the start label to offset each button from, and find the starting position.
		var startLabel = getHeaderLabel(MenuButtonType.PRIMITIVES);
		Vector3 startPosition = startLabel.transform.position;
		
		for(int i = 0; i < buttonTexts.Length; i++)
		{
			//Run through each button string, and instantiate a button for this 
			var button = (Button)Instantiate(buttonPrefab, startPosition, Quaternion.identity);
			
			//Set this menu as the button's root, and set the text of the button to this string.
			button.gameObject.transform.SetParent (transform);
			button.GetComponentInChildren<Text>().text = buttonTexts[i];
			
			//Get the button's position, height and the height of the starting label
			var position = button.transform.position;
			var height = button.GetComponent<RectTransform>().rect.height;
			var startLabelHeight = startLabel.rect.height;
			
			//Calculate this button's position:
			//Start label height + offset + (the amount of buttons so far, combined height) + padding
			position.y -= (startLabelHeight + buttonPadding) + (height * i) + buttonPadding;
			
			//Hook the callback
			button.onClick.AddListener(delegate
			{ 
				referrer.onPrimitivesButtonClick(button, button.GetComponentInChildren<Text>().text);
			});
			
			//Finally, set the button's position properly and add it to the list of primitive buttons.
			button.transform.position = position;	
			primitiveButtons.Add(button);
		}
	}
	
	
	/// <summary>
	/// Adds a number of instance buttons underneath the correct header. A button is created and it's text is set for every string passed into this function.
	/// </summary>
	/// <param name="strs">The strings for each button.</param>
	/// <param name="referrer">The referring script to hook the each button's callback.</param>
	public void addInstancesButtons(string[] buttonTexts, Bootup referrer)
	{
		//Get the start label to offset each button from, and find the starting position.
		var startLabel = getHeaderLabel(MenuButtonType.INSTANCES);
		Vector3 startPosition = startLabel.transform.position;
			
		for(int i = 0; i < buttonTexts.Length; i++)
		{
			//Run through each button string, and instantiate a button for this string.
			var button = (Button)Instantiate(buttonPrefab, startPosition, Quaternion.identity);
			
			//Set this menu as the button's root, and set the text of the button to this stri
			button.transform.SetParent (transform);
			button.GetComponentInChildren<Text>().text = buttonTexts[i];
			
			//Get the button's position, height and the height of the starting label
			var position = button.transform.position;
			var height = button.GetComponent<RectTransform>().rect.height;
			var startLabelHeight = startLabel.rect.height;
			
			//Calculate this button's position:
			//Start label height + offset + (the amount of buttons so far, combined height) + padding
			position.y -= (startLabelHeight + buttonPadding) + (height * i) + buttonPadding;
			
			//Hook the callback
			button.onClick.AddListener(delegate
			{ 
				referrer.onInstancesButtonClick(button, button.GetComponentInChildren<Text>().text);
			});
			
			//Finally, set the button's position properly and add it to the list of primitive buttons.
			button.transform.position = position;	
			instanceButtons.Add(button);
		}
	}
	
	
	/// <summary>
	/// Removes all primitive buttons.
	/// </summary>
	public void removePrimitiveButtons()
	{
		//Destroy all buttons
		for(int i = 0; i < primitiveButtons.Count; i++)
			Destroy (primitiveButtons[i].gameObject);
		
		//Clear list
		primitiveButtons.Clear ();
	}
	
	
	/// <summary>
	/// Removes all instance buttons.
	/// </summary>
	public void removeInstanceButtons()
	{
		//Destroy all buttons
		for(int i = 0; i < instanceButtons.Count; i++)
			Destroy (instanceButtons[i].gameObject);
		
		//Clear list
		instanceButtons.Clear ();
	}
	
	
	/// <summary>
	/// Gives a view enum type from a specific index.
	/// </summary>
	/// <returns>A enum type (View) for this specific index.</returns>
	/// <param name="index">The index into the enum.</param>
	private View getViewFromIndex(int index)
	{
		return (new View[] { View.ENVIRONMENT, View.GAMEMODE, View.CHARACTER })[index];
	}
	
	
	/// <summary>
	/// Gives a header label string from a specific index.
	/// </summary>
	/// <returns>A string for a header label, for this specific index.</returns>
	/// <param name="index">The index into the string array.</param>
	private string getHeaderFromIndex(int index)
	{
		return (new string[] { "Environment", "Gamemode", "Character" })[index];
	}
	
	
	/// <summary>
	/// Gets the current view as an enum type.
	/// </summary>
	/// <returns>The current view.</returns>
	public View getCurrentView()
	{
		return currentView;
	}
	
	
	/// <summary>
	/// Updates the view, and the entire menu system, also invokes onUpdateView callback
	/// </summary>
	private void updateView()
	{
		//Remove buttons
		removeInstanceButtons();
		removePrimitiveButtons();
		
		//Update headers
		primitivesMenuLabel.GetComponentInChildren<Text>().text = getHeaderFromIndex(currentViewIndex) + " Primitives";
		instancesMenuLabel.GetComponentInChildren<Text>().text  = getHeaderFromIndex(currentViewIndex) + " Instances";
		
		//Call event to listeners
		onViewUpdate.Invoke(currentView);
	}
	
	
	/// <summary>
	/// Changes the current view depending on whether or not it exceeds the number of possible views.
	/// </summary>
	/// <param name="movement">The movement: -ve values go left, +ve values go right</param>
	public void moveView(int movement)
	{
		//The index hasn't changed. We don't need to move or update.
		if(Mathf.Clamp (currentViewIndex + movement, 0, 2) == currentViewIndex)
			return;
		
		//Get next view to display, and update view index for the next click.
		currentView = getViewFromIndex(Mathf.Clamp (currentViewIndex + movement, 0, 2));
		currentViewIndex += movement;
		
		//Update the view if something has changed
		updateView();
	}
}
