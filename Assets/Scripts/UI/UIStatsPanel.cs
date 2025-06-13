using TMPro;
using UnityEngine;

public class UIStatsPanel : MonoBehaviour
{

    [SerializeField] private TextMeshProUGUI healthInfo;
    [SerializeField] private TextMeshProUGUI levelInfo;
    [SerializeField] private TextMeshProUGUI dungeonLevelInfo;
    [SerializeField] private TextMeshProUGUI enemiesKilled;
    [SerializeField] private TextMeshProUGUI experience;

    private void Start()
    {
        UpdateInfo();
    }
    private void OnEnable() => Stats.StatsUpdated += UpdateInfo;

    private void OnDisable() => Stats.StatsUpdated -= UpdateInfo;

    public void UpdateInfo()
    {
        healthInfo.text = "HP: " + Stats.Instance.Health+"/"+Stats.Instance.MaxHealth;
        levelInfo.text = "Level: " + Stats.Instance.Level;
        dungeonLevelInfo.text = "Dungeon: " + Stats.Instance.DungeonLevel;
        enemiesKilled.text = "Kills: " + Stats.Instance.EnemiesKilled;
        experience.text = "XP: " + Stats.Instance.XP + "/" + Stats.Instance.CurrentMaxXP;
    }


}
