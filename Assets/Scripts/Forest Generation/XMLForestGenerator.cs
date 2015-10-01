using UnityEngine;
using System.Collections;
using System.IO;
using System.Xml;
using System.Linq;
using System.Xml.Linq;
using System.Collections.Generic;

public class XMLForestGenerator : IForestGenerator 
{	
	private float generatorRange = 150f; 
	private int treeSpawnAmount;
	public string pathToXMLFile = "Assets/Config/treePoint.xml"; 
	public static List<Vector3> points = new List<Vector3>();

	public override void generateTreePositions()
	{

		if(density == ForestGeneration.Density.LOW)
			treeSpawnAmount = 250;
		
		else if(density == ForestGeneration.Density.MEDIUM)
			treeSpawnAmount = 450;
		
		else
			treeSpawnAmount = 800;

		
		if(!File.Exists(pathToXMLFile))
		{ 
			createTreePointXML();
			
		}
		
		treePositions.Clear();
		
		string contents = File.ReadAllText(pathToXMLFile);
		parseXML(contents);
											
		foreach(var point in points)
		{
			treePositions.Add(point);
		}
		
		cullTrees();
	}	
	
	private void cullTrees()
	{
		treePositions.RemoveAll(t => t.y <= yCulling);
	}

	private void createTreePointXML()
	{
		for(int i = 0; i < treeSpawnAmount; i++)
		{
			Vector3 consideredTree = ForestGeneration.randomXZAroundPoint(originPoint, generatorRange);
			treePositions.Add(consideredTree);
		}

		cullTrees();
		using (XmlWriter writer = XmlWriter.Create(pathToXMLFile)) {
			writer.WriteStartDocument();
			writer.WriteStartElement("points");

			foreach(var point in treePositions){
				writer.WriteStartElement("point");
				writer.WriteElementString("x", point.x.ToString());
				writer.WriteElementString("y", point.y.ToString());
				writer.WriteElementString("z", point.z.ToString());
				writer.WriteEndElement();
			}

			writer.WriteEndElement();
			writer.WriteEndDocument();
		}
	
	}

	private void parseXML(string contents)
	{
		XElement root = XElement.Load (pathToXMLFile);
		
		foreach(var point in root.Elements ("point"))
		{
			Vector3 position = new Vector3(
				float.Parse(point.Element("x").Value),
				float.Parse(point.Element("y").Value),
			    float.Parse(point.Element("z").Value)
			);
			points.Add (position);
		}
	}
}
