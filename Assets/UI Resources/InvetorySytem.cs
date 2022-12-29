using System.Collections;
using System.Collections.Generic;
using UnityEngine;
 using UnityEngine.UI;

public class InvetorySytem : MonoBehaviour
{
    public List<GameObject> WandInventory = new List<GameObject>();
    public List<Image> WandInventoryIconHighlights = new List<Image>();
    private List<Spell> SpellInvetory = new List<Spell>();

    public int currentActiveItem = 0;
    public int numberofWands = 4;

    public float wheelDelta = 0;
    private float scrollMultiplier = 2f;


    private void Start()
    {

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

            HighlightActiveWand();
            UpdatePlayerWand();
        }

        else if (wheelDelta >= 1)
        {
            currentActiveItem--;
            wheelDelta = 0;

            if (currentActiveItem == -1)
            {
                currentActiveItem = numberofWands - 1;
            }

            HighlightActiveWand();
            UpdatePlayerWand();
        }
    }


    public void UpdatePlayerWand()
    {
        for (int i = 0; i < numberofWands; i++)
        {
            if (WandInventory[i] != null)
            {
                WandInventory.SetActive(false);
            }
        }

        if (WandInventory[currentActiveItem] != null)
        {
            WandInventory[currentActiveItem].SetActive(true);
        }
    }


    public void HighlightActiveWand()
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
