using UnityEngine;



public enum AnimationType{Idle, Attack1hThrust, AttackBow, Attack1h}

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

    private void Start()
    {
        // Initiate with a sword
        PlayerEquipmentsVisualizer.Instance.ActivateEquipment(EquipmentType.Sword);        
    }



    public void PlayAnimation(AnimationType type)
    {
        if(type == AnimationType.Attack1hThrust) {
            PlayerEquipmentsVisualizer.Instance.ActivateEquipment(EquipmentType.Sword);
        }
        else if (type == AnimationType.AttackBow) {
            PlayerEquipmentsVisualizer.Instance.ActivateEquipment(EquipmentType.Bow);
        }

        string animation = type.ToString();
        Debug.Log("PLayer animation: "+animation);
        animator.CrossFade(animation,0.1f);
    }

}
