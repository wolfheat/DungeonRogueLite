using UnityEngine;

public enum EquipMentType{Sword,Bow}
public class PlayerEquipmentsVisualizer : MonoBehaviour
{
    [Header("Equipments: Sword | Bow")]
    [SerializeField] private GameObject[] equipments;

    public static PlayerEquipmentsVisualizer Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null) {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }


    public void ActivateEquipment(EquipmentType type)
    {
        Debug.Log("ActivateEquipment "+type);
        for (int i = 0; i < equipments.Length; i++) {
            equipments[i].gameObject.SetActive(i == (int)type);
            Debug.Log("Setting equipment "+i+" active = " + equipments[i].gameObject.activeSelf);
        }
    }
}
