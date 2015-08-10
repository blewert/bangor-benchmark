using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

//modified from source: http://forum.unity3d.com/threads/maze-builder-in-c.252507/

public class MazeGenerator2
{
	public int[,] maze    { get; private set; }
	
	private int mazeHeight,mazeWidth;
	
	public int[,] GenerateMaze(int height,int width)
	{
		maze = new int[height,width];
		// Initialize
		for (int i = 0; i < height; i++)
			for (int j = 0; j < width; j++)
				maze[i,j] = 1;
		
		UnityEngine.Random rand = new UnityEngine.Random();
		// r for row、c for column
		// Generate random r
		int r = (int)Math.Floor(UnityEngine.Random.value * height);
		while (r % 2 == 0)
		{
			r = (int)Math.Floor(UnityEngine.Random.value*height);
		}
		// Generate random c
		int c = (int)Math.Floor(UnityEngine.Random.value*width);
		while (c % 2 == 0)
		{
			c = (int)Math.Floor(UnityEngine.Random.value*width);
		}
		// Starting cell
		maze[r,c] = 0;
		
		mazeHeight    = height;
		mazeWidth     = width;
		
		//　Allocate the maze with recursive method
		recursion(r, c);
		
		return maze;
	}
	
	public void recursion(int r, int c)
	{
		// 4 random directions
		int[] directions = new int[]{1,2,3,4};
		
		//directions = generateRandomDirections();
		Shuffle(directions);
		
		// Examine each direction
		for (int i = 0; i < directions.Length; i++)
		{
			
			switch(directions[i]){
			case 1: // Up
				//　Whether 2 cells up is out or not
				if (r - 2 <= 0)
					continue;
				if (maze[r - 2,c] != 0)
				{
					maze[r-2,c] = 0;
					maze[r-1,c] = 0;
					recursion(r - 2, c);
				}
				break;
			case 2: // Right
				// Whether 2 cells to the right is out or not
				if (c + 2 >= mazeWidth - 1)
					continue;
				if (maze[r,c + 2] != 0)
				{
					maze[r,c + 2] = 0;
					maze[r,c + 1] = 0;
					recursion(r, c + 2);
				}
				break;
			case 3: // Down
				// Whether 2 cells down is out or not
				if (r + 2 >= mazeHeight - 1)
					continue;
				if (maze[r + 2,c] != 0)
				{
					maze[r + 2,c] = 0;
					maze[r + 1,c] = 0;
					recursion(r + 2, c);
				}
				break;
			case 4: // Left
				// Whether 2 cells to the left is out or not
				if (c - 2 <= 0)
					continue;
				if (maze[r,c - 2] != 0)
				{
					maze[r,c - 2] = 0;
					maze[r,c - 1] = 0;
					recursion(r, c - 2);
				}
				break;
			}
		}
		
	}
	
	// Fisher Yates Shuffle
	public void Shuffle<T>(T[] array)
	{
		UnityEngine.Random _random = new UnityEngine.Random();
		for (int i = array.Length; i > 1; i--)
		{
			// Pick random element to swap.
			int j = (int)Math.Floor(UnityEngine.Random.value * i); // 0 <= j <= i-1
			// Swap.
			T tmp = array[j];
			array[j] = array[i - 1];
			array[i - 1] = tmp;
		}
	}
	
}

