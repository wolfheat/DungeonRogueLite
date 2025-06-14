using UnityEngine;
using Wolfheat.StartMenu;

public class PlayerColliderController : MonoBehaviour
{

    [SerializeField] private LayerMask layerMaskWalls;
    [SerializeField] private LayerMask layerMaskEnemies;
    [SerializeField] private LayerMask floorLayers;
    
    private LayerMask blockedLayers;
    private LayerMask walkableLayers;

    public static PlayerColliderController Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null) {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        walkableLayers = floorLayers;
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

    public bool CheckIfLegalMoveTo(Vector3 pos)
    {
        Debug.Log("Checking if player can go to pos "+pos);
        // Check for Walls
        Collider[] colliders = Physics.OverlapBox(pos,new Vector3(0.4f,0.4f,0.4f),Quaternion.identity, blockedLayers);
        
        Debug.Log("Walls "+colliders.Length);
        if(colliders.Length > 0)
            return false;

        // Check for Floor
        Collider[] floorColliders = Physics.OverlapBox(pos,new Vector3(0.4f,0.4f,0.4f),Quaternion.identity, walkableLayers);
        Debug.Log("Floors "+ floorColliders.Length);

        if(floorColliders.Length == 0)
            return false;

        return true;
    }

}
