using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

// Perlin Noise Map Generation
public class LevelGenThree : MonoBehaviour
{
	public Tilemap FloorMap;
	public Tile FloorTile;
	
	// MapArea: The area that we will fill with noise
	// Offset:  Offsets the perlin cordinates
	public Vector2Int MapArea = new Vector2Int(200, 75);
	public Vector2Int Offset  = new Vector2Int(0, 0);
	
	// Scale:       Used to scale my perlin coord
	// RoundCutOff: Used to round the perlin values
	public float Scale = 20f;
	public float RoundCutOff = 0.5f;
	
	public void Start()
	{
		GenerateZone(MapArea, new Vector2Int(0, 0));
	}
	
	// Lays floor tiles for a given area by calling perlin noise
	public void GenerateZone(Vector2Int ZoneSize, Vector2Int ZoneOffet)
	{
		// Iterate through the rows
		for (int row = 0; row < ZoneSize.y + ZoneOffet.y; row++)
		{
			// Iterate through the cols
			for (int col = 0; col < ZoneSize.x + ZoneOffet.x; col++)
			{
				int sample = CalcNoise(col, row);
				
				if (sample == 1)
				{
					FloorMap.SetTile(new Vector2Int(col, row), FloorTile);
				}
				else
				{
					// Can set an empy tile if we want
				}
			}
		}
	}
	
	// Returns rounded Perlin Noise Values: (0 -or- 1)
	public int CalcNoise(int x, int y)
	{
		// Perlin Coord x = 0 -> 0, x = width -> 1
		float xCoord = (float)x / MapArea.x * Scale + Offset.x;
		float yCoord = (float)y / MapArea.y * Scale + Offset.y;
		
		// Get the perlin value for the Coords
		float sample = Mathf.PerlinNoise(xCoord, yCoord);
		
		// Round the Perlin value
		if (sample >= RoundCutOff)
		{
			return 1;
		}
		else
		{
			return 0;
		}
	}


}