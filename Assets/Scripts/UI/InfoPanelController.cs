using System;
using UnityEngine;

public class InfoPanelController : MonoBehaviour
{
    [SerializeField] private InfoPanelItem mainInfo; 
    [SerializeField] private InfoPanelItem equippedInfo;

    public static InfoPanelController Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null) {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        // Hide both panels
        HideInfo();
    }


    private void ShowInfo(ItemData activeItem)
    {
        mainInfo.gameObject.SetActive(true);

        // No equipped item
        equippedInfo.gameObject.SetActive(false);

        mainInfo.UpdateInfoPanelFromItemData(activeItem);
    }
    public void ShowInfo(ItemData activeItem, ItemData equippedItem)
    {
        if(equippedItem == null || activeItem == equippedItem) {
            ShowInfo(activeItem);
            return;
        }
        mainInfo.gameObject.SetActive(true);

        // No equipped item
        equippedInfo.gameObject.SetActive(true);

        mainInfo.UpdateInfoPanelFromItemData(activeItem);
        equippedInfo.UpdateInfoPanelFromItemData(equippedItem);
    }

    internal void HideInfo()
    {
        mainInfo.gameObject.SetActive(false);
        equippedInfo.gameObject.SetActive(false);

    }
}
