using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Linq;

/// <summary>
/// The start-up script for loading dynamically loading in primitives/instances; provides logic for MenuHandler script.
/// </summary>
public class Bootup : MonoBehaviour 
{
	//The menu object and it's MenuHandler component
	[Header("Bootup menu options")]
	public GameObject menu;
	private MenuHandler menuComponent;
	
	//Environment primitives and instances
	private EnvironmentPrimitive[] environmentPrimitives;
	private EnvironmentInstance[] environmentInstances;
	
	
	// Use this for initialization
	void Start () 
	{
		//Get all environment primitives and instances
		environmentPrimitives = PrimitivesParser.getEnvironmentPrimitives();
		environmentInstances  = PrimitivesParser.getEnvironmentInstances();

		//Does the menu object exist?
		if(menu == null)
			throw new UnityException("The menu object which has been specified is null.");
		
		//Instantiate the menu
		menu = Instantiate (menu, Vector3.zero, Quaternion.identity) as GameObject;
			
		//If so, get it's menu handler script (for the UI changing functionality)
		menuComponent = menu.GetComponent<MenuHandler>();
		
		//Does this exist?
		if(menuComponent == null)
			throw new UnityException("Could not find attached MenuHandler script in menu object.");
		
		//Do the left/right buttons exist?
		if(menuComponent.leftButton == null || menuComponent.rightButton == null)
			throw new UnityException("Left/right navigation buttons for MenuHandler script are not set.");
		
		//If everything is fine, register left/right button handlers and hook view update event
		menuComponent.registerNavigationButtonHandlers(this);
		menuComponent.onViewUpdate += onViewUpdate;
		
		//Add the initial primitive buttons (initially the environment buttons)
		menuComponent.addPrimitivesButtons(environmentPrimitives.Select(x => x.name), this);
		
		/*foreach(var primitive in primitives)
		{
			var linkedInstances = instances.Where (x => primitive.name == x.primitiveName);
			
			foreach(var instance in linkedInstances)
			{
				Debug.Log("Instance linked to " + primitive.name + " -> " + instance.name);
			}
		}
		
		var environment = gameObject.AddComponent<ForestPrimitive>();
		environment.instance = instances[0]; //change to selected value*/
		
	} 
	
	/// <summary>
	/// Callback when a primitives button is clicked in the menu. This could be an environment primitive button, a gamemode primitive button or a character primitive button.
	/// </summary>
	/// <param name="button">The button which was clicked.</param>
	/// <param name="buttonText">The text of the button.</param>
	public void onPrimitivesButtonClick(Button button, string buttonText)
	{
		//Debug.Log ("Primitive button clicked: " + buttonText);
		
		var associatedInstances = environmentInstances.Where(x => x.primitiveName.Equals(buttonText));
		
		menuComponent.removeInstanceButtons();
		menuComponent.addInstancesButtons(associatedInstances.Select (x => x.name), this);		
	}
	
	/// <summary>
	/// Callback when a instances button is clicked in the menu. This could be an environment instance button, a gamemode instance button or a character instance button.
	/// </summary>
	/// <param name="button">The button which was clicked.</param>
	/// <param name="buttonText">The text of the button.</param>
	public void onInstancesButtonClick(Button button, string buttonText)
	{
		//Debug.Log ("Instance button clicked: " + buttonText);
	}
	
	/// <summary>
	/// Called when the left arrow of the menu is clicked.
	/// </summary>
	public void onLeftArrowClick()
	{	
		menuComponent.moveView (-1);
	}
	
	/// <summary>
	/// Called when the right arrow of the menu is clicked.
	/// </summary>
	public void onRightArrowClick()
	{	
		menuComponent.moveView (+1);
	}
	
	/// <summary>
	/// Called when the view has updated from it's old value.
	/// </summary>
	/// <param name="newView">For convenience, the new view value</param>
	public void onViewUpdate(MenuHandler.View view)
	{
		//At this point, all the buttons in the menu have been deleted.
		//..
		
		if(view == MenuHandler.View.ENVIRONMENT)
		{
			//Environment
			menuComponent.addPrimitivesButtons(environmentPrimitives.Select (x => x.name), this);
		}
		else if(view == MenuHandler.View.GAMEMODE)
		{
			//Add gamemode buttons...
		}
		else if(view == MenuHandler.View.CHARACTER)
		{
			//Add character buttons...
		}
	}
	
}
