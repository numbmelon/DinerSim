using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemDrop : MonoBehaviour
{
    private ItemPickUp itemPickUp;

    void Awake()
    {
        itemPickUp = GetComponent<ItemPickUp>();
    }

    public void DropItem()
    {
        if (itemPickUp == null || itemPickUp.holdItem == null) return;
        Vector3Int gridPos = GridMapManager.Instance.grid.WorldToCell(transform.position);
        // Debug.Log(gridPos);
        Vector3Int dropPosition = GridMapManager.Instance.GetDropItemPosition(gridPos);
        // Debug.Log(dropPosition);
        Vector3 dropPositionGlobal = GridMapManager.Instance.grid.CellToWorld(dropPosition);
        dropPositionGlobal = new Vector3(dropPositionGlobal.x + Settings.gridCellSize / 2f, dropPositionGlobal.y + Settings.gridCellSize / 2f, 0);
        GameObject holdItem = itemPickUp.holdItem;
        itemPickUp.holdItem = null;
        holdItem.transform.position = dropPositionGlobal;
        holdItem.GetComponent<Item>().ispicked = false;
    }
}
