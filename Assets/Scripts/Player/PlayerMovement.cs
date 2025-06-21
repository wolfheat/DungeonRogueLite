using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using Wolfheat.StartMenu;
using static TMPro.SpriteAssetUtilities.TexturePacker_JsonArray;

public class PlayerMovement : MonoBehaviour
{

    public static PlayerMovement Instance { get; private set; }

    private Vector3 forward = Vector3.forward;
    private int rotation = 0;
    [SerializeField] private LayerMask enemyLayer; 

    public bool PerformingAction { get; set; } = false;

    private void Awake()
    {
        if (Instance != null) {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        PerformingAction = false;
    }

    private void Start()
    {
        Inputs.Instance.PlayerControls.Player.Move.performed += Move;
        Inputs.Instance.PlayerControls.Player.Turn.performed += Turn;
        //Inputs.Instance.PlayerControls.Player.SideStep.performed += SideStep;

    }

    public IEnumerator ReturnToStartPosition()
    {
        Debug.Log("ReturnToStartPosition wait 0.1s");
        yield return new WaitForSeconds(0.1f);

        Debug.Log("Setting player to startposition at "+ LevelCreator.Instance.StartPosition);

    }

    private void OnDisable()
    {
        Inputs.Instance.PlayerControls.Player.Move.performed -= Move;
        Inputs.Instance.PlayerControls.Player.Turn.performed -= Turn;
       // Inputs.Instance.PlayerControls.Player.SideStep.performed -= SideStep;        
    }

    private void Update() => LookAtMouse();

    private void LookAtMouse()
    {
        if (Stats.Instance.IsDead || Stats.Instance.IsPaused) return;

        // Step 1: Get the mouse position in screen space
        Vector2 mouseScreenPosition = Mouse.current.position.ReadValue();

        // Step 2: Create a ray from the camera through the mouse position
        Ray ray = Camera.main.ScreenPointToRay(mouseScreenPosition);
        RaycastHit hit;

        Vector3 tileAimPosition = Vector3.zero;



        // Find Enemy under cursor
        if (Physics.Raycast(ray, out hit, 20f, enemyLayer)) {
            GameObject enemy = hit.collider.gameObject;
            //Debug.Log("Enemy hit: " + enemy.name);

            tileAimPosition = enemy.transform.position;


        }
        else {
            // Step 3: Create a plane at y = 0 (the ground)
            Plane groundPlane = new Plane(Vector3.up, Vector3.zero);

            // Step 4: Find the point where the ray hits the ground plane
            if (groundPlane.Raycast(ray, out float enter)) {
                tileAimPosition = ray.GetPoint(enter);

                // Show position in game
                MousePositionInGame.Instance.transform.position = tileAimPosition;
            }        
        }

        // Show position in game
        MousePositionInGame.Instance.transform.position = tileAimPosition;

        // Step 5: Calculate direction from player to mouse
        Vector3 lookDirection = tileAimPosition - transform.position;

        // Step 6: Apply rotation
        transform.rotation = Quaternion.LookRotation(lookDirection, Vector3.up);

    }

    private void Turn(InputAction.CallbackContext context)
    {
        TurnPlayer(context.ReadValue<float>());
    }

    public void Reset()
    {
        //Debug.Log("** Resetting player **");
        StopAllCoroutines();
        forward = Vector3.forward;
        rotation = 0;
        PerformingAction = false;

        // Instantly set player position
        //Debug.Log("** Setting Player position instantly **");
        transform.position = Convert.Align(LevelCreator.Instance.StartPosition);

        CenterOverPlayer.Instance.ResetToPosition(transform.position);
    }
    private void TurnPlayer(float v)
    {
        //Debug.Log("** Turning Player rotationIndex was "+rotation);

        // Rotate here
        rotation = (rotation +(v > 0 ? 1 : 3)) % 4;
        //Debug.Log("** Forward index becomes" + rotation);

        forward = rotation switch
        {
            0 => Vector3.forward,
            1 => Vector3.right,
            2 => -Vector3.forward,
            3 => -Vector3.right,
            _ => Vector3.forward
        };
        //Debug.Log("Turning to forward "+forward);

        CenterOverPlayer.Instance.SetRotation(forward);

        //transform.rotation = Quaternion.LookRotation(transform.right*v);
        //transform.rotation = Quaternion.LookRotation(transform.right*v,Vector3.up);
    }

    private void Move(InputAction.CallbackContext context)
    {
        if (Stats.Instance.IsDead || Stats.Instance.IsPaused) return;
        
        
        if (PerformingAction) return;
        
        //Debug.Log("Player Moving "+context.action.ReadValue<Vector2>());
        MovePlayer(context.action.ReadValue<Vector2>());
    }

    private void MovePlayer(Vector2 vector2)
    {
        vector2 = new Vector2(Math.Sign((int)vector2.x), Math.Sign((int)vector2.y));

        Transform camera = CenterOverPlayer.Instance.transform;


        Vector3 movement = camera.forward * vector2.y + camera.right * vector2.x;
        

        // Instant movement to position

        Vector3 movePosition = transform.position + movement;

        // Check if move is legal
        bool legal = PlayerColliderController.Instance.CheckIfLegalMoveTo(movePosition);

        if (!legal) {
            return;
        }

        // Center
        movePosition = Convert.Align(movePosition);

        //Debug.Log("Tweening to "+movePosition);
        TweenMovement(movePosition);
                
        SoundMaster.Instance.PlayStepSound(3);

        // Have player action end call a tick
        TickManager.Instance.TickRequest();


    }

    private void CheckForHold()
    {
        if (PerformingAction) return;

        Vector2 held = Inputs.Instance.PlayerControls.Player.Move.ReadValue<Vector2>();

        if (held.x == 0 && held.y == 0) return;
        //Debug.Log("Continious held input");
        MovePlayer(held);
    }

    private void TweenMovement(Vector3 movement)
    {
        //Debug.Log("Performing action TRUE");
        PerformingAction = true;

        StartCoroutine(TweenToPosition(movement));

        //transform.rotation = Quaternion.LookRotation(forward, Vector3.up);

        IEnumerator TweenToPosition(Vector3 forward)
        {
            //Debug.Log("TWEEN MOVEMENT");
            Vector3 startPosition = transform.position;
            Vector3 endPosition = movement;
            float tweenTime = 0.04f;
            float timer = 0f;
            while (timer < tweenTime) {
                timer += Time.deltaTime;
                float t = timer / tweenTime;
                transform.position = Vector3.Lerp(startPosition, endPosition, t);
                CenterOverPlayer.Instance.Center(transform.position);
                yield return null;
            }
            transform.position = endPosition;
            yield return new WaitForSeconds(0.1f);
            PerformingAction = false;
            //Debug.Log("Performing action FALSE");
            CheckForHold();
        }
        
    }

    private void SideStep(InputAction.CallbackContext context)
    {
        Debug.Log("Player SideStep "+context.action.ReadValue<float>());
    }
}
