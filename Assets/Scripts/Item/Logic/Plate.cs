using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Plate : Item
{
    public Food food;


    void Awake()
    {
        visibleRole = RestaurantRole.Cook;
    }

    void Update()
    {
        if (food != null) {
            food.transform.position = transform.position;
        }        
    }

    public void PutFoodIntoPlate(Food food) 
    {
        if (food.plate != null) {
            food.plate.food = null;
        }
        food.plate = this;
        this.food = food;
    }

    public override void InitName()
    {
        itemName = "plate";
    }

    public override string GetMessage()
    {
        string mes = itemName;
        string extraMes = "";
        if (ispicked) extraMes += "has been picked";
        if (food != null) {
            if (extraMes != "") extraMes += ", ";
            extraMes += food.GetFoodMessage();
        }
        else {
            if (extraMes != "") extraMes += ", ";
            extraMes += "empty";
        }

        return mes + " (" + extraMes + ")";
    }

    public override void PutInto(GameObject item = null, HistoryInstruction instruction = null)
    {
        if (food != null) {
            if (instruction != null) {
                instruction.status = InstructionStatus.Failed;
                instruction.message = "the target plate is not empty";
            }
        }
        Food item_food = item.GetComponent<Food>();
        if (item_food == null) {
            if (instruction != null) {
                instruction.status = InstructionStatus.Failed;
                instruction.message = "only the food item could be put into a plate";
            }
        }
        else {
            PutFoodIntoPlate(item_food);
        }
    }

    public void OccupiedByCustomer()
    {
        // remove food, while food still has "food.plate = this"
        food.gameObject.transform.position = gameObject.transform.position;
        food = null;
        
        // can not pick while eating
        pickable = false;
    }

    private void OnDestroy() 
    {
        if (food != null) {
            Destroy(food.gameObject);
        }    
    }

}
