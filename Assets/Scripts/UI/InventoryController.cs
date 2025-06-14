using System;
using UnityEngine;

public class InventoryController : MonoBehaviour
{
    [SerializeField] private UISlot[] slots;

    [SerializeField] private GameObject preplaceItemsHolder; 
    public static InventoryController Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null) {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        PlacePreplaceItems();
    }

    private void OnEnable()
    {
        // Align all Items

        foreach (var slot in slots) {
            if (slot.HeldItem != null)
                slot.AlignItem();
        }
    }

    private void PlacePreplaceItems()
    {
        UIItem[] items = preplaceItemsHolder.transform.GetComponentsInChildren<UIItem>();
        foreach (var item in items) {
            UISlot slot = FindFreeSlot();
            slot.PlaceItem(item);
            item.UpdateItem();
        }
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
