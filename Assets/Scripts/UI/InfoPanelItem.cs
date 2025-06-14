using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InfoPanelItem : MonoBehaviour
{
    [SerializeField] private ItemStatsController itemController; 
    [SerializeField] private Image image; 
    [SerializeField] private TextMeshProUGUI itemName; 

    public void UpdateInfoPanelFromItemData(ItemData data)
    {
        image.sprite = data.Picture;
        itemController.UpdateStats(data.GetStatsArray());
        itemName.text = data.ItemName;
    }
}
