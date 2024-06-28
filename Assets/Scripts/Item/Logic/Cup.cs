using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Cup : Item
{
    private Animator animator;
    public bool isEmpty;

    void Awake()
    {
        animator = GetComponent<Animator>();
        visibleRole = RestaurantRole.Waiter;
        isEmpty = false;
    }


    void Update()
    {
        animator.SetBool("isEmpty", isEmpty);
    }

    public override void InitName()
    {
        itemName = "cup";
    }

    public override string GetMessage()
    {
        string mes = itemName;
        string extraMes;
        if (isEmpty) {
            extraMes = "empty";
        }
        else {
            extraMes = "filled with coffee";
        }

        return mes + " (" + extraMes + ")";
    }
}
