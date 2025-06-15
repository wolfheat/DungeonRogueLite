using TMPro;
using UnityEngine;
using UnityEngine.UI;
public enum CharacterType{Fighter,Paladin,Ranger,Thief,Wizard}

public class CharacterSelectionPanel : MonoBehaviour
{
    private int selectedIndex = 0;

    [SerializeField] private TextMeshProUGUI characterInfo; 

    [SerializeField] private CharacterClassData[] characterDatas; 

    [SerializeField] private Selectable mainSelected;

    private void OnEnable()
    {
        mainSelected.Select();
        CharacterSelected(0);
    }

    public void CharacterSelected(int index)
    {
        //Debug.Log("Selecting character "+index);
        selectedIndex = index;
        characterInfo.text = characterDatas[selectedIndex].Information;

        Stats.Instance.ChangeCharacterData(characterDatas[index]);
    }

    public void StartGamePressed()
    {
        Debug.Log("Starting the Game with selected character [" +selectedIndex+ "]: " + (CharacterType)selectedIndex);
        gameObject.SetActive(false);
        UIController.Instance.StartGame();
    }
}
