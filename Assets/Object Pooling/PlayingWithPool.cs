using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayingWithPool : MonoBehaviour
{
    public bool IsShooting = false;
    public float ShotTimeDelay = 1f;
    private IEnumerator ShootingControler = null;
    private GameObject MagicMissile;
    private Rigidbody2D MagicMissileRB;

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            GameObject Spell = ObjectPooling.SharedInstance.GetPooledObject();

            if (Spell != null)
            {
                Spell.SetActive(true);
            }
        }

    }

    public IEnumerator ShootingTimer()
    {
        yield return new WaitForSeconds(ShotTimeDelay);
            
    }
}
