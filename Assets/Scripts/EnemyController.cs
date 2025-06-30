using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Wolfheat.StartMenu;


public interface IDamageable
{
    bool TakeDamage(int damage);

}
public interface IBeingHitByArrow
{
    public void BeingHitByArrow(ArrowData data);

}
public class EnemyController : BaseCharacterInteract, IDamageable, IBeingHitByArrow
{
    [SerializeField] private EnemyData data;
    [SerializeField] private IngameHealthBar healthBar;
    [SerializeField] private EnemyAnimation enemyAnimation;
    [SerializeField] private List<ItemData> dropsItems;

    private LayerMask playerMask; // Include walls and player layers
    private LayerMask wallMask; // Include walls and player layers
    private LayerMask visionMask; // Include walls and player layers

    private int health = 100;

    private int actionTimer = 0;    
    private int attackTimer = 0;

    public bool IsDead { get; private set; }
    [HideInInspector] public EnemyData Data => data;

    private void Start()
    {
        // Set Health
        health = data.MaxHealth;

        // Layermasks
        playerMask = LayerMask.GetMask("Player");
        wallMask = LayerMask.GetMask("Wall");

        // Visionmask
        visionMask = playerMask | wallMask;

        // When generated also apply the dropItems by code?

        // Set the enemies start Tile
        InitiateStartTileFromWorldPosition();
    }

    private void InitiateStartTileFromWorldPosition() => TilePosition = Convert.V3ToV2Int(transform.position);

    public void Tick()
    {
        //Debug.Log("Enemy "+name+" recieved tick event.");

        if (IsDead || Stats.Instance.IsDead || Stats.Instance.IsPaused)
            return;
        // Enemy ticks ahead one tick and attacks if ready to
        actionTimer--;

        if (actionTimer <= 0) {
            DoAction();
        }

    }

    private void DoAction()
    {
        //Debug.Log(" Enemy "+name+" performing action!");

        actionTimer = data.ActionSpeed;

        // Update PLayer distance
        UpdatePlayerDistance(Convert.V3ToV2Int(PlayerMovement.Instance.transform.position));

        // If to far away move closer to player
        if (EvaluateDistanceToPlayer_ReturnCanAttack()) {
            
            Debug.Log("Enemy attacking player! DMG:"+data.Damage);
            // Attack Player here
            enemyAnimation.PlayAttackAnimation();

        }
    }

    private void ChasePlayer()
    {
        // Close enough to chase

        Debug.Log("** Try To Move towards player");
        // Move towards Player
        List<Vector2Int> path = LevelCreator.Instance.GetPath(TilePosition, PlayerMovement.Instance.TilePosition);
        //Debug.Log("** path "+path.Count);

        // Move enemy maxsteps towards player

        if (path.Count <= 1) {
            //Debug.Log("Enemy is already next to player - should not print since player should be seen and close enoug to be targeted");
        }
        else {
            // Moving towards player
            int stepsToTake = Math.Min(data.MovementSpeed, path.Count - 1);
            //Debug.Log("Enemy can take "+stepsToTake+" steps.");
            MoveEnemy(path[stepsToTake]);
        }
    }

    public float PlayerDistance { get; private set; } = 0;
    public Vector2Int TilePosition { get; private set; }
    public void UpdatePlayerDistance(Vector2Int playerTilePosition) => PlayerDistance = (playerTilePosition - Convert.V3ToV2Int(transform.position)).magnitude;



    private bool EvaluateDistanceToPlayer_ReturnCanAttack()
    {
        
        if(PlayerDistance > data.SightDistance) {

            Debug.Log("Player is to far away to be seen by enemy " + name);

            return false;
        }

        //Debug.Log("Enemy "+name+" are close enough to see the player. Distance "+ PlayerDistance + (PlayerDistance <= data.AttackDistance ? " = can attack.":" can not attack."));
        
        // Check if can Attack
        if (PlayerDistance <= data.AttackDistance) {
            Debug.Log("Can attack from here if player is visible");

            // Is player visible?
            if (CanSeePlayer()) {
                //Debug.Log("Attacking Player");
                FacePlayer();
                return true;
            }
            else {
                Debug.Log("Player is close enough but not visible to the enemy - pathfind");
            }
        }
        else {
            Debug.Log("Player is to far away to be attacked by enemy "+name+" ");
            ChasePlayer();
            return false;
        }

        return false;
    }

    private bool CanSeePlayer()
    {
        // Always show the view direction for the enemy
        Vector3 eye = transform.position + Vector3.up * 0.3f;
        Vector3 target = PlayerColliderController.Instance.transform.position + Vector3.up * 0.3f;
        Vector3 dir = (target - eye);
        float dist = dir.magnitude+0.5f;
        dir.Normalize();

        Debug.DrawRay(eye, dir * dist, Color.blue,3f);

        if (Physics.Raycast(eye, dir, out RaycastHit hit, dist)) {
            Debug.Log("Hit: " + hit.collider.name + " Layer: " + LayerMask.LayerToName(hit.collider.gameObject.layer));
            if(hit.collider.TryGetComponent(out PlayerColliderController player)) {
                return true;
            }
        }
        else {
            Debug.Log("FAILED TO SEE PLAYER");
            Debug.Log("Nothing was hit by the raycast from "+eye+" to "+target);

            return false;
        }
        return false;

    }

    private void MoveEnemy(Vector2Int movement)
    {
        transform.position = Convert.V2IntToV3(movement);
        TilePosition = movement;

        FacePlayer();

    }

    private void FacePlayer()
    {
        // When chasing player look at him
        transform.LookAt(PlayerColliderController.Instance.transform.position);
    }

    private Vector2Int GetMovementVectorTowardsPlayer(Vector2Int playerTile)
    {
        Debug.Log("Enemy moves towards players tile " + playerTile);
        Vector2Int myPosition = Convert.V3ToV2Int(transform.position);

        int XDist = Mathf.Abs(playerTile.x - myPosition.x);
        int YDist = Mathf.Abs(playerTile.y - myPosition.y);

        // Check X movement needed
        if (XDist > 0) {
            int distanceToGetNextTo = XDist - (YDist==0?1:0);
            int dir = Convert.Sign(playerTile.x - myPosition.x);
            Debug.Log("X Distance to player "+distanceToGetNextTo);
            if (distanceToGetNextTo > 0) {
                // move towards player as many steps as enemy are allowed
                int moveSteps = Mathf.Min(distanceToGetNextTo, data.MovementSpeed);
                return new Vector2Int(moveSteps*dir, 0);
            }
        }else if (YDist > 0) {
            int distanceToGetNextTo = YDist - (XDist==0?1:0);
            int dir = Convert.Sign(playerTile.y - myPosition.y);

            if (distanceToGetNextTo > 0) {
                // move towards player as many steps as enemy are allowed
                int moveSteps = Mathf.Min(distanceToGetNextTo, data.MovementSpeed);
                return new Vector2Int(0, moveSteps * dir);
            }
        }
        return new Vector2Int();
    }

    public bool TakeDamage(int damage)
    {
        Debug.Log("Enemy recieved "+damage+" damage.");
        health -= damage;

        // Spawn Damage Text
        ItemSpawner.Instance.SpawnWorldDamage(damage, transform.position);

        // Update in game healthbar
        healthBar.UpdateHealthBar(Math.Max(0,health), data.MaxHealth);

        if (health < 0) {
            health = 0;
            Debug.Log("Enemy Dies");
            ItemSpawner.Instance.SpawnWorldXP(data.XP, transform.position);

            StartCoroutine(DeathCoroutine());
            return true;
        }
        return false;
    }

    // Death Coroutine
    private IEnumerator DeathCoroutine()
    {
        IsDead = true;
        Debug.Log("Waiting 0.6 seconds to remove the enemy");
        yield return new WaitForSeconds(0.6f);
        Debug.Log("Removing the enemy");

        // Generate the Loot now
        ItemSpawner.Instance.SpawnItems(dropsItems,transform.position);

        Destroy(gameObject);

        // Generate Loot here?

        // Give player Experience?

    }

    public void BeingHitByArrow(ArrowData data)
    {
        if (IsDead) return;

        // Show being hit animation here?

        // Do attack the enemy if there is one here
        if (TakeDamage(Stats.Instance.RangeDamage)) { // Move damage into the arrow?
            SoundMaster.Instance.PlaySound(SoundName.EnemyDie);
            Stats.Instance.AddEnemyKilled(Data.XP);
        }
        else {
            // Do animation for taking an arrow
        }


    }

    public override void BowAttackCompleted()
    {
        // Send an arrow towards player here

        ItemSpawner.Instance.SpawnArrow(new ArrowData(transform.position,PlayerInteract.Instance.transform.position, PlayerInteract.Instance,data.Damage));

    }

    public override void AnyAttackCompleted()
    {
        // Deal melee damage to player here
        Stats.Instance.TakeDamage(Data.Damage);
    }
}
