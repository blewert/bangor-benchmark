using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Linq;

public class StartMenu : MonoBehaviour 
{
	public RectTransform leftColumnLabel;
	public RectTransform midColumnLabel;
	public RectTransform rightColumnLabel;
	public Button envButtonPrefab;
	public Button charButtonPrefab;
	
	public int offset;
	
	public void Start()
	{
		var mode = ModeConfigurationParser.modes[0];
		
		for(int i = 0; i < mode.supportedEnvironments.Count(); i++)
		{
			var button = (Button)Instantiate(envButtonPrefab, midColumnLabel.transform.position, Quaternion.identity);
			
			button.transform.SetParent (transform);
			button.GetComponentInChildren<Text>().text = mode.supportedEnvironments[i].name;
			
			var position = button.transform.position;
			
			position.y -= offset + i * offset;
			
			button.onClick.AddListener(() => 
			{ 
				GetComponent<EnvironmentButtonListener>().OnButtonClick();
			});
			
			button.transform.position = position; 
		}
		
		for(int i = 0; i < mode.supportedCharacters.Count(); i++)
		{
			var button = (Button)Instantiate(charButtonPrefab, rightColumnLabel.transform.position, Quaternion.identity);
			
			button.transform.SetParent (transform);
			button.GetComponentInChildren<Text>().text = mode.supportedCharacters[i].name;
			
			var position = button.transform.position;
			
			position.y -= offset + i * offset;
			
			button.onClick.AddListener(() => 
			{ 
				GetComponent<CharacterButtonListener>().OnButtonClick();
			});
			
			button.transform.position = position; 
		}
	}
}
