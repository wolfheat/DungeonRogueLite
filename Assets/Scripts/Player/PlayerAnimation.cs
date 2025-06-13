using UnityEngine;



public enum AnimationType{Idle,Attack1h,AttackBow}

public class PlayerAnimation : MonoBehaviour
{
    [SerializeField] private Animator animator;


    public static PlayerAnimation Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null) {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }



    public void PlayAnimation(AnimationType type)
    {
        string animation = type.ToString();
        Debug.Log("PLayer animation: "+animation);
        animator.CrossFade(animation,0.1f);
    }

}
