using UnityEngine;

public class InventoryController : MonoBehaviour
{
    [SerializeField] private UISlot[] slots;


    public static InventoryController Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null) {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public bool TryPlaceItem(WorldItem item)
    {
        UISlot slot = FindFreeSlot();

        if (slot == null || item.Data == null) return false;

        // Generate the Item
        slot.PlaceItem(ItemSpawner.Instance.SpawnUIItem(item.Data));
        return true;
    }

    public UISlot FindFreeSlot()
    {
        foreach (var slot in slots) {
            if(slot.HeldItem == null)
                return slot;
        }
        return null;
    }


}
