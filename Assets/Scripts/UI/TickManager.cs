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

    List<EnemyController> activeEnemies = new List<EnemyController>();

    public void RemoveEnemyDoingAction(EnemyController enemy)
    {
        if(activeEnemies.Contains(enemy))
            activeEnemies.Remove(enemy);

        if (activeEnemies.Count == 0)
            PlayerActionHandeler.Instance.StartPlayerTurn();
    }

    public void TickRequest()
    {
        Debug.Log("** TICK REQUEST");

        // DelayTick one frame?
        StartCoroutine(DelayTick());
    }

    private IEnumerator DelayTick()
    {
        yield return new WaitForSecondsRealtime(Time.fixedDeltaTime);
        //yield return new WaitForSeconds(0.05f);
        Debug.Log("** TICK");
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

    internal void EndEnemyTicks()
    {
        Debug.Log("Ended Enemy turns - let player take over");
        PlayerActionHandeler.Instance.StartPlayerTurn();
    }
}
