using System;
using TMPro;
using UnityEngine;

public class DungeonPanel : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI dungeonLevel;

    private void Start() => Stats.StatsUpdated += StatsUpdated;
    private void OnDisable() => Stats.StatsUpdated -= StatsUpdated;

    private void StatsUpdated()
    {
        dungeonLevel.text = Stats.Instance.DungeonLevel.ToString();
    }
}
