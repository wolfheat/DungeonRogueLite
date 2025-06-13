using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UICharacterPanel : MonoBehaviour
{

    [SerializeField] private TextMeshProUGUI characterName;
    [SerializeField] private TextMeshProUGUI coinsAmount;
    [SerializeField] private TextMeshProUGUI characterLevel;
    [SerializeField] private BarController xpBar;
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

    private void Start()
    {
        Stats.CharacterUpdated += CharacterUpdated;
        Stats.StatsUpdated += StatsUpdated;
    }
    private void OnDisable()
    {
        Stats.CharacterUpdated -= CharacterUpdated;
        Stats.StatsUpdated -= StatsUpdated;
    }

    private void StatsUpdated()
    {
        Debug.Log("** Updating XP BAR");
        // Update XP bar
        xpBar.SetBar(Stats.Instance.XP, Stats.Instance.CurrentMaxXP);
    }
    private void CharacterUpdated()
    {
        
        // Update Text Level and images
        SetCharacterName(Stats.Instance.ActiveCharacter.CharacterType.ToString());
        SetCharacterIcon(Stats.Instance.ActiveCharacter.Picture);
        SetCharacterLevel(Stats.Instance.Level);

        // Update XP bar
        xpBar.SetBar(Stats.Instance.XP,Stats.Instance.CurrentMaxXP);
    }

    public void SetXP(string newName) => characterName.text = newName;
    public void SetCharacterName(string newName) => characterName.text = newName;
    public void SetCharacterLevel(int level) => characterLevel.text = level.ToString();

    public void UpdateCoins(int amt) => coinsAmount.text = amt.ToString();

    internal void SetCharacterIcon(Sprite picture) => image.sprite = picture;
}
