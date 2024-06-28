using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlateStack : Item
{
    public GameObject platePrefab;

    void Awake()
    {
        visibleRole = RestaurantRole.Common;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void GivePlate(GameObject npc)
    {
        if (npc == null) return;
        ItemPickUp itemPickup = npc.GetComponentInChildren<ItemPickUp>();
        if (itemPickup != null) {
            GameObject newPlate = Instantiate(platePrefab, itemPickup.gameObject.transform.position, Quaternion.identity);
            itemPickup.PickUpTarget(newPlate);
            newPlate.transform.SetParent(GameObject.Find("Items").transform);
            if (npc.GetComponent<NPCInnerVoice>().npcInfo.npcRole == RestaurantRole.Waiter) {
                newPlate.GetComponent<Item>().visibleRole = RestaurantRole.Common;
            }
        }
    }

    public override void InitName()
    {
        itemName = "plate_stack";
    }


    public override string GetMessage()
    {
        return base.GetMessage();
    }

    public override void PutInto(GameObject item = null, HistoryInstruction instruction = null)
    {
        if (instruction != null) {
            instruction.status = InstructionStatus.Failed;
            instruction.message = "illegal [PutInto] command for a plate_stack instace, do you mean '[TakeOut] plate_stack'?";
        }
    }


    public override void TakeOut(GameObject item = null, GameObject npc = null, string insideItemName = null, HistoryInstruction instruction = null)
    {
        GivePlate(npc);
    }
}
