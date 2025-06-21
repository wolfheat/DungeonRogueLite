using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using Wolfheat.StartMenu;

public class PlayerInteract : MonoBehaviour
{
    [SerializeField] private TargetSelection tileSelector;
    [SerializeField] private LayerMask enemyLayerMask;
    [SerializeField] private GameObject overlapBoxShow;

    private Vector3 overlapExtent = new Vector3(0.5f, 0.5f, 0.5f);

    public static PlayerInteract Instance { get; private set; }

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
        Inputs.Instance.PlayerControls.Player.Click.performed += OnPlayerClick;
    }

    public void AnyAttackCompleted()
    {
        PlayerMovement.Instance.PerformingAction = false;
        //Debug.Log("Performing action Click FALSE");

        // Check if player is holding the attack?
        if (Inputs.Instance.PlayerControls.Player.Click.ReadValue<float>() != 0) {
            //Debug.Log("Player Is holding mouse attack");
            OnPlayerClick();
        }
    }

    private void Update()
    {
        if (Stats.Instance.IsDead) return;
        if (PlayerMovement.Instance.PerformingAction) return;

        //Debug.Log("alive and not performin an action");
        // No action is performed check for held mouse button
        if (Inputs.Instance.PlayerControls.Player.Click.ReadValue<float>() != 0) {
            //Debug.Log("Mouse held - in Update");
            OnPlayerClick();
        }
    }

    private void OnPlayerClick(InputAction.CallbackContext context) => OnPlayerClick();

    public bool IsAttackable()
    {
        if (Stats.Instance.IsDead) return false;
        //if (PlayerMovement.Instance.PerformingAction) return false;

        float directDistance = (transform.position - tileSelector.transform.position).magnitude;

        // Get Enemy at this position
        Collider[] colliders = Physics.OverlapBox(tileSelector.transform.position, overlapExtent, Quaternion.identity, enemyLayerMask);

        EnemyController enemy = colliders.Where(x => x.GetComponent<EnemyController>() != null).FirstOrDefault()?.GetComponent<EnemyController>();

        if (enemy == null || enemy.IsDead) {
            return false;
        }

        if (directDistance < Stats.Instance.BowReach && directDistance > 1.5 && EquippedManager.Instance.EquippedRangedWeapon())
            return true;

        else if (directDistance <= Stats.Instance.SwordReach)
            return true;
        return false;
    }

    private void OnPlayerClick()
    {
        if (Stats.Instance.IsDead) return;
        if (PlayerMovement.Instance.PerformingAction) return;

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

        //Debug.Log("Selector is at " + tileSelector.transform.position+" distance = "+directDistance+" enemy = "+enemy);

        if (enemy == null || enemy.IsDead) {
            return;
        }

        if(directDistance < Stats.Instance.BowReach && directDistance > 1.5) {
            // If holding a bow or staff
            if (EquippedManager.Instance.EquippedRangedWeapon()) {
                Debug.Log("Bow Attack");
                
                // Start Attacking
                PlayerMovement.Instance.PerformingAction = true;
                //Debug.Log("Performing action Click TRUE");
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

            // Start Attacking
            PlayerMovement.Instance.PerformingAction = true;
            //Debug.Log("Performing action Click TRUE"); 
            PlayerAnimation.Instance.PlayAnimation(AnimationType.Attack1hThrust);

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
