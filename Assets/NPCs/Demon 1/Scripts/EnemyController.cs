using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public AStar AstarController;
    
    [SerializeField] private float updatePathTimeDelay = 2f;
    private Vector2Int currentNode;

    void Start()
    {
        StartCoroutine(callAstarPathUpdate(updatePathTimeDelay));
    }

    IEnumerator callAstarPathUpdate(float updatePathTimeDelay)
    {
        yield return new WaitForSeconds(updatePathTimeDelay);

        currentNode = new Vector2Int(Mathf.RoundToInt(this.transform.position.x), Mathf.RoundToInt(this.transform.position.y));

        AstarController.UpdatePath(currentNode, currentNode + new Vector2Int(5,5));
    }
}
