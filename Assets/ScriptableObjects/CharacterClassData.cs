using UnityEngine;

[CreateAssetMenu(fileName = "Character", menuName = "CharacterClassData")]
public class CharacterClassData : ScriptableObject
{
    public Sprite Picture;
    public CharacterType CharacterType;
    public int BaseMaxHealth;
    public int BaseMaxMP;
    public int BaseDamage;
    public int MovementSpeed;
    public int SightDistance;
    public int AttackSpeed;
    public float AttackDistance;
    public string Information = "Information about the character";
}
