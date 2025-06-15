using System;
using UnityEngine;

public class LevelDataController : MonoBehaviour
{
    [SerializeField] private LevelData[] leveldatas;

    public static LevelDataController Instance { get; private set; }

    public int TotalLevels => leveldatas.Length;
    private void Awake()
    {
        if (Instance != null) {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public int[,] GetLevelData(int index) => leveldatas[Math.Min(index, leveldatas.Length - 1)].Level;
}
