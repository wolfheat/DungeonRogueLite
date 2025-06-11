using System;
using System.Collections;
using UnityEngine;


public interface IDamageable
{
    bool TakeDamage(int damage);

}

public class EnemyController : MonoBehaviour, IDamageable
{

    private const int ActionSpeed = 1;
    private const int MovementSpeed = 2;

    private int health = 100;
    private int MaxHealth = 100;

    private int actionTimer = 0;    
    private int attackTimer = 0;

    public bool IsDead { get; private set; }

    private void Start()
    {
        //Subscribe to the tick manager
        TickManager.TickGame += Tick;
    }

    public void Tick()
    {
        Debug.Log("Enemy "+name+" recieved tick ahead event.");
        // Enemy ticks ahead one tick and attacks if ready to
        actionTimer--;

        if (actionTimer <= 0) {
            DoAction();
        }

    }

    private void DoAction()
    {
        Debug.Log("Enemy "+name+" doing action!");
        actionTimer = ActionSpeed;

        // If to far away move closer to player

        if (EvaluateDistanceToPlayer()) {
            Debug.Log("Enemy moved toward player");
        }

    }

    private bool EvaluateDistanceToPlayer()
    {
        Vector2Int playerTile = Convert.V3ToV2Int(PlayerColliderController.Instance.transform.position);
        Debug.Log("PLayer is at "+playerTile);

        Vector2Int myPosition = Convert.V3ToV2Int(transform.position);

        int cardinalDistance = Mathf.Abs(playerTile.x - myPosition.x)+Mathf.Abs(playerTile.y - myPosition.y);
        Debug.Log("Distance is "+cardinalDistance);

        if(cardinalDistance > 1) {
            Vector2Int movement = GetMovementVectorTowardsPlayer(playerTile);
            MoveEnemy(movement);
            return true;
        }
        return false;
    }

    private void MoveEnemy(Vector2Int movement)
    {
        Debug.Log("Enemy Moves "+movement);

        // Check so the movement is valid

        transform.position += Convert.V2IntToV3(movement);

        // Align Enemy to center of tile
        transform.position = Convert.Align(transform.position);

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
                int moveSteps = Mathf.Min(distanceToGetNextTo, MovementSpeed);
                return new Vector2Int(moveSteps*dir, 0);
            }
        }else if (YDist > 0) {
            int distanceToGetNextTo = YDist - (XDist==0?1:0);
            int dir = Convert.Sign(playerTile.y - myPosition.y);

            if (distanceToGetNextTo > 0) {
                // move towards player as many steps as enemy are allowed
                int moveSteps = Mathf.Min(distanceToGetNextTo, MovementSpeed);
                return new Vector2Int(0, moveSteps * dir);
            }
        }
        return new Vector2Int();
    }

    public bool TakeDamage(int damage)
    {
        Debug.Log("Enemy recieved "+damage+" damage.");
        health -= damage;
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
        Debug.Log("Waiting 2 seconds to remove the enemy");
        yield return new WaitForSeconds(2f);
        Debug.Log("Removing the enemy");
        Destroy(gameObject);
    }
}
