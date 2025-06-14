using System;
using UnityEngine;

public class DragObject : MonoBehaviour
{
    [SerializeField] Transform draggedParent;
    [SerializeField] Transform normalParent;
    private UIItem draggedItem;

    public static DragObject Instance;

    public void UnSetDragedItem()
    {
        draggedItem?.SetParent(normalParent);
        draggedItem?.ResetPosition();
        draggedItem = null;
    }

    public void SetDragedItem(UIItem item)
    {
        // Save the old parent if item is released and has to go back
        normalParent = item.transform.parent;
        draggedItem = item;
        draggedItem?.SetParent(draggedParent);
    }

    private void Awake()
    {
        if (Instance != null) {
            Destroy(this);
            return;
        }
        Instance = this;
    }

    internal void ReturnItem()
    {
        // Set parent correctly
        draggedItem.transform.SetParent(normalParent);

        // Return to center of spot
        draggedItem.transform.localPosition = new Vector3(50,50,0);
    }

    internal void MoveItemToSlot(UISlot slot)
    {
        UISlot sourceSlot = normalParent.GetComponent<UISlot>();

        // If there is an item here already swap them
        if(slot.HeldItem != null) {
            UIItem targetSlotItem = slot.HeldItem;
            sourceSlot.PlaceItem(targetSlotItem);
        }
        else {
            sourceSlot.FreeFromItem();
        }
        slot.PlaceItem(draggedItem);

        Stats.Instance.UpdateInventoryStatsAddon();

        // Handles wrong info showing after item swaps
        InfoPanelController.Instance.HideInfo();
    }

    internal UISlot OriginalSlot() => normalParent.GetComponent<UISlot>();

    internal void SellItem()
    {
        UISlot sourceSlot = normalParent.GetComponent<UISlot>();
        sourceSlot.FreeFromItem();
        Stats.Instance.AddCoins(draggedItem.data.value);
        ItemSpawner.Instance.RemoveUIItem(draggedItem);

        // Handles wrong info showing after item swaps
        InfoPanelController.Instance.HideInfo();
    }
}
