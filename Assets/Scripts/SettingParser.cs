using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;


class SettingParser
{
	public enum SettingType
	{
		STRING_TEXT, STRING_PATH, STRING_CHOICE,
		FLOAT, FLOAT_RANGE, FLOAT_CHOICE,
		INTEGER, INTEGER_RANGE, INTEGER_CHOICE,
		BOOLEAN, COLOR, COLOR_CHOICE,
		INVALID
	}
	
	private static string[] matchStrings =
	{
		"string/text", "string/path", "string/choice",
		"float/scalar", "float/range", "float/choice",
		"int/scalar", "int/range", "int/choice",
		"boolean", "color/scalar", "color/choice",
		"terrain"
	};
	
	private static object parseColor(string input)
	{
		input = input.Substring(1);
		
		int value = Convert.ToInt32(input, 16);
		
		float r = ((value >> 16) & 0xff) / 255f;
		float g = ((value >> 8) & 0xff) / 255f;
		float b = ((value >> 0) & 0xff) / 255f;
		
		//return new Color(r, g, b);
		return new Color(r, g, b);
	}
	 
	private static string getTypeFromPrimitive(Instance instance, string settingName) 
	{
		if(instance is EnvironmentInstance)
		{
			var primitive = EnvironmentPrimitivesParser.getLastPrimitives().First (x => x.name == instance.primitiveName);
			return primitive.possibleSettings[settingName];
		}
		else if(instance is GamemodeInstance)
		{
			var primitive = GamemodePrimitivesParser.getLastPrimitives().First (x => x.name == instance.primitiveName);
			return primitive.possibleSettings[settingName];
		}		
		else if(instance is CharacterInstance)
		{
			var primitive = CharacterPrimitivesParser.getLastParsedPrimitives().First (x => x.name == instance.primitiveName);
			return primitive.possibleSettings[settingName];
		}
		
		return null;
	}
	
	public static Vector3 getTerrainOriginPoint(Terrain terrain)
	{
		foreach(Transform ter in terrain.transform)
		{
			if(ter.tag.Equals("Origin"))
				return ter.position;
		}
		
		return Vector3.zero;
	}
	
	/// <summary>
	/// Parses a given setting value from an instance's XML file. Returns the data to be used for usage with scripts.
	/// </summary>
	/// <returns>The setting value if valid (the value adheres to the type given), otherwise null.</returns>
	/// <param name="instance">The instance which the lookup is for.</param>
	/// <param name="settingName">The name of the setting for lookup.</param>
	public static object getSetting(Instance instance, string settingName)
	{
		string stype = getTypeFromPrimitive(instance, settingName);
		string input = instance.settings[settingName];
		
		SettingType type = getSettingType(stype);
				
		object returnValue = null;
		
		switch (type)
		{
			//Scalars
			case SettingType.INTEGER:
			case SettingType.FLOAT:
			case SettingType.BOOLEAN:
			case SettingType.COLOR:
			case SettingType.STRING_PATH:
			case SettingType.STRING_TEXT:
				returnValue = getScalarSetting(input, type);
				break;
				
			//Ranges
			case SettingType.FLOAT_RANGE:
			case SettingType.INTEGER_RANGE:
				returnValue = getRangeSetting(input, stype, type);
				break;
				
			//Choices
			//...
			case SettingType.COLOR_CHOICE:
			case SettingType.FLOAT_CHOICE:
			case SettingType.INTEGER_CHOICE:
			case SettingType.STRING_CHOICE:
				returnValue = getChoiceSetting(input, stype, type);
				break;

		}
				
		if(returnValue == null)
			throw new SettingTypeInvalidException("Setting type '" + stype + "' for name '" + settingName + "' has an invalid value, or was unmatched.");
			
		else
			return returnValue;		
	}
	
	private static object getChoiceSetting(string input, string stype, SettingType type)
	{
		//match choices depending on type - using dif regex
		//see if that 
		
		string regexToApply = @"";
		
		//Colors, Floats, Integers, Strings
		if (type == SettingType.INTEGER_CHOICE || type == SettingType.FLOAT_CHOICE)
			regexToApply = @"(\d+\.?\d*)\s*(,|})";
		
		else
			regexToApply = "\"(.+?)\"\\s*(,|})";
		
		Regex regex = new Regex(regexToApply);
		
		var matches = regex.Matches(stype);
		
		if (matches.Count <= 0)
			return null;
		
		object[] valueArray = new object[matches.Count];
		object comparisonValue = input;
		
		for (int i = 0; i < matches.Count; i++)
		{
			var term = matches[i].Groups[1].Value;
			
			switch (type)
			{
			case SettingType.INTEGER_CHOICE:
				valueArray[i]   = int.Parse(term);
				comparisonValue = int.Parse(input);
				break;
				
			case SettingType.FLOAT_CHOICE:
				valueArray[i]   = float.Parse(term);
				comparisonValue = float.Parse(input);
				break;
				
			case SettingType.COLOR_CHOICE:
				valueArray[i]   = parseColor(term);
				comparisonValue = parseColor(input);
				break;
				
			case SettingType.STRING_CHOICE:
				valueArray[i]   = term;
				comparisonValue = input;
				break;
				
			default:
				return null;
			}
		}
				
		var containsValue = valueArray.Contains(comparisonValue);
		
		if (containsValue != true)
			return null;
		else
			return comparisonValue;
	}
	
	private static object getRangeSetting(string input, string stype, SettingType type)
	{
		string regexToApply = "";
		
		if (type == SettingType.INTEGER_RANGE)
			regexToApply = @"(\d+)\s*,\s*(\d+)";
		else
			regexToApply = @"(\d+\.?\d*)\s*,\s*(\d+\.?\d*)";
		
		Regex regex = new Regex(regexToApply);
		
		var matches = regex.Matches(stype);
		
		//Range is invalid in prim xml
		if (matches.Count <= 0)
			return null;
		
		float v1 = float.Parse(matches[0].Groups[1].Value);
		float v2 = float.Parse(matches[0].Groups[2].Value);
		
		float min = Math.Min(v1, v2);
		float max = Math.Max(v1, v2);
		
		float comparisonInput = float.Parse(input);
		
		if (type == SettingType.INTEGER_RANGE)
		{
			if (comparisonInput >= min && comparisonInput <= max)
				return int.Parse(input);
		}
		else
		{
			if (comparisonInput >= min && comparisonInput <= max)
				return comparisonInput;
		}
		
		return null;
	}
	
	private static object getScalarSetting(string input, SettingType type)
	{
		switch (type)
		{
		case SettingType.INTEGER:
			return int.Parse(input);
			
		case SettingType.FLOAT:
			return float.Parse(input);
			
		case SettingType.BOOLEAN:
			return input.ToLower().Equals("true");
			
		case SettingType.STRING_TEXT:
		case SettingType.STRING_PATH:
			return input;
			
		case SettingType.COLOR:
			return parseColor(input);
			
		default:
			return null;
		}
	}
	
	private static SettingType getSettingType(string input)
	{
		var values = Enum.GetValues(typeof(SettingType));
		
		for (int i = 0; i < matchStrings.Length; i++)
		{
			if (input.StartsWith(matchStrings[i]))
			{
				string name = Enum.GetName(typeof(SettingType), i);
				
				return (SettingType)Enum.Parse(typeof(SettingType), name);
			}
		}
		
		return SettingType.INVALID;
	}
	
	public class SettingTypeInvalidException : Exception
	{
		public SettingTypeInvalidException() : base() { }
		public SettingTypeInvalidException(string message) : base(message) { }
	}
}
