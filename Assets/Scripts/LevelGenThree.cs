using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

// Perlin Noise Map Generation
public class LevelGenThree : MonoBehaviour
{
	public enum Mode
    {
		Perlin,
		Maze,
		Combo
    }

	public Tilemap FloorMap;
	public Tile FloorTile;
	
	// MapArea: The area that we will fill with noise
	// Offset:  Offsets the perlin cordinates
	public Vector2Int MapArea = new Vector2Int(201, 75);
	public Vector2Int Offset  = new Vector2Int(0, 0);
	public Mode       OperationMode = Mode.Maze;
	
	// Scale:       Used to scale my perlin coord
	// RoundCutOff: Used to round the perlin values
	public float Scale = 20f;
	public float RoundCutOff = 0.5f;
	
	public void Start()
	{
		GenerateZone(MapArea, new Vector2Int(0, 0));
	}

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
			FloorMap.ClearAllTiles();
			GenerateZone(MapArea, new Vector2Int(0, 0));
		}
    }

    // Lays floor tiles for a given area by calling perlin noise
    public void GenerateZone(Vector2Int ZoneSize, Vector2Int ZoneOffet)
	{
		if (OperationMode == Mode.Perlin)
        {
			// Iterate through the rows
			for (int row = 0; row < ZoneSize.y + ZoneOffet.y; row++)
			{
				// Iterate through the cols
				for (int col = 0; col < ZoneSize.x + ZoneOffet.x; col++)
				{
					int sample = CalcNoise(col, row);

					if (sample == 0)
					{
						FloorMap.SetTile(new Vector3Int(col, row), FloorTile);
					}
					else
					{
						// Can set an empy tile if we want
					}
				}
			}
		}
		else if (OperationMode == Mode.Maze)
        {
			int[,] Maze = GenerateMaze(ZoneSize);

			// Laying down floor tiles according the Maze
			for (int row = 0; row < ZoneSize.y + ZoneOffet.y; row++)
			{
				for (int col = 0; col < ZoneSize.x + ZoneOffet.x; col++)
				{
					if (Maze[row, col] == 0)
                    {
						FloorMap.SetTile(new Vector3Int(col, row), FloorTile);
					}
				}
			}

		}
	}

	public int[,] GenerateMaze(Vector2Int MazeSize)
    {
		int[,] Maze = new int[MazeSize.y, MazeSize.x];

		// Setting the Seeds
		for (int row = 1; row < MazeSize.y - 1; row+=2)
        {
			for (int col = 1; col < MazeSize.x - 1; col+=2)
            {
				Maze[row, col] = 1;

				int dir = Random.Range(0, 4);

				switch (dir)
				{
					case 0:
						Maze[row + 1, col] = 1;
						break;
					case 1:
						Maze[row, col + 1] = 1;
						break;
					case 2:
						Maze[row - 1, col] = 1;
						break;
					case 3:
						Maze[row, col - 1] = 1;
						break;
				}
			}
        }

		return Maze;
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