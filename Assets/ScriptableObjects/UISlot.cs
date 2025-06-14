using UnityEngine;

public class UISlot : MonoBehaviour
{    
    public UIItem HeldItem { get; private set; }

    public void PlaceItem(UIItem item)
    {
        HeldItem = item;

        // Set parent correctly
        HeldItem.SetParent(transform);

        // Return to center of spot
        HeldItem.transform.localPosition = new Vector3(50, 50, 0);
    }

    public void FreeFromItem() => HeldItem = null;
}

