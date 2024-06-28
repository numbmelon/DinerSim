using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    public string itemName;
    public bool pickable;
    public bool interactable;
    public int interactPriority;
    public bool ispicked;
    public RestaurantRole visibleRole;
    public GameObject physicalParent;
    void Start()
    {
        
    }

    protected virtual void OnEnable()
    {
        StartCoroutine(InitItemCoroutine());
    }

    IEnumerator InitItemCoroutine()
    {
        while (ItemManager.Instance == null)
        {
            yield return null;
        }
        InitName();
        ItemManager.Instance.InitItem(this);
    }

    void Update()
    {
        
    }

    virtual public bool IsMeetInteractionCondition(Item item)
    {
        return true;
    }

    virtual public void Interact(Item item = null) 
    {
        
    }

    void InitItem()
    {
        InitName();
        ItemManager.Instance.InitItem(this);
    }

    virtual public void InitName()
    {
        itemName = "item";
    }

    virtual public string GetMessage()
    {
        return itemName;
    }

    virtual public void PutInto(GameObject item=null, HistoryInstruction instruction=null)
    {
        // TODO: check if there is a bug ?
        if (item?.GetComponent<Item>() != null) {
            item.GetComponent<Item>().physicalParent = this.gameObject;
        }
    }

    virtual public void TakeOut(GameObject item=null, GameObject npc=null, string insideItemName=null, HistoryInstruction instruction=null)
    {
        if (instruction != null) {
            instruction.status = InstructionStatus.Failed;
            instruction.message = "you can not use [TakeOut] command for this item, check the type of this item";
        }
    }

    virtual public void Open()
    {

    }

    virtual public void Close()
    {
        
    }

    virtual public void Season(Food food, string flavor, HistoryInstruction instruction=null)
    {
        if (instruction != null) {
            instruction.status = InstructionStatus.Failed;
            instruction.message = "you could only use the [Season] command for a 'seasoning_station'";
        }
    }

    virtual public IEnumerator Cut(HistoryInstruction instruction=null, Action callback = null)
    {
        if (instruction != null) {
            instruction.status = InstructionStatus.Failed;
            instruction.message = "you could only use the [Cut] command for a 'chopping_board'";
        }
        yield return null;
    }

    virtual public void ItemBePickedUp()
    {

    }
}
