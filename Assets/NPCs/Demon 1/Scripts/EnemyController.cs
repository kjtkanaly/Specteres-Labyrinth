using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public LevelGeneration levelGen;
    public Transform PlayerTrans;
    public AStar AstarController;
    public List<AStar.Node> PathToPlayer;
    public Vector2Int thisPos;
    public Vector2Int playerPos;

    [SerializeField] private float updatePathTimeDelay = 2f;

    void Start()
    {
        StartCoroutine(callAstarPathUpdate(updatePathTimeDelay));
    }

    IEnumerator callAstarPathUpdate(float updatePathTimeDelay)
    {
        yield return new WaitForSeconds(updatePathTimeDelay);

        thisPos = new Vector2Int(Mathf.RoundToInt(this.transform.position.x), Mathf.RoundToInt(this.transform.position.y));
        playerPos = new Vector2Int(Mathf.RoundToInt(PlayerTrans.transform.position.x), Mathf.RoundToInt(PlayerTrans.transform.position.y));

        PathToPlayer = AstarController.FindPath(thisPos, thisPos + new Vector2Int(5,5), levelGen.nodeMap);
    }
}
