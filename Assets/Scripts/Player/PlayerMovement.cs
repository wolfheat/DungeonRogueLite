using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using Wolfheat.StartMenu;

public class PlayerMovement : MonoBehaviour
{
    public static PlayerMovement Instance { get; private set; }

    private Vector3 forward = Vector3.forward;
    private int rotation = 0;
    [SerializeField] private LayerMask enemyLayer; 


    private void Awake()
    {
        if (Instance != null) {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public IEnumerator ReturnToStartPosition()
    {
        Debug.Log("ReturnToStartPosition wait 0.1s");
        yield return new WaitForSeconds(0.1f);

        Debug.Log("Setting player to startposition at "+ LevelCreator.Instance.StartPosition);

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

    public void Reset()
    {
        //Debug.Log("** Resetting player **");
        StopAllCoroutines();
        forward = Vector3.forward;
        rotation = 0;

        // Instantly set player position
        //Debug.Log("** Setting Player position instantly **");
        transform.position = Convert.Align(LevelCreator.Instance.StartPosition);

        CenterOverPlayer.Instance.ResetToPosition(transform.position);
    }
    public bool TryTurnPlayer(float turn)
    {
        //Debug.Log("** Turning Player rotationIndex was "+rotation);

        // Rotate here
        rotation = (rotation +(turn > 0 ? 1 : 3)) % 4;
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

        return true;
    }

    public bool TryMovePlayer(Vector2Int move)
    {
        // Get the resulting world position after the move
        Vector3 newPosition = GetMovementsNewPosition(move);

        // Check if move is legal
        if (!PlayerColliderController.Instance.CheckIfLegalMoveTo(newPosition)) 
            return false;

        PlayerActionHandeler.Instance.PerformingAction = true;

        TilePosition = Convert.V3ToV2Int(newPosition);

        TweenMovement(newPosition);

        SoundMaster.Instance.PlayStepSound(3);
        return true;
    }

    private Vector3 GetMovementsNewPosition(Vector2Int move)
    {
        Transform camera = CenterOverPlayer.Instance.transform;
        Vector3 movementFromCameraPOV = camera.forward * move.y + camera.right * move.x;

        // Instant movement to position and center
        return Convert.Align(transform.position + movementFromCameraPOV);
    }

    public Vector2Int TilePosition { get; private set; }
    private void TweenMovement(Vector3 movement)
    {
        StartCoroutine(TweenToPosition(movement));

        //transform.rotation = Quaternion.LookRotation(forward, Vector3.up);

        IEnumerator TweenToPosition(Vector3 forward)
        {

            int random = UnityEngine.Random.Range(0, 10000);
            //Debug.Log("Performing action TRUE");
            Debug.Log("TWEEN MOVEMENT - STARTED "+random);

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

            Debug.Log("TWEEN MOVEMENT - ENDED" + random);

            PlayerActionHandeler.Instance.EndPlayerTurn();
        }
    }

}
