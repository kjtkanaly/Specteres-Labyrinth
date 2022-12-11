using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticlePooling : MonoBehaviour
{
    public static ParticlePooling SharedInstance;

    public List<GenericParticle> particlePool;
    public GenericParticle particle;

    public int amountToPool;

    private void Awake()
    {
        SharedInstance = this;
    }

    // Start is called before the first frame update
    private void Start()
    {
        particlePool = new List<GenericParticle>();
        GameObject tmp;

        for (int i = 0; i < amountToPool; i++)
        {
            tmp = Instantiate(particle.gameObject);
            tmp.SetActive(false);
            particlePool.Add(tmp.GetComponent<GenericParticle>());
        }

    }

    // Update is called once per frame
    public GenericParticle GetPooledParticle()
    {
        for (int i = 0; i < amountToPool; i++)
        {
            if (!particlePool[i].gameObject.activeInHierarchy)
            {
                return particlePool[i];
            }
        }

        return null;
    }
}
