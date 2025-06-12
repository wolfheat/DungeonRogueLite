using UnityEngine;

[CreateAssetMenu(fileName = "Enemy", menuName = "Enemy")]
public class EnemyData : ScriptableObject
{
    public int MaxHealth;
    public int Damage;
    public int MovementSpeed;
    public int SightDistance;
    public int AttackSpeed;
    public int ActionSpeed ;
    public float AttackDistance;
    public int XP = 10;
}
