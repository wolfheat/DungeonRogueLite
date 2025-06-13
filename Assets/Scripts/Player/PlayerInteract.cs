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
        float directDistance = Mathf.Sqrt(distanceX*distanceX+distanceY*distanceY);

        // Get Enemy at this position
        Collider[] colliders = Physics.OverlapBox(tileSelector.transform.position, overlapExtent, Quaternion.identity, enemyLayerMask);

        // Show the colliderbox
        overlapBoxShow.SetActive(true);
        overlapBoxShow.transform.position = tileSelector.transform.position;

        EnemyController enemy = colliders.Where(x => x.GetComponent<EnemyController>() != null).FirstOrDefault()?.GetComponent<EnemyController>();


        if (enemy == null || enemy.IsDead) {
            return;
        }

        if(directDistance < Stats.Instance.BowReach && directDistance > 1.5) {
            // If holding a bow or staff
            if (InventoryController.Instance.EquippedRangedWeapon()) {
                Debug.Log("Bow Attack");
                
                PlayerAnimation.Instance.PlayAnimation(AnimationType.AttackBow);

                // Do attack the enemy if there is one here
                if (enemy.TakeDamage(Stats.Instance.RangeDamage)) {
                    SoundMaster.Instance.PlaySound(SoundName.EnemyDie);
                    Stats.Instance.AddEnemyKilled(enemy.Data.XP);
                }
                else {
                    // Change this to sound for ranged damage
                    SoundMaster.Instance.PlayWeaponHitEnemy();
                }

                // Have player action end call a tick
                TickManager.Instance.TickRequest();
            }
        }
        else if(directDistance <= Stats.Instance.SwordReach) {

            Debug.Log("Melee Attack");

            PlayerAnimation.Instance.PlayAnimation(AnimationType.Attack1h);

            // Do attack the enmy if there is one here
            if (enemy.TakeDamage(Stats.Instance.MeleeDamage)) {
                SoundMaster.Instance.PlaySound(SoundName.EnemyDie);
                Stats.Instance.AddEnemyKilled(enemy.Data.XP);
            }
            else
                SoundMaster.Instance.PlayWeaponHitEnemy();

            // Have player action end call a tick
            TickManager.Instance.TickRequest();
        }
        else {
            Debug.Log("To far to Attack");            
        }

    }
}
