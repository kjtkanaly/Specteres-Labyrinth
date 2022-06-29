using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Tilemaps;
using UnityEngine;
using UnityEditor;

public class TilemapManager : MonoBehaviour
{

	//private SavedObjects TempSavedObject;

	[SerializeField] private GameObject _ParentInteractObject;

	[SerializeField] private Tilemap _groundMap, _topWallMap, _bottomWallMap, _nonCollideFurnitureTop,
									 _nonCollideFurnitureBottom1, _nonCollideFurnitureBottom2, _collideFurnitureTop,
									 _collideFurnitureBottom1, _collideFurnitureBottom2;
	[SerializeField] private int _levelIndex;
	[SerializeField] private string _levelType, _levelName;
	[SerializeField] private string[] _levelExits;
	[SerializeField] private Vector2Int _levelFootPrint;

	[SerializeField] private Vector3Int _levelFootPrintOffset;

	public void SaveMap()
	{
		var newLevel = ScriptableObject.CreateInstance<ScriptableLevel>();

		newLevel.LevelIndex = _levelIndex;
		newLevel.LevelType = _levelType;
		newLevel.LevelExits = _levelExits;
		newLevel.LevelFootPrint = _levelFootPrint;
		newLevel.name = ($"Level {_levelIndex}" + "- " + _levelName);

		newLevel.FloorTiles = GetTilesFromMap(_groundMap).ToList();
		newLevel.TopWallTiles = GetTilesFromMap(_topWallMap).ToList();
		newLevel.BottomWallTiles = GetTilesFromMap(_bottomWallMap).ToList();
		newLevel.CollideFurnitureTop = GetTilesFromMap(_collideFurnitureTop).ToList();
		newLevel.CollideFurnitureBottom_1 = GetTilesFromMap(_collideFurnitureBottom1).ToList();
		newLevel.CollideFurnitureBottom_2 = GetTilesFromMap(_collideFurnitureBottom2).ToList();
		newLevel.NonCollideFurnitureTop = GetTilesFromMap(_nonCollideFurnitureTop).ToList();
		newLevel.NonCollideFurnitureBottom_1 = GetTilesFromMap(_nonCollideFurnitureBottom1).ToList();
		newLevel.NonCollideFurnitureBottom_2 = GetTilesFromMap(_nonCollideFurnitureBottom2).ToList();

		newLevel.InteractableObjects1 = GetPrefabObjects(GameObject.FindGameObjectsWithTag("Interactable Objects")).ToList();

		ScriptableObjectUtility.SaveLevelFile(newLevel);

		IEnumerable<SavedTile> GetTilesFromMap(Tilemap map)
		{
			foreach (var pos in map.cellBounds.allPositionsWithin)
			{
				if (map.HasTile(pos))
				{
					if (map.GetTile(pos).GetType().ToString() == "LevelTile")
					{
						var levelTile = map.GetTile<LevelTile>(pos);

						yield return new SavedTile()
						{
							Position = pos,
							Tile = levelTile,
							TileTypeString = "Level Tile"
						};
					}
					else if (map.GetTile(pos).GetType().ToString() == "UnityEngine.Tilemaps.Tile")
					{
						var normalTile = map.GetTile<Tile>(pos);

						yield return new SavedTile()
						{
							Position = pos,
							NormalTile = normalTile,
							TileTypeString = "Normal Tile"
						};
					}
				}
			}
		}

		IEnumerable<SavedObjects> GetPrefabObjects(GameObject[] objArray)
		{
			foreach (var obj in objArray)
			{
				string[] objName = obj.name.Split(char.Parse(" "));

				yield return new SavedObjects()
				{
					Position = obj.transform.position,
					PrefabTitle = objName[0],
					SortingLyr = obj.GetComponent<SpriteRenderer>().sortingOrder
				};
			}
		}
	}

	public void ClearMap()
	{
		var maps = FindObjectsOfType<Tilemap>();

		foreach (var tilemap in maps)
        {
			tilemap.ClearAllTiles();
        }

		foreach (var obj in GameObject.FindGameObjectsWithTag("Interactable Objects"))
        {
			DestroyImmediate(obj);
		}

	}

	public void LoadMap()
	{
		var level = Resources.Load<ScriptableLevel>("Levels/" + $"Level {_levelIndex}" + "- " + _levelName);

		if (level == null)
        {
			Debug.LogError($"Level {_levelIndex} does not exist.");
			return;
        }

		ClearMap();

		foreach (var savedTile in level.FloorTiles)
        {
			//Debug.Log(savedTile.TileTypeString);

			if (savedTile.TileTypeString == "Normal Tile")
            {
				_groundMap.SetTile(savedTile.Position + _levelFootPrintOffset, savedTile.NormalTile);
			}
			else if (savedTile.TileTypeString == "Level Tile")
			{
				_groundMap.SetTile(savedTile.Position + _levelFootPrintOffset, savedTile.Tile);
				/*
				switch (savedTile.Tile.Type)
				{
					case TileType.Floor:
						_groundMap.SetTile(savedTile.Position + _levelFootPrintOffset, savedTile.Tile);
						break;
					default:
						throw new ArgumentOutOfRangeException();
				}
				*/
			}
        }

		foreach (var savedTile in level.TopWallTiles)
		{
			_topWallMap.SetTile(savedTile.Position + _levelFootPrintOffset, savedTile.Tile);
			/*
			switch (savedTile.Tile.Type)
			{
				case TileType.TopWall:
					_topWallMap.SetTile(savedTile.Position + _levelFootPrintOffset, savedTile.Tile);
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
			*/
		}

		foreach (var savedTile in level.BottomWallTiles)
		{
			_bottomWallMap.SetTile(savedTile.Position + _levelFootPrintOffset, savedTile.Tile);
			/*
			switch (savedTile.Tile.Type)
			{
				case TileType.BottomWall:
					_bottomWallMap.SetTile(savedTile.Position + _levelFootPrintOffset, savedTile.Tile);
					break;
				default:
					Debug.Log(savedTile.Tile.Type);
					throw new ArgumentOutOfRangeException();
			}*/

		}

        foreach (var savedTile in level.CollideFurnitureTop)
        {
            if (savedTile.TileTypeString == "Normal Tile")
            {
                _collideFurnitureTop.SetTile(savedTile.Position + _levelFootPrintOffset, savedTile.NormalTile);
            }
        }

        foreach (var savedTile in level.CollideFurnitureBottom_1)
        {
            if (savedTile.TileTypeString == "Normal Tile")
            {
                _collideFurnitureBottom1.SetTile(savedTile.Position + _levelFootPrintOffset, savedTile.NormalTile);
            }
        }

        foreach (var savedTile in level.CollideFurnitureBottom_2)
        {
            if (savedTile.TileTypeString == "Normal Tile")
            {
                _collideFurnitureBottom2.SetTile(savedTile.Position + _levelFootPrintOffset, savedTile.NormalTile);
            }
        }

        foreach (var savedTile in level.NonCollideFurnitureTop)
		{
			if (savedTile.TileTypeString == "Normal Tile")
			{
				_nonCollideFurnitureTop.SetTile(savedTile.Position + _levelFootPrintOffset, savedTile.NormalTile);
			}
		}

		foreach (var savedTile in level.NonCollideFurnitureBottom_1)
		{
			if (savedTile.TileTypeString == "Normal Tile")
			{
				_nonCollideFurnitureBottom1.SetTile(savedTile.Position + _levelFootPrintOffset, savedTile.NormalTile);
			}
		}

		foreach (var savedTile in level.NonCollideFurnitureBottom_2)
		{
			if (savedTile.TileTypeString == "Normal Tile")
			{
				_nonCollideFurnitureBottom2.SetTile(savedTile.Position + _levelFootPrintOffset, savedTile.NormalTile);
			}
		}

		foreach (var savedObjects in level.InteractableObjects1)
        {
			Debug.Log("PreFabs/" + savedObjects.PrefabTitle);
			var prefab = Resources.Load<GameObject>("PreFabs/" + savedObjects.PrefabTitle);

			prefab.transform.position = savedObjects.Position + _levelFootPrintOffset;
			prefab.GetComponent<SpriteRenderer>().sortingOrder = savedObjects.SortingLyr;

			PrefabUtility.InstantiatePrefab(prefab);

		}
		
	}
}


#if UNITY_EDITOR

	public static class ScriptableObjectUtility{
		public static void SaveLevelFile(ScriptableLevel level){
			AssetDatabase.CreateAsset(level, $"Assets/Resources/Levels/{level.name}.asset");

			AssetDatabase.SaveAssets();
			AssetDatabase.Refresh();
		}
	}

#endif
