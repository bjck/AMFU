using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;

[CustomEditor(typeof(GridManager))]
public class GridCreator : Editor {

	GridManager grid;

	void DrawGrid()
	{
		if (grid == null)
		{
			return;
		}
		else if (grid.grid == null)
		{
			if (grid.GridProperties != null)
			{
				grid.grid = new bool[(int)grid.GridProperties.x, (int)grid.GridProperties.y];
			}

		}

		float gridWidth = grid.GridProperties.x * grid.GridProperties.width;
		float gridHeight = grid.GridProperties.y * grid.GridProperties.height;
		
		Vector3 startPos = grid.CenterOfGrid - new Vector3(gridWidth / 2, 0, gridHeight / 2);

		Color drawColor = Color.white;
		
		for (int x = 0; x < grid.GridProperties.x + 1; x++)
		{
			Debug.DrawLine(startPos + new Vector3(x * grid.GridProperties.width, 0, 0), startPos + new Vector3(x * grid.GridProperties.width, 0, gridHeight), drawColor);
		}

		for (int y = 0; y < grid.GridProperties.y + 1; y++)
		{
			Debug.DrawLine(startPos + new Vector3(0, 0, y * grid.GridProperties.height), startPos + new Vector3(gridWidth, 0, y * grid.GridProperties.height), drawColor);
		}
		
		drawColor = Color.red;

		for (int x = 0; x < grid.GridProperties.x; x++)
		{
			for (int y = 0; y < grid.GridProperties.y; y++)
			{
				if (grid.grid[x,y])
				{
					Vector3 currentStartPos = startPos + new Vector3(x * grid.GridProperties.width, 0, y * grid.GridProperties.width);

					Debug.DrawLine(currentStartPos, 
					               currentStartPos + new Vector3(grid.GridProperties.width, 0, 0), 
					               drawColor);
					Debug.DrawLine(currentStartPos, 
					               currentStartPos + new Vector3(0, 0, grid.GridProperties.height), 
					               drawColor);
					Debug.DrawLine(currentStartPos + new Vector3(grid.GridProperties.width, 0, 0), 
					               currentStartPos + new Vector3(grid.GridProperties.width, 0, grid.GridProperties.height), 
					               drawColor);
					Debug.DrawLine(currentStartPos + new Vector3(0, 0, grid.GridProperties.height), 
					               currentStartPos + new Vector3(grid.GridProperties.width, 0, grid.GridProperties.height), 
					               drawColor);
				}
			}
		}
	}
	
	private void RecalculateGrid()
	{
		if (grid == null)
		{
			return;
		}
		else if (!(grid.GridProperties.x > 0 && grid.GridProperties.y > 0 && grid.GridProperties.width > 0 && grid.GridProperties.height > 0))
		{
			return;
		}
		else if (grid.grid == null)
		{
			if (grid.GridProperties != null)
			{
				grid.grid = new bool[(int)grid.GridProperties.x, (int)grid.GridProperties.y];
			}

		}
		
		float gridWidth = grid.GridProperties.x * grid.GridProperties.width;
		float gridHeight = grid.GridProperties.y * grid.GridProperties.height;
		
		Vector3 startPos = grid.CenterOfGrid - new Vector3(gridWidth / 2, 0, gridHeight / 2);
		
		for (int x = 0; x < grid.GridProperties.x; x++)
		{
			for (int y = 0; y < grid.GridProperties.y; y++)
			{
				Vector3 currentStartPos = startPos + new Vector3(x * grid.GridProperties.width, 0, y * grid.GridProperties.width);
				
				Collider[] colliders = Physics.OverlapSphere(currentStartPos + new Vector3((grid.GridProperties.width / 2), 
				                                                                           0, 
				                                                                           (grid.GridProperties.height / 2)), 
				                                             (grid.GridProperties.height / 2));
				
				if (colliders.Length > 0)
				{
					//Checking for correct collider hit
					List<Collider> colList = new List<Collider>(colliders);
					
					for (int i = 0; i < colliders.Length; i++)
					{
						if (LayerMask.LayerToName(colliders[i].gameObject.layer) != "Environment")
						{
							colList.Remove(colliders[i]);
						}
					}
					
					grid.grid[x,y] = colList.Count > 0;
				}
			}
		}
	}

	private Rect currentRect;
	private Vector3 currentPos;

	public override void OnInspectorGUI ()
	{
		//TODO Add button for recalculation, instead of doing it on changes
		//TODO Change how settings for grids are saved.
		//TODO Check if grid works in the game.

		if (grid == null)
		{
			grid = (GridManager)target;
		}
		if (grid.grid == null)
		{
			RecalculateGrid();
		}
		else if (currentRect != grid.GridProperties)
		{
			currentRect = grid.GridProperties;
			RecalculateGrid();
		}
		else if (currentPos != grid.transform.position)
		{
			currentPos = grid.transform.position;
			RecalculateGrid();
		}

		DrawDefaultInspector();

		DrawGrid();
	}
}
