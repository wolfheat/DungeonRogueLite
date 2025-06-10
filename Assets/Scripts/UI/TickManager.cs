using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class TickManager : MonoBehaviour
{

    private int currentTick = 0;
    private TickBox[] tickBoxes;
    [SerializeField] private int TotalTicks = 10; 
    [SerializeField] private TickBox tickBoxPrefab;

    public static TickManager Instance { get; private set; }

    public static Action TickGame;

    private void Awake()
    {
        if (Instance != null) {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }


    void Start()
    {
        GenerateTickBoxes();

        Inputs.Instance.PlayerControls.Player.Space.performed += TickRequested;

    }

    private void TickRequested(InputAction.CallbackContext context)
    {
        Tick();
        TickGame?.Invoke();
    }

    private void GenerateTickBoxes()
    {
        DeleteOld();

        tickBoxes = new TickBox[TotalTicks];
        for (int i = 0; i < TotalTicks; i++) {
            TickBox box = Instantiate(tickBoxPrefab,transform);
            box.name = "TickBox " + i;

            box.SetActive(i==0);
            tickBoxes[i] = box;
        }
    }

    private void DeleteOld()
    {
        int amt = 0;
        TickBox[] tickBoxes = GetComponentsInChildren<TickBox>();
        for (int i = tickBoxes.Length - 1; i >= 0; i--) {
            Destroy(tickBoxes[i].gameObject);
            amt++;
        }
        Debug.Log("Deleted "+amt+" tickbox objects.");
    }

    public void Tick()
    {
        // Unload current tickbox
        tickBoxes[currentTick].SetActive(false);

        // Steps ahead one tick
        currentTick = (currentTick +1)%TotalTicks;
        tickBoxes[currentTick].SetActive(true);
    }

}
