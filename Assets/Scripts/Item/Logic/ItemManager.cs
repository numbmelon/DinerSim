using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ItemManager : Singleton<ItemManager>
{
    Dictionary<string, int> itemIndexDeliverDict = new();
    public Dictionary<string, GameObject> itemDict = new();
    public List<Item> itemList = new();
    // public List<Item> presetItemList = new(); 

    void Start()
    {
        EventHandler.BeforeEnterNextLevelEvent += ClearItem;
        EventHandler.BeginChallengeEvent += ClearItem;
        EventHandler.BeforeNextChallengeDayEvent += ClearItem;
    }

    void Update()
    {

    }

    public void ClearItem()
    {
        itemIndexDeliverDict = new();
        itemDict = new();
        itemList = new();
        // presetItemList = new();
        GameObject presetItems = GameObject.Find("PresetItems");
        presetItems.SetActive(false);
        presetItems.SetActive(true);
        GameObject itemsParent = GameObject.Find("Items");
        if (itemsParent != null) {
            foreach (Transform child in itemsParent.transform)
            {
                Destroy(child.gameObject);
            }
        }
    }

    public void InitItem(Item item)
    {
        if (itemIndexDeliverDict.GetValueOrDefault(item.itemName, -1) == -1) {
            itemIndexDeliverDict.Add(item.itemName, 1);
        }
        else {
            int nowIdx = itemIndexDeliverDict[item.itemName];
            itemIndexDeliverDict[item.itemName]++;
            item.itemName += "_" + nowIdx.ToString();
        }

        itemList.Add(item);
        itemDict.Add(item.itemName, item.gameObject);
    }

    public string GetAllItemMessage(RestaurantRole npcRole)
    {
        List<string> mesList = new();
        string mes;
        for (int i = 0; i < itemList.Count; i++) {
            if (itemList[i] == null) {
                itemList.RemoveAt(i);
                i--;
                continue;
            }
            if (itemList[i].GetMessage() == "" || !IsVisible(npcRole, itemList[i].visibleRole)) {
                continue;
            }
            Vector2Int itemGridPos = (Vector2Int)GridMapManager.Instance.grid.WorldToCell(itemList[i].transform.position);


            mesList.Add(itemList[i].GetMessage() + " {location: " + itemGridPos.ToString() + "}");
        }
        mes = String.Join("\n", mesList.OrderBy(x=>x).ToList());
        // Debug.Log(mes);
        return mes;
    }

    private bool IsVisible(RestaurantRole npcRole, RestaurantRole itemVisibility) 
    {
        if (itemVisibility == RestaurantRole.Common || npcRole == RestaurantRole.Common) {
            return true;
        }
        if (npcRole == itemVisibility) {
            return true;
        }
        return false;
    }
}
