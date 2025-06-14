using TMPro;
using UnityEngine;

public class UIHealthBarPanel : MonoBehaviour
{

    [SerializeField] private BarController healthBar;
    [SerializeField] private BarController manaBar;

    private void Start()
    {
        UpdateInfo();
    }

    private void OnEnable() => Stats.StatsUpdated += UpdateInfo;

    private void OnDisable() => Stats.StatsUpdated -= UpdateInfo;

    public void UpdateInfo()
    {
        //Set Bars
        healthBar.SetBar(Stats.Instance.Health, Stats.Instance.MaxHealth);
        manaBar.SetBar(Stats.Instance.MP, Stats.Instance.MaxMP);
    }


}
