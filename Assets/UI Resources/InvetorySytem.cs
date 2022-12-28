using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InvetorySytem : MonoBehaviour
{
    private List<Spell> SpellInvetory = new List<Spell>();

    public int currentActiveItem = 0;
    public int numberofWands = 4;

    public float wheelDelta = 0;
    private float scrollMultiplier = 2f;

    private void OnGUI()
    {
        wheelDelta += Input.mouseScrollDelta.y / (2 * scrollMultiplier);
    }


    private void Update()
    {
        if (wheelDelta <= -1)
        {
            currentActiveItem++;
            wheelDelta = 0;

            if (currentActiveItem >= numberofWands)
            {
                currentActiveItem = 0;
            }
        }

        else if (wheelDelta >= 1)
        {
            currentActiveItem--;
            wheelDelta = 0;

            if (currentActiveItem == -1)
            {
                currentActiveItem = numberofWands - 1;
            }
        }
    }

}
