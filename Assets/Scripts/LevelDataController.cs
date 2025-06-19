using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class LevelDataController : MonoBehaviour
{
    [SerializeField] private LevelData[] leveldatas;
    
    [SerializeField] private SegmentData[] aSegments;
    [SerializeField] private SegmentData[] bSegments;
    [SerializeField] private SegmentData[] cSegments;
    [SerializeField] private SegmentData[] aBots;
    [SerializeField] private SegmentData[] bBots;
    [SerializeField] private SegmentData[] cBots;
    [SerializeField] private SegmentData[] aCaps;
    [SerializeField] private SegmentData[] bCaps;
    [SerializeField] private SegmentData[] cCaps;
    [SerializeField] private SegmentData[][] segments;

    private Vector2Int segmentSize = new Vector2Int(22,12);

    public static LevelDataController Instance { get; private set; }

    public int TotalLevels => leveldatas.Length;


    private void Awake()
    {
        if (Instance != null) {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        segments = new SegmentData[][]{aSegments,bSegments,cSegments,aCaps,bCaps,cCaps, aBots, bBots, cBots };
    }

    // Level data generator

    public int[,] GetRandomLevelData(int sectionsToUse = 5)
    {
        //Debug.Log("*** *** GetRandomLevelData *** ***");
        // Make sections at random and return the resulting Level

        // Get the sections dictionary to use
        Dictionary<Vector2Int, int> placedSections = PlaceAllSections(sectionsToUse);


        // Add cap-sections here?
        AddCapsToPlacedSections(ref placedSections);

        //Debug.Log("*** *** RandomLevelData  Created *** ***");

        (Vector2Int sectionsOffset, Vector2Int size) = GetSectionsOffetAndSize(placedSections);

        // Now place all into a new array
        int[,] ans = new int[size.x * segmentSize.x, size.y * segmentSize.y];

        // Place all these sections into one array        
        foreach (var section in placedSections) {
            Vector2Int pos = section.Key;
            int type = section.Value;
            int variation = UnityEngine.Random.Range(0, segments[type].Length);

            //Debug.Log("Placing section "+type+" variation "+variation+" at "+pos);
            SegmentData data = segments[type][variation];
            int[,] segmentArray = data.Level;

            int offsetX = (section.Key.x - sectionsOffset.x) * segmentSize.x;
            int offsetY = (section.Key.y - sectionsOffset.y) * segmentSize.y;

            for (int i = 0; i < segmentArray.GetLength(0); i++) {
                for (int j = 0; j < segmentArray.GetLength(1); j++) {
                    ans[i + offsetX, j + offsetY] = segmentArray[i, j];
                }
            }
        }
        
        int[,] ansInverted = new int[size.x * segmentSize.x, size.y * segmentSize.y];

        //Invert Y???
        for (int i = 0; i < ans.GetLength(0); i++) {
            for (int j = 0; j < ans.GetLength(1); j++) {
                ansInverted[i, ans.GetLength(1)-j-1] = ans[i, j];
            }
        }

        return ansInverted;
    }

    private void AddCapsToPlacedSections(ref Dictionary<Vector2Int, int> placedSections)
    {
        Dictionary<int,Vector2Int> bottomSections = new Dictionary<int,Vector2Int>();
        Dictionary<int,Vector2Int> topSections = new Dictionary<int,Vector2Int>();

        foreach(var pos in placedSections.Keys) {
            int xPos = pos.x;
            if (bottomSections.ContainsKey(xPos)) {
                // Placed Y max
                int yPos = bottomSections[xPos].y;
                if(pos.y > yPos) {
                    bottomSections[xPos] = pos;
                }
            }
            else {
                bottomSections[xPos] = pos;
            }
            if (topSections.ContainsKey(xPos)) {
                // Placed Y max
                int yPos = topSections[xPos].y;
                if(pos.y < yPos) {
                    topSections[xPos] = pos;
                }
            }
            else {
                topSections[xPos] = pos;
            }
        }

        int amt = 0;
        // Now add cap above all these
        foreach(var pos in bottomSections.Values) {
            Vector2Int newPos = pos + Vector2Int.up;
            placedSections[newPos] = placedSections[pos] + 3;
            amt++;
        }
        Debug.Log("Added "+amt+" Bottom Caps");
        amt = 0;
        // Now add bottoms above all these
        foreach(var pos in topSections.Values) {
            Vector2Int newPos = pos - Vector2Int.up;
            placedSections[newPos] = placedSections[pos] + 6;
            amt++;
        }
        Debug.Log("Added "+amt+" Top Caps");
    }

    private (Vector2Int sectionsOffset, Vector2Int size) GetSectionsOffetAndSize(Dictionary<Vector2Int, int> placedSections)
    {
        Vector2Int minCorner = Vector2Int.zero;
        Vector2Int maxCorner = Vector2Int.zero;
        // Finds Corners
        foreach (var section in placedSections.Keys) {
            if (section.x < minCorner.x) minCorner = new Vector2Int(section.x, minCorner.y);
            if (section.y < minCorner.y) minCorner = new Vector2Int(minCorner.x, section.y);
            if (section.x > maxCorner.x) maxCorner = new Vector2Int(section.x, maxCorner.y);
            if (section.y > maxCorner.y) maxCorner = new Vector2Int(maxCorner.x, section.y);
        }
        return (minCorner, maxCorner-minCorner+Vector2Int.one);
    }

    private Dictionary<Vector2Int, int> PlaceAllSections(int sectionsToUse)
    {
        // Start tile is 0,0
        Dictionary<Vector2Int,int> openPositions = new();

        // Randomize the start
        int segmentType = UnityEngine.Random.Range(0, 3);
        //Debug.Log("Starting with segment " + segmentType);

        Vector2Int pos = new Vector2Int(0, 0);

        openPositions.Add(pos, segmentType);

        Dictionary<Vector2Int, int> placedSections = new Dictionary<Vector2Int, int>();

        // Recursive step through all
        Step(pos, sectionsToUse);


        return placedSections;

        void Step(Vector2Int pos, int stepsLeft = 0)
        {
            if (stepsLeft <= 0) return;

            //Debug.Log("PLace Position "+pos+" in placed dictionary");
            PlaceRandomSection(pos);

            openPositions.Remove(pos);

            // Get all unused new directions from this tile
            //Debug.Log(" *** From position "+pos +" Type: " + placedSections[pos]);
            AddPositionIfFree(new Vector2Int(pos.x - 1, pos.y), (placedSections[pos] + 2) % 3); // Left -1
            //Debug.Log(" *** Left position ["+(pos.x - 1)+","+(pos.y)+"]" +" Type: " + ((placedSections[pos] + 2) % 3));
            AddPositionIfFree(new Vector2Int(pos.x + 1, pos.y), (placedSections[pos] + 1) % 3); // Right +1
            //Debug.Log(" *** Right position ["+(pos.x + 1)+","+(pos.y)+"]" +" Type: " + ((placedSections[pos] + 1) % 3));
            AddPositionIfFree(new Vector2Int(pos.x, pos.y - 1), (placedSections[pos] + 1) % 3); // Up -1
            AddPositionIfFree(new Vector2Int(pos.x, pos.y + 1), (placedSections[pos] + 2) % 3); // Down +1

            //Debug.Log("** Openpositions when stepsleft = "+stepsLeft+" = "+openPositions.Count);

            // Goto random free position
            int index = UnityEngine.Random.Range(0, openPositions.Keys.Count);
            //Debug.Log("Getting Next position "+index+" out of "+ openPositions.Keys.Count);
            Vector2Int nextPosition = openPositions.Keys.ToList()[index];
            //Debug.Log("Next position = "+nextPosition);

            //Debug.Log("** Successfully placed "+placedSections.Count);
            Step(nextPosition, stepsLeft-1);
        }

        void PlaceRandomSection(Vector2Int newPosition)
        {
            int segmentIndex = openPositions[newPosition];
            placedSections.Add(newPosition, segmentIndex);
            //Debug.Log("Adding "+newPosition+" to Placed size = "+placedSections.Count);
        }


        void AddPositionIfFree(Vector2Int newPosition, int type)
        {
            //Debug.Log("Checking to add neighbor "+newPosition);
            if (placedSections.ContainsKey(newPosition) || openPositions.ContainsKey(newPosition))
                return;
                        
            //Debug.Log("Added Neighbor - "+newPosition);
            openPositions.Add(newPosition,type);
            
        }
    }

    public int[,] GetLevelData(int index) => leveldatas[Math.Min(index, leveldatas.Length - 1)].Level;
}
