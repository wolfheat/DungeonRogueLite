using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public interface IDamageable
{
    bool TakeDamage(int damage);

}
public class EnemyController : MonoBehaviour, IDamageable
{
    [SerializeField] private EnemyData data;
    [SerializeField] private IngameHealthBar healthBar;
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

        //Subscribe to the tick manager
        TickManager.TickGame += Tick;

        // When generated also apply the dropItems by code?


    }
    private void OnDisable() => TickManager.TickGame -= Tick;


    public void Tick()
    {
        //Debug.Log("Enemy "+name+" recieved tick event.");
        if (IsDead || Stats.Instance.IsDead)
            return;
        // Enemy ticks ahead one tick and attacks if ready to
        actionTimer--;

        if (actionTimer <= 0) {
            DoAction();
        }

    }

    private void DoAction()
    {
        Debug.Log(" Enemy "+name+" action!");
        actionTimer = data.ActionSpeed;

        // If to far away move closer to player

        if (EvaluateDistanceToPlayer()) {
            Debug.Log("Enemy attacking player! DMG:"+data.Damage);
            Stats.Instance.TakeDamage(data.Damage);
            // Face Player ?
        }

    }

    private bool EvaluateDistanceToPlayer()
    {
        // Check distance to player - if closer than visual range and in sight start pathfinding?

        Vector2Int playerTile = Convert.V3ToV2Int(PlayerColliderController.Instance.transform.position);
        //Debug.Log("Player is at "+playerTile);

        Vector2Int myPosition = Convert.V3ToV2Int(transform.position);

        float distance = (playerTile - myPosition).magnitude;

        if(distance > data.SightDistance) {
            //Debug.Log("Player To Far away to See");
            return false;
        }

        //Debug.Log("Checking attack distance, "+distance+" Enemy can attack if less or equal to "+data.AttackDistance);
        // Check if can Attack
        if (distance <= data.AttackDistance) {
            //Debug.Log("Can attack from here if player is visible");

            // Is player visible?
            bool playerVisible = CanSeePlayer();

            if (playerVisible) {
                //Debug.Log("Attacking Player");
                FacePlayer();
                return true;
            }
            else {
                Debug.Log("Player is close enough but not visible to the enemy");
            }
        }
        else {
            //Debug.Log("PLayer is to far away to be seen by enemy");
        }

        // Move towards Player
        List<Vector2Int> path = LevelCreator.Instance.GetPath(myPosition,playerTile);

        // Move enemy maxsteps towards player

        if (path.Count <= 1) {
            Debug.Log("Enemy is already next to player - should not print since player should be seen and close enoug to be targeted");
        }
        else {
            // Moving towards player
            int stepsToTake = Math.Min(data.MovementSpeed, path.Count - 1);
            //Debug.Log("Enemy can take "+stepsToTake+" steps.");
            MoveEnemy(path[stepsToTake]);   
        }

        return false;


        /*
        int cardinalDistance = Mathf.Abs(playerTile.x - myPosition.x)+Mathf.Abs(playerTile.y - myPosition.y);

        Debug.Log("Distance is "+cardinalDistance);

        if(cardinalDistance > 1) {
            Vector2Int movement = GetMovementVectorTowardsPlayer(playerTile);
            MoveEnemy(movement);
            return true;
        }
        return false;
        */
    }

    private bool CanSeePlayer()
    {
        // Always show the view direction for the enemy
        Vector3 eye = transform.position + Vector3.up * 0.3f;
        Vector3 target = PlayerColliderController.Instance.transform.position + Vector3.up * 0.3f;
        Vector3 dir = (target - eye);
        float dist = dir.magnitude;
        dir.Normalize();

        Debug.DrawRay(eye, dir * dist, Color.blue);

        if (Physics.Raycast(eye, dir, out RaycastHit hit, dist)) {
            Debug.Log("Hit: " + hit.collider.name + " Layer: " + LayerMask.LayerToName(hit.collider.gameObject.layer));
            if(hit.collider.TryGetComponent(out PlayerColliderController player)) {
                return true;
            }
        }
        else {
            return false;
        }
        return false;

    }

    private void MoveEnemy(Vector2Int movement)
    {
        Debug.Log("Enemy Moves to " + movement);

        // Check so the movement is valid

        transform.position = Convert.V2IntToV3(movement);

        // Align Enemy to center of tile
        transform.position = Convert.Align(transform.position);
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
}
