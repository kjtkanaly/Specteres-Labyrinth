using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class AStar : MonoBehaviour
{
    public class Node
    {
		public Vector2Int Pos;
        public bool  walkable;
		
		public Node  previousNode;
		public float gValue;
		public float hValue;
		public float fValue;

        // Create a class constructor for the Pathmap class
        public Node(Vector2Int PosValue, bool walkableTableValue)
        {
            Pos = PosValue;
            walkable = walkableTableValue;
        }
    }

    public Node[,] nodeMap;
    public Tilemap FloorMap, WallMap, DebugMap;
    public Tile    notWalkableMarker, walkableMarker;
	
    public Vector2Int mapSize;
	
    public bool debugMode = false;

    public void SetupAStarNodeMap(Tilemap FloorMap, Tilemap WallMap)
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
    }


    public List<Node> FindPath(Vector2Int APos, Vector2Int BPos)
    {
        Node A = nodeMap[APos.y + mapSize.y/2, APos.x + mapSize.x/2];
        Node B = nodeMap[APos.y + mapSize.y/2, APos.x + mapSize.x/2];
		
		List<Node> openNodes   = new List<Node>();
		List<Node> closedNodes = new List<Node>();
		List<Node> nodePath    = new List<Node>();
		
		float minFValue    = Mathf.Infinity;
		bool  pathComplete = false;

        openNodes.Add(calcNodeValue(A, B));

        int debugCount = 0;

        
		// Keep iterating through the Open Nodes till we find the path
		while(pathComplete == false)
		{
            debugCount += 1;

            if (debugCount > 100)
            {
                return null;
            }

			minFValue = findMinFvalue(openNodes);   // Finds the lowest value currently in the Open Node lists
            Debug.Log(minFValue);
            /*
            int numbOfOpenNodes = openNodes.Count;
			// Iterate through the Open Node list
			for (int openNodeCount = 0; openNodeCount < numbOfOpenNodes; openNodeCount++)
			{
				// Check if the current Open Node has the current lowest F value
				if (openNodes[openNodeCount].fValue == minFValue)
				{
					// Iterate through the Open Node's neighbors
					for (int row = openNodes[openNodeCount].Pos.y - 1; row <= openNodes[openNodeCount].Pos.y + 1; row++)
					{
						for (int col = openNodes[openNodeCount].Pos.x - 1; col <= openNodes[openNodeCount].Pos.x + 1; col++)
						{
							Node newNode = calcNodeValue(nodeMap[row + mapSize.y/2,col + mapSize.x/2], B);
                            newNode.previousNode = openNodes[openNodeCount];
							openNodes.Add(newNode);
							
							// Checking if the new open node is the goal node
							if (newNode == B)
							{
								pathComplete = true;
								nodePath.Add(newNode);

                                Debug.Log("Path Complete");
							}
						}
					}	

				closedNodes.Add(openNodes[openNodeCount]);
				openNodes.RemoveAt(openNodeCount);
				
				}
			}
            */
		}
        /*
        debugCount = 0;

        if (debugCount > 100)
        {
            return null;
        }

        // Tracing back through the most effcient path
        while (nodePath[nodePath.Count - 1] != A)
		{
			nodePath.Add(nodePath[nodePath.Count - 1].previousNode);
		}
		nodePath.Reverse();

        return nodePath;
        */

        return null;
    }
	
	public Node calcNodeValue(Node A, Node B)
	{
        Node newNode = new Node(new Vector2Int(A.Pos.x, A.Pos.y), true);
        if (A.previousNode != null)
        {
            newNode.gValue = A.previousNode.gValue + Mathf.Sqrt((A.Pos.x + A.previousNode.Pos.x) ^ 2 + (A.Pos.y + A.previousNode.Pos.y) ^ 2);
        }
        else
        {
            newNode.gValue = 0;
        }

		newNode.hValue = Mathf.Sqrt((A.Pos.x + B.Pos.x)^2 + (A.Pos.y + B.Pos.y)^2);
		newNode.fValue = newNode.gValue + newNode.hValue;

        return newNode;
	}
	
	public float findMinFvalue(List<Node> openNodes)
	{
		float minFValue = Mathf.Infinity;
		
		for (int openNodeCount = 0; openNodeCount < openNodes.Count; openNodeCount++)
		{
			if (openNodes[openNodeCount].fValue < minFValue)
			{
				minFValue = openNodes[openNodeCount].fValue;
			}
		}

        return minFValue;
	}

    public Node FindTheNextNode(Node A, Node B)
    {
        float currentMinF = Mathf.Infinity;
        Node nextNode = A;

        for (int row = A.Pos.y - 1; row <= A.Pos.y + 1; row++)
        {
            for (int col = A.Pos.x - 1; col <= A.Pos.x + 1; col++)
            {
                if (((row != A.Pos.y) && (col != A.Pos.x)) && (nodeMap[row,col].walkable == true))
                {
                    float G = Mathf.Sqrt((A.Pos.y - row)^2 + (A.Pos.x - col)^2);
                    float H = Mathf.Sqrt((B.Pos.y - row)^2 + (B.Pos.x - col)^2);
                    float F = G + H;

                    if ((F < currentMinF) && (nodeMap[row + A.Pos.y,col + A.Pos.x] != null))
                    {
                        currentMinF = F;
                        nextNode = nodeMap[row + A.Pos.y,col + A.Pos.x];
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
