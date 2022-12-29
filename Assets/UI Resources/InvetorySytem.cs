using System.Collections;
using System.Collections.Generic;
using UnityEngine;
 using UnityEngine.UI;

public class InvetorySytem : MonoBehaviour
{
    public List<Wand> WandInventory = new List<Wand>();
    public List<Image> WandInventoryIconHighlights = new List<Image>();
    private List<Spell> SpellInvetory = new List<Spell>();

    public Wand PlayerWandObject;

    public int currentActiveItem = 0;
    public int numberofWands = 4;

    public float wheelDelta = 0;
    private float scrollMultiplier = 2f;


    private void Start()
    {
        for (int i = 0; i < numberofWands; i++)
        {
            Wand newWand = new Wand();
            WandInventory[i] = Instantiate();
        }
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
        Debug.Log(WandInventory[currentActiveItem]);
        if (WandInventory[currentActiveItem] != null)
        {
            Debug.Log(WandInventory[currentActiveItem].CastRate);

            PlayerWandObject.Shuffle = 
                WandInventory[currentActiveItem].Shuffle;
            PlayerWandObject.CastRate = 
                WandInventory[currentActiveItem].CastRate;
            PlayerWandObject.ManaMax = 
                WandInventory[currentActiveItem].ManaMax;
            PlayerWandObject.ManaCharge = 
                WandInventory[currentActiveItem].ManaCharge;
            PlayerWandObject.Capacity = 
                WandInventory[currentActiveItem].Capacity;
            PlayerWandObject.CastDelay = 
                WandInventory[currentActiveItem].CastDelay;
            PlayerWandObject.RechargeTime = 
                WandInventory[currentActiveItem].RechargeTime;
            PlayerWandObject.Spread = 
                WandInventory[currentActiveItem].Spread; 
        }
        else
        {
            PlayerWandObject.Shuffle = false;
            PlayerWandObject.CastRate = 0;
            PlayerWandObject.ManaMax = 0;
            PlayerWandObject.ManaCharge = 0;
            PlayerWandObject.Capacity = 0;
            PlayerWandObject.CastDelay = 0;
            PlayerWandObject.RechargeTime = 0;
            PlayerWandObject.Spread = 0; 
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
