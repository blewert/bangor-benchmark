/** 
 * @file EvolutionResultsParser.cs
 * ---
 * @author Benjamin Williams <eeu222@bangor.ac.uk>
 * @copyright Bangor University
 * @date 2016
 */
 
using UnityEngine;
using System.Collections;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

/// <summary>
/// Parses evolution and fitness result data from (hl-multiagent/evolution) result files - that is, ini files
/// usually residing in /evolution/results/{run}/{generations.txt, fitnesses.txt}
/// </summary>
public static class EvolutionResultsParser
{
	//Results directory to read in runs
	private static string resultsDirectory;
	
	/// <summary>
	/// Sets the results directory prior to parsing.
	/// </summary>
	/// <param name="dir">The directory.</param>
	public static void setResultsDirectory(string dir)
	{
		resultsDirectory = dir;
	}
	
	/// <summary>
	/// Gets the combined evolution data, including both fitness and genes for an entire generation.
	/// </summary>
	/// <returns>The combined evolution data for a generation and run.</returns>
	/// <param name="runNumber">The specified run number.</param>
	/// <param name="generation">The generation number</param>
	public static Dictionary<string, EvolutionAgent> getEvolutionData(int runNumber, int generation)
	{
		var generationData = getGenerationData(runNumber, generation);
		var fitnessData    = getFitnessData(runNumber, generation);
		
		//Return data: agent key => (genes, fitness)
		var returnData = new Dictionary<string, EvolutionAgent>();
		
		//Map keys to genes and fitness data
		foreach(var key in generationData.Keys)
			returnData.Add (key, new EvolutionAgent(generationData[key], fitnessData[key]));

		//And return the return data
		return returnData;
	}
	
	/// <summary>
	/// Gets the fitness data for a specified generation and run number.
	/// </summary>
	/// <returns>The fitness data for a specified generation.</returns>
	/// <param name="runNumber">The specified run number.</param>
	/// <param name="generation">The specified generation of that run number.</param>
	private static Dictionary<string, float> getFitnessData(int runNumber, int generation)
	{
		//The return value for parsing - a key of agents => genes
		var returnValue = new Dictionary<string, float>();
		
		//Find available generations and select the first which matches the specified generation
		var availableGenerations = getAvailableGenerations(runNumber, "fitnesses.txt");
		var selectedGeneration = availableGenerations.First (x => x.first == generation);
		
		using (var fileStream = File.OpenRead(resultsDirectory + "/" + runNumber + "/fitnesses.txt"))
		{
			//Line count and temp var for line
			int lineCount = 0;
			string line;
			
			using (var streamReader = new StreamReader(fileStream, Encoding.UTF8, true, 128)) 
			{
				while ((line = streamReader.ReadLine()) != null)
				{
					//Have we skipped to the right line yet?
					if(lineCount++ <= selectedGeneration.second)
						continue;
					
					//Is it a key value?
					if(line.StartsWith("[") || line == "")
						break;
					
					//If not, it requires parsing.
					Regex re = new Regex(@"(.+)\s=\s(.+)");
					
					//Match
					var match = re.Match(line);
					
					//Grab key and value from parsing, add to return values
					var key   = match.Groups[1].Value;
					var value = float.Parse (match.Groups[2].Value);
					//--
					returnValue.Add (key, value);
				}
			}
		}
		
		//Return the list of agent => genes
		return returnValue;
	}
	
	/// <summary>
	/// Gets all the genes for a specific generation and run number.
	/// </summary>
	/// <returns>The genes for all agents in this generation.</returns>
	/// <param name="runNumber">The specified run number.</param>
	/// <param name="generation">The generation to choose.</param>
	public static Dictionary<string, int[]> getGenerationData(int runNumber, int generation)
	{
		//The return value for parsing - a key of agents => genes
		var returnValue = new Dictionary<string, int[]>();
		
		//Find available generations and select the first which matches the specified generation
		var availableGenerations = getAvailableGenerations(runNumber, "generations.txt");
		var selectedGeneration = availableGenerations.First (x => x.first == generation);
				
		using (var fileStream = File.OpenRead(resultsDirectory + "/" + runNumber + "/generations.txt"))
		{
			//Line count and temp var for line
			int lineCount = 0;
			string line;
			
			using (var streamReader = new StreamReader(fileStream, Encoding.UTF8, true, 128)) 
			{
				while ((line = streamReader.ReadLine()) != null)
				{
					//Have we skipped to the right line yet?
					if(lineCount++ <= selectedGeneration.second)
						continue;
					
					//Is it a key value?
					if(line.StartsWith("[") || line == "")
						break;
						
					//If not, it requires parsing.
					Regex re = new Regex(@"(.+)\s=\s(.+)");
					
					//Do matching
					var match = re.Match(line);
					
					//Find key and values from line
					var key   = match.Groups[1].Value;
					var value = match.Groups[2].Value.Replace(" ", "").Split(',').Select (x => int.Parse(x)).ToArray();
					
					//Add the parsed key and value
					returnValue.Add (key, value);
				}
			}
		}
		
		//Return the list of agent => genes
		return returnValue;
	}
	
	/// <summary>
	/// Gets the available generations for a specific run.
	/// </summary>
	/// <returns>The available generations for this run.</returns>
	/// <param name="runNumber">The run number to specify.</param>
	public static List<Pair<int, int>> getAvailableGenerations(int runNumber, string file)
	{		
		//Return values for the number of available generations.
		var returnValues = new List<Pair<int, int>>();
		
		//Line number count
		int lineNumber = 0;
		
		using (var fileStream = File.OpenRead(resultsDirectory + "/" + runNumber + "/" + file))
		{
			using (var streamReader = new StreamReader(fileStream, Encoding.UTF8, true, 128)) 
			{
				string line;
				
				while ((line = streamReader.ReadLine()) != null)
				{
					if(line.StartsWith("["))
					{
						//We've found a key value, extract with regex
						Regex re = new Regex(@"\[.+\s(\d+)\]");
						
						//Match and add to return list
						var match = re.Match(line);
						
						//Make a tuple
						var pair = new Pair<int, int>();
						
						//Add the first and second elements (first being the generation num, the next being the line num)
						pair.first  = int.Parse (match.Groups[1].Value);
						pair.second = lineNumber;
						
						//Add to return values
						returnValues.Add(pair);
					}
					
					//Increment line count
					lineNumber++;
				}
			}
		}
		
		return returnValues;
	}
	
}

/// <summary>
/// Represents an evolutionary agent found in the results - this agent has a calculated fitness
/// and a set of genes which are numerical indices into another set of actions.
/// </summary>
public class EvolutionAgent
{
	//The internal data for this wrapper
	private Pair<int[], float> data = new Pair<int[], float>();
	
	/// <summary>
	/// Gets or sets the genes for this evolutionary agent.
	/// </summary>
	/// <value>The genes to set/get.</value>
	public int[] genes 
	{
		get { return data.first;  }
		set { data.first = value; }
	}
	
	/// <summary>
	/// Gets or sets the fitness for this evolutionary agent.
	/// </summary>
	/// <value>The fitness to set/get.</value>
	public float fitness
	{
		get { return data.second;  }
		set { data.second = value; }
	}
	
	/// <summary>
	/// Initializes a new evolutionary agent result (see <see cref="EvolutionAgent"/>).
	/// </summary>
	/// <param name="genes">The genes for this agent.</param>
	/// <param name="fitness">The fitness of this agent.</param>
	internal EvolutionAgent(int[] genes, float fitness)
	{
		data.first = genes;
		data.second = fitness;
	}
}

/// <summary>
/// Represents a pair of values with two generic types.
/// </summary>
public class Pair<T, U>
{
	//The first and second elements of this pair, with the specified generic types
	public T first;
	public U second;
	
	/// <summary>
	/// Creates a new pair, sets the pair of values to their default values for their types.
	/// </summary>
	internal Pair() 
	{
		this.first = default(T);
		this.second = default(U);
	}
	
	/// <summary>
	/// Creates a new pair, sets the pair of values to the given values.
	/// </summary>
	/// <param name="first">The first value.</param>
	/// <param name="second">The second value.</param>
	internal Pair(T first, U second)
	{
		this.first = first;
		this.second = second;
	}
}
