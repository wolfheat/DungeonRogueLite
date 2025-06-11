using TMPro;
using UnityEngine;

public class UIStatsPanel : MonoBehaviour
{

    [SerializeField] private TextMeshProUGUI healthInfo;
    [SerializeField] private TextMeshProUGUI levelInfo;
    [SerializeField] private TextMeshProUGUI dungeonLevelInfo;
    [SerializeField] private TextMeshProUGUI enemiesKilled;

    private void Start()
    {
        UpdateAllInfo();
    }
    private void OnEnable() => Stats.StatsUpdated += UpdateAllInfo;

    private void OnDisable() => Stats.StatsUpdated -= UpdateAllInfo;

    public void UpdateAllInfo()
    {
        healthInfo.text = "HP: " + Stats.Instance.Health+"/"+Stats.Instance.MaxHealth;
        levelInfo.text = "Level: " + Stats.Instance.Level;
        dungeonLevelInfo.text = "Dungeon: " + Stats.Instance.DungeonLevel;
        enemiesKilled.text = "Kills: " + Stats.Instance.EnemiesKilled;
    }


}
