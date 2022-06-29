using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Tilemaps;
using UnityEngine;

public class ScriptableLevel : ScriptableObject
{

	public int LevelIndex;
	public string LevelType;
	public string[] LevelExits;
	public Vector2Int LevelFootPrint;
	public List<SavedTile> FloorTiles;
	public List<SavedTile> TopWallTiles;
	public List<SavedTile> BottomWallTiles;
	public List<SavedTile> CollideFurnitureTop;
	public List<SavedTile> CollideFurnitureBottom_1;
	public List<SavedTile> CollideFurnitureBottom_2;
	public List<SavedTile> NonCollideFurnitureTop;
	public List<SavedTile> NonCollideFurnitureBottom_1;
	public List<SavedTile> NonCollideFurnitureBottom_2;

	public List<SavedObjects> InteractableObjects1;
}

[Serializable]
public class SavedTile
{

	public Vector3Int Position;
	public LevelTile Tile;
	public Tile NormalTile;
	public string TileTypeString;

}


[Serializable]
public class SavedObjects
{
	public Vector3 Position;
	public string PrefabTitle;
	public int SortingLyr;
}
