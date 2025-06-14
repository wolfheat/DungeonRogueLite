using TMPro;
using UnityEngine;

public class UIStatsPanel : MonoBehaviour
{

    [SerializeField] private TextMeshProUGUI healthInfo;
    [SerializeField] private TextMeshProUGUI levelInfo;
    [SerializeField] private TextMeshProUGUI dungeonLevelInfo;
    [SerializeField] private TextMeshProUGUI enemiesKilled;
    [SerializeField] private TextMeshProUGUI experience;
    [SerializeField] private TextMeshProUGUI coins;

    [SerializeField] private TextMeshProUGUI strength;
    [SerializeField] private TextMeshProUGUI stamina;
    [SerializeField] private TextMeshProUGUI intelligence;
    [SerializeField] private TextMeshProUGUI willpower;

    private void Start()
    {
        UpdateInfo();
    }
    private void OnEnable()
    {
        Stats.StatsUpdated += UpdateInfo;
        UpdateInfo();
    }

    private void OnDisable() => Stats.StatsUpdated -= UpdateInfo;

    public void UpdateInfo()
    {
        healthInfo.text = "HP: " + Stats.Instance.Health+"/"+Stats.Instance.MaxHealth;
        levelInfo.text = "Level: " + Stats.Instance.Level;
        dungeonLevelInfo.text = "Dungeon: " + Stats.Instance.DungeonLevel;
        enemiesKilled.text = "Kills: " + Stats.Instance.EnemiesKilled;
        experience.text = "XP: " + Stats.Instance.XP + "/" + Stats.Instance.CurrentMaxXP;
        coins.text = "Coins: " + Stats.Instance.Coins;

        strength.text = "Str: " + Stats.Instance.BaseStrength.ToString() + (Stats.Instance.ItemStrength == 0 ? "":(" + " + Stats.Instance.ItemStrength));
        stamina.text = "Sta: " + Stats.Instance.BaseStamina.ToString() + (Stats.Instance.ItemStamina == 0 ? "" : (" + " + Stats.Instance.ItemStamina));
        intelligence.text = "Int: " + Stats.Instance.BaseIntelligence.ToString() + (Stats.Instance.ItemIntelligence == 0 ? "" : (" + " + Stats.Instance.ItemIntelligence));
        willpower.text = "Wil: " + Stats.Instance.BaseWillpower.ToString() + (Stats.Instance.ItemWillpower == 0 ? "" : (" + " + Stats.Instance.ItemWillpower));
    }


}
