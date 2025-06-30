using UnityEngine;

public class ArrowData
{
    public Vector3 StartPosition { get; set; }
    public Vector3 TargetPosition { get; set; }
    public IBeingHitByArrow TargetCharacter { get; set; }
    public int Damage { get; set; }

    public ArrowData(Vector3 startPosition, Vector3 targetPosition, IBeingHitByArrow enemy, int damage)
    {
        StartPosition = startPosition;
        TargetPosition = targetPosition;
        TargetCharacter = enemy;
        Damage = damage;
    }   
}

