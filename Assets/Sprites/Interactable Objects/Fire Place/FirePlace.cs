using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirePlace : MonoBehaviour
{
    public Animator animator;

    void OnMouseOver()
    {
        
        if (Input.GetKeyDown("e"))
        {
            if (animator.GetBool("FireToggle"))
            {
                animator.SetBool("FireToggle", false);
            }
            else if(!(animator.GetBool("FireToggle")))
            {
                animator.SetBool("FireToggle", true);
            }
        }
    }



}
