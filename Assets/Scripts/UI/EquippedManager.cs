using System;
using UnityEngine;

public class EquippedManager : MonoBehaviour
{
    [SerializeField] private UISlot[] slots;


	public static EquippedManager Instance { get; private set; }

	private void Awake()
	{
		if (Instance != null) {
			Destroy(gameObject);
			return;
		}
		Instance = this;
	}

	public int[] GetEquippedItemsStats()
	{
		int[] equipmentAddons = new int[4];

		foreach (var slot in slots) {
			UIItem item = slot.HeldItem;
			if (item == null) continue;
			equipmentAddons[0] += item.data.Strength;
			equipmentAddons[1] += item.data.Stamina;
			equipmentAddons[2] += item.data.Intelligence;
			equipmentAddons[3] += item.data.Willpower;
		}
		return equipmentAddons;
	}

    internal ItemData GetEquipped(EquipmentType equipmentType)
    {
		Debug.Log("Return equipped item of type "+equipmentType);

		foreach (var slot in slots) {
			if(slot == null) {
				Debug.LogWarning("Could not find any slot item assigned. Did you forget to assign it in the equipment inspector?");
				continue;
			}
			if (slot.HeldItem != null && slot.HeldItem.data.EquipmentType == equipmentType)
				return slot.HeldItem.data;
		}
		return null;
    }

    internal void PickUpItem(WorldItem item)
    {




        Debug.Log("Picking up item.");
		Destroy(item.gameObject);
    }

    internal bool EquippedRangedWeapon()
    {
        foreach (var slot in slots) {
            if (slot.HeldItem != null && slot.HeldItem.data.EquipmentType == EquipmentType.Bow)
                return true;
        }
		return false;
    }
}
