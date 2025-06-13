using System;
using TMPro;
using UnityEngine;

public class Stats : MonoBehaviour,IDamageable
{
    public const int UpgradePointsPerLevel = 4;

    public AnimationCurve XPCurve;

    public int Strength { get; private set; } = 10;
    public int Stamina { get; private set; } = 10;
    public int Intelligence { get; private set; } = 10;
    public int Willpower { get; private set; } = 10;

    public int Health { get; private set; } = 100;

    public int MaxHealth { get; private set; } = 100;
    
    public int MP { get; private set; } = 100;
    public int MaxMP { get; private set; } = 100;
    public int AvailableUpgradePoints { get; private set; } = 4;

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
    public CharacterClassData ActiveCharacter { get; internal set; }

    public int CurrentMaxXP => (int)XPCurve.Evaluate((float)Level);

    public int MeleeDamage => BaseDamage + (int)(Strength*0.8f);
    public int RangeDamage => BaseDamage + (int)(Intelligence*0.8f);


    public static Action StatsUpdated;
    public static Action CharacterUpdated;

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

        EnemiesKilled++;

        XP += experience;
        if(XP >= CurrentMaxXP) {
            XP -= CurrentMaxXP;
            Level++;
            AvailableUpgradePoints += UpgradePointsPerLevel;
            UIController.Instance.ShowLevelUpPanel();
        }

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
        ActiveCharacter = characterClassData;

        UpdateCharacterClassData();

        CharacterUpdated?.Invoke();
        StatsUpdated?.Invoke();
    }

    private void UpdateCharacterClassData()
    {
        Debug.Log("Updating Stats with character data: "+ ActiveCharacter.name);
        MaxHealth = ActiveCharacter.BaseMaxHealth;
        Health = MaxHealth;
        MaxMP = ActiveCharacter.BaseMaxMP;
        MP = MaxMP;
        BaseDamage = ActiveCharacter.BaseDamage;
        MovementSpeed = ActiveCharacter.MovementSpeed;
        SightDistance = ActiveCharacter.SightDistance;
        AttackSpeed = ActiveCharacter.AttackSpeed;
        AttackDistance = ActiveCharacter.AttackDistance;
        Debug.Log("Health: "+MaxHealth+" MP:"+MaxMP+ " BaseDamage:" + BaseDamage + " MovementSpeed:" + MovementSpeed + " SightDistance:" + SightDistance + " AttackSpeed:" + AttackSpeed + " AttackDistance:" + AttackDistance);

    }

    internal void ApplyUpgradePoints(int[] upgrades)
    {
        Debug.Log("Applying " + upgrades[0]+" points to Strength");
        Debug.Log("Applying " + upgrades[1]+" points to Stamina");
        Debug.Log("Applying " + upgrades[2]+" points to Intelligence");
        Debug.Log("Applying " + upgrades[3]+" points to Willpower");

        Strength += upgrades[0];

        Stamina += upgrades[1];

        Intelligence += upgrades[2];

        Willpower += upgrades[3];

        // 1 Strength = 1 Strength
        // 1 Stamina = 5 Health
        // 1 Intelligence = 1 Range Damage
        // 1 Willpower = 5 Mana

        UpdateMaxHPandMP();
    }

    private void UpdateMaxHPandMP()
    {
        MaxHealth = ActiveCharacter.BaseMaxHealth + Stamina * 5;
        MaxMP = ActiveCharacter.BaseMaxMP + Intelligence * 5;
        StatsUpdated?.Invoke();
    }
}
