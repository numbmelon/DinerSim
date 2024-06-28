using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Oven : Item
{
    private Animator animator;
    public bool isOpen;


    void Awake()
    {
        animator = GetComponent<Animator>();
        visibleRole = RestaurantRole.Cook;
    }

    protected override void OnEnable() {
        base.OnEnable();
        isOpen = false;
    }

    void Update()
    {
        animator.SetBool("isOn", isOpen);
    }

    public void OpenOven()
    {
        isOpen = true;
        animator.SetBool("isOn", true);
    }

    public void CloseOven()
    {
        isOpen = false;
        animator.SetBool("isOn", false);
    }

    public override void Interact(Item item = null)
    {
        isOpen = !isOpen;
    }

    public override void InitName()
    {
        itemName = "oven";
    }

    public override string GetMessage()
    {
        string mes = itemName;
        if (isOpen) {
            mes += " (opened)";
        }
        else {
            mes += " (closed)";
        }
        return mes;
    }

    public override void Open()
    {
        OpenOven();
    }

    public override void Close()
    {
        CloseOven();
    }


    public override void TakeOut(GameObject item=null, GameObject npc=null, string insideItemName=null, HistoryInstruction instruction=null)
    {
        if (instruction != null) {
            instruction.status = InstructionStatus.Failed;
            instruction.message = "can not use [Takout] command for oven item, do you mean '[Takeout] pan'?";
        }
        return;
    }

    public override void PutInto(GameObject item = null, HistoryInstruction instruction = null)
    {
        if (instruction != null) {
            instruction.status = InstructionStatus.Failed;
            instruction.message = "illegal [PutInto] command for a oven instace, do you mean '[PutInto] pan'?";
        }
    }

}
