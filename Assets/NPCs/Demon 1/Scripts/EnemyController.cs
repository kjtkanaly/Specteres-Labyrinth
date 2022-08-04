using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class EnemyController : MonoBehaviour
{
    public LevelGeneration LevelGen;
    public AStar           AstarController;

    public AStar.Node[,]    nodeMap;
    public List<AStar.Node> PathToPlayer;

    public Transform  npcTruePos;
    public Vector2Int npcNodePos;
    public Transform  playerTruePos;
    public Vector2Int playerNodePos;
    public bool debugPath = true;

    public float updatePathTimeDelay = 2f;

    void Awake()
    {
        // Initializing General Enemy Controller Parameters
        AstarController = this.GetComponent<AStar>();
        LevelGen        = GameObject.FindGameObjectWithTag("Main Game").GetComponent<LevelGeneration>();
        npcTruePos      = this.transform;
        playerTruePos   = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
        PathToPlayer    = new List<AStar.Node>();

        // Initializing A Star Components Parameters
        this.GetComponent<AStar>().LevelGen = GameObject.FindGameObjectWithTag("Main Game").GetComponent<LevelGeneration>();
        this.GetComponent<AStar>().NPCController = this;
        this.GetComponent<AStar>().PathToPlayer = new List<AStar.Node>();
        this.GetComponent<AStar>().openNodes = new List<AStar.Node>();
        this.GetComponent<AStar>().closedNodes = new List<AStar.Node>();

        StartCoroutine(callAstarPathUpdate());
    }

    IEnumerator callAstarPathUpdate()
    {
        while (true)
        {
            yield return new WaitForSeconds(updatePathTimeDelay);

            npcNodePos = new Vector2Int(Mathf.RoundToInt(Mathf.Floor(npcTruePos.position.x)), Mathf.RoundToInt(Mathf.Floor(npcTruePos.position.y))) + (LevelGen.nodeMapSize / 2);
            playerNodePos = new Vector2Int(Mathf.RoundToInt(Mathf.Floor(playerTruePos.position.x)), Mathf.RoundToInt(Mathf.Floor(playerTruePos.position.y))) + (LevelGen.nodeMapSize / 2);

            yield return StartCoroutine(AstarController.FindPath(npcNodePos, playerNodePos, true));

            Debug.Log(PathToPlayer.Count);

            if (PathToPlayer != null)
            {
                if (debugPath == true)
                {
                    for (int listIndex = 0; listIndex < PathToPlayer.Count - 1; listIndex++)
                    {
                        Debug.DrawLine(new Vector3(PathToPlayer[listIndex].worldX + 0.5f, PathToPlayer[listIndex].worldY + 0.5f, 0), new Vector3(PathToPlayer[listIndex + 1].worldX + 0.5f, PathToPlayer[listIndex + 1].worldY + 0.5f, 0), Color.green,
                                                    updatePathTimeDelay, true);
                    }
                }
            }
        }
    }
}


// Add logic to follow the path, now that we have it.
