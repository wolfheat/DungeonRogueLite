using TMPro;
using UnityEngine;

public class ItemStatsController : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI[] texts;
    string[] statsStrings = { "Str: ", "Sta: ", "Int: ", "Wil: " };

    public void UpdateStats(int[] stats)
    {
        for (int i = 0; i < texts.Length; i++) {
            TextMeshProUGUI text = texts[i];
            text.text = statsStrings[i] + (stats[i] == 0 ? "":stats[i].ToString());
        }
    }
}
