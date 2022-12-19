using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevitateBarScript : MonoBehaviour
{
    public Slider slider;

    public void SetMaxLevitation(int mana)
    {
        slider.maxValue = mana;
        slider.value = mana;
    }

    public void SetLeviataion(int mana)
    {
        slider.value = mana;
    }

    public IEnumerator fadeAway(int timeToFade, bool fadeDirection)
    {   
        // Fade in
        if ()
        {

        }
        // Fade out
        else
        {

        }
    }
}

