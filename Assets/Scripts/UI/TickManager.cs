using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TickManager : MonoBehaviour
{
    private int currentTick = 0;
    private TickBox[] tickBoxes;
    [SerializeField] private int TotalTicks = 10; 
    [SerializeField] private TickBox tickBoxPrefab;


    List<EnemyController> activeEnemies = new List<EnemyController>();
    List<Arrow> activeArrows = new List<Arrow>();

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
    }

    public void SubscribeEnemy(EnemyController enemy)
    {
        if (!activeEnemies.Contains(enemy)) {
            activeEnemies.Add(enemy);
            Debug.Log("Subscribed enemy ["+activeEnemies.Count+"]");
        }
    }
    
    public void SubscribeArrow(Arrow arrow)
    {
        if (activeArrows.Contains(arrow)) {
            activeArrows.Add(arrow);
            Debug.Log("Subscribed arrow ["+activeArrows.Count+"]");
        }
    }
    public void CheckForSubscriptions()
    {
        CheckForGivingPlayerTheTurn();
    }

    public void RemoveEnemyDoingAction(EnemyController enemy)
    {
        if (activeEnemies.Contains(enemy)) {
            activeEnemies.Remove(enemy);
            Debug.Log("Un-Subscribed enemy [" + activeEnemies.Count + "]");
        }

        CheckForGivingPlayerTheTurn();
    }
    
    public void RemoveArrowDoingAction(Arrow arrow)
    {
        if (activeArrows.Contains(arrow)) {
            activeArrows.Remove(arrow);
            Debug.Log("Un-Subscribed arrow [" + activeArrows.Count + "]");
        }
        CheckForGivingPlayerTheTurn();
    }

    private void CheckForGivingPlayerTheTurn()
    {
        if (activeEnemies.Count == 0 && activeArrows.Count == 0) {
            Debug.Log("No Enemies or arrows in tickmaster - let player have the turn");
            PlayerActionHandeler.Instance.StartPlayerTurn();
        }
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

    internal void EndEnemyTicks()
    {
        Debug.Log("Ended Enemy turns - let player take over");
        PlayerActionHandeler.Instance.StartPlayerTurn();
    }
}
