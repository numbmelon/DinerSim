using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridMapManager : Singleton<GridMapManager>
{
    [Header("Map Information")]
    public Grid grid;
    public MapData_SO mapData;
    private Dictionary<string, TileDetails> tileDetailsDict = new ();

    override protected void Awake()
    {
        base.Awake();
        InitTileDetailsDict(mapData);
    }

    private void Start()
    {
        
    }

    private void InitTileDetailsDict(MapData_SO mapData) 
    {
        foreach (TileProperty tileProperty in mapData.tileProperties)
        {
            TileDetails tileDetails = new TileDetails
            {
                gridX = tileProperty.tileCoordinate.x,
                gridY = tileProperty.tileCoordinate.y
            };

            string key = tileDetails.gridX.ToString()  + "x" + tileDetails.gridY.ToString()  + "y"; // {gridX}x{gridY}y

            TileDetails tmpDetails = GetTileDetails(key);
            if (tmpDetails != null) {
                tileDetails = tmpDetails;
            }

            // TODO: more grid types
            switch (tileProperty.gridType) {
                case GridType.Obstacle:
                    tileDetails.isObstacle = true;
                    break;
                case GridType.CanDropItem:
                    tileDetails.isCanDropItem = true;
                    break;    
                default:
                    break;
            }

            if (tmpDetails != null) {
                tileDetailsDict[key] = tileDetails;
            }
            else {
                tileDetailsDict.Add(key, tileDetails);
            }

        } 
    }

    private TileDetails GetTileDetails(string key)
    {
        if (tileDetailsDict.ContainsKey(key)) {
            return tileDetailsDict[key];
        }
        return null;
    }

    public TileDetails GetTileDetailsByGridPos(Vector3Int gridPos) 
    {
        string key = gridPos.x.ToString() + "x" + gridPos.y.ToString()  + "y" ;
        return GetTileDetails(key);
    }

    public void AddItemToGridPos(Vector3Int gridPos, GameObject item)
    {
        string key = gridPos.x.ToString()  + "x" + gridPos.y.ToString()  + "y" ;
        if (GetTileDetails(key) != null) {
            tileDetailsDict[key].item = item;
        }
    }

    public bool GetGridDimensions(out Vector2Int gridDimensions, out Vector2Int gridOrigin)
    {
        gridDimensions = Vector2Int.zero;
        gridOrigin = Vector2Int.zero;

        gridDimensions.x = mapData.gridWidth;
        gridDimensions.y = mapData.gridHeight;

        gridOrigin.x = mapData.originX;
        gridOrigin.y = mapData.originY;

        return true;
    }

    public Vector3Int GetDropItemPosition(Vector3Int gridPos)
    {
        List<Vector3Int> positionList = new List<Vector3Int>
                                        {
                                            new Vector3Int(gridPos.x, gridPos.y),
                                            new Vector3Int(gridPos.x -1 , gridPos.y),
                                            new Vector3Int(gridPos.x + 1, gridPos.y),
                                            new Vector3Int(gridPos.x, gridPos.y - 1),
                                            new Vector3Int(gridPos.x, gridPos.y + 1)
                                        };
        foreach (Vector3Int position in positionList) {
            string key = position.x.ToString()  + "x" + position.y.ToString()  + "y";
            if (GetTileDetails(key) != null && GetTileDetails(key).isCanDropItem) {
                return position;
            }
        }
        return gridPos;
    }

    public Vector3Int GetReachablePosition(Vector3Int gridPos)
    {
        List<Vector3Int> positionList = new List<Vector3Int>
                                        {
                                            new Vector3Int(gridPos.x + 1 , gridPos.y),
                                            new Vector3Int(gridPos.x - 1, gridPos.y),
                                            new Vector3Int(gridPos.x, gridPos.y + 1),
                                            new Vector3Int(gridPos.x, gridPos.y - 1),
                                            new Vector3Int(gridPos.x, gridPos.y),
                                            new Vector3Int(gridPos.x + 1, gridPos.y - 1)
                                        };
        foreach (Vector3Int position in positionList) {
            string key = position.x.ToString()  + "x" + position.y.ToString()  + "y";
            if (GetTileDetails(key) == null || (GetTileDetails(key) != null && !GetTileDetails(key).isObstacle)) {
                return position;
            }
        }
        return gridPos;
    }

    public Vector3 GetWorldPosition(Vector3Int gridPos)
    {
        Vector3 worldPos = Instance.grid.CellToWorld(gridPos);
        return new Vector3(worldPos.x + Settings.gridCellSize / 2f, worldPos.y + Settings.gridCellSize / 2f);
    }
}
