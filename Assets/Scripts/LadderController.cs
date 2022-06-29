using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LadderController : MonoBehaviour
{
    [SerializeField] private LevelGeneration levelGenerator;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            levelGenerator.ClearMap();
            levelGenerator.Start();
        }
    }

}
