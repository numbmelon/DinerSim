using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

[ExecuteInEditMode]
public class GridMap : MonoBehaviour
{
    public MapData_SO mapData;
    public GridType gridType;
    public Tilemap baseTilemap;
    private Tilemap currentTilemap;

    private void OnEnable() {
        if (!Application.IsPlaying(this)) {
            currentTilemap = baseTilemap == null ? GetComponent<Tilemap>() : baseTilemap;

            mapData?.tileProperties.Clear();
        }
    }

    private void OnDisable() {
        if (!Application.IsPlaying(this)) {
            currentTilemap = baseTilemap == null ? GetComponent<Tilemap>() : baseTilemap;

            UpdateTileProperties();
#if UNITY_EDITOR
            if (mapData != null) {
                EditorUtility.SetDirty(mapData);
            }
#endif
        }
    }

    private void UpdateTileProperties()
    {
        currentTilemap.CompressBounds();
        if (!Application.IsPlaying(this)) {
            if (mapData != null) {
                Vector3Int startPos = currentTilemap.cellBounds.min;  // bottom-left
                Vector3Int endPos = currentTilemap.cellBounds.max;  // top-right
                for (int x = startPos.x; x < endPos.x; x++) {
                    for (int y = startPos.y; y < endPos.y; y++) {
                        TileBase tile = currentTilemap.GetTile(new Vector3Int(x, y, 0));

                        if (tile != null) {
                            TileProperty newTile = new TileProperty {
                                tileCoordinate = new Vector2Int(x, y),
                                gridType = this.gridType,
                                boolTypeValue = true
                            };

                            mapData.tileProperties.Add(newTile);
                        }

                    }
                }
            }
        }
    }

}
