using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class EnemyController : MonoBehaviour
{
    public LevelGeneration LevelGen;
    public Transform       PlayerTrans;
    public AStar           AstarController;

    public AStar.Node[,]    nodeMap;
    public List<AStar.Node> PathToPlayer;
	
    public Vector2Int thisPos;
    public Vector2Int playerPos;
    public bool debugPath = true;

    [SerializeField] private float updatePathTimeDelay = 2f;

    void Start()
    {
        PlayerTrans     = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
        LevelGen        = GameObject.FindGameObjectWithTag("Main Game").GetComponent<LevelGeneration>();
        AstarController = this.GetComponent<AStar>();
		
		//AstarController.FloorMap = GameObject.FindGameObjectWithTag("Floor Map").GetComponent<Tilemap>();
        //AstarController.DebugMap = GameObject.FindGameObjectWithTag("Debug Map").GetComponent<Tilemap>();

        StartCoroutine(callAstarPathUpdate(updatePathTimeDelay));
    }

    IEnumerator callAstarPathUpdate(float updatePathTimeDelay)
    {
        while (true)
        {
            yield return new WaitForSeconds(updatePathTimeDelay);

            AStar.Node[,] nodeMap = LevelGen.nodeMap;

            if (nodeMap != null)
            {
                thisPos = new Vector2Int(Mathf.RoundToInt(Mathf.Floor(this.transform.position.x)), Mathf.RoundToInt(Mathf.Floor(this.transform.position.y))) + (LevelGen.nodeMapSize / 2);
                playerPos = new Vector2Int(Mathf.RoundToInt(Mathf.Floor(PlayerTrans.transform.position.x)), Mathf.RoundToInt(Mathf.Floor(PlayerTrans.transform.position.y))) + (LevelGen.nodeMapSize / 2);

                PathToPlayer = AstarController.FindPath(thisPos, playerPos, nodeMap, true);

                if (PathToPlayer != null)
                {
                    Debug.Log("Number of Nodes: " + PathToPlayer.Count);
                    for (int listIndex = 0; listIndex < PathToPlayer.Count - 1; listIndex++)
                    {
                        //AStar.DrawBox(new Vector2(PathToPlayer[listIndex].worldX + 0.5f, PathToPlayer[listIndex].worldY + 0.5f), new Vector2(PathToPlayer[listIndex + 1].worldX + 0.5f, PathToPlayer[listIndex + 1].worldY + 0.5f),);
                        Debug.DrawLine(new Vector3(PathToPlayer[listIndex].worldX + 0.5f, PathToPlayer[listIndex].worldY + 0.5f, 0), new Vector3(PathToPlayer[listIndex + 1].worldX + 0.5f, PathToPlayer[listIndex + 1].worldY + 0.5f, 0), Color.green,
                                                    Mathf.Infinity, true);
                    }   
                }
            }
        }
    }
}


// Add logic to follow the path, now that we have it.
