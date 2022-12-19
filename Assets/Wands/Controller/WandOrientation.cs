using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WandOrientation : MonoBehaviour
{
    public Transform WandTrans;
    public Transform PlayerTrans;
    public SpriteRenderer PlayerSpr;
    public Vector2 MousePos;

    private void Awake()
    {
        WandTrans = this.transform;
        PlayerTrans = GameObject.FindWithTag("Player").transform;
        PlayerSpr = GameObject.FindWithTag("Player").GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        MousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition) - PlayerTrans.position;

        WandTrans.rotation = Quaternion.Euler(new Vector3(WandTrans.rotation.x, WandTrans.rotation.y, 
                            Mathf.Rad2Deg * Mathf.Atan2(MousePos.y, MousePos.x)));

        if (Mathf.Sign(MousePos.x) == 1)
        {
            PlayerSpr.flipX = false;
        }
        else
        {
            PlayerSpr.flipX = true;
        }
    }
}
