using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GridManager : MonoBehaviour {

	[SerializeField]
	private GameObject floorObject;
	public bool UseFloorObject = false;

	[SerializeField]
	private Vector2 NrTileXNrTile;

	[SerializeField]
	private Vector2 sizeOfTiles;

	[SerializeField]
	private bool useDiagonalMoves = false;

	public Vector3 CenterOfGrid
	{
		get
		{
			if (UseFloorObject) 
			{
				return floorObject.collider.bounds.center + new Vector3(0, (floorObject.collider.bounds.size.y / 2) + sizeOfTiles.x, 0);
			}
			else 
			{
				return this.transform.position;
			}
		}
	}

	public Rect GridProperties
	{
		get {
			if (UseFloorObject) {
				return new Rect((int)(floorObject.collider.bounds.size.x / sizeOfTiles.x), (int)(floorObject.collider.bounds.size.z / sizeOfTiles.y), sizeOfTiles.x, sizeOfTiles.y);}
			else {
				return new Rect(NrTileXNrTile.x, NrTileXNrTile.y, sizeOfTiles.x, sizeOfTiles.y);}
		}
	}

	public bool[,] grid;

	public static Graph LevelGraph;

	void Start()
	{
		if (grid == null)
		{
			RecalculateGrid();
		}
	}

	void LateUpdate()
	{
		if (grid != null && LevelGraph == null)
		{
			LevelGraph = GraphHelper.GenerateGraph0(grid, CenterOfGrid - new Vector3((GridProperties.x * GridProperties.width) / 2, 0, (GridProperties.y * GridProperties.height) / 2), sizeOfTiles, useDiagonalMoves);
		}
	}
	
	private void RecalculateGrid()
	{
		if (!(GridProperties.x > 0 && GridProperties.y > 0 && GridProperties.width > 0 && GridProperties.height > 0))
		{
			return;
		}
		else if (grid == null)
		{
			if (GridProperties != null)
			{
				grid = new bool[(int)GridProperties.x, (int)GridProperties.y];
			}
		}
		
		float gridWidth = GridProperties.x * GridProperties.width;
		float gridHeight = GridProperties.y * GridProperties.height;
		
		Vector3 startPos = CenterOfGrid - new Vector3(gridWidth / 2, 0, gridHeight / 2);
		
		for (int x = 0; x < GridProperties.x; x++)
		{
			for (int y = 0; y < GridProperties.y; y++)
			{
				Vector3 currentStartPos = startPos + new Vector3(x * GridProperties.width, 0, y * GridProperties.width);
				
				Collider[] colliders = Physics.OverlapSphere(currentStartPos + new Vector3((GridProperties.width / 2), 
				                                                                           0, 
				                                                                           (GridProperties.height / 2)), 
				                                             (GridProperties.height / 2));
				
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
					
					grid[x,y] = colList.Count > 0;
				}
			}
		}
	}
}
