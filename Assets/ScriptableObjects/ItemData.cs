using System;
using UnityEngine;

[CreateAssetMenu(fileName = "Item", menuName = "Item")]
public class ItemData : ScriptableObject
{
    public string ItemName;
    public EquipmentType EquipmentType;
    public Sprite Picture;

    [Range(0,20)]
    public int Strength = 0;
    [Range(0,20)]
    public int Stamina = 0;
    [Range(0,20)]
    public int Intelligence = 0;
    [Range(0,20)]
    public int Willpower = 0;

    internal int[] GetStatsArray() => new int[] { Strength, Stamina, Intelligence, Willpower };
}
