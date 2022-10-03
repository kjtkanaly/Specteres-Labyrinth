using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WandController : MonoBehaviour
{
    public WandOrientation WandDirection;

    public float FireRate = 0.5f;
    public float RechargeRate = 0.5f;

    private bool CanCastSpell = true;

    private void Update()
    {
        if (Input.GetMouseButton(0) && CanCastSpell)
        {
            GameObject Spell = ObjectPooling.SharedInstance.GetPooledObject();

            if (Spell != null)
            {
                Spell.SetActive(true);
                Spell.GetComponent<BouncingBallPhysics>().Parent         = this.transform;
                Spell.GetComponent<BouncingBallPhysics>().MouseDirection = WandDirection.MousePos;
                Spell.GetComponent<BouncingBallPhysics>().OnActive();
            }

            StartCoroutine(GeneralTimer(FireRate));
            CanCastSpell = false;
        }
    }

    public IEnumerator GeneralTimer(float Duration)
    {
        yield return new WaitForSeconds(Duration);

        CanCastSpell = true;
    }
}
