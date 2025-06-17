using UnityEngine;

[CreateAssetMenu(fileName = "Segment", menuName = "SegmentData")]
public class SegmentData : BaseLevelData
{
    [Header("Prefab to use for level")]
    public GameObject SegmentPrefab;
    [HideInInspector] public int[,] Level => ParseLevel(LevelString);



    private static int[,] ParseLevel(string data)
    {
        string[] lines = data.Replace("[","").Replace("]","").Trim().Split('\n'); // Split lines
        int rows = lines.Length;
        int cols = lines[0].Split(',').Length;
        
        int[,] level = new int[cols, rows];

        for (int i = 0; i < rows; i++) {
            string[] numbers = lines[i].Trim().Split(',');
            for (int j = 0; j < cols; j++) {
                level[j, rows-1-i] = int.Parse(numbers[j].Trim());
            }
        }
        return level;
    }

    [ContextMenu("ReadInWorldAsSegmentData")]
    public void ReadInWorldAsSegmentData()
    {
        Debug.Log("Reading new Segment data from world.");
        if(SegmentPrefab != null)
            LevelString = FindFirstObjectByType<LevelCreator>().GetLevelAsSegment(SegmentPrefab);
        else
            LevelString = FindFirstObjectByType<LevelCreator>().GetLevelAsSegment();
    }


}
