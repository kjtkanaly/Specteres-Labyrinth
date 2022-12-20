using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevitateBarScript : MonoBehaviour
{
    public Slider slider;
    public Image FillImage;
    public Color FillColor;

    public IEnumerator fadeCoroutine;

    public float timeToFade = 0.1f;
    public float alphaStepCount = 100f;
    public float alphaStepSize;
    
    private void Start()
    {
        alphaStepSize = timeToFade / alphaStepCount;

        // fadeCoroutine = StartCoroutine(fade());
        
    }

    public void SetMaxLevitation(int mana)
    {
        slider.maxValue = mana;
        slider.value = mana;
    }

    public void SetLeviataion(int mana)
    {
        slider.value = mana;
    }

    public IEnumerator fade(bool fadeDirection)
    {  
        // Fade in
        if (fadeDirection)
        {
            while (FillImage.color.a >= alphaStepSize)
            {
                yield return new WaitForSeconds(timeToFade);

                FillColor = FillImage.color;
                FillColor.a += alphaStepSize;
                FillImage.color = FillColor;
            }            
        }
        // Fade out
        else
        {
            while (FillImage.color.a <= (1 - alphaStepSize))
            {
                yield return new WaitForSeconds(timeToFade);

                FillColor = FillImage.color;
                FillColor.a -= alphaStepSize;
                FillImage.color = FillColor;
            }
        }

        // StopCoroutine(fadeCoroutine);

    }
}

