using System.Collections;
using System.Collections.Generic;
using UnityEngine;
 using UnityEngine.UI;

public class InvetorySytem : MonoBehaviour
{
    public List<Wand> WandInventory = new List<Wand>();
    public List<Image> WandInventoryIconHighlights = new List<Image>();
    private List<Spell> SpellInvetory = new List<Spell>();

    public int currentActiveItem = 0;
    public int numberofWands = 4;

    public float wheelDelta = 0;
    private float scrollMultiplier = 2f;


    public void Start()
    {
        WandInventory = new List<Wand>(numberofWands);
    }


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

        highlightActiveWand();
    }

    public void highlightActiveWand()
    {
        Color c;

        for (int i = 0; i < numberofWands; i++)
        {
            c = WandInventoryIconHighlights[i].color;
            c.a = 0;
            WandInventoryIconHighlights[i].color = c;
        }

        c = WandInventoryIconHighlights[currentActiveItem].color;
        c.a = 1f;
        WandInventoryIconHighlights[currentActiveItem].color = c;
    }

}
