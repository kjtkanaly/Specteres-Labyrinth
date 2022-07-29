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
			previousNode = null
			gValue       = Mathf.Infinity;
			hValue       = 0;
			fValue       = 0;
        }
    }
	
	/////////////////////////////////////////////////////////////////////////
	// Parameters
	// nodeMap:           Node map that is the size of the loaded map
	// 					  [row, col], nodeMap[1,2] = node in row 1 & col 2
	// FloorMap:          Tile map that holds floor tiles
	// WallMap:           Tile map that holds wall tiles
	// DebugMap:          Tile map that is used for debugging
	// notWalkableMarker: Debug Tile that indicates a node as "Unwalkable"
	// walkableMarker: 	  Debug Tile that indicates a node as "Walkable"
    public Node[,] nodeMap;
    public Tilemap FloorMap, WallMap, DebugMap;
    public Tile    notWalkableMarker, walkableMarker;
	
    public Vector2Int mapSize;
    public bool       debugInitMode = false;
	public bool       debugPathMode = false;
	

    public List<Node> FindPath(Vector2Int APos, Vector2Int BPos)
    {
		SetupAStarNodeMap(FloorMap);
		
        Node startingNode = nodeMap[APos.y + (mapSize.y/2), APos.x + (mapSize.x/2)];
        Node endNode = nodeMap[APos.y + (mapSize.y/2), APos.x + (mapSize.x/2)];
		
		if (debugPathMode == true)
		{
			DrawBox(new Vector2(startingNode.pos.x - 0.5f, startingNode.pos.y - 0.5f), new Vector2(1f, 1f)): 
			// ^--- This will prob be int plus float error
		}
		
		List<Node> openNodes   = new List<Node>();
		List<Node> closedNodes = new List<Node>();
		List<Node> nodePath    = new List<Node>();
		
		// Initializing the starting node
		startingNode.previousNode = null;
		startingNode.gValue       = 0;
		startingNode.hValue       = calcHValue(startingNode, endNode);
		startingNode.fValue       = calcFValue(startingNode);
        openNodes.Add(startingNode);
		
		// currentNode: The node currently being considered from the open list
		Node currentNode;
		Node neighborNode;

		// Debug Value used to try and avoid infinte loops
        int loopCount = 0;
        
		// Keep iterating through the Open Nodes till we find the path
		while(openNodes.Count > 0)
		{
            loopCount += 1;

            if (loopCount > 100)
            {
				Debug.Log("Path could not be completed: Loop too Big, Ouchie");
                return null;
            }
			
			// Log the lowest F value from the current Open list
			currentNode = getNodeWithLowestFValue(openNodes);   
            Debug.Log(currentNode.fValue);
			
			if (currentNode == endNode)
			{
				return calculateFinalPath(currentNode);
			}
			
			openNodes.Remove(currentNode);			
			closedNodes.Add(currentNode);
            

			// Iterate through the Node's touching neighbors, Pos.x = row value
			for (int row = currentNode.Pos.x - 1; row <= currentNode.Pos.x + 1; row++)
			{
				// Iterate through the Node's touching neighbors, Pos.y = col value
				for (int col = currentNode.Pos.y - 1; col <= currentNode.Pos.y + 1; col++)
				{
					neighborNode = nodeMap[row, col];
					
					if ((neighborNode.walkable == true) && !(closedNodes.Contains(neighborNode)))
					{						
						float tentativeGValue = calcGValue(neighborNode);
						
						if (tentativeGValue < neighborNode.gValue)
						{	
							nodeMap[row, col].gValue = tentativeGValue;
					
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
		
		// If we ran out open nodes and no complete path
        return null;
    }
	
	
	public void SetupAStarNodeMap(Tilemap FloorMap)
    {
        mapSize = ((Vector2Int)FloorMap.size);
		
        NodeMap = new Node[mapSize.y,mapSize.x];

        for (int row = 0; row < mapSize.y; row++)
        {
            for (int col = 0; col < mapSize.x; col++)
            {
                NodeMap[row, col] = new Node(row, col, false);
				nodeMap[row, col].row = row; // nodeMap[row + mapSize.y / 2, col + mapSize.x / 2].Pos = new Vector2Int(row, col);
				nodeMap[row, col].col = col;
				nodeMap[row, col].worldY = row - (mapSize.y / 2);
				nodeMap[row, col].worldX = col - (mapSize.x / 2);
				
				// Marking the non-Floor tiles as "not walkable"
                if (FloorMap.GetTile(new Vector3Int(col, row, 0)) == null)
                {
                    nodeMap[row + mapSize.y / 2, col + mapSize.x / 2].walkable = false;

                    if (debugInitMode == true)
                    {
                        DebugMap.SetTile(new Vector3Int(col, row, 0), notWalkableMarker);
                    }
                    
                }
				
                else
                {
                    nodeMap[row + mapSize.y / 2, col + mapSize.x / 2].walkable = true;

                    if (debugInitMode == true)
                    {
                        DebugMap.SetTile(new Vector3Int(col, row, 0), walkableMarker);
                    }
                }
            }
        }
    }
	
	
	public List<Node> calculateFinalPath(Node node)
	{
		List<Node> nodePath = new List<Node>();
		// Tracing back through the most effcient path
        while (node.previousNode != null)
		{
			nodePath.Add(node);
			node = node.previousNode;
		}
		nodePath.Reverse();

        return nodePath;
	}
	
	
	public float calcGValue(Node A)
	{
		float gValue;
        if (A.previousNode != null)
        {
            gValue = A.previousNode.gValue + Mathf.Sqrt((A.col + A.previousNode.col) ^ 2 + (A.row + A.previousNode.row) ^ 2);
        }
        else
        {
            gValue = 0;
        }
		return gValue;
	}
	
	
	public float calcHValue(Node A, Node B)
	{
		float hValue = Mathf.Sqrt((A.col + B.col)^2 + (A.row + B.row)^2);
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
	
	
	public void DrawBox(Vector2 origin, Vector2 size, Color color = Color.white, float duration = Mathf.Infinity, bool depthTest = true;)
	{		
		// Draw the left side
		// Draw the top side
		// Draw the right side
		// Draw the bottom side
		Debug.DrawLine(new Vector3(origin.x         , origin.y         , 0), Vector3(origin.x         , origin.y + size.y, 0), color, duration, depthTest):
		Debug.DrawLine(new Vector3(origin.x         , origin.y + size.y, 0), Vector3(origin.x + size.x, origin.y + size.y, 0), color, duration, depthTest):
		Debug.DrawLine(new Vector3(origin.x + size.x, origin.y + size.y, 0), Vector3(origin.x + size.x, origin.y         , 0), color, duration, depthTest):
		Debug.DrawLine(new Vector3(origin.x + size.x, origin.y         , 0), Vector3(origin.x         , origin.y         , 0), color, duration, depthTest):
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