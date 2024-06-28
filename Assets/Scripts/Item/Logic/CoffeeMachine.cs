using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoffeeMachine : Item
{
    public GameObject cup;
    void Awake()
    {
        visibleRole = RestaurantRole.Waiter;
    }


    void Update()
    {
        
    }


    public override void InitName()
    {
        itemName = "coffee_machine";
    }

    public override string GetMessage()
    {

        return itemName + " (you can take infinite cups filled with coffee from it)";
    }

    public override void TakeOut(GameObject item=null, GameObject npc=null, string insideItemName=null, HistoryInstruction instruction=null)
    {
        if (npc == null) return;
        ItemPickUp itemPickUp = npc.GetComponentInChildren<ItemPickUp>();
        GameObject newCup = Instantiate(cup, itemPickUp.gameObject.transform.position, Quaternion.identity);
        newCup.transform.SetParent(GameObject.Find("Items").transform);
        itemPickUp.ExchangeItem(newCup);
        if (TaskManager.Instance.taskLevel > 2 || TaskManager.Instance.gameMode == GameMode.Challenge) CoinManager.Instance.CoinNum -= 1 + Settings.extraBeverageCost;
    }
}
