using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenericParticle : MonoBehaviour
{
    public enum ParticleType
    {
        Falling,
        Trail
    }

    public Rigidbody2D RB;
    public Transform Trans;
    public Transform PlayerTrans;
    public SpriteRenderer SP;

    public Vector2 lifeTimeRange = new Vector2(0.1f, 0.4f);
    public Vector2 AlphaRange = new Vector2(0.4f, 0.85f);

    public float downwardVelocity = -6f;
    public float verticalSapwnPos = -0.5f;
    public float horizontalVelocityRange = 0.5f;
    public float horizontalSpawnRange = 0.15f;
    public float airDrag = 0.2f;
    public float alphaStep = 0.1f;
    public float lifeTime;

    public ParticleType type;

    public void Awake()
    {
        RB = this.GetComponent<Rigidbody2D>();
        Trans = this.GetComponent<Transform>();    
        SP = this.GetComponent<SpriteRenderer>();

        PlayerTrans = GameObject.FindGameObjectWithTag("Player").transform;
    }

    // Update is called once per frame
    void OnEnable()
    {   
        if (type == ParticleType.Falling)
        {
            float horizontalVelocity = Random.Range(-horizontalVelocityRange,
                                                horizontalVelocityRange);
            RB.velocity = new Vector2(horizontalVelocity, downwardVelocity);

            lifeTime = Random.Range(lifeTimeRange.x, lifeTimeRange.y);

            float SprAlpha = Random.Range(AlphaRange.x, AlphaRange.y);
            Color temp = SP.color;
            temp.a = SprAlpha;
            SP.color = temp;

            // Setting the particle's spawn location
            this.transform.SetParent(PlayerTrans);
            float HorizontalSpawnPos = Random.Range(-horizontalSpawnRange,
                                                    horizontalSpawnRange);
            this.transform.localPosition = new Vector3(HorizontalSpawnPos, 
                                                       verticalSapwnPos);
            this.transform.SetParent(null);

            StartCoroutine(lifeTimeCounter());
        }
        else if (type == ParticleType.Trail)
        {
        }
    }


    private void Update()
    {   
        if (type == ParticleType.Falling)
        {
            float VerticalSpeed = Mathf.MoveTowards(RB.velocity.y, -1f, 
                                                    airDrag * Time.deltaTime);
            RB.velocity = new Vector2(RB.velocity.x, VerticalSpeed);
        }
        else if (type == ParticleType.Trail)
        {
            Color temp = SP.color;
            temp.a -= alphaStep * Time.deltaTime;
            SP.color = temp;
        }
    }
    

    public IEnumerator lifeTimeCounter()
    {
        yield return new WaitForSeconds(lifeTime);
        this.gameObject.SetActive(false);
    }

    public IEnumerator lifeTimeCounter(float lifeTime)
    {
        yield return new WaitForSeconds(lifeTime);
        this.gameObject.SetActive(false);
    }
}
