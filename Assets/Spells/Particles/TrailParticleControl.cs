using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrailParticleControl : MonoBehaviour
{
    public SpriteRenderer Spr;
    public Color color;

    public float fadeTimeDelay = 0.05f;
    public float colorAlphaStep = 0.05f;

    private void Awake()
    {
        Spr = this.gameObject.GetComponent<SpriteRenderer>();
    }


    private void FixedUpdate()
    {
        if (Spr.color.a < colorAlphaStep)
        {
            this.gameObject.SetActive(false);

            color = Spr.color;
            color.a = 1f;
            Spr.color = color;
        }
    }


    public IEnumerator FadeTimer()
    {
        while(this.gameObject.activeInHierarchy)
        {
            yield return new WaitForSeconds(fadeTimeDelay);
            color = Spr.color;
            color.a -= colorAlphaStep;
            Spr.color = color;
        }
    }
}
