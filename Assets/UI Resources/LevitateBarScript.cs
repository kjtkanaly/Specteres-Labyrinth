using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevitateBarScript : MonoBehaviour
{
    public Slider slider;
    public Image FillImage;
    public Color FillColor;

    public IEnumerator fadeInCoroutine;
    public IEnumerator fadeOutCoroutine;

    public float alphaFadeTime = 0.1f;
    public float alphaStepSize = 0.01f;
    public float updateAlphaTime;

    public bool fadeInRunning = false;
    public bool fadeOutRunning = false;
    
    private void Start()
    {
        updateAlphaTime = alphaStepSize * alphaFadeTime;

        fadeInCoroutine = AlphaFadeInTimer();
        fadeOutCoroutine = AlphaFadeOutTimer();
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


    public IEnumerator AlphaFadeOutTimer()
    {
        fadeOutRunning = true;

        // Check if the Fade In Corouting is running
        if (fadeInRunning)
        {
            StopCoroutine(fadeInCoroutine);
        }

        while (FillImage.color.a >= alphaStepSize)
        {
            yield return new WaitForSeconds(updateAlphaTime);

            FillColor = FillImage.color;
            FillColor.a -= alphaStepSize;
            FillImage.color = FillColor;
        }        
    }

    public IEnumerator AlphaFadeInTimer()
    {
        fadeInRunning = true;

        // Check if the Fade In Corouting is running
        if (fadeOutRunning)
        {
            StopCoroutine(fadeOutCoroutine);
        }

        while (FillImage.color.a <= (1 - alphaStepSize))
        {
            yield return new WaitForSeconds(updateAlphaTime);

            FillColor = FillImage.color;
            FillColor.a += alphaStepSize;
            FillImage.color = FillColor;
        }        
    }

    public void SetFillBarTrancparent()
    {
        FillColor = FillImage.color;
        FillColor.a = 0f;
        FillImage.color = FillColor;
    }


    public void SetFillBarLucent()
    {
        FillColor = FillImage.color;
        FillColor.a = 1f;
        FillImage.color = FillColor;
    }
}

