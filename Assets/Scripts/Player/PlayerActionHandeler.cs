using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerActionHandeler : MonoBehaviour
{
    [SerializeField] private TargetSelection tileSelector;
    public bool AllowReadingStoredPlayerInput { get; private set; } = true;
    public bool PerformingAction { get; set; } = false;

    public static PlayerActionHandeler Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null) {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }
    public PlayerAction StoredAction { get; set; } = null;


    private void Start()
    {
        Inputs.Instance.PlayerControls.Player.Move.performed += Move;
        Inputs.Instance.PlayerControls.Player.Turn.performed += Turn;
        Inputs.Instance.PlayerControls.Player.Click.performed += OnPlayerClick;
        Inputs.Instance.PlayerControls.Player.Space.performed += SkipTurn;
        //Inputs.Instance.PlayerControls.Player.SideStep.performed += SideStep;

    }
    private void OnDisable()
    {
        Inputs.Instance.PlayerControls.Player.Move.performed -= Move;
        Inputs.Instance.PlayerControls.Player.Turn.performed -= Turn;
        Inputs.Instance.PlayerControls.Player.Click.performed -= OnPlayerClick;
        Inputs.Instance.PlayerControls.Player.Space.performed -= SkipTurn;
        // Inputs.Instance.PlayerControls.Player.SideStep.performed -= SideStep;        
    }

    private void SkipTurn(InputAction.CallbackContext context) => EndPlayerTurn();

    // Only stores the last player input, when player are allowed to do action this stores action is removed and executed
    private void Move(InputAction.CallbackContext context)
    {
        if (Stats.Instance.IsDead || Stats.Instance.IsPaused) return;

        Vector2 move = context.action.ReadValue<Vector2>();
        StoredAction = new PlayerAction(ActionType.Movement, new Vector2Int(Math.Sign((int)move.x), Math.Sign((int)move.y)));
        Debug.Log("Storing move "+StoredAction.movement);
    }

    private void Turn(InputAction.CallbackContext context)
    {
        StoredAction = new PlayerAction(ActionType.Turn, context.ReadValue<float>());
        Debug.Log("Storing turn "+StoredAction.turn);
    }

    private void OnPlayerClick(InputAction.CallbackContext context)
    {
        StoredAction = new PlayerAction(ActionType.Attack, tileSelector.transform.position);
        Debug.Log("Storing click "+StoredAction.position);
    }

    private void Update()
    {
        // If its the players turn keep waiting for the first input
        if (!AllowReadingStoredPlayerInput) return;

        // Check if there is a stored input
        if(StoredAction != null) {

            // After deciding to read stop reading any further
            AllowReadingStoredPlayerInput = false;

            // Execute this action
            switch (StoredAction.actionType) {
                case ActionType.Movement:
                    if (!PlayerMovement.Instance.TryMovePlayer(StoredAction.movement))
                        AllowReadingStoredPlayerInput = true; // re-allow reading stored input
                    break;
                case ActionType.Turn:
                    PlayerMovement.Instance.TryTurnPlayer(StoredAction.turn); // Currently always allowed, also wont take action since not calling end turn
                    
                    AllowReadingStoredPlayerInput = true; // re-allow reading stored input

                    break;
                case ActionType.Attack:
                    if (!PlayerInteract.Instance.TryAttack(StoredAction.position))
                        AllowReadingStoredPlayerInput = true; // re-allow reading stored input
                    break;
            }

            // After evaluating an action, remove it even if its an illegal action
            StoredAction = null;
        }
    }

    internal void StartPlayerTurn()
    {
        Debug.Log("Start PLayers Turn");
        AllowReadingStoredPlayerInput = true;
    }

    internal void EndPlayerTurn()
    {
        PerformingAction = false;

        Debug.Log("Player turn ended");
        // Have all enemies do their turns
        Debug.Log("Enemies performing their action now");
        TickManager.TickGame?.Invoke();
    }

    internal void RemoveCurrentAction()
    {
        StoredAction = null;
    }

    // Player Actions

    public enum ActionType { Movement, Turn, Attack }

    public class PlayerAction
    {
        public ActionType actionType;
        public Vector2Int movement;
        public Vector3 position;
        public float turn;

        public PlayerAction(ActionType type, Vector3 pos)
        {
            actionType = type;
            position = pos;
        }
        public PlayerAction(ActionType type, Vector2Int move)
        {
            actionType = type;
            movement = move;
        }
        public PlayerAction(ActionType type, float move)
        {
            actionType = type;
            turn = move;
        }
    }

}
