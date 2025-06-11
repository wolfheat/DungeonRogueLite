using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using Wolfheat.StartMenu;

public class PlayerInteract : MonoBehaviour
{
    [SerializeField] private GameObject tileSelector;
    [SerializeField] private LayerMask enemyLayerMask;
    [SerializeField] private GameObject overlapBoxShow;

    private Vector3 overlapExtent = new Vector3(0.5f, 0.5f, 0.5f);

    private void Start()
    {
        Inputs.Instance.PlayerControls.Player.Click.performed += OnPlayerClick;
    }

    private void OnPlayerClick(InputAction.CallbackContext context)
    {
        //Debug.Log("Player Clicks");
        //Debug.Log("Selector is at " + tileSelector.transform.position);
        //Debug.Log("Selector is at " + Convert.V3ToV2Int(tileSelector.transform.position));

        float distanceX = Mathf.Abs(transform.position.x-tileSelector.transform.position.x);
        float distanceY = Mathf.Abs(transform.position.z-tileSelector.transform.position.z);
        int totDistance = Mathf.RoundToInt(distanceX+distanceY);


        // Get Enemy at this position
        Collider[] colliders = Physics.OverlapBox(tileSelector.transform.position, overlapExtent, Quaternion.identity, enemyLayerMask);

        // Show the colliderbox
        overlapBoxShow.SetActive(true);
        overlapBoxShow.transform.position = tileSelector.transform.position;

        EnemyController enemy = colliders.Where(x => x.GetComponent<EnemyController>() != null).FirstOrDefault()?.GetComponent<EnemyController>();


        if (enemy == null) {
            //Debug.Log("NO ENEMY HERE");
            return;
        }

        Debug.Log("Distance from player "+totDistance);

        if(totDistance < 5 && totDistance > 1)
            Debug.Log("Can Attack with Bow");
        else if(totDistance <= 1) {
            Debug.Log("Can Attack with Melee");

            if (enemy.IsDead)
                return;

            // Do attack the enmy if there is one here
            if (enemy.TakeDamage(8)) {
                SoundMaster.Instance.PlaySound(SoundName.EnemyDie);
                Stats.Instance.AddEnemyKilled();
            }
            else
                SoundMaster.Instance.PlayWeaponHitEnemy();
        }
        else {
            Debug.Log("To far to Attack");            
        }

    }
}
