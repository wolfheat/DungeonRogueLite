using UnityEngine;

[CreateAssetMenu(fileName = "Enemy", menuName = "Enemy")]
public class EnemyData : ScriptableObject
{
    [Header("General Stats")]
    public int MaxHealth;
    public int XP = 10;
    public int Damage;

    [Header("Actions")]
    public int MovementSpeed;
    public int SightDistance;
    public int AttackSpeed;
    public int ActionSpeed ;
    public float AttackDistance;
}
