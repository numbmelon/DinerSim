using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MapData_SO", menuName = "Map/MapData")]
public class MapData_SO : ScriptableObject
{
    public List<TileProperty> tileProperties;

    [Header("Map Information")]
    public int gridWidth;
    public int gridHeight;
    [Header("Bottom Left Coordinates")]
    public int originX;
    public int originY;
}
