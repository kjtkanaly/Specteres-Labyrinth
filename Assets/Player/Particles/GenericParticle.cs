using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenericParticle : MonoBehaviour
{
    public Rigidbody2D RB;
    public Transform Trans;
    public SpriteRenderer SP;

    public Vector2 lifeTimeRange = new Vector2(0.1f, 0.4f);
    public Vector2 AlphaRange = new Vector2(0.4f, 0.85f);

    public float downwardVelocity = -6f;
    public float verticalSapwnPos = -0.5f;
    public float horizontalVelocityRange = 0.5f;
    public float horizontalSpawnRange = 0.15f;
    public float airDrag = 0.2f;

    public float lifeTime;

    public void Awake()
    {
        RB = this.GetComponent<Rigidbody2D>();
        Trans = this.GetComponent<Transform>();    
        SP = this.GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void OnEnable()
    {   
        float horizontalVelocity = Random.Range(-horizontalVelocityRange,
                                                horizontalVelocityRange);
        RB.velocity = new Vector2(horizontalVelocity, downwardVelocity);

        lifeTime = Random.Range(lifeTimeRange.x, lifeTimeRange.y);

        float SprAlpha = Random.Range(AlphaRange.x, AlphaRange.y);
        Color temp = SP.color;
        temp.a = SprAlpha;
        SP.color = temp;
    }

    private void Update()
    {   
        float VerticalSpeed = Mathf.MoveTowards(RB.velocity.y, -1f, 
                                                airDrag * Time.deltaTime);
        RB.velocity = new Vector2(RB.velocity.x, VerticalSpeed);
    }

    public IEnumerator lifeTimeCounter()
    {
        yield return new WaitForSeconds(lifeTime);
        this.gameObject.SetActive(false);
    }
}
