using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WandOrientation : MonoBehaviour
{
    public WandController WandControler;
    public Transform WandTrans;
    public Transform PlayerTrans;
    public Vector2 MousePos;

    private void Awake()
    {
        WandControler = this.gameObject.GetComponent<WandController>();

        WandTrans = this.transform;
        PlayerTrans = GameObject.FindWithTag("Player").transform;
    }

    private void Update()
    {
        if (WandControler.WandProperties.wandState == Wand.WandState.Equipped)
        {
            MousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition) - 
                       PlayerTrans.position;

            WandTrans.rotation = Quaternion.Euler(
                                 new Vector3(WandTrans.rotation.x, 
                                             WandTrans.rotation.y, 
                                             Mathf.Rad2Deg * 
                                             Mathf.Atan2(MousePos.y, 
                                                         MousePos.x)));
        }
    }
}
