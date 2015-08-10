using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class IForestGenerator
{
	protected float yCulling;
	protected List<Vector3> treePositions = new List<Vector3>();
	protected ForestGeneration.Density density; 
	protected Vector3 originPoint;
	 	
	public abstract void generateTreePositions();
	
	public List<Vector3> getTreePositions()
	{
		return treePositions;
	}
	
	public virtual void setOriginPoint(Vector3 position)
	{
		this.originPoint = position;
	}
	
	public virtual void setDensity(ForestGeneration.Density density)
	{
		this.density = density;
	}
	
	public virtual void setCullY(float y)
	{
		yCulling = y;
	}	
}

public static class ForestGeneration
{
	public enum Density
	{
		LOW, MEDIUM, HIGH
	}
	
	public static Vector3 randomXZAroundPoint(Vector3 point, float range)
	{
		Vector3 returnPoint = new Vector3(Random.Range(-range, range), 0f, Random.Range (-range, range));
		
		returnPoint += point;
		returnPoint.y = Terrain.activeTerrain.SampleHeight(returnPoint);
		
		return returnPoint;
	}
	
	public static Quaternion randomYRotation(Quaternion original)
	{	
		var randomRotation = Random.rotation;
		
		return new Quaternion(original.x, randomRotation.y, original.z, original.w);
	}
}