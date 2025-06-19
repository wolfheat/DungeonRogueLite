using System;
using System.Collections.Generic;
using UnityEngine;

public class ItemSpawner : MonoBehaviour
{
	public static ItemSpawner Instance { get; private set; }
    [SerializeField] private Transform itemHolder; 
    [SerializeField] private Transform damgesHolder; 
    [SerializeField] private Transform enemyHolder; 

    [SerializeField] private WorldItem worldItemPrefab; 
    [SerializeField] private WorldDamage worldDamagePrefab; 
    [SerializeField] private WorldDamage worldXPPrefab; 

    [SerializeField] private UIItem UIItemPrefab; 
    [SerializeField] private EnemyController[] enemyPrefabs; 
    //[SerializeField] private WorldDamage worldDamagePrefab; 

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
        //Debug.Log("Requested spawning items "+dropsItems.Count+" at "+position);

        foreach (ItemData data in dropsItems) {
            WorldItem worldItem = Instantiate(worldItemPrefab, position, Quaternion.identity, itemHolder);
            worldItem.SetData(data);
        }
    }

    internal void SpawnWorldDamage(int damage, Vector3 pos)
    {
        //Debug.Log("Spawning damage text "+damage+" at "+pos);
        WorldDamage worldDamage = Instantiate(worldDamagePrefab);
        worldDamage.transform.position = pos + Vector3.up*1.5f;
        worldDamage.StartText(damage);
    }
    
    internal void SpawnWorldXP(int xp, Vector3 pos)
    {
        //Debug.Log("Spawning damage text "+damage+" at "+pos);
        WorldDamage worldDamage = Instantiate(worldXPPrefab);
        worldDamage.transform.position = pos + Vector3.up*1.5f;
        worldDamage.StartText(xp," XP");
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

    internal void SpawnEnemy(int enemyType, Vector2Int pos)
    {
        //Debug.Log("Spawning enemy " + enemyType + ".");

        EnemyController enemy = Instantiate(enemyPrefabs[enemyType],enemyHolder);
        enemy.transform.position = Convert.V2IntToV3(pos);
    }
}
