using UnityEngine;

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
        Debug.Log("Player entered trigger "+other.name);
        if(other.TryGetComponent(out DungeonExit dungeonExit)) {
            Debug.Log("Exiting Dungeon");
        }
    }

    public bool CheckForFreeSpot(Vector3 pos)
    {
        // Check for Walls
        Collider[] colliders = Physics.OverlapBox(pos,new Vector3(0.4f,0.4f,0.4f),Quaternion.identity, blockedLayers);

        Debug.Log("Colliders at this position "+ (colliders.Length > 0));

        return colliders.Length > 0;
    }

}
