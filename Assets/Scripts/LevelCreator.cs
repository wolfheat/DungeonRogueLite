using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.InputSystem;


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
    [SerializeField] private Transform itemsHolder;

    // Biomes? 1-grassland 2-snowland 3-lavaland ???

    [Header("Floor")]
    [SerializeField] private GameObject[] floorPrefabs;
    [Header("Walls")]
    [SerializeField] private GameObject[] wallPrefabs;
    [Header("Portals")]
    [SerializeField] private GameObject[] dungeonPortals;

    public Vector3 StartPosition => dungeonPortals[0].transform.position;

    [SerializeField] private int currentBiome = 1;
    
    [SerializeField] private LayerMask floorMask;
    [SerializeField] private LayerMask wallMask;
    [SerializeField] private LayerMask enemyMask;



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

        Inputs.Instance.PlayerControls.Player.P.performed += PrintLevelCode;
    }
    private void OnDisable() => Inputs.Instance.PlayerControls.Player.P.performed -= PrintLevelCode;

    private void PrintLevelCode(InputAction.CallbackContext context) => PrintLevelCode();

    void Start()
    {
        ReadLevelObjectsIntoArray();

        //PrintLevel();

        // Now Use this level to create a snow world
        ResetLevel();
    }

    private void ReadLevelObjectsIntoArray(GameObject prefabToUse = null,bool unset = false)
    {
        SetLevelFromFloors(prefabToUse,unset);

        AddWallsToArray(prefabToUse);
    }

    private void ResetLevel()
    {
        currentBiome = (Stats.Instance.DungeonLevel-1) % 3;   
        Debug.Log("Biome set to "+currentBiome);

        SetRandomLevelArray();
        
        CreateLevelFromArray();

        //StartCoroutine(PlayerMovement.Instance.ReturnToStartPosition());

        //PlayerMovement.Instance.Reset();

    }

    private void SetRandomLevelArray()
    {
        int sections = 6;
        Debug.Log("Setting a new level by creating a level data using "+sections+" sections");
        level = LevelDataController.Instance.GetRandomLevelData(sections);
        
        // Create the right size of the array and place the numbers
        levelWidth = level.GetLength(0);
        levelHeight = level.GetLength(1);
        
        Debug.Log("*** Level is set from generated data");
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
    private string PrintLevelCode(bool print = true)
    {

        StringBuilder sb = new StringBuilder("\n");
        for (int j = level.GetLength(1) - 1; j >= 0; j--) {
            sb.Append("[");
            for (int i = 0; i < level.GetLength(0); i++) {
                sb.Append(level[i,j] + (i<(level.GetLength(0)-1)?",":"]\n"));
            }
            //sb.Append(j==0?"};\n":",\n");
        }
        if(print)
            Debug.Log(sb.ToString());   
        return sb.ToString();
    }

    // SET ARRAY LEVEL FROM OBJECTS
    #region objects->array REGION
    private void SetLevelFromFloors(GameObject prefabParent = null,bool unset = false)
    {
        // Read all floor pieces and add them to the array with an offset?
        

        Transform[] tiles = prefabParent != null ? prefabParent.transform.GetComponentsInChildren<Transform>().Where(x => x.gameObject.layer == LayerMask.NameToLayer("Floor")).ToArray()
                                                 : floorHolder.transform.GetComponentsInChildren<Transform>().Where(x => x.gameObject.layer == LayerMask.NameToLayer("Floor")).ToArray();

        Transform[] startPortal = prefabParent != null ? prefabParent.transform.GetComponentsInChildren<Transform>().Where(x => x.gameObject.layer == LayerMask.NameToLayer("StartPortal")).ToArray()
                                                 : floorHolder.transform.GetComponentsInChildren<Transform>().Where(x => x.gameObject.layer == LayerMask.NameToLayer("StartPortal")).ToArray();

        Transform[] exitPortal = prefabParent != null ? prefabParent.transform.GetComponentsInChildren<Transform>().Where(x => x.gameObject.layer == LayerMask.NameToLayer("ExitPortal")).ToArray()
                                                 : floorHolder.transform.GetComponentsInChildren<Transform>().Where(x => x.gameObject.layer == LayerMask.NameToLayer("ExitPortal")).ToArray();



        //Debug.Log(tiles.Length+" Floors found");
        if (tiles.Length <= 2) return;

        (Vector2Int bottomLeft, Vector2Int topRight) = GetDimensions(tiles);
        offset = bottomLeft;
        //Debug.Log("OFFSET = "+offset);

        levelWidth = topRight.x - bottomLeft.x + 1;
        levelHeight = topRight.y - bottomLeft.y + 1;

        Vector2Int topRightIndex = new Vector2Int(levelWidth-1, levelHeight-1);

        // Create the right size of the array and place the numbers
        level = new int[levelWidth, levelHeight];

        foreach(var tile in tiles) {
            Vector2Int tilePos = Convert.V3ToV2Int(tile.transform.position)-offset;
            if (tilePos == Vector2Int.zero || tilePos == topRightIndex) continue;
            level[tilePos.x, tilePos.y] = 1;
        }
        foreach(var tile in startPortal) {
            Vector2Int tilePos = Convert.V3ToV2Int(tile.transform.position)-offset;
            level[tilePos.x, tilePos.y] = 8;
        }
        foreach(var tile in exitPortal) {
            Vector2Int tilePos = Convert.V3ToV2Int(tile.transform.position)-offset;
            level[tilePos.x, tilePos.y] = 9;
        }
    }

    private void AddWallsToArray(GameObject prefabParent = null)
    {
        Transform[] tiles = prefabParent != null ? prefabParent.transform.GetComponentsInChildren<Transform>().Where(x => x.gameObject.layer == LayerMask.NameToLayer("Wall")).ToArray()
                                                 : wallsHolder.transform.GetComponentsInChildren<Transform>().Where(x => x.gameObject.layer == LayerMask.NameToLayer("Wall")).ToArray();
        foreach (var tile in tiles) {
            if (tile == wallsHolder.transform) continue;
            Vector2Int tilePos = Convert.V3ToV2Int(tile.transform.position) - offset;
            level[tilePos.x, tilePos.y] = 2; // Change this to 2 later to differentiate?
            if(tilePos.x == -offset.x && tilePos.y == -offset.y)
                Debug.Log("0,0 removed as wall cause of object "+tile.name);
        }

    }

    #endregion



    internal void GotoNextLevel()
    {
        Debug.Log("Creating and going to next dungeon level");

        // Generate new Level array

        ResetLevel();
    }

    // SET OBJECTS FROM ARRAY
    #region array->objects REGION
    private void CreateLevelFromArray()
    {
        Debug.Log("REMOVING ALL OLD TILES");
        RemoveAllWalls();
        RemoveAllFloors();
        RemoveAllEnemies();
        RemoveAllItems();

        // Place artificaial start and end
        //PrintLevel();
            
        Debug.Log("");
        Debug.Log("");
        PrintLevelCode();
        Debug.Log("");

        /*
        int levelTypeToLoad = (Stats.Instance.DungeonLevel - 1)% LevelDataController.Instance.TotalLevels;
        Debug.Log("Loading Level ID "+levelTypeToLoad+" cause level = "+Stats.Instance.DungeonLevel);
        // Get Data for next level
        level = LevelDataController.Instance.GetLevelData(levelTypeToLoad);
        */

        // Creating Level

        Debug.Log("Level "+Stats.Instance.DungeonLevel+" generated difficulty = "+Stats.Instance.Difficulty);



        // Enemies placed
        PlaceEnemies(10,0);
        PlaceEnemies(10,1);


        //PrintLevel();

        offset = Vector2Int.zero;

        Debug.Log("CREATING ALL NEW TILES");
        
        PlacePortals();

        AddFloors();
        AddWalls();

        // Send back player to start position?
        //StartCoroutine(PlayerMovement.Instance.ReturnToStartPosition());

        Debug.Log("ALIGNING PLAYER");
        //PlayerMovement.Instance.ReturnToStartPosition();
        PlayerMovement.Instance.Reset();
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
        Debug.Log("Placed All Enemies With "+(150-triesAllowed)+" failed tries.");
    }

    private void PlacePortals()
    {
        // Method that Finds all of an index and replace these with floors = 1 and returns one of them at random

        // Player portal
        dungeonPortals[0].transform.position = FindPortalPosition(8);
        Debug.Log("Entry set to "+ dungeonPortals[0].transform.position);
        // Exit Portal
        dungeonPortals[1].transform.position = FindPortalPosition(9);
        Debug.Log("Exit set to "+ dungeonPortals[1].transform.position);
    }

    private Vector3 FindPortalPosition(int index)
    {
        List<Vector2Int> positions = new List<Vector2Int>();
        // finds all of this type and set them to floor tile but sending back one
        for (int i = 0; i < level.GetLength(0); i++) {
            for (int j = 0; j < level.GetLength(1); j++) {
                if (level[i, j] != index) continue;
                level[i, j] = 1; // Overwrite with floortile for next steps
                positions.Add(new Vector2Int(i, j));
            }
        }
        
        return positions.Count > 0 ? Convert.V2IntToV3(positions[UnityEngine.Random.Range(0,positions.Count)]) : new Vector3();
    }

    // Adding tiles
    private void AddFloors() => GenerateChildren(floorHolder, floorPrefabs[currentBiome],TileType.Floor);
    private void AddWalls() => GenerateChildren(wallsHolder, wallPrefabs[currentBiome],TileType.Wall);
        
    
    // Removing tiles
    private void RemoveAllWalls() => DestroyAllChildren(wallsHolder);
    private void RemoveAllFloors() => DestroyAllChildren(floorHolder);
    private void RemoveAllEnemies() => DestroyAllChildren(enemyHolder);
    private void RemoveAllItems() => DestroyAllChildren(itemsHolder);


    private void GenerateChildren(Transform holder, GameObject prefab, TileType type)
    {
        float yPos = holder.position.y;
        int amt = 0;
        //Debug.Log("Generating tiles for "+holder.name+" using prefab "+prefab.name+" TiletypeMask: "+ System.Convert.ToString((int)type, 2).PadLeft(8,'0'));
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
        //Debug.Log("Created "+amt+" items.");
    }

    private void DestroyAllChildren(Transform holder)
    {
        //Debug.Log("Destroy children");
        int amt = 0;    
        Transform[] items = holder.GetComponentsInChildren<Transform>();
        foreach (Transform item in items) {
            if (item == holder) continue;
            Destroy(item.gameObject);
            amt++;
        }
        //Debug.Log("Destroyed "+amt+" children of "+holder.name+".");
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
        //Debug.Log("** Trying to find a path from "+from+" to "+to);
        PrintLevel();



        // Make sure the positions are normalized
        from -= offset;
        to -= offset;

        Debug.Log("** From "+from+" to "+to);

        int[,] levelCopy = (int[,])level.Clone();

        // Add Enemies to this copy

        levelCopy = AddEnemiesToLevel(levelCopy);

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
                //Debug.Log("** Target Found at " + current);
                //LogWalk(walk);
                return walk;
            }

            // Add current step
            List<Vector2Int> newWalk = new List<Vector2Int>(walk);
            newWalk.Add(current+offset);

            // Check all neighbors that are walkable
            List<Vector2Int> neighbors = GetNeighbors(current);
            //Debug.Log("** Found "+neighbors.Count+" neighbors");
            neighbors.Sort((a, b) => (Mathf.Abs(a.x - target.x) + Mathf.Abs(a.y - target.y)).CompareTo(Mathf.Abs(b.x - target.x) + Mathf.Abs(b.y - target.y)));

            // Go to all
            foreach (Vector2Int neighbor in neighbors) {
                List<Vector2Int> ans = WalkPath(neighbor, target, newWalk, cost + 1);
                if (ans.Count > 0) {
                    return ans;
                }
            }
            //Debug.Log("** No Path found");
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

    private int[,] AddEnemiesToLevel(int[,] levelCopy)
    {
        EnemyController[] enemies = enemyHolder.gameObject.GetComponentsInChildren<EnemyController>();

        //EnemyController[] enemies = enemyHolder.GetComponentsInChildren<Transform>().Where(x => x.gameObject.GetComponent<EnemyController>() != null).Select(x => x.gameObject.GetComponent<EnemyController>()).ToArray();


        foreach (EnemyController enemy in enemies) {
            Vector2Int pos = Convert.V3ToV2Int(enemy.transform.position);
            levelCopy[pos.x, pos.y] = 0;
        }
        return levelCopy;
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

    internal string GetLevelAsSegment(GameObject prefabParent,bool unset = false)
    {
        ReadLevelObjectsIntoArray(prefabParent,unset);
        return PrintLevelCode(false);
    }
    internal string GetLevelAsSegment()
    {
        ReadLevelObjectsIntoArray();
        return PrintLevelCode(false);
    }
}
