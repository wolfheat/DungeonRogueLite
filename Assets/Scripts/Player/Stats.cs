using System;
using UnityEngine;

public class Stats : MonoBehaviour,IDamageable
{
    public int Health { get; private set; } = 100;
    public int MaxHealth { get; private set; } = 100;
 
    public int MP { get; private set; } = 100;
    public int MaxMP { get; private set; } = 100;

    public int Level { get; private set; } = 1;
    public int DungeonLevel { get; private set; } = 1;

    public int BaseDamage { get; private set; } = 1;
    public int MovementSpeed { get; private set; } = 1;
    public int SightDistance { get; private set; } = 1;
    public int AttackSpeed { get; private set; } = 1;
    public float AttackDistance { get; private set; } = 1;


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
        Debug.Log("Taking damage health was "+Health + "/" + MaxHealth);
        Health -= damage;
        Debug.Log("Taking damage health becomes "+Health+"/"+MaxHealth);
        if (Health < 0) {
            Health = 0;
            StatsUpdated?.Invoke();
            Debug.Log("Player Died - Show Death Screen");
            UIController.Instance.ShowDeathPanel();
            return true;
        }
        StatsUpdated?.Invoke();
        return false;
    }

    internal void Reset()
    {
        Health = MaxHealth;
        MP = MaxMP;
        Level = 1;
        DungeonLevel = 1;
        EnemiesKilled = 0;
        StatsUpdated?.Invoke();

        // Send back player to start position?
        PlayerMovement.Instance.ReturnToStartPosition();

    }

    internal void ChangeCharacterData(CharacterClassData characterClassData)
    {
        Debug.Log("Updating Stats with character data: "+characterClassData.name);
        MaxHealth = characterClassData.BaseMaxHealth;
        Health = MaxHealth;
        MaxMP = characterClassData.BaseMaxMP;
        MP = MaxMP;
        BaseDamage = characterClassData.BaseDamage;
        MovementSpeed = characterClassData.MovementSpeed;
        SightDistance = characterClassData.SightDistance;
        AttackSpeed = characterClassData.AttackSpeed;
        AttackDistance = characterClassData.AttackDistance;
        Debug.Log("Health: "+MaxHealth+" MP:"+MaxMP+ " BaseDamage:" + BaseDamage + " MovementSpeed:" + MovementSpeed + " SightDistance:" + SightDistance + " AttackSpeed:" + AttackSpeed + " AttackDistance:" + AttackDistance);

        UICharacterPanel.Instance.SetCharacterName(characterClassData.CharacterType.ToString());
        UICharacterPanel.Instance.SetCharacterIcon(characterClassData.Picture);
        UICharacterPanel.Instance.SetCharacterLevel(Level);
        StatsUpdated?.Invoke();
    }
}
