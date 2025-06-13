using System;
using TMPro;
using UnityEngine;

public class UpgradePanelController : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI strength;
    [SerializeField] private TextMeshProUGUI stamina;
    [SerializeField] private TextMeshProUGUI intelligence;
    [SerializeField] private TextMeshProUGUI willpower;
    [SerializeField] private TextMeshProUGUI availablePointsText;

    [SerializeField] private GameObject[] plusses;
    [SerializeField] private GameObject[] minuses;

    public static UpgradePanelController Instance { get; private set; }

    int[] upgrades = new int[4];
    int availablePoints = 0;

    private void Awake()
    {
        if (Instance != null) {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }
    private void OnEnable()
    {
        Debug.Log("Upgrade panel Enabled - Update");
        availablePoints = Stats.Instance.AvailableUpgradePoints;
        HideAllMinus();
        UpdatePanel();
    }


    public void RequestRemovePointAt(int index)
    {
        Debug.Log("Requesting remove point "+index);
        if (upgrades[index] <= 0) return;

        upgrades[index]--;
        availablePoints++;

        // Hide Minus if no points here
        if (upgrades[index] == 0)
            minuses[index].SetActive(false);

        UpdatePanel();
    }
    
    public void RequestUpgradePointAt(int index)
    {
        Debug.Log("Requesting to upgrade at "+index);
        if (availablePoints == 0) return;

        availablePoints--;
        upgrades[index]++;
        minuses[index].SetActive(true);

        UpdatePanel();
    }

    public void UpdatePanel()
    {
        strength.text = Stats.Instance.Strength.ToString() + (upgrades[0] > 0 ? (" + " + upgrades[0]) : "");
        stamina.text = Stats.Instance.Stamina.ToString() + (upgrades[1] > 0 ? (" + " + upgrades[1]) : "");
        intelligence.text = Stats.Instance.Intelligence.ToString() + (upgrades[2] > 0 ? (" + " + upgrades[2]) : "");
        willpower.text = Stats.Instance.Willpower.ToString() + (upgrades[3] > 0 ? (" + " + upgrades[3]) : "");
        availablePointsText.text = "Available Points: " + availablePoints.ToString();

        // Hide all plusses if all points are assigned
        HidePlusses();
    }

    private void HideAllMinus()
    {
        foreach (var minus in minuses) {
            minus.gameObject.SetActive(false);
        }
    }
    private void HidePlusses()
    {        
        foreach (var plus in plusses) {
            plus.gameObject.SetActive(availablePoints > 0);
        }        
    }

    public void OkClicked()
    {
        Debug.Log("Clicking OK in Character Stats point");
        if (availablePoints > 0) return;

        // Apply the points to stats
        Stats.Instance.ApplyUpgradePoints(upgrades);
        gameObject.SetActive(false);
    }

}
