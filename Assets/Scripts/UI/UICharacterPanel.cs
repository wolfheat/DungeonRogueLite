using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UICharacterPanel : MonoBehaviour
{

    [SerializeField] private TextMeshProUGUI characterName;
    [SerializeField] private TextMeshProUGUI coinsAmount;
    [SerializeField] private TextMeshProUGUI characterLevel;
    [SerializeField] private Image image;


    public static UICharacterPanel Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null) {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }


    public void SetCharacterName(string newName) => characterName.text = newName;
    public void SetCharacterLevel(int level) => characterLevel.text = level.ToString();

    public void UpdateCoins(int amt) => coinsAmount.text = amt.ToString();

    internal void SetCharacterIcon(Sprite picture) => image.sprite = picture;
}
