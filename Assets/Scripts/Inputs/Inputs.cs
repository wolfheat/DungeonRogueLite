using UnityEngine;

public class Inputs : MonoBehaviour
{
    public PlayerControls PlayerControls;

    public static Inputs Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null) {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        
        PlayerControls = new PlayerControls();
        PlayerControls.Enable();
    }


}
