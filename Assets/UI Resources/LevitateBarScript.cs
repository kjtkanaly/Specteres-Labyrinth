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

