using System;
using UnityEngine;

public class EnemyAnimation : MonoBehaviour, IAnimateCharacter
{
    [SerializeField] private Animator animator;
    [SerializeField] private AnimationType defaultAttackType = AnimationType.Attack1hThrust;
    public void PlayAnimation(AnimationType type)
    {
        string animation = type.ToString();
        Debug.Log("Enemy animation: " + animation);
        animator.CrossFade(animation, 0.1f);
    }

    internal void PlayAttackAnimation() => PlayAnimation(defaultAttackType);
}
