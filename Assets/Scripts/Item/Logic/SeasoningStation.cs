using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeasoningStation : Item
{
    // Start is called before the first frame update
    void Awake()
    {
        visibleRole = RestaurantRole.Cook;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override string GetMessage()
    {
        return base.GetMessage();
    }

    public override void InitName()
    {
        itemName = "seasoning_station";
    }

    public override void Season(Food food, string flavor, HistoryInstruction instruction=null)
    {
        if (food == null) {
            if (instruction != null) {
                instruction.status = InstructionStatus.Failed;
                instruction.message = "you could only seaon when you hold a food item or a plate item with food";
            }
            return;
        }
        bool flavorRes = food.foodProperty.Season(flavor);
        if (!flavorRes && instruction != null) {
            instruction.status = InstructionStatus.Failed;
            instruction.message = "invaild flavor option '" + flavor + "', using 'spicy', 'salty', 'sour' or 'sweet'";
        }
    }
}
