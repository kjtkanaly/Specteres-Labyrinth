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
		Combo,
		PerlinRooms
    }

	public Tilemap FloorMap;
	public Tile FloorTile;
	public Tile[] FloorTileSet;
	
	// MapArea: The area that we will fill with noise
	// Offset:  Offsets the perlin cordinates
	// RoomsSizes: The dimensions of room footprints (height, width)
	public Vector2Int MapArea   = new Vector2Int(201, 75);
	public Vector2Int Offset    = new Vector2Int(0, 0);
	public Vector2Int RoomSizes = new Vector2Int(20, 10);
	public Mode       OperationMode = Mode.Maze;
	
	// Scale:       Used to scale my perlin coord
	// RoundCutOff: Used to round the perlin values
	public float Scale = 20f;
	public float RoundCutOff = 0.5f;
	public int   MazeScale = 2;
	public int   TotalRooms = 3;


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

			int row = 0;

			while (row < ZoneSize.y/MazeScale)
            {
				int col = 0;

				while (col < ZoneSize.x/MazeScale)
                {
					for (int tileRow = row * MazeScale; tileRow < row * MazeScale + MazeScale; tileRow++)
                    {
						for (int tileCol = col * MazeScale; tileCol < col * MazeScale + MazeScale; tileCol++)
                        {
							if (Maze[row, col] == 0)
							{
								FloorMap.SetTile(new Vector3Int(tileCol, tileRow), FloorTile);
							}
							else
							{
							}
						}
                    }

					col += 1;
				}

				row += 1;
			}
		}

		else if (OperationMode == Mode.Combo)
		{
			int[,] Maze = GenerateMaze(ZoneSize);

			int row = 0;

			while (row < ZoneSize.y / MazeScale)
			{
				int col = 0;

				while (col < ZoneSize.x / MazeScale)
				{
					for (int tileRow = row * MazeScale; tileRow < row * MazeScale + MazeScale; tileRow++)
					{
						for (int tileCol = col * MazeScale; tileCol < col * MazeScale + MazeScale; tileCol++)
						{
							int sample = CalcNoise(tileCol, tileRow);

							if (Maze[row,col] == 0)
                            {
								FloorMap.SetTile(new Vector3Int(tileCol, tileRow), FloorTileSet[0]);
							}

							if (sample == 0)
                            {
								FloorMap.SetTile(new Vector3Int(tileCol, tileRow), FloorTileSet[1]);
							}

							/*
							if ((Maze[row, col] == 0) || (sample == 0))
							{
								int tileNumber = Random.Range(0, FloorTileSet.Length);

								FloorMap.SetTile(new Vector3Int(tileCol, tileRow), FloorTileSet[tileNumber]);
							}
							else
							{
							}*/
						}
					}

					col += 1;
				}

				row += 1;
			}
		}
		else if (OperationMode == Mode.PerlinRooms)
        {
			// Perlin Section
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

			// Add N Rooms Randomly
			for (int RoomCount = 0; RoomCount < TotalRooms; RoomCount++)
			{
				Vector2Int RoomOrigin = new Vector2Int(Random.Range(1, MapArea.x - RoomSizes.x), Random.Range(1, MapArea.y - RoomSizes.y));

				for (int row = RoomOrigin.y; row < RoomOrigin.y + RoomSizes.y; row++)
                {
					for (int col = RoomOrigin.x; col < RoomOrigin.x + RoomSizes.x; col++)
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
		for (int row = 1; row < MazeSize.y; row+=2)
        {
			for (int col = 1; col < MazeSize.x; col+=2)
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

	public void setArrayRangeOfElements(int[,] array, int start, int end, int dir)
    {

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