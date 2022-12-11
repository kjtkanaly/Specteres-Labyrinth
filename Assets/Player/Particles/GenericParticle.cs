using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenericParticle : MonoBehaviour
{
    public Rigidbody2D RB;
    public Transform Trans;

    public Vector2 lifeTimeRange = new Vector2(1f,3f);
    public Vector2 velocity = new Vector2(0f, -1f);

    public float lifeTime;


    public void Awake()
    {
        RB = this.GetComponent<Rigidbody2D>();
        Trans   = this.GetComponent<Transform>();    
    }

    // Update is called once per frame
    void OnEnable()
    {   
        RB.velocity = velocity;

        lifeTime = Random.Range(lifeTimeRange.x, lifeTimeRange.y);
    }

    public IEnumerator lifeTimeCounter()
    {
        yield return new WaitForSeconds(lifeTime);
        this.gameObject.SetActive(false);
    }
}
