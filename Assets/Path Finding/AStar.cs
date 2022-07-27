using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class AStar : MonoBehaviour
{
    public class Node
    {
        public Vector2Int Pos;
        public bool walkable;

        // Create a class constructor for the Pathmap class
        public Node(Vector2Int PosValue, bool walkableTableValue)
        {
            Pos = PosValue;
            walkable = walkableTableValue;
        }
    }

    public Node[,] nodeMap;
    //public List<Node> nodePath;
    public Tilemap FloorMap, WallMap, DebugMap;
    public Tile notWalkableMarker, walkableMarker;
    public Vector2Int mapSize;
    public bool debugMode = false;

    public Node[,] SetupAStarNodes(Tilemap FloorMap, Tilemap WallMap)
    {
        mapSize = ((Vector2Int)WallMap.size);
        nodeMap = initNodeMap(mapSize);

        for (int row = -mapSize.y/2; row < mapSize.y/2; row++)
        {
            for(int col = -mapSize.x/2; col < mapSize.x/2; col++)
            {
                nodeMap[row + mapSize.y / 2, col + mapSize.x / 2].Pos = new Vector2Int(row, col);

                // Marking the non-Floor tiles as "not walkable"
                if (FloorMap.GetTile(new Vector3Int(col, row, 0)) == null)
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

        return nodeMap;
    }


    public List<Node> FindPath(Vector2Int APos, Vector2Int BPos, Node[,] nodeMap)
    {
        List<Node> nodePath = new List<Node>();
        Node A = new Node(APos, true);
        Node B = new Node(BPos, true);

        nodePath.Add(A);

        while(nodePath[nodePath.Count - 1] != B)
        {
            Node nextNode = FindTheNextNode(nodePath[nodePath.Count - 1], B, nodeMap);
            nodePath.Add(nextNode);
        }

        return nodePath;
    }

    public Node FindTheNextNode(Node A, Node B, Node[,] nodeMap)
    {
        float currentMinF = Mathf.Infinity;
        Node nextNode = A;

        for (int row = A.Pos.y - 1; row <= A.Pos.y + 1; row++)
        {
            for (int col = A.Pos.x - 1; col <= A.Pos.x + 1; col++)
            {
                if (((row != A.Pos.y) && (col != A.Pos.x)) || (nodeMap[row,col].walkable == true))
                {
                    float G = Mathf.Sqrt((A.Pos.y - row)^2 + (A.Pos.x - col)^2);
                    float H = Mathf.Sqrt((B.Pos.y - row)^2 + (B.Pos.x - col)^2);
                    float F = G + H;

                    if (F < currentMinF)
                    {
                        currentMinF = F;
                        nextNode = nodeMap[row,col];
                    }
                }
            }
        }

        return nextNode;
    }


    public void findObjsNode(Vector2Int pos)
    {

    }


    public Node[,] initNodeMap(Vector2Int mapSize)
    {
        Node[,] NodeMap = new Node[mapSize.y,mapSize.x];

        for (int row = 0; row < mapSize.y; row++)
        {
            for (int col = 0; col < mapSize.x; col++)
            {
                NodeMap[row, col] = new Node(new Vector2Int(row,col), false);
            }
        }

        return NodeMap;
    }

    

}
