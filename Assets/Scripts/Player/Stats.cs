using System;
using UnityEngine;

public class Stats : MonoBehaviour,IDamageable
{
    public int Health { get; private set; } = 100;
    public int MaxHealth { get; private set; } = 100;
    public int Level { get; private set; } = 1;
    public int DungeonLevel { get; private set; } = 1;


    public static Stats Instance { get; private set; }
    public int EnemiesKilled { get; private set; }
    public float BowReach { get; internal set; } = 8f;
    public float SwordReach { get; internal set; } = 1.42f;
    public int XP { get; internal set; } = 0;

    public static Action StatsUpdated;

    private void Awake()
    {
        if (Instance != null) {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public void AddEnemyKilled(int experience = 0)
    {
        XP += experience;
        EnemiesKilled++;
        StatsUpdated?.Invoke();
    }

    public void PlayerGainLevel()
    {
        Level++;
        StatsUpdated?.Invoke();
    }

    public void NextDungeonLevel()
    {
        DungeonLevel++;
        StatsUpdated?.Invoke();
    }

    public bool TakeDamage(int damage)
    {
        Health -= damage;
        if (Health < 0) {
            Health = 0;
            StatsUpdated?.Invoke();
            return true;
        }
        StatsUpdated?.Invoke();
        return false;
    }

}
