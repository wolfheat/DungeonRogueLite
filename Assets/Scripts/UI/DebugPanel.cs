using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
public class DebugPanel : MonoBehaviour
{
    [SerializeField] private GameObject panel;
    [SerializeField] private GameObject debugHolder;
    [SerializeField] private DebugItems debugItemsPrefab;

    private Queue<DebugItems> debugItems = new();
    private const int DebugMessagesMax = 16;


    public static DebugPanel Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null) {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }


    private void OnEnable()
    {
        Inputs.Instance.PlayerControls.Player.F5.performed += TogglePanel;
    }

    private void OnDisable()
    {
        Inputs.Instance.PlayerControls.Player.F5.performed -= TogglePanel;        
    }

    private void TogglePanel(InputAction.CallbackContext context) => panel.SetActive(!panel.activeSelf);

    public void AddDebugText(string message)
    {
        DebugItems debugItem = Instantiate(debugItemsPrefab,debugHolder.transform);
        debugItem.SetText(message);
        if (debugItems.Count == DebugMessagesMax) {
            DebugItems item = debugItems.Dequeue();
            Destroy(item.gameObject);
        }
        debugItems.Enqueue(debugItem);
    }

    public void Clear()
    {
        Debug.Log("Deleting all text");
        while (debugItems.Count > 0) {
            DebugItems item = debugItems.Dequeue();
            Destroy(item.gameObject);
        }
    }
}
