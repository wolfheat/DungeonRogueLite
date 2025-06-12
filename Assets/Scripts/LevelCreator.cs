using System;
using System.Collections.Generic;
using System.Text;
using Unity.VisualScripting;
using UnityEngine;

public class LevelCreator : MonoBehaviour
{

    [SerializeField] private GameObject floorHolder;
    [SerializeField] private GameObject wallsHolder;


    private int[,] level;
    private int levelWidth;
    private int levelHeight;
    private Vector2Int offset = new Vector2Int();


    public static LevelCreator Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null) {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        SetLevelFromFloors();


        //PrintLevel();

        //Debug.Log("StartPos = " + level[-offset.x,-offset.y] + " after first print ");

        AddWallsToArray();

        //PrintLevel();
    }

    private void PrintLevel()
    {

        StringBuilder sb = new StringBuilder("LEVEL\n");
        for (int j = level.GetLength(1) - 1; j >= 0; j--) {
            sb.Append("[");
            for (int i = 0; i < level.GetLength(0); i++) {
                sb.Append(level[i,j] + (i<(level.GetLength(0)-1)?",":"]"));
            }
            sb.Append("\n");
        }
        Debug.Log(sb.ToString());
    }

    private void SetLevelFromFloors()
    {
        // Read all floor pieces and add them to the array with an offset?

        Transform[] tiles = floorHolder.transform.GetComponentsInChildren<Transform>();

        (Vector2Int bottomLeft, Vector2Int topRight) = GetDimensions(tiles);
        offset = bottomLeft;

        levelWidth = topRight.x - bottomLeft.x + 1;
        levelHeight = topRight.y - bottomLeft.y + 1;


        // Create the right size of the array and place the numbers
        level = new int[levelWidth, levelHeight];

        foreach(var tile in tiles) {
            Vector2Int tilePos = Convert.V3ToV2Int(tile.transform.position)-offset;
            level[tilePos.x, tilePos.y] = 1;
        }
    }

    private void AddWallsToArray()
    {
        Transform[] tiles = wallsHolder.transform.GetComponentsInChildren<Transform>();
        foreach (var tile in tiles) {
            if (tile == wallsHolder.transform) continue;
            Vector2Int tilePos = Convert.V3ToV2Int(tile.transform.position) - offset;
            level[tilePos.x, tilePos.y] = 0; // Change this to 2 later to differentiate?
            if(tilePos.x == -offset.x && tilePos.y == -offset.y)
                Debug.Log("0,0 removed as wall cause of object "+tile.name);
        }

    }


    private (Vector2Int bottomLeft, Vector2Int topRight) GetDimensions(Transform[] floors)
    {
        // Go through all tiles to get the bottom left and top right one
        Vector2Int firstTilePosition = Convert.V3ToV2Int(floors[0].transform.position);
        int[] bottomLeft = {firstTilePosition.x,firstTilePosition.y};
        int[] topRight   = {firstTilePosition.x,firstTilePosition.y};
        
        foreach (Transform floor in floors) {
            Vector2Int pos = Convert.V3ToV2Int(floor.transform.position);
            bottomLeft[0] = Math.Min(bottomLeft[0],pos.x);
            bottomLeft[1] = Math.Min(bottomLeft[1],pos.y);
            topRight[0] = Math.Max(topRight[0],pos.x);
            topRight[1] = Math.Max(topRight[1],pos.y);
        }
        return (new Vector2Int(bottomLeft[0], bottomLeft[1]), new Vector2Int(topRight[0], topRight[1]));
    }

    public List<Vector2Int> GetPath(Vector2Int from, Vector2Int to)
    {
        //Debug.Log("Trying to find a path from "+from+" to "+to);
        //PrintLevel();



        // Make sure the positions are normalized
        from -= offset;
        to -= offset;

        int[,] levelCopy = (int[,])level.Clone();


        // Do A*

        List<Vector2Int> activeNodes = new List<Vector2Int>();
        activeNodes.Add(from);
        List<Vector2Int> answer = WalkPath(from, to, new List<Vector2Int>(), 0);

        //LogWalk(answer);


        /// LOCAL METHODS

        return answer;
        // Maybe use to make pathfinding better later

        // Walk Cost
        

        List<Vector2Int> WalkPath(Vector2Int current, Vector2Int target, List<Vector2Int> walk, int cost = 0)
        {
            //LogWalk(walk);

            if (current == target) {
                //Debug.Log("Target Found at " + current);
                //LogWalk(walk);
                return walk;
            }

            // Add current step
            List<Vector2Int> newWalk = new List<Vector2Int>(walk);
            newWalk.Add(current+offset);

            // Check all neighbors that are walkable
            List<Vector2Int> neighbors = GetNeighbors(current);
            neighbors.Sort((a, b) => (Mathf.Abs(a.x - target.x) + Mathf.Abs(a.y - target.y)).CompareTo(Mathf.Abs(b.x - target.x) + Mathf.Abs(b.y - target.y)));

            // Order These Later?
            // Go to all
            foreach (Vector2Int neighbor in neighbors) {
                List<Vector2Int> ans = WalkPath(neighbor, target, newWalk, cost + 1);
                if (ans.Count > 0) {
                    return ans;
                }
            }
            // No path found
            return new();
        }

        List<Vector2Int> GetNeighbors(Vector2Int current)
        {
            List<Vector2Int> answer = new();
            // Find all neighbors that are walkable
            for (int i = -1; i<= 1; i++) {
                for (int j = -1; j <= 1; j++) {
                    if (current.x + i < 0 || current.x + i >= levelWidth - 1 || current.y + j < 0 || current.y + j >= levelHeight - 1) continue; //OOB
                    if(i==0 && j==0) continue;
                    if (levelCopy[current.x + i, current.y + j] == 1) {
                        answer.Add(new Vector2Int(current.x + i, current.y + j));
                        // Remove this as an available target
                        levelCopy[current.x + i, current.y + j] = 0;
                    }
                }
            }     
            return answer;
        }
    }

    private void LogWalk(List<Vector2Int> walk)
    {
        if (walk.Count > 0) {
            StringBuilder sb = new StringBuilder();
            foreach (Vector2Int step in walk) {
                sb.Append("[" + step.x + "," + step.y + "] -> ");
            }
            Debug.Log("Steps: " + sb.ToString());
        }

    }
}
