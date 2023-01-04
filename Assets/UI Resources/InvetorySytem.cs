using System.Collections;
using System.Collections.Generic;
using UnityEngine;
 using UnityEngine.UI;

public class InvetorySytem : MonoBehaviour
{
    public BoxCollider2D PlayerItemTrigger;
    public Transform PlayerTrans;

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
    public void AddWandToInventoryUI(int index, WandController NewWand)
    {
        WandInventory[index] = NewWand;
        WandInventoryIcons[index].sprite = NewWand.WandSprite.sprite;

        Color c = WandInventoryIcons[index].color;
        c.a = 1f;
        WandInventoryIcons[index].color = c;
    }

    // ------------------------------------------------------------------------
    public void AddWandToInventory(WandController NewWand)
    {   
        Debug.Log("Adding to Inveotory UI");
        bool AddedToInventory = false;
        // Check for open inveotry slot
        for (int i = 0; i < numberofWands; i++)
        {
            Debug.Log(i);
            if (WandInventory[i] == null)
            {
                AddWandToInventoryUI(i, NewWand);
                AddedToInventory = true;
                break;
            }
        }

        // Replacing current wand with the new wand if now slot is open
        if (!AddedToInventory)
        {
            AddWandToInventoryUI(activeWandIndex, NewWand);
        }

        Debug.Log("UI Updated");

        // Set the new wand invisible
        Color c = NewWand.WandSprite.color;
        c.a = 0;
        NewWand.WandSprite.color = c;

        NewWand.gameObject.transform.SetParent(PlayerTrans);
        NewWand.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
        NewWand.transform.position = new Vector3(0f, 0f, 0f);

        // Update the wand status
        NewWand.WandProperties.wandState = Wand.WandState.InInventory;
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
