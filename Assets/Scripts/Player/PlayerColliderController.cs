using UnityEngine;
using Wolfheat.StartMenu;

public class PlayerColliderController : MonoBehaviour
{

    [SerializeField] private LayerMask layerMaskWalls;
    [SerializeField] private LayerMask layerMaskEnemies;
    
    private LayerMask blockedLayers;

    public static PlayerColliderController Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null) {
            Destroy(gameObject);
            return;
        }
        Instance = this;


        blockedLayers = layerMaskEnemies | layerMaskWalls;
    }

    private void OnTriggerEnter(Collider other)
    {
        //Debug.Log("Player entered trigger "+other.name);
        if(other.TryGetComponent(out WorldItem item)) {
            Debug.Log("Colliding with item "+item.Data.ItemName);

            // If player can pick up item 

            if (InventoryController.Instance.TryPlaceItem(item)) {
                SoundMaster.Instance.PlaySound(SoundName.ItemPickup);
                ItemSpawner.Instance.RemoveWorldItem(item);
            }
        }
        if(other.TryGetComponent(out DungeonExit dungeonExit)) {
            Debug.Log("Exiting Dungeon");
            SoundMaster.Instance.PlaySound(SoundName.ExitStairReached);
            Stats.Instance.NextDungeonLevel();
        }
    }

    public bool CheckForFreeSpot(Vector3 pos)
    {
        // Check for Walls
        Collider[] colliders = Physics.OverlapBox(pos,new Vector3(0.4f,0.4f,0.4f),Quaternion.identity, blockedLayers);

        //Debug.Log("Colliders at this position "+ (colliders.Length > 0));

        return colliders.Length > 0;
    }

}
