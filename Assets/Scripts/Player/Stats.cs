using System;
using TMPro;
using UnityEngine;

public class Stats : MonoBehaviour,IDamageable
{
    public const int UpgradePointsPerLevel = 4;

    public AnimationCurve XPCurve;

    public int BaseStrength { get; private set; } = 10;
    public int BaseStamina { get; private set; } = 10;
    public int BaseIntelligence { get; private set; } = 10;
    public int BaseWillpower { get; private set; } = 10;

    public int ItemStrength { get; private set; } = 0;
    public int ItemStamina { get; private set; } = 0;
    public int ItemIntelligence { get; private set; } = 0;
    public int ItemWillpower { get; private set; } = 0;


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

    public int MeleeDamage => BaseDamage + (int)(BaseStrength*0.8f);
    public int RangeDamage => BaseDamage + (int)(BaseIntelligence*0.8f);


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

        BaseStrength += upgrades[0];

        BaseStamina += upgrades[1];

        BaseIntelligence += upgrades[2];

        BaseWillpower += upgrades[3];

        // 1 Strength = 1 Strength
        // 1 Stamina = 5 Health
        // 1 Intelligence = 1 Range Damage
        // 1 Willpower = 5 Mana

        UpdateMaxHPandMP();
    }

    private void UpdateMaxHPandMP()
    {
        MaxHealth = ActiveCharacter.BaseMaxHealth + BaseStamina * 5;
        MaxMP = ActiveCharacter.BaseMaxMP + BaseIntelligence * 5;
        StatsUpdated?.Invoke();
    }

    internal void UpdateInventoryStatsAddon()
    {
        Debug.Log("Update Equipped addons");

        int[] addons = EquippedManager.Instance.GetEquippedItemsStats();

        ItemStrength = addons[0];
        ItemStamina = addons[1];
        ItemIntelligence = addons[2];
        ItemWillpower = addons[3];

        StatsUpdated?.Invoke();
    }

}
