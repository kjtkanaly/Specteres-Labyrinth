using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCheckForEnemeies : MonoBehaviour
{
    private Collider2D playerIsWithinRange;
    [SerializeField] private LayerMask scanLayerMask;
    public float scanRadius = 5f;

    IEnumerator checkForEnemeies(float updateTime)
    {
        yield return new WaitForSeconds(updateTime);

        playerIsWithinRange = Physics2D.OverlapCircle((Vector2)this.transform.position, scanRadius, scanLayerMask);
    }
}
