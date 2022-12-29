using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WorldGeneration : MonoBehaviour
{
    public Vector2 playerStartingCord = new Vector2(0f, 10f);
    public Vector2 chunkSize = new Vector2(100f, 50f);

    public float seed = 0;
    public float noiseScale = 1f;
    public float noiseThreshold = 0.5f;

    private void Start()
    {   
        seed = Mathf.round(seed);
        Vector2 noiseOrigin = new Vector2(0f + seed, 0f + seed); 

        float[][] NoiseMap = new float[chunkSize.y][chunkSize.x];
        for (int col = 0; col < chunkSize.x; col++)
        {
            for (int row = 0; row < chunkSize.y; row++)
            {   
                NoiseMap[row][col] = CalcNoise(col, row);
            }
        }
    }


    // Returns rounded Perlin Noise Values: (0 -or- 1)
	public int CalcNoise(int x, int y)
	{
		// Perlin Coord x = 0 -> 0, x = width -> 1
		float xCoord = (float)x / chunkSize.y * NoiseScale + seed;
		float yCoord = (float)y / chunkSize.x * NoiseScale + seed;
		
		// Get the perlin value for the Coords
		float sample = Mathf.PerlinNoise(xCoord, yCoord);
		
        return sample;
	}
   
}

