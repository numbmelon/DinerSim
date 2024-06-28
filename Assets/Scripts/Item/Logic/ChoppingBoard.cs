using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChoppingBoard : Item
{
    public GameObject item;
    void Awake()
    {
        visibleRole = RestaurantRole.Cook;
    }

    void Start()
    {
        
    }

    void Update()
    {
        
    }
    public override IEnumerator Cut(HistoryInstruction instruction = null, Action callback = null)
    {
        // StartCoroutine
        if (item == null) {
            if (instruction != null) {
                instruction.status = InstructionStatus.Failed;
                instruction.message = "the chopping board is empty, nothing to cut";
            }
            callback?.Invoke();
            yield break;
        }
        Food food = item.GetComponent<Food>();
        if (food == null || !food.foodProperty.cuttable) {
            if (instruction != null) {
                instruction.status = InstructionStatus.Failed;
                instruction.message = "the item on the chopping board is not food that can be cut";
            }
            callback?.Invoke();
            yield break;
        }

        if (food.foodProperty.isCut) {
            if (instruction != null) {
                instruction.status = InstructionStatus.Failed;
                instruction.message = "the food is already cut";
            }
            callback?.Invoke();
            yield break;
        }
        float tikTime = 0f;
        float leftTime = Settings.cuttingFoodTime;
        while (leftTime > 0) {
            if (TimeManager.Instance.IsPaused()) yield return null;
            tikTime += Time.deltaTime;
            if (tikTime >= Settings.secondThreshold) {
                tikTime = 0;
                leftTime -= 1f;
            }
            yield return null;
        }
        
        // TODO: change prefabs
        item.GetComponent<Food>().foodProperty.isCut = true;
        item.GetComponent<Food>().isPerfect = true;
        DishManager.Instance.ChangeFoodPrefab(item);
        // Debug.Log(11111111);

        callback?.Invoke();

        yield return null;
    }

    // public IEnumerator CutFoodCoroutine(HistoryInstruction instruction = null)
    // {
    //     yield return null;
    // }

    public override void PutInto(GameObject item = null, HistoryInstruction instruction = null)
    {
        // TODO: complete a method that put the food in the plate into something
        Plate plate = item?.GetComponentInChildren<Plate>();
        if (plate != null) {
            Food food = plate.food;
            if (food != null) {
                food.plate = null;
                plate.food = null;
                item = food.gameObject;
            }
            
        } 
        if (item != null && this.item == null) {
            this.item = item;
            this.item.transform.position = transform.position;
            this.item.GetComponent<Item>().ispicked = false;
            base.PutInto(this.item);
        }
        else if (item != null && this.item != null) {
            this.item.transform.position = item.transform.position;
            this.item.GetComponent<Item>().physicalParent = null;
            this.item = item;
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
        if (this.item != null) this.item.transform.position = transform.position;
    }

    public override void InitName()
    {
        itemName = "chopping_board";
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
