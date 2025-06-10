using UnityEngine;

public class PlayerColliderController : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Player entered trigger "+other.name);
        if(other.TryGetComponent(out DungeonExit dungeonExit)) {
            Debug.Log("Exiting Dungeon");
        }
    }
}
