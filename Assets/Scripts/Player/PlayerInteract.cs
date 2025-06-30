using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using Wolfheat.StartMenu;

public class PlayerInteract : BaseCharacterInteract, IBeingHitByArrow
{
    [SerializeField] private LayerMask enemyLayerMask;
    [SerializeField] private GameObject overlapBoxShow;

    private Vector3 overlapExtent = new Vector3(0.5f, 0.5f, 0.5f);

    private ArrowData arrowData;
    public static PlayerInteract Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null) {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public override void BowAttackCompleted()
    {
        // Send away the arrow
        Debug.Log("Sending of an arrow");
        if(arrowData != null)
            ItemSpawner.Instance.SpawnArrow(arrowData);

        PlayerActionHandeler.Instance.EndPlayerTurn();
    }
    public override void AnyAttackCompleted()
    {
        PlayerActionHandeler.Instance.EndPlayerTurn();
    }

    public bool IsAttackable(Vector3 selectorPosition)
    {
        if (Stats.Instance.IsDead) return false;
        //if (PlayerMovement.Instance.PerformingAction) return false;

        float directDistance = (transform.position - selectorPosition).magnitude;

        // Get Enemy at this position
        Collider[] colliders = Physics.OverlapBox(selectorPosition, overlapExtent, Quaternion.identity, enemyLayerMask);

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

    public bool TryAttack(Vector3 selectorPosition)
    {
        if (Stats.Instance.IsDead) return false;

        //Debug.Log("Selector is at " + Convert.V3ToV2Int(selectorPosition.position));

        float distanceX = Mathf.Abs(transform.position.x - selectorPosition.x);
        float distanceY = Mathf.Abs(transform.position.z-selectorPosition.z);
        int totDistance = Mathf.RoundToInt(distanceX+distanceY);
        float directDistance = Mathf.Sqrt(distanceX*distanceX+distanceY*distanceY);

        // Get Enemy at this position
        Collider[] colliders = Physics.OverlapBox(selectorPosition, overlapExtent, Quaternion.identity, enemyLayerMask);

        // Show the colliderbox
        overlapBoxShow.SetActive(true);
        overlapBoxShow.transform.position = selectorPosition;

        EnemyController enemy = colliders.Where(x => x.GetComponent<EnemyController>() != null).FirstOrDefault()?.GetComponent<EnemyController>();

        //Debug.Log("Selector is at " + selectorPosition.position+" distance = "+directDistance+" enemy = "+enemy);

        if (enemy == null || enemy.IsDead) {
            return false;
        }

        if(directDistance < Stats.Instance.BowReach && directDistance > 1.5) {
            // If holding a bow or staff
            if (EquippedManager.Instance.EquippedRangedWeapon()) {
                Debug.Log("Bow Attack");
                
                SetArrowData(transform.position, enemy.transform.position, enemy, Stats.Instance.RangeDamage);

                PlayerAnimation.Instance.PlayAnimation(AnimationType.AttackBow);

            }
        }
        else if(directDistance <= Stats.Instance.SwordReach) {

            Debug.Log("Melee Attack");

            //Debug.Log("Performing action Click TRUE"); 
            PlayerAnimation.Instance.PlayAnimation(AnimationType.Attack1hThrust);

            // Do attack the enmy if there is one here
            if (enemy.TakeDamage(Stats.Instance.MeleeDamage)) {
                SoundMaster.Instance.PlaySound(SoundName.EnemyDie);
                Stats.Instance.AddEnemyKilled(enemy.Data.XP);
            }
            else
                SoundMaster.Instance.PlayWeaponHitEnemy();

            return true; // Attack was allowed, end players turn
        }
        else {
            Debug.Log("To far to Attack");
        }
        return false;
    }


    private void SetArrowData(Vector3 position1, Vector3 position2, IBeingHitByArrow target, int damage) => arrowData = new ArrowData(position1, position2, target, damage);

    public void BeingHitByArrow(ArrowData data)
    {
        Stats.Instance.TakeDamage(data.Damage);        
    }
}

