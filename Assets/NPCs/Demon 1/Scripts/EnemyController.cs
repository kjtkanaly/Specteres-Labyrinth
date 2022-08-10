using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class EnemyController : MonoBehaviour
{
    ///////////////////////////////////////
    // Generic Methods
    public LevelGeneration LevelGen;
    public AStar           npcAstarControl;
    public Transform       PlayerPos;
    public Rigidbody2D     npcRigidBody;
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
    public Vector2Int playerPrevNodePos;
    public Vector2    pathNodePos;

    /////////////////////////////////////////////////////////////////////////
    // Generic Variables w/ Values
    //
    // velocity:            Velocity of the NPC
    // acceleration:        Acceleration of the NPC during path Finding
    // movementSpeed:       Speed of the NPC during path finding
    // distanceToPlayer:    Radial distance to player
    // updatePathTimeDelta: Time between updating path
    // followPlayerRadius:  The radial cutoff to start following the player
    // pathNodeRadius:      A node's radial cutoff
    // attackRadius:        The radial cutoff to attack the player
    // debugPath:           Toggles debug mode for the path following
    // pathFinding:         Path following flag
    // followPath:          Following the path flag

    public Vector2 velocity            = new Vector2(0, 0);
    public float   acceleration        = 100f;
    public float   movementSpeed       = 12f;
    public float   distanceToPlayer    = float.MaxValue;
    public float   updatePathTimeDelta = 2f;
    public float   followPlayerRadius  = 10f;
    public float   pathNodeRadius      = 0.5f;
    public int     attackRadius        = 2;
    public bool    debugPath           = true;
    public bool    canUpdatePath       = true;
    public bool    pathFinding         = false;
    public bool    followPath          = false;

    //////////////////////////////
    // Generic Variables w/out Values
    public  float       step;
    private IEnumerator callPathFinding;
    private IEnumerator pathFindingFx;

    void Awake()
    {
        // Initializing General Enemy Controller Parameters
        PlayerObject    = GameObject.FindGameObjectWithTag("Player");
        playerLayer     = LayerMask.GetMask("Player");
        LevelGen        = GameObject.FindGameObjectWithTag("Main Game").GetComponent<LevelGeneration>();
        npcAstarControl = this.GetComponent<AStar>();
        npcTruePos      = this.transform;
        npcRigidBody    = this.GetComponent<Rigidbody2D>();
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

        //////////////////////////////////
        // Path Finding Control
        distanceToPlayer = Vector2.Distance(playerTruePos.position, npcTruePos.position);

        if ((distanceToPlayer <= followPlayerRadius) && (distanceToPlayer > attackRadius) && (canUpdatePath == true))
        {            
            // Ray Cast to check if player is visible
            if (Physics2D.Raycast(npcTruePos.position, playerTruePos.position - npcTruePos.position, followPlayerRadius).collider.tag == "Player")
            {
                Debug.Log("Starting Path Finding");            
                callPathFinding = UpdateAstarPath();
                StartCoroutine(callPathFinding);
            }
        }


        if ((distanceToPlayer > followPlayerRadius) && (followPath == true))
        {
            Debug.Log("Haulting Path Finding");
            followPath = false;
        }

        //////////////////////////////////
        // Following Path Control
        if ((PathToPlayer.Count > attackRadius) && (followPath == true))
        {
            pathNodePos = new Vector2(PathToPlayer[0].worldX, PathToPlayer[0].worldY);

            step = movementSpeed * Time.fixedDeltaTime;

            npcTruePos.position = Vector2.MoveTowards(npcTruePos.position, pathNodePos, step);

            //velocity += ((pathNodePos - (Vector2)npcTruePos.position) * (acceleration * Time.fixedDeltaTime));
            //npcRigidBody.velocity = Vector2.ClampMagnitude(velocity, npcMaxVelocity);

            if (Vector2.Distance(this.transform.position, pathNodePos) < pathNodeRadius)
            {
                PathToPlayer.RemoveAt(0);
            }
        }

        //////////////////////////////////
        // Slowing Down the NPC
        if ((followPath == false) && (velocity.magnitude > 0.001f))
        {
            velocity = velocity / 2f;
            npcRigidBody.velocity = velocity;            
        }
        
    }

    // Used to hault all Path Finding Coroutines
    public void stopPathFindingCoroutines()
    {
        StopCoroutine(callPathFinding);
        StopCoroutine(pathFindingFx);
    }

    IEnumerator UpdateAstarPath()
    {
        // Updating the NPC and Player's Node Positions
        npcNodePos = new Vector2Int(Mathf.RoundToInt(Mathf.Floor(npcTruePos.position.x)), Mathf.RoundToInt(Mathf.Floor(npcTruePos.position.y))) + (LevelGen.nodeMapSize / 2);
        playerNodePos = new Vector2Int(Mathf.RoundToInt(Mathf.Floor(playerTruePos.position.x)), Mathf.RoundToInt(Mathf.Floor(playerTruePos.position.y))) + (LevelGen.nodeMapSize / 2);

        // Checking if the player is in a new node
        if (playerNodePos != playerPrevNodePos)
        {
            // Updating the Player's Previous Node
            playerPrevNodePos = playerNodePos;

            // Calling for the NPC's path to be updated
            pathFindingFx = npcAstarControl.FindPath(npcNodePos, playerNodePos, true);
            yield return StartCoroutine(pathFindingFx);

            canUpdatePath = false;
            followPath = true;

            StartCoroutine(UpdateAstarPathTimer(updatePathTimeDelta));

            // Checking if the path exsist and if the path finding is in debug mode
            if ((PathToPlayer != null) && (debugPath == true))
            {
                for (int listIndex = 0; listIndex < PathToPlayer.Count - 1; listIndex++)
                {
                    Debug.DrawLine(new Vector3(PathToPlayer[listIndex].worldX + 0.5f, PathToPlayer[listIndex].worldY + 0.5f, 0), new Vector3(PathToPlayer[listIndex + 1].worldX + 0.5f, PathToPlayer[listIndex + 1].worldY + 0.5f, 0), Color.green,
                                                updatePathTimeDelta, true);
                }
            }
        }
    }

    IEnumerator UpdateAstarPathTimer(float updatePathTimeDelay)
    {
        yield return new WaitForSeconds(updatePathTimeDelay);
        canUpdatePath = true;
    }
}


// Add logic to follow the path, now that we have it.


//////////////////////////////////
/// Old Code
/// IEnumerator callAstarPathUpdate()
/*
{
    while (pathFinding)
    {
        npcNodePos = new Vector2Int(Mathf.RoundToInt(Mathf.Floor(npcTruePos.position.x)), Mathf.RoundToInt(Mathf.Floor(npcTruePos.position.y))) + (LevelGen.nodeMapSize / 2);
        playerNodePos = new Vector2Int(Mathf.RoundToInt(Mathf.Floor(playerTruePos.position.x)), Mathf.RoundToInt(Mathf.Floor(playerTruePos.position.y))) + (LevelGen.nodeMapSize / 2);

        yield return StartCoroutine(npcAstarControl.FindPath(npcNodePos, playerNodePos, true));

        if (PathToPlayer != null)
        {

            if (debugPath == true)
            {
                for (int listIndex = 0; listIndex < PathToPlayer.Count - 1; listIndex++)
                {
                    Debug.DrawLine(new Vector3(PathToPlayer[listIndex].worldX + 0.5f, PathToPlayer[listIndex].worldY + 0.5f, 0), new Vector3(PathToPlayer[listIndex + 1].worldX + 0.5f, PathToPlayer[listIndex + 1].worldY + 0.5f, 0), Color.green,
                                                updatePathTimeDelta, true);
                }
            }
        }

        yield return new WaitForSeconds(updatePathTimeDelta);
    }
}*/