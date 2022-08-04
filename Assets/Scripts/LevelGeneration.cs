using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class LevelGeneration : MonoBehaviour
{
    public enum RoomType
    {
        Nothing = 0,
        Intro = 1,
        Branch = 2,
        Generic = 3,
        LoopBack = 4
    }

    public enum DivideDirection
    {
        VerticalDivide = 0,
        HorizontalDivide = 1
    }

    [System.Serializable]
    public class Room
    {
        public Vector2Int btmLeft;
        public Vector2Int topRight;
        public Vector2Int btmLeftTile;
        public Vector2Int topRightTile;
        public RoomType roomType;
        public Room parent;
        public int treeLevel;
        public int connections;
        public int siblingDirection;
        public bool hasSibling;
        public bool connected;

        // Create a class constructor for the Car class
        public Room(Vector2Int bottomLeftValue, Vector2Int topRightValue, RoomType roomTypeValue, Room parentValue, int treeLevelValue, int siblingDirectionValue, bool hasSiblingValue, bool connectedValue)
        {
            btmLeft = bottomLeftValue;
            topRight = topRightValue;
            roomType = roomTypeValue;
            parent = parentValue;
            treeLevel = treeLevelValue;
            siblingDirection = siblingDirectionValue;
            hasSibling = hasSiblingValue;
            connected = connectedValue;
            connections = 0;
        }
    }

    public Transform PlayerTrans;

    public static List<List<Room>> Tree = new List<List<Room>>();

    public Vector2Int DungeonDim = new Vector2Int(75, 50);
    public Vector2 RandomRangeMultiplyer = new Vector2(0.2f, 0.15f);
    public int bspIterations = 3;
    public int minRoomDim = 15;
    public int roomPadding = 4;
    public int hallwayPadding = 4;

    public GameObject Enemy;
    public GameObject Ladder;

    public Tilemap FloorMap, WallMap, TopLayerWallMap;
    public RuleTile wallTile;
    public Tile markerTile, floorTile, ladderTile, emptySpaceTile;
    public LevelTile wallBtmMidleTile, wallBtmLeftTile, wallBtmRightTile, wallBtmLeftCornner, wallBtmRightCornner;

    public AStar.Node[,] nodeMap;
    public Vector2Int nodeMapSize  = new Vector2Int(140, 100);
    public bool       nodeMapDebug = false;

    // A* Parameters

    public void Start()
    {
        FillInEmptySpaceTiles(WallMap, emptySpaceTile, DungeonDim + new Vector2Int(20, 20));

        var Root = new List<Room>();
        Root.Add(new Room(-DungeonDim / 2, DungeonDim / 2, RoomType.Generic, null, 0, -1, false, false));
        Tree.Add(Root);
        Tree = BspAlgorithm(Tree, bspIterations, RandomRangeMultiplyer, minRoomDim);

        Tree = CreateRooms(Tree, roomPadding, minRoomDim, floorTile, FloorMap, wallTile, WallMap, TopLayerWallMap, wallBtmMidleTile, wallBtmLeftCornner, wallBtmRightCornner);

        Tree = ConnectRooms(Tree, FloorMap, floorTile, wallTile, WallMap, TopLayerWallMap, wallBtmMidleTile, wallBtmLeftTile, wallBtmRightTile, hallwayPadding);

        /////////////////////////////////////////////
        // Creating the Spawn Logic
        List<int> potentialStartingRoomIndex = new List<int>();
        for (int i = 0; i < Tree[Tree.Count - 1].Count; i++)
        {
            if (Tree[Tree.Count - 1][i].connections == 1)
            {
                potentialStartingRoomIndex.Add(i);
            }
        }

        int startingRoomIndex = Random.Range(0, potentialStartingRoomIndex.Count);

        Room startingRoom = Tree[Tree.Count - 1][potentialStartingRoomIndex[startingRoomIndex]];
        Vector2Int startingRoomCenter = new Vector2Int((startingRoom.topRightTile.x + startingRoom.btmLeftTile.x) / 2, (startingRoom.topRightTile.y + startingRoom.btmLeftTile.y) / 2);

        PlayerTrans.position = new Vector3Int(startingRoomCenter.x, startingRoomCenter.y, 0);

        /////////////////////////////////////////////
        // Creating the Ladder Room
        Room ladderRoom = Tree[Tree.Count - 1][potentialStartingRoomIndex[0]];
        float furthestRoomDistance = 0;
        for (int i = 0; i <potentialStartingRoomIndex.Count; i++)
        { 
            if (potentialStartingRoomIndex[i] != potentialStartingRoomIndex[startingRoomIndex])
            {
                Room PotentialLadderRoom = Tree[Tree.Count - 1][potentialStartingRoomIndex[i]];
                Vector2Int roomCenter = new Vector2Int((PotentialLadderRoom.topRightTile.x + PotentialLadderRoom.btmLeftTile.x) / 2, (PotentialLadderRoom.topRightTile.y + PotentialLadderRoom.btmLeftTile.y) / 2);
                float distanceToStartingRoom = (roomCenter - startingRoomCenter).magnitude;
                
                if (distanceToStartingRoom > furthestRoomDistance)
                {
                    furthestRoomDistance = distanceToStartingRoom;
                    ladderRoom = Tree[Tree.Count - 1][potentialStartingRoomIndex[i]];
                }
            }
        }

        Vector2Int ladderPosition = new Vector2Int((ladderRoom.topRightTile.x + ladderRoom.btmLeftTile.x) / 2, ladderRoom.topRightTile.y - 1);
        FloorMap.SetTile((Vector3Int)ladderPosition, ladderTile);
        Ladder.transform.position = (Vector3Int)(ladderPosition);

        //nodeMap = this.GetComponent<AStar>().SetupAStarNodeMap(FloorMap, nodeMapSize, nodeMapDebug);

        /////////////////////////////////////////////
        for (int i = 0; i < Tree[Tree.Count - 1].Count; i++)
        {
            if (Tree[Tree.Count - 1][i] != startingRoom)
            {
                SpawnEnemies(Tree[Tree.Count - 1][i].btmLeftTile, Tree[Tree.Count - 1][i].topRightTile, Enemy);
            }
        }
    }

    public void FillInEmptySpaceTiles(Tilemap Map, Tile EmptySpaceTile, Vector2Int DungeonDim)
    {
        for (int row = -DungeonDim.x/2; row < DungeonDim.x/2; row++)
        {
            for (int col = -DungeonDim.y/2; col < DungeonDim.y/2; col++)
            {
                Map.SetTile(new Vector3Int(row, col, 0), EmptySpaceTile);
            }
        }
    }

    public void SpawnEnemies(Vector2Int btmLeft, Vector2Int topRight, GameObject Enemy)
    {
        GameObject enemy = (GameObject)Instantiate(Enemy);

        Vector2Int RoomCenter = (topRight + btmLeft) / 2;
        enemy.transform.position = new Vector3Int(RoomCenter.x, RoomCenter.y, 0);

    }

    static List<List<Room>> BspAlgorithm(List<List<Room>> Tree, int bspIterations, Vector2 RandomRangeMultiplyer, int minRoomDim)
    {
        DivideDirection Direction = DivideDirection.VerticalDivide;

        for (int bspCount = 0; bspCount < bspIterations; bspCount++)
        {
            List<Room> Parents = Tree[(Tree.Count - 1)];
            List<Room> Children = new List<Room>();

            for (int i = 0; i < Parents.Count; i++)
            {
                int DividePointX = Parents[i].topRight.x - (Parents[i].topRight.x - Parents[i].btmLeft.x) / 2;
                int DividePointY = Parents[i].topRight.y - (Parents[i].topRight.y - Parents[i].btmLeft.y) / 2;

                Vector2Int btmLeft = Parents[i].btmLeft;
                Vector2Int topRight = Parents[i].topRight;

                if ((Direction == DivideDirection.VerticalDivide) && (DividePointX - btmLeft.x > minRoomDim))
                {
                    DividePointX += Mathf.RoundToInt(Random.Range(-1 * (DividePointX * RandomRangeMultiplyer.x), DividePointX * RandomRangeMultiplyer.x));

                    Children.Add(new Room(Parents[i].btmLeft, new Vector2Int(DividePointX, Parents[i].topRight.y), RoomType.Nothing, Parents[i], Tree.Count, 0, true, false));
                    Children.Add(new Room(new Vector2Int(DividePointX, Parents[i].btmLeft.y), Parents[i].topRight, RoomType.Nothing, Parents[i], Tree.Count, 0, true, false));
                }
                else if ((Direction == DivideDirection.HorizontalDivide) && (DividePointY - btmLeft.y > minRoomDim))
                {
                    DividePointY += Mathf.RoundToInt(Random.Range(-1 * (DividePointY * RandomRangeMultiplyer.y), DividePointY * RandomRangeMultiplyer.y));

                    Children.Add(new Room(Parents[i].btmLeft, new Vector2Int(Parents[i].topRight.x, DividePointY), RoomType.Nothing, Parents[i], Tree.Count, 1, true, false));
                    Children.Add(new Room(new Vector2Int(Parents[i].btmLeft.x, DividePointY), Parents[i].topRight, RoomType.Nothing, Parents[i], Tree.Count, 1, true, false));
                }
                else if ((Direction == DivideDirection.VerticalDivide) && (DividePointY - btmLeft.y > minRoomDim))
                {
                    DividePointY += Mathf.RoundToInt(Random.Range(-1 * (DividePointY * RandomRangeMultiplyer.y), DividePointY * RandomRangeMultiplyer.y));

                    Children.Add(new Room(Parents[i].btmLeft, new Vector2Int(Parents[i].topRight.x, DividePointY), RoomType.Nothing, Parents[i], Tree.Count, 1, true, false));
                    Children.Add(new Room(new Vector2Int(Parents[i].btmLeft.x, DividePointY), Parents[i].topRight, RoomType.Nothing, Parents[i], Tree.Count, 1, true, false));
                }
                else if ((Direction == DivideDirection.HorizontalDivide) && (DividePointX - btmLeft.x > minRoomDim))
                {
                    DividePointX += Mathf.RoundToInt(Random.Range(-1 * (DividePointX * RandomRangeMultiplyer.x), DividePointX * RandomRangeMultiplyer.x));

                    Children.Add(new Room(Parents[i].btmLeft, new Vector2Int(DividePointX, Parents[i].topRight.y), RoomType.Nothing, Parents[i], Tree.Count, 0, true, false));
                    Children.Add(new Room(new Vector2Int(DividePointX, Parents[i].btmLeft.y), Parents[i].topRight, RoomType.Nothing, Parents[i], Tree.Count, 0, true, false));
                }
                else
                {
                    Debug.Log("Can't Divide the " + i + "th room any further");
                    return null;
                }
            }

            Tree.Add(Children);

            if (Direction == DivideDirection.VerticalDivide)
            {
                Direction = DivideDirection.HorizontalDivide;
            }
            else
            {
                Direction = DivideDirection.VerticalDivide;
            }
        }

        return Tree;
    }

    static List<List<Room>> CreateRooms(List<List<Room>> Tree, int roomPadding, int minRoomDim, Tile floorTile, Tilemap FloorMap, RuleTile wallTile, Tilemap WallMap, Tilemap TopLayerWallMap, LevelTile wallBtmMiddleTile, LevelTile wallBtmLeftCornner, LevelTile wallBtmRightCornner)
    {
        List<Room> bspLeafs = Tree[Tree.Count - 1];

        for (int i = 0; i < bspLeafs.Count; i++)
        {
            Vector2Int leafCenter = (bspLeafs[i].topRight + bspLeafs[i].btmLeft) / 2;

            Vector2Int roomPaddingBtmLeft = new Vector2Int(Random.Range(bspLeafs[i].btmLeft.x + roomPadding, leafCenter.x - minRoomDim / 2), Random.Range(bspLeafs[i].btmLeft.y + roomPadding, leafCenter.y - minRoomDim / 2));
            Vector2Int roomPaddingTopRight = new Vector2Int(Random.Range(leafCenter.x + minRoomDim / 2, bspLeafs[i].topRight.x - roomPadding), Random.Range(leafCenter.y + minRoomDim / 2, bspLeafs[i].topRight.y - roomPadding));

            Tree[Tree.Count - 1][i].btmLeftTile = roomPaddingBtmLeft;
            Tree[Tree.Count - 1][i].topRightTile = roomPaddingTopRight;

            LayLevelTile(FloorMap, floorTile, roomPaddingBtmLeft, roomPaddingTopRight);

            LayRuleTile(WallMap, wallTile, new Vector2Int(roomPaddingBtmLeft.x - 1, roomPaddingBtmLeft.y - 1), new Vector2Int(roomPaddingTopRight.x + 1, roomPaddingTopRight.y + 2));

            //LayLevelTile(TopLayerWallMap, wallBtmMiddleTile, new Vector2Int(roomPaddingBtmLeft.x, roomPaddingBtmLeft.y), new Vector2Int(roomPaddingTopRight.x, roomPaddingBtmLeft.y + 1));

            //TopLayerWallMap.SetTile(new Vector3Int(roomPaddingBtmLeft.x - 1, roomPaddingBtmLeft.y, 0), wallBtmLeftCornner);
            //TopLayerWallMap.SetTile(new Vector3Int(roomPaddingTopRight.x, roomPaddingBtmLeft.y, 0), wallBtmRightCornner);

        }

        return Tree;
    }

    static List<List<Room>> ConnectRooms(List<List<Room>> Tree, Tilemap FloorMap, Tile floorTile, RuleTile wallTile, Tilemap WallMap, Tilemap TopLayerWallMap, LevelTile wallBtmMidleTile, LevelTile wallBtmLeftTile, LevelTile wallBtmRightTile, int hallwayPadding)
    {
        List<Room> Rooms = Tree[Tree.Count - 1];

        for (int generationCount = Tree.Count - 1; generationCount > 0; generationCount--)
        {
            int subGroupSize = Rooms.Count / Tree[generationCount].Count;
            int GroupSize = subGroupSize * 2;

            for(int i = 0; i < Rooms.Count; i += GroupSize)
            {
                float smallestDistance = Mathf.Infinity;
                int closestEvenIndex = 0;
                int closestOddIndex = 0;

                for (int evenCount = 0 + i; evenCount < subGroupSize + i; evenCount++)
                {
                    for (int oddCount = subGroupSize + i; oddCount < GroupSize + i; oddCount++)
                    {
                        Vector2Int EvenCenter, OddCenter;

                        EvenCenter = Rooms[evenCount].topRightTile - (Rooms[evenCount].topRightTile - Rooms[evenCount].btmLeftTile) / 2;
                        OddCenter = Rooms[oddCount].topRightTile - (Rooms[oddCount].topRightTile - Rooms[oddCount].btmLeftTile) / 2;

                        float EvenOddDistance = Vector2.Distance(EvenCenter, OddCenter);

                        if(EvenOddDistance < smallestDistance)
                        {
                            smallestDistance = EvenOddDistance;
                            closestEvenIndex = evenCount;
                            closestOddIndex = oddCount;
                        }
                    }
                }

                Tree[Tree.Count - 1][closestEvenIndex].connections += 1;
                Tree[Tree.Count - 1][closestOddIndex].connections += 1;

                Vector2Int topRightI = Rooms[closestEvenIndex].topRightTile;
                Vector2Int topRightJ = Rooms[closestOddIndex].topRightTile;
                Vector2Int btmLeftI = Rooms[closestEvenIndex].btmLeftTile;
                Vector2Int btmLeftJ = Rooms[closestOddIndex].btmLeftTile;

                
                if (Tree[generationCount][i/subGroupSize].siblingDirection == 0)
                {
                    int tileTopY;
                    int tileBtmY;

                    if (topRightI.y > topRightJ.y)
                    {
                        tileTopY = topRightJ.y;
                    }
                    else
                    {
                        tileTopY = topRightI.y;
                    }

                    if (btmLeftI.y > btmLeftJ.y)
                    {
                        tileBtmY = btmLeftI.y;
                    }
                    else
                    {
                        tileBtmY = btmLeftJ.y;
                    }

                    int startY = Random.Range(tileBtmY + hallwayPadding, tileTopY - hallwayPadding);

                    LayLevelTile(FloorMap, floorTile, new Vector2Int(topRightI.x, startY), new Vector2Int(topRightJ.x, startY + 3));

                    LayRuleTile(WallMap, wallTile, new Vector2Int(topRightI.x, startY - 1), new Vector2Int(btmLeftJ.x, startY + 5));

                    //LayLevelTile(TopLayerWallMap, wallBtmMidleTile, new Vector2Int(topRightI.x + 1, startY), new Vector2Int(btmLeftJ.x - 1, startY + 1));

                    //TopLayerWallMap.SetTile(new Vector3Int(topRightI.x, startY, 0), wallBtmLeftTile);
                    //TopLayerWallMap.SetTile(new Vector3Int(btmLeftJ.x - 1, startY, 0), wallBtmRightTile);

                }

                else if (Tree[generationCount][i/subGroupSize].siblingDirection == 1)
                {
                    int tileRightX;
                    int tileLeftX;

                    if (topRightI.x > topRightJ.x)
                    {
                        tileRightX = topRightJ.x;
                    }
                    else
                    {
                        tileRightX = topRightI.x;
                    }

                    if (btmLeftI.x > btmLeftJ.x)
                    {
                        tileLeftX = btmLeftI.x;
                    }
                    else
                    {
                        tileLeftX = btmLeftJ.x;
                    }

                    int startX = Random.Range(tileLeftX + hallwayPadding, tileRightX - hallwayPadding);

                    LayLevelTile(FloorMap, floorTile, new Vector2Int(startX, topRightI.y), new Vector2Int(startX + 3, btmLeftJ.y));

                    LayRuleTile(WallMap, wallTile, new Vector2Int(startX - 1, topRightI.y), new Vector2Int(startX + 4, btmLeftJ.y));

                    //LayLevelTile(TopLayerWallMap, null, new Vector2Int(startX - 1, btmLeftJ.y), new Vector2Int(startX + 4, btmLeftJ.y + 1));

                    //TopLayerWallMap.SetTile(new Vector3Int(startX + 3, btmLeftJ.y, 0), wallBtmLeftTile);
                    //TopLayerWallMap.SetTile(new Vector3Int(startX - 1, btmLeftJ.y, 0), wallBtmRightTile);
                }
            }

        }

        return Tree;
    }

    public static void LayLevelTile(Tilemap Map, Tile Tile, Vector2Int BottomLeft, Vector2Int TopRight)
    {
        for (int row = BottomLeft.y; row < TopRight.y; row++)
        {
            for (int col = BottomLeft.x; col < TopRight.x; col++)
            {
                Map.SetTile(new Vector3Int(col, row, 0), Tile);
            }
        }
    }

    public static void LayRuleTile(Tilemap Map, RuleTile Tile, Vector2Int BottomLeft, Vector2Int TopRight)
    {
        for (int row = BottomLeft.y; row < TopRight.y; row++)
        {
            for (int col = BottomLeft.x; col < TopRight.x; col++)
            {
                Map.SetTile(new Vector3Int(col, row, 0), Tile);
            }
        }
    }

    public void ResetVariables()
    {
        
    }

    public void ClearMap()
    {
        Tree = new List<List<Room>>();

        foreach (var obj in GameObject.FindGameObjectsWithTag("Enemy"))
        {
            Destroy(obj);
        }

        var maps = FindObjectsOfType<Tilemap>();

        foreach (var tilemap in maps)
        {
            tilemap.ClearAllTiles();
        }

        /*
        foreach (var obj in GameObject.FindGameObjectsWithTag("Interactable Objects"))
        {
            DestroyImmediate(obj);
        }*/

    }

}
