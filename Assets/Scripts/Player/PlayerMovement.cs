using System;
using UnityEngine;
using UnityEngine.InputSystem;
using Wolfheat.StartMenu;

public class PlayerMovement : MonoBehaviour
{

    private void Start()
    {
        Inputs.Instance.PlayerControls.Player.Move.performed += Move;
        //Inputs.Instance.PlayerControls.Player.Turn.performed += Turn;
        //Inputs.Instance.PlayerControls.Player.SideStep.performed += SideStep;        
    }

    private void OnDisable()
    {
        Inputs.Instance.PlayerControls.Player.Move.performed -= Move;
        //Inputs.Instance.PlayerControls.Player.Turn.performed -= Turn;
       // Inputs.Instance.PlayerControls.Player.SideStep.performed -= SideStep;        
    }

    private void Update() => LookAtMouse();

    private void LookAtMouse()
    {
        // Step 1: Get the mouse position in screen space
        Vector2 mouseScreenPosition = Mouse.current.position.ReadValue();

        // Step 2: Create a ray from the camera through the mouse position
        Ray ray = Camera.main.ScreenPointToRay(mouseScreenPosition);

        // Step 3: Create a plane at y = 0 (the ground)
        Plane groundPlane = new Plane(Vector3.up, Vector3.zero);

        // Step 4: Find the point where the ray hits the ground plane
        if (groundPlane.Raycast(ray, out float enter)) {
            Vector3 mouseWorldPosition = ray.GetPoint(enter);

            // Show position in game
            MousePositionInGame.Instance.transform.position = mouseWorldPosition;

            // Step 5: Calculate direction from player to mouse
            Vector3 lookDirection = mouseWorldPosition - transform.position;

            // Step 6: Apply rotation
            transform.rotation = Quaternion.LookRotation(lookDirection, Vector3.up);
        }        
    }

    private void Turn(InputAction.CallbackContext context)
    {
        TurnPlayer(context.ReadValue<float>());
    }

    private void TurnPlayer(float v)
    {
        transform.rotation = Quaternion.LookRotation(transform.right*v);
        //transform.rotation = Quaternion.LookRotation(transform.right*v,Vector3.up);
    }

    private void Move(InputAction.CallbackContext context)
    {
        //Debug.Log("Player Moving "+context.action.ReadValue<Vector2>());
        MovePlayer(context.action.ReadValue<Vector2>());
    }

    private void MovePlayer(Vector2 vector2)
    {

        Vector3 movement = Vector3.forward * vector2.y + Vector3.right * vector2.x;
        //Vector3 movement = transform.forward * vector2.y + transform.right * vector2.x;

        // Instant movement to position
        Vector3 movePosition = transform.position + movement;

        // Check if move is legal
        bool illegal = PlayerColliderController.Instance.CheckForFreeSpot(movePosition);

        if (illegal) return;


        // Center
        transform.position = Convert.Align(movePosition);

        CenterOverPlayer.Instance.Center(transform.position);

        SoundMaster.Instance.PlayStepSound(3);

        // Have player action end call a tick
        TickManager.Instance.TickRequest();
    }

    private void SideStep(InputAction.CallbackContext context)
    {
        Debug.Log("Player SideStep "+context.action.ReadValue<float>());
    }
}
