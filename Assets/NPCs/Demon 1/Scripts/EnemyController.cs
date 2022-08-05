using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class EnemyController : MonoBehaviour
{
    ///////////////////////////////////////
    // Generic Methods
    public LevelGeneration LevelGen;
    public AStar           AstarController;
    public Transform       PlayerPos;
    public GameObject      PlayerObject;
    public RaycastHit2D    rayCastHit;
    public LayerMask       playerLayer;

    //////////////////////////////
    // Path Finding Parameters
    public AStar.Node[,]    nodeMap;
    public List<AStar.Node> PathToPlayer;
    public Transform  npcTruePos;
    public Vector2Int npcNodePos;
    public Transform  playerTruePos;
    public Vector2Int playerNodePos;

    //////////////////////////////
    // Generic Variables
    // distanceToPlayer:    The radial distance to the player
    // updatePathTimeDelay: Delta time between updating A* path
    // debugPath:           Flag used to display NPC's path
    public float distanceToPlayer    = float.MaxValue;
    public float updatePathTimeDelay = 2f;
    public float checkRadius         = 10f;
    public bool  debugPath           = true;
    public bool  pathFindingRN       = false;

    void Awake()
    {
        // Initializing General Enemy Controller Parameters
        PlayerObject    = GameObject.FindGameObjectWithTag("Player");
        playerLayer     = LayerMask.GetMask("Player");
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

        //StartCoroutine(callAstarPathUpdate());
    }

    public void FixedUpdate()
    {
        distanceToPlayer = Vector2.Distance(playerTruePos.position, npcTruePos.position);

        if ((distanceToPlayer <= checkRadius) && (pathFindingRN == false))
        {
            Debug.DrawRay(npcTruePos.position, playerTruePos.position - npcTruePos.position);
            
            // Ray Cast to check if player is visible
            if (Physics2D.Raycast(npcTruePos.position, playerTruePos.position - npcTruePos.position, checkRadius).collider.tag == "Player")
            {
                Debug.Log("Starting Path Finding");
                pathFindingRN = true;
                StartCoroutine(callAstarPathUpdate());
            }
        }
        else if ((distanceToPlayer > checkRadius) && (pathFindingRN == true))
        {
            pathFindingRN = false;
            StopCoroutine(callAstarPathUpdate());
        }
    }

    IEnumerator callAstarPathUpdate()
    {
        while (pathFindingRN)
        {
            yield return new WaitForSeconds(updatePathTimeDelay);

            npcNodePos = new Vector2Int(Mathf.RoundToInt(Mathf.Floor(npcTruePos.position.x)), Mathf.RoundToInt(Mathf.Floor(npcTruePos.position.y))) + (LevelGen.nodeMapSize / 2);
            playerNodePos = new Vector2Int(Mathf.RoundToInt(Mathf.Floor(playerTruePos.position.x)), Mathf.RoundToInt(Mathf.Floor(playerTruePos.position.y))) + (LevelGen.nodeMapSize / 2);

            yield return StartCoroutine(AstarController.FindPath(npcNodePos, playerNodePos, true));

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
