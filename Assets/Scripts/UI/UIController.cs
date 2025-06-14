using System;
using UnityEngine;

public class UIController : MonoBehaviour
{

    [SerializeField] private GameObject deathPanel;
    [SerializeField] private GameObject upgradePanel;
    [SerializeField] private GameObject inventoryPanel;


    public static UIController Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null) {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public void ShowDeathPanel() => deathPanel.SetActive(true);


    public void AcceptOnDeathScreen()
    {
        Debug.Log("PLayer Clicked K on deathScreen");
        deathPanel.SetActive(false);

        Debug.Log("Goto Main menu, or upgrade menu?");
        Stats.Instance.Reset();
        
    }

    internal void StartGame()
    {
        Debug.Log("Startiong game");
    }

    public void OpenInventoryPanel()
    {
        if (Stats.Instance.IsDead) return;

        inventoryPanel.SetActive(true);
    }
    internal void ShowLevelUpPanel()
    {
        if (Stats.Instance.IsDead) return;

        upgradePanel.SetActive(true);
    }
}
