using System;
using System.Collections.Generic;
using System.Text;
using Unity.VisualScripting;
using UnityEngine;


[System.Flags]
public enum TileType
{
    None = 0,
    Walkable = 1, // 2
    Wall = 2, // 1
    Floor = Wall | Walkable // 3
}

public class LevelCreator : MonoBehaviour
{
    [SerializeField] private Transform floorHolder;
    [SerializeField] private Transform wallsHolder;
    [SerializeField] private Transform enemyHolder;
    
    // Biomes? 1-grassland 2-snowland 3-lavaland ???

    [SerializeField] private GameObject[] wallPrefabs;
    [SerializeField] private GameObject[] floorPrefabs;
    [SerializeField] private GameObject[] dungeonEntryExitsPrefabs;

    [SerializeField] private int currentBiome = 1;


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

    void Start()
    {
        SetLevelFromFloors();

        AddWallsToArray();

        PrintLevel();

        // Now Use this level to create a snow world
        CreateLevelFromArray();

        PlayerMovement.Instance.ReturnToStartPosition();
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

    // SET ARRAY LEVEL FROM OBJECTS
    #region objects->array REGION
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
            level[tilePos.x, tilePos.y] = 2; // Change this to 2 later to differentiate?
            if(tilePos.x == -offset.x && tilePos.y == -offset.y)
                Debug.Log("0,0 removed as wall cause of object "+tile.name);
        }

    }

    #endregion


    // SET OBJECTS FROM ARRAY

    #region array->objects REGION
    private void CreateLevelFromArray()
    {
        Debug.Log("REMOVING ALL OLD TILES");
        RemoveAllWalls();
        RemoveAllFloors();
        RemoveAllEnemies();

        // Place artificaial start and end
        PrintLevel();
            
        level[15, 2] = 8;
        level[22, 6] = 9;

        // Enemies placed
        PlaceEnemies(10,0);
        PlaceEnemies(10,1);


        PrintLevel();

        offset = Vector2Int.zero;

        Debug.Log("CREATING ALL NEW TILES");
        PlacePortals();
        AddFloors();
        AddWalls();
    }

    private void PlaceEnemies(int amt, int enemyType)
    {
        int xdimention = level.GetLength(0);
        int ydimention = level.GetLength(1);

        int placed = 0;
        int triesAllowed = 150;
        while(placed < amt && triesAllowed > 0) {
            int x = UnityEngine.Random.Range(0, xdimention);
            int y = UnityEngine.Random.Range(0, ydimention);
            if (level[x,y] == 1) {
                Vector2Int pos = new Vector2Int(x,y);
                ItemSpawner.Instance.SpawnEnemy(enemyType, pos); 
                //level[x,y] = 5;
                placed++;
                continue;
            }
            triesAllowed--;
        }
        Debug.Log("Placed enemies after "+(150-triesAllowed)+" tries.");
    }

    private void PlacePortals()
    {
        dungeonEntryExitsPrefabs[0].transform.position = FindPortalPosition(8);
        dungeonEntryExitsPrefabs[1].transform.position = FindPortalPosition(9);
    }

    private Vector3 FindPortalPosition(int index)
    {
        for (int i = 0; i < level.GetLength(0); i++) {
            for (int j = 0; j < level.GetLength(1); j++) {
                if (level[i, j] != index) continue;
                level[i, j] = 1; // Overwrite with floortile for next steps
                return new Vector3(i,0,j);
            }
        }
        return Vector3.zero;
    }

    // Adding tiles
    private void AddFloors() => GenerateChildren(floorHolder, floorPrefabs[currentBiome],TileType.Floor);
    private void AddWalls() => GenerateChildren(wallsHolder, wallPrefabs[currentBiome],TileType.Wall);
        
    
    // Removing tiles
    private void RemoveAllWalls() => DestroyAllChildren(wallsHolder);
    private void RemoveAllFloors() => DestroyAllChildren(floorHolder);
    private void RemoveAllEnemies() => DestroyAllChildren(enemyHolder);


    private void GenerateChildren(Transform holder, GameObject prefab, TileType type)
    {
        float yPos = holder.position.y;
        int amt = 0;
        Debug.Log("Generating tiles for "+holder.name+" using prefab "+prefab.name+" TiletypeMask: "+ System.Convert.ToString((int)type, 2).PadLeft(8,'0'));
        for (int i = 0; i < level.GetLength(0); i++) {
            for (int j = 0; j < level.GetLength(1); j++) {
                //if (level[i, j] != typeToUse) continue;
                TileType cellType = (TileType)level[i, j];
                if (cellType == TileType.None) continue;
                if (!type.HasFlag(cellType)) continue;

                GameObject tile = Instantiate(prefab, new Vector3(i, yPos, j),Quaternion.identity,holder);
                amt++;
            }
        }
        Debug.Log("Created "+amt+" items.");
    }

    private void DestroyAllChildren(Transform holder)
    {
        Debug.Log("Destroy children");
        int amt = 0;    
        Transform[] items = holder.GetComponentsInChildren<Transform>();
        foreach (Transform item in items) {
            if (item == holder) continue;
            Destroy(item.gameObject);
            amt++;
        }
        Debug.Log("Destroyed "+amt+" children of "+holder.name+".");
    }
    #endregion

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
                    if (levelCopy[current.x + i, current.y + j] == 1) { // Walkables
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
