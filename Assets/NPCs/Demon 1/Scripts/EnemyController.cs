using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public LevelGeneration LevelGen;
    public Transform       PlayerTrans;
    public AStar           AstarController;
	public Tilemap 		
	
    public List<AStar.Node> PathToPlayer;
	
    public Vector2Int thisPos;
    public Vector2Int playerPos;

    [SerializeField] private float updatePathTimeDelay = 2f;

    void Start()
    {
        PlayerTrans     = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
        AstarController = this.GetComponent<AStar>();
		
		AstarController.FloorMap = 1;

        StartCoroutine(callAstarPathUpdate(updatePathTimeDelay));
    }

    IEnumerator callAstarPathUpdate(float updatePathTimeDelay)
    {
        yield return new WaitForSeconds(updatePathTimeDelay);

        thisPos = new Vector2Int(Mathf.RoundToInt(this.transform.position.x), Mathf.RoundToInt(this.transform.position.y));
        playerPos = new Vector2Int(Mathf.RoundToInt(PlayerTrans.transform.position.x), Mathf.RoundToInt(PlayerTrans.transform.position.y));
		
		if (AstarController.FloorMap != null)
		{
			PathToPlayer = AstarController.FindPath(thisPos, thisPos + new Vector2Int(5,5));
			
			for (int listIndex = 0; listIndex < PathToPlayer.Count - 1; listIndex++)
			{
				Debug.DrawLine(new Vector3(PathToPlayer[listIndex].Pos.x, PathToPlayer[listIndex].Pos.y, 0), new Vector3(PathToPlayer[listIndex + 1].Pos.x, PathToPlayer[listIndex + 1].Pos.y, 0), Color.red,
											Mathf.Infinity, false);
			}
		}
    }
}


// Add logic to follow the path, now that we have it.