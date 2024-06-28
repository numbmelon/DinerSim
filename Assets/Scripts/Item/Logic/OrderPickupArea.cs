using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrderPickupArea : Item
{
    public GameObject item;

    
    void Awake()
    {
        visibleRole = RestaurantRole.Common;
    }

    public override void InitName()
    {
        itemName = "order_pickup_area";
    }

    public override void PutInto(GameObject item = null, HistoryInstruction instruction = null)
    {
        if (item != null && this.item == null) {
            this.item = item;
            this.item.GetComponentInChildren<Item>().visibleRole = RestaurantRole.Waiter;
            this.item.transform.position = transform.position;
            this.item.GetComponent<Item>().ispicked = false;
            base.PutInto(this.item);
        }
        else if (item != null && this.item != null) {
            this.item.GetComponentInChildren<Item>().visibleRole = RestaurantRole.Common;
            this.item.transform.position = item.transform.position;
            this.item.GetComponent<Item>().ispicked = false;
            this.item.GetComponent<Item>().physicalParent = null;
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
        if (itemPickUp == null) return;
        if (this.item == null) {
            if (instruction != null) {
                instruction.status = InstructionStatus.Failed;
                instruction.message = "this area is empty";
            }
            return;
        }
        // itemPickUp.ExchangeItem(this.item);
        itemPickUp.PickUpTarget(this.item);
        // this.item.GetComponent<Item>().physicalParent = null;
        // this.item = null;
        // this.item = item;
        // if (item != null) {
        //     this.item.transform.position = transform.position;
        //     this.item.GetComponentInChildren<Item>().visibleRole = npc.GetComponent<NPCInnerVoice>().npcInfo.npcRole;
        // }
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
