using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class AStar : MonoBehaviour
{
    public class Node
    {
        public bool walkable;
        public int gValue;
        public int hValue;
        public int fValue;

        // Create a class constructor for the Pathmap class
        public Node(bool walkableTableValue)
        {
            walkable = walkableTableValue;
        }
    }

    public Node[,] nodeMap;
    public Tilemap FloorMap, WallMap, DebugMap;
    public Tile notWalkableMarker, walkableMarker;
    public Vector2Int mapSize;
    public bool debugMode = false;

    public void SetupAStarNodes(Tilemap FloorMap, Tilemap WallMap)
    {
        mapSize = ((Vector2Int)WallMap.size);
        nodeMap = initNodeMap(mapSize);

        Debug.Log((int)(nodeMap.Length-1) % nodeMap.Length);

        for (int row = -mapSize.y/2; row < mapSize.y/2; row++)
        {
            for(int col = -mapSize.x/2; col < mapSize.x/2; col++)
            {
                // Marking the non-Floor tiles as "not walkable"
                if(FloorMap.GetTile(new Vector3Int(col, row, 0)) == null)
                {
                    nodeMap[row + mapSize.y / 2, col + mapSize.x / 2].walkable = false;

                    if (debugMode == true)
                    {
                        DebugMap.SetTile(new Vector3Int(col, row, 0), notWalkableMarker);
                    }
                    
                }

                // Marking the outer most floor tiles as "not walkable"
                else if((FloorMap.GetTile(new Vector3Int(col - 1, row, 0)) == null) || (FloorMap.GetTile(new Vector3Int(col + 1, row, 0)) == null) ||
                    (FloorMap.GetTile(new Vector3Int(col, row - 1, 0)) == null) || (FloorMap.GetTile(new Vector3Int(col, row + 1, 0)) == null))
                {
                    nodeMap[row + mapSize.y / 2, col + mapSize.x / 2].walkable = false;

                    if (debugMode == true)
                    {
                        DebugMap.SetTile(new Vector3Int(col, row, 0), notWalkableMarker);    
                    }
                }

                else
                {
                    nodeMap[row + mapSize.y / 2, col + mapSize.x / 2].walkable = true;

                    if (debugMode == true)
                    {
                        DebugMap.SetTile(new Vector3Int(col, row, 0), walkableMarker);
                    }
                }
            }
        }
    }


    public void UpdatePath(Vector2Int A, Vector2Int B)
    {
        for (int row = 0; row < mapSize.y; row++)
        {
            for (int col = 0; col < mapSize.x; col++)
            {

            }
        }
    }


    public Node[,] initNodeMap(Vector2Int mapSize)
    {
        Node[,] NodeMap = new Node[mapSize.y,mapSize.x];

        for (int row = 0; row < mapSize.y; row++)
        {
            for (int col = 0; col < mapSize.x; col++)
            {
                NodeMap[row, col] = new Node(false);
            }
        }

        return NodeMap;
    }

    

}
