using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Food : Item
{
    public Plate plate;
    public FoodProperty foodProperty;
    public bool isPerfect;


    void Awake()
    {
        visibleRole = RestaurantRole.Cook;
    }

    void Update()
    {
        pickable = plate == null;
    }

    public override void InitName()
    {
        itemName = foodProperty.foodName.ToString();
    }

    public override string GetMessage()
    {
        string mes = itemName;
        string extraMes = "";
        List<string> propertyList = new();
        if (plate != null) return "";
        if (foodProperty.isCut == true && foodProperty.isCooked == false) propertyList.Add("cut");
        if (foodProperty.spicy) propertyList.Add("spicy");
        if (foodProperty.sweet) propertyList.Add("sweet");
        if (foodProperty.sour) propertyList.Add("sour");
        if (foodProperty.salty) propertyList.Add("salty");

        if (ispicked) extraMes += "unpickable";

        if (propertyList.Count > 0) {
            if (extraMes != "") extraMes += ", ";
            extraMes += string.Join(", ", propertyList);
        }
        if (extraMes != "") mes += " (" + extraMes +  ")";
        return mes;
    }

    public string GetFoodMessage()
    {
        string mes = itemName;
        List<string> propertyList = new();
        // if (plate != null) return "";
        if (foodProperty.isCut == true) propertyList.Add("cut");
        if (foodProperty.spicy) propertyList.Add("spicy");
        if (foodProperty.sweet) propertyList.Add("sweet");
        if (foodProperty.sour) propertyList.Add("sour");
        if (foodProperty.salty) propertyList.Add("salty");
        if (propertyList.Count > 0) {
            mes += " (" + string.Join(", ", propertyList) + ")";
        }
        return mes;
    }

}
