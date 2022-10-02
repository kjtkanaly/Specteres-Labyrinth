using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WandOrientation : MonoBehaviour
{
    public Transform WandTrans;
    public Vector2 MousePos;

    private void Start()
    {
        WandTrans = this.transform;
    }

    private void Update()
    {
        MousePos = new Vector2(Input.mousePosition.x - Screen.width/2, Input.mousePosition.y - Screen.height/2);

        WandTrans.rotation = Quaternion.Euler(new Vector3(WandTrans.rotation.x, WandTrans.rotation.y, Mathf.Rad2Deg * Mathf.Atan2(MousePos.y, MousePos.x)));
    }
}
