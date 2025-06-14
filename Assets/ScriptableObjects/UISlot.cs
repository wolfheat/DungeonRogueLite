using System;
using UnityEngine;

public class UISlot : MonoBehaviour
{    
    public UIItem HeldItem { get; private set; }

    public void PlaceItem(UIItem item)
    {
        HeldItem = item;

        Debug.Log("Setting item "+item.name+" to parent slot "+transform.name);
        // Set parent correctly
        HeldItem.SetParent(transform);

        // Return to center of spot
        HeldItem.transform.localScale = new Vector3(1, 1, 1);
        HeldItem.transform.localPosition = new Vector3(50, 50, 0);
    }

    public void FreeFromItem() => HeldItem = null;

    internal void AlignItem()
    {
        // Return to center of spot
        HeldItem.transform.localScale = new Vector3(1, 1, 1);
        HeldItem.transform.localPosition = new Vector3(50, 50, 0);
    }
}

