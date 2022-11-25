using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WandOrientation : MonoBehaviour
{
    public Transform WandTrans;
    public Transform PlayerTrans;
    public Vector2 MousePos;

    private void Start()
    {
        WandTrans = this.transform;
        PlayerTrans = GameObject.FindWithTag("Player").transform;
    }

    private void Update()
    {
        MousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition) - PlayerTrans.position;

        WandTrans.rotation = Quaternion.Euler(new Vector3(WandTrans.rotation.x, WandTrans.rotation.y, 
                            Mathf.Rad2Deg * Mathf.Atan2(MousePos.y, MousePos.x)));
    }
}
