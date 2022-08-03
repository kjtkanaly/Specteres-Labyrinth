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

    public float updatePathTimeDelay = 2f;

    void Start()
    {
        PlayerTrans     = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
        LevelGen        = GameObject.FindGameObjectWithTag("Main Game").GetComponent<LevelGeneration>();
        AstarController = this.GetComponent<AStar>();

        StartCoroutine(callAstarPathUpdate(updatePathTimeDelay));
    }

    IEnumerator callAstarPathUpdate(float updatePathTimeDelay)
    {
        while (true)
        {
            yield return new WaitForSeconds(updatePathTimeDelay);

            //nodeMap = AstarController.SetupAStarNodeMap(LevelGen.FloorMap, LevelGen.nodeMapSize, LevelGen.nodeMapDebug);

            // Debug
            /*
            for (int col = 0; col < LevelGen.nodeMapSize.x; col++)
            {
                for (int row = 0; row < LevelGen.nodeMapSize.y; row++)
                {
                    if (nodeMap[row, col].fValue != 0)
                    {
                        Debug.Log(LevelGen.nodeMap[row, col].fValue);
                    }
                }
            }*/

            thisPos = new Vector2Int(Mathf.RoundToInt(Mathf.Floor(this.transform.position.x)), Mathf.RoundToInt(Mathf.Floor(this.transform.position.y))) + (LevelGen.nodeMapSize / 2);
            playerPos = new Vector2Int(Mathf.RoundToInt(Mathf.Floor(PlayerTrans.transform.position.x)), Mathf.RoundToInt(Mathf.Floor(PlayerTrans.transform.position.y))) + (LevelGen.nodeMapSize / 2);

            yield return StartCoroutine(AstarController.FindPath(thisPos, playerPos, true));

            if (PathToPlayer != null)
            {
                if (debugPath == true)
                {
                    Debug.Log("Number of Nodes: " + PathToPlayer.Count);
                    for (int listIndex = 0; listIndex < PathToPlayer.Count - 1; listIndex++)
                    {
                        //AStar.DrawBox(new Vector2(PathToPlayer[listIndex].worldX + 0.5f, PathToPlayer[listIndex].worldY + 0.5f), new Vector2(PathToPlayer[listIndex + 1].worldX + 0.5f, PathToPlayer[listIndex + 1].worldY + 0.5f),);
                        Debug.DrawLine(new Vector3(PathToPlayer[listIndex].worldX + 0.5f, PathToPlayer[listIndex].worldY + 0.5f, 0), new Vector3(PathToPlayer[listIndex + 1].worldX + 0.5f, PathToPlayer[listIndex + 1].worldY + 0.5f, 0), Color.green,
                                                    updatePathTimeDelay, true);
                    }
                }
            }
        }
    }
}


// Add logic to follow the path, now that we have it.
