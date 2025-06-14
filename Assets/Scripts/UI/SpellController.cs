using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class SpellController : MonoBehaviour
{

    public static SpellController Instance { get; private set; }

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
        Inputs.Instance.PlayerControls.Player.One.performed += ItemOne;
        Inputs.Instance.PlayerControls.Player.Two.performed += ItemTwo;
        Inputs.Instance.PlayerControls.Player.Three.performed += ItemThree;
        Inputs.Instance.PlayerControls.Player.Four.performed += ItemFour;
    }

    private void OnDisable()
    {
        Inputs.Instance.PlayerControls.Player.One.performed -= ItemOne;
        Inputs.Instance.PlayerControls.Player.Two.performed -= ItemTwo;
        Inputs.Instance.PlayerControls.Player.Three.performed -= ItemThree;
        Inputs.Instance.PlayerControls.Player.Four.performed -= ItemFour;
    }

    private void ItemOne(InputAction.CallbackContext context) => UseItem(1);
    private void ItemTwo(InputAction.CallbackContext context) => UseItem(2);
    private void ItemThree(InputAction.CallbackContext context) => UseItem(3);
    private void ItemFour(InputAction.CallbackContext context) => UseItem(4);

    private void UseItem(int itemIndex)
    {
        Debug.Log("Using item "+itemIndex);
    }


}
