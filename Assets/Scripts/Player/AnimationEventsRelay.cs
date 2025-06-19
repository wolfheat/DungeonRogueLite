using UnityEngine;

public class AnimationEventsRelay : MonoBehaviour
{
    [SerializeField] PlayerInteract playerInteract;

    public void AttackCompleted() => playerInteract.AnyAttackCompleted();
}
