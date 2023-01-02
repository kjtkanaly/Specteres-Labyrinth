using System.Collections;
using System.Collections.Generic;
using UnityEngine;
 using UnityEngine.UI;

public class InvetorySytem : MonoBehaviour
{
    public BoxCollider2D PlayerItemTrigger;


    public List<WandController> WandInventory = new List<WandController>();
    public List<Image> WandInventoryIconHighlights = new List<Image>();
    public List<Image> WandInventoryIcons = new List<Image>();
    private List<Spell> SpellInvetory = new List<Spell>();

    public int activeWandIndex = 0;
    public int numberofWands = 4;

    public float wheelDelta = 0;
    private float scrollMultiplier = 1f;

    // ------------------------------------------------------------------------
    private void Start()
    {
        UnEquipWand();
        EquipActiveWand();
    }

    // ------------------------------------------------------------------------
    private void OnGUI()
    {
        wheelDelta += Input.mouseScrollDelta.y / (2 * scrollMultiplier);
    }

    // ------------------------------------------------------------------------
    private void Update()
    {
        if (wheelDelta <= -1)
        {
            activeWandIndex++;
            wheelDelta = 0;

            if (activeWandIndex >= numberofWands)
            {
                activeWandIndex = 0;
            }

            HighlightActiveWand();
            UnEquipWand();
            EquipActiveWand();
        }

        else if (wheelDelta >= 1)
        {
            activeWandIndex--;
            wheelDelta = 0;

            if (activeWandIndex == -1)
            {
                activeWandIndex = numberofWands - 1;
            }

            HighlightActiveWand();
            UnEquipWand();
            EquipActiveWand();
        }

    }

    // ------------------------------------------------------------------------
    public void AddWandToInventory()
    {

    }

    // ------------------------------------------------------------------------
    public void EquipActiveWand()
    {
        if (WandInventory[activeWandIndex] != null)
        {
            WandInventory[activeWandIndex].WandProperties.wandState = 
                    Wand.WandState.Equipped;

            // Setting the wand visible again
            Color c = WandInventory[activeWandIndex].WandSprite.color;
            c.a = 1;
            WandInventory[activeWandIndex].WandSprite.color = c;

            // Setting the mana bar equal to the new wand's current mana
            WandInventory[activeWandIndex].ManaBarCtrl.SetMana(
                WandInventory[activeWandIndex].currentMagicMana);
        }
    }

    // ------------------------------------------------------------------------
    public void UnEquipWand()
    {
        for (int i = 0; i < numberofWands; i++)
        {
            if (WandInventory[i] != null)
            {
                WandInventory[i].WandProperties.wandState = 
                    Wand.WandState.InInventory;
                Color c = WandInventory[i].WandSprite.color;
                c.a = 0;
                WandInventory[i].WandSprite.color = c;
            }
        }
    }

    // ------------------------------------------------------------------------
    public void HighlightActiveWand()
    {
        Color c;

        for (int i = 0; i < numberofWands; i++)
        {
            c = WandInventoryIconHighlights[i].color;
            c.a = 0;
            WandInventoryIconHighlights[i].color = c;
        }

        c = WandInventoryIconHighlights[activeWandIndex].color;
        c.a = 1f;
        WandInventoryIconHighlights[activeWandIndex].color = c;
    }

}
