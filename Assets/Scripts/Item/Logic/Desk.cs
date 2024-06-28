using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Desk : Item
{

    public GameObject item;
    public bool isTalking;
    public GameObject talkingToWaiter;
    public HistoryInstruction instruction;

    void Awake()
    {
        visibleRole = RestaurantRole.Waiter;
    }

    public override void InitName()
    {
        itemName = "desk";
    }

    void Update()
    {
        
    }

    public override void PutInto(GameObject item = null, HistoryInstruction instruction = null)
    {
        if (this.item != null && this.item.GetComponent<Plate>() != null && this.item?.GetComponent<Plate>().food == null && item?.GetComponent<Food>() != null) {
            this.item.GetComponent<Plate>().PutInto(item);
            return;
        }
        if (item != null && this.item == null) {
            this.item = item;
            this.item.GetComponentInChildren<Item>().visibleRole = RestaurantRole.Waiter;
            this.item.transform.position = transform.position;
            this.item.GetComponent<Item>().ispicked = false;
            base.PutInto(this.item);
        }
        else if (item != null && this.item != null) {
            this.item.transform.position = item.transform.position;
            this.item.GetComponent<Item>().physicalParent = null;
            this.item.GetComponent<Item>().visibleRole = RestaurantRole.Common;
            this.item = item;
            this.item.GetComponentInChildren<Item>().visibleRole = RestaurantRole.Waiter;
            this.item.transform.position = transform.position;
            this.item.GetComponent<Item>().ispicked = false;
            base.PutInto(this.item);
        }
    }

    public override void TakeOut(GameObject item = null, GameObject npc = null, string insideItemName = null, HistoryInstruction instruction = null)
    {
        ItemPickUp itemPickUp = npc?.GetComponentInChildren<ItemPickUp>();
        if (itemPickUp == null) {
            return;
        }
        // itemPickUp.ExchangeItem(this.item);
        itemPickUp.PickUpTarget(this.item);
        // this.item.GetComponent<Item>().physicalParent = null;
        // this.item = item;
        if (this.item != null) this.item.transform.position = transform.position;
    }

    public override string GetMessage()
    {
        string mes = itemName;
        if (item == null) {
            mes += " (empty)";
        }
        else {
            if (item.GetComponent<Item>() != null) {
                mes += " (with " + item.GetComponent<Item>().GetMessage() + ")";
            }
        }
        return mes;
    }

    public override void ItemBePickedUp()
    {
        if (item == null || item.GetComponent<Item>() == null) return;
        item.GetComponent<Item>().physicalParent = null;
        item = null;
    }
}
