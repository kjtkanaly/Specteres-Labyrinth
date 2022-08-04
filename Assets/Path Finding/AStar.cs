using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class AStar : MonoBehaviour
{
    public class Node
    {
		// row:      Node row index
		// col: 	 Node col index
		// worldX:   Node X position
		// worldY:   Node Y position
		// walkable: Can an NPC walk on this node 
		public int   row;
		public int   col;
		public int   worldX;
		public int 	 worldY;
        public bool  walkable;
		
		/////////////////////////////////////
		// Path Finding Parameters
		// previousNode: The node back one in the path
		// gValue: 		 The cost to get to the node
		// hValue:  	 The cost to get to the target
		// fValue: 		 The Node's wholistic cost
		public Node  previousNode;
		public float gValue;
		public float hValue;
		public float fValue;

        // Create a class constructor for the Pathmap class
        public Node(int rowValue, int colValue, bool walkableTableValue)
        {
            row          = rowValue;
			col 	     = colValue;
            walkable     = walkableTableValue;
			previousNode = null;
			gValue       = Mathf.Infinity;
			hValue       = 0;
			fValue       = 0;
        }
    }

	/////////////////////////////////////////////////////////////////////////
	// Parameters
	// nodeMap:           Node map that is the size of the loaded map
	// 					  [row, col], nodeMap[1,2] = node in row 1 & col 2
	// LevelGen:		  Reference to the level Gen Component of the main game object
	public Node[,]         nodeMap;
	public LevelGeneration LevelGen;
	public EnemyController NPCController;
	public List<Node> openNodes;
	public List<Node> closedNodes;
	public List<Node> PathToPlayer;
	public Node currentNode;
	public Node neighborNode;

	//public List<Node> FindPath(Vector2Int APos, Vector2Int BPos, Node[,] nodeMap, bool debugMode = false)
	public IEnumerator FindPath(Vector2Int APos, Vector2Int BPos, bool debugMode = false)
	{		
		yield return StartCoroutine(SetupAStarNodeMap(LevelGen.FloorMap, LevelGen.nodeMapSize, LevelGen.nodeMapDebug));
		
		Node startingNode = nodeMap[APos.y, APos.x];
		Node endNode = nodeMap[BPos.y, BPos.x];

		if (debugMode == true)
		{
			DrawBox(new Vector2(startingNode.worldX, startingNode.worldY), new Vector2(1f, 1f),Color.green);
			// ^--- This will prob be int plus float error
		}

		PathToPlayer.Clear();
		openNodes.Clear();
		closedNodes.Clear();
		
		// Initializing the starting node
		startingNode.previousNode = null;
		startingNode.gValue       = 0;
		startingNode.hValue       = calcHValue(startingNode, endNode);
		startingNode.fValue       = calcFValue(startingNode);
        openNodes.Add(startingNode);

		// Debug Value used to try and avoid infinte loops
        int loopCount = 0;
        
		// Keep iterating through the Open Nodes till we find the path
		while(openNodes.Count > 0)
		{
            loopCount += 1;

            if (loopCount > 100000)
            {
				Debug.Log("Path could not be completed: Loop too Big, Ouchie");
				yield break;
			}
			
			// Log the lowest F value from the current Open list
			currentNode = getNodeWithLowestFValue(openNodes);   
            //Debug.Log(currentNode.fValue);
			
			if (currentNode == endNode)
			{
				NPCController.PathToPlayer = calculateFinalPath(currentNode);
				yield break;
			}
			
			openNodes.Remove(currentNode);			
			closedNodes.Add(currentNode);
            

			// Iterate through the Node's touching neighbors, Pos.x = row value
			for (int row = currentNode.row - 1; row <= currentNode.row + 1; row++)
			{
				// Iterate through the Node's touching neighbors, Pos.y = col value
				for (int col = currentNode.col - 1; col <= currentNode.col + 1; col++)
				{
					neighborNode = nodeMap[row, col];
					
					if ((neighborNode.walkable == true) && !(closedNodes.Contains(neighborNode)))
					{
						float tentativeGValue = calcGValue(neighborNode, currentNode);
						
						if (tentativeGValue < neighborNode.gValue)
						{
							//nodeMap[row, col].gValue = tentativeGValue;
					
							neighborNode.previousNode = currentNode;
							neighborNode.gValue 	  = tentativeGValue;
							neighborNode.hValue       = calcHValue(neighborNode, endNode);
							neighborNode.fValue       = calcFValue(neighborNode);
							
							openNodes.Add(neighborNode);

						}
					}
				}
			}	
		}

		Debug.Log("No Path Found");
		yield break;
	}
	
	
	public IEnumerator SetupAStarNodeMap(Tilemap FloorMap, Vector2Int mapSize, bool debugMode = false)
    {
		nodeMap = new Node[mapSize.y,mapSize.x];

        for (int row = 0; row < mapSize.y; row++)
        {
            for (int col = 0; col < mapSize.x; col++)
            {
				nodeMap[row, col] = new Node(row, col, false);
				nodeMap[row, col].row = row; // nodeMap[row + mapSize.y / 2, col + mapSize.x / 2].Pos = new Vector2Int(row, col);
				nodeMap[row, col].col = col;
				nodeMap[row, col].worldY = row - (mapSize.y / 2);
				nodeMap[row, col].worldX = col - (mapSize.x / 2);

				// Marking the non-Floor tiles as "not walkable"
				if (FloorMap.GetTile(new Vector3Int(nodeMap[row, col].worldX, nodeMap[row, col].worldY, 0)) == null)
                {
                    nodeMap[row, col].walkable = false;

					if (debugMode == true)
                    {
						DrawBox(new Vector2(nodeMap[row, col].worldX, nodeMap[row, col].worldY), new Vector2(1f, 1f), new Color(0, 0, 0, 0.5f) + Color.red);
                    }
                    
                }
				
                else
                {
                    nodeMap[row, col].walkable = true;

					if (debugMode == true)
                    {
						DrawBox(new Vector2(nodeMap[row, col].worldX, nodeMap[row, col].worldY), new Vector2(1f, 1f), new Color(1f, 1f, 1f, 0.5f));
					}
                }
            }
        }

		yield break;
    }
	
	
	public List<Node> calculateFinalPath(Node node, bool debugMode = false)
	{
		List<Node> nodePath = new List<Node>();
		// Tracing back through the most effcient path
        while (node.previousNode != null)
		{
			
			if (debugMode == true)
			{
				//DrawBox(new Vector2(node.worldX, node.worldY), new Vector2(1f, 1f), Color.white);
				//Debug.Log("World Pos: " + node.worldX + "x" + node.worldY);
				//Debug.Log("G Value: " + node.gValue);
				//Debug.Log("H Value: " + node.hValue);
				//Debug.Log("F Value: " + node.fValue);
				//Debug.Log("------------------------");
			}

			nodePath.Add(node);
			node = node.previousNode;
		}
		nodePath.Add(node);

		nodePath.Reverse();

        return nodePath;
	}
	
	
	public float calcGValue(Node neighborNode, Node currentNode)
	{
		float gValue;

		gValue = currentNode.gValue + Mathf.Sqrt(Mathf.Pow(neighborNode.col - currentNode.col, 2f) + Mathf.Pow(neighborNode.row - currentNode.row, 2f));

		return gValue;
	}
	
	
	public float calcHValue(Node A, Node B)
	{
		float hValue = Mathf.Sqrt(Mathf.Pow(A.col - B.col,2f) + Mathf.Pow(A.row - B.row,2f));
		return hValue;
	}
	
	
	public float calcFValue(Node A)
	{
		float fValue = A.gValue + A.hValue;
		return fValue;
	}
	
	
	public Node getNodeWithLowestFValue(List<Node> nodeList)
	{
		// Desired Node
		Node desNode = nodeList[nodeList.Count - 1];
		
		for (int listIndex = 0; listIndex < nodeList.Count; listIndex++)
		{
			if (nodeList[listIndex].fValue < desNode.fValue)
			{
				desNode = nodeList[listIndex];
			}
		}

        return desNode;
	}
	
	
	public void DrawBox(Vector2 origin, Vector2 size, Color color, float duration = Mathf.Infinity, bool depthTest = true)
	{
		// Draw the left side
		// Draw the top side
		// Draw the right side
		// Draw the bottom side
		Debug.DrawLine(new Vector3(origin.x, origin.y, 0), new Vector3(origin.x, origin.y + size.y, 0), color, duration, depthTest);
		Debug.DrawLine(new Vector3(origin.x, origin.y + size.y, 0), new Vector3(origin.x + size.x, origin.y + size.y, 0), color, duration, depthTest);
		Debug.DrawLine(new Vector3(origin.x + size.x, origin.y + size.y, 0), new Vector3(origin.x + size.x, origin.y, 0), color, duration, depthTest);
		Debug.DrawLine(new Vector3(origin.x + size.x, origin.y, 0), new Vector3(origin.x, origin.y, 0), color, duration, depthTest);
	}

	public void showNodeGrid()
	{
		// Make the debug map
	}

}


/*
for (int row = -mapSize.y/2; row < mapSize.y/2; row++)
        {
            for(int col = -mapSize.x/2; col < mapSize.x/2; col++)
            {
				
				
                // Marking the outer most floor tiles as "not walkable"
                else if((FloorMap.GetTile(new Vector3Int(col - 1, row, 0)) == null) || (FloorMap.GetTile(new Vector3Int(col + 1, row, 0)) == null) ||
                    (FloorMap.GetTile(new Vector3Int(col, row - 1, 0)) == null) || (FloorMap.GetTile(new Vector3Int(col, row + 1, 0)) == null))
                {
                    nodeMap[row + mapSize.y / 2, col + mapSize.x / 2].walkable = false;

                    if (debugInitMode == true)
                    {
                        DebugMap.SetTile(new Vector3Int(col, row, 0), notWalkableMarker);    
                    }
                }				
*/