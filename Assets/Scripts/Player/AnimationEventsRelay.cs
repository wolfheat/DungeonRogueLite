using UnityEngine;

public class AnimationEventsRelay : MonoBehaviour
{
    [SerializeField] BaseCharacterInteract characterInteractRelay;

    public void AttackCompleted() => characterInteractRelay.AnyAttackCompleted();
    public void BowAttackCompleted() => characterInteractRelay.BowAttackCompleted();
}
