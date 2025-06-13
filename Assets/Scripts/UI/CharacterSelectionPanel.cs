using UnityEngine;
public enum CharacterType{Fighter,Paladin,Ranger,Thief,Wizard}

public class CharacterSelectionPanel : MonoBehaviour
{
    private int selectedIndex = 0;

    [SerializeField] private CharacterClassData[] characterDatas; 


    public void CharacterSelected(int index)
    {
        Debug.Log("Selecting character "+index);
        selectedIndex = index;
        Stats.Instance.ChangeCharacterData(characterDatas[index]);
    }

    public void StartGamePressed()
    {
        Debug.Log("Starting the Game with selected character [" +selectedIndex+ "]: " + (CharacterType)selectedIndex);
        gameObject.SetActive(false);
        UIController.Instance.StartGame();
    }
}
