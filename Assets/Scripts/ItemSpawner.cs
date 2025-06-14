using System;
using System.Collections.Generic;
using UnityEngine;

public class ItemSpawner : MonoBehaviour
{
	public static ItemSpawner Instance { get; private set; }
    [SerializeField] private Transform itemHolder; 
    [SerializeField] private WorldItem worldItemPrefab; 
    [SerializeField] private UIItem UIItemPrefab; 

    private void Awake()
    {
        if (Instance != null) {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    internal void SpawnItems(List<ItemData> dropsItems, Vector3 position)
    {
        Debug.Log("Requested spawning items "+dropsItems.Count+" at "+position);

        foreach (ItemData data in dropsItems) {
            WorldItem worldItem = Instantiate(worldItemPrefab, position, Quaternion.identity, itemHolder);
            worldItem.SetData(data);
        }
    }

    internal UIItem SpawnUIItem(ItemData data)
    {

        UIItem uiItem = Instantiate(UIItemPrefab);
        uiItem.SetData(data);
        return uiItem;

    }

    internal void RemoveWorldItem(WorldItem item)
    {
        Destroy(item.gameObject);
    }

    internal void RemoveUIItem(UIItem draggedItem)
    {
        Destroy(draggedItem.gameObject);
    }
}
