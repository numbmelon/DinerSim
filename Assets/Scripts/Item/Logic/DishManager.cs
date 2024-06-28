using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DishManager : Singleton<DishManager>
{
    public RecipeInfo_SO recipeInfo;
    private Dictionary<string, int> recipeIDs = new();
    public List<FoodProperty> foodSprite;

    protected override void Awake()
    {
        base.Awake();
        int nowRecipeID = 0;
        foreach (Recipe recipe in recipeInfo.recipes) {
            recipeIDs.Add(recipe.dishName, nowRecipeID++);
        }
    }

    void Start()
    {

    }

    public void ChangeFoodPrefab(GameObject item)
    {
        if (item.GetComponent<Food>() == null) {
            return;
        }
        foreach (FoodProperty foodProperty in foodSprite) {
            if (foodProperty.EqualBaseProperty(item.GetComponent<Food>().foodProperty)) {
                item.GetComponentInChildren<SpriteRenderer>().sprite = foodProperty.sprite;
                break;
            }
        }
    }


    public int GetRecipeID(string dishName) 
    {
        return recipeIDs.GetValueOrDefault(dishName, -1);
    }


    public Recipe GetRecipe(string dishName) 
    {
        int recipeID = GetRecipeID(dishName);
        if (recipeID == -1) {
            return null;
        }
        return recipeInfo.recipes[recipeID];
    }


    public Recipe MatchRecipe(List<FoodProperty> givenFoodList) 
    {
        foreach (Recipe recipe in recipeInfo.recipes) {
            List<FoodProperty> foodProperties = recipe.ingredients;
            if (AreIngredientsMatch(givenFoodList, foodProperties)) {
                return recipe;
            }
        }
        return null;
    }


    public bool AreIngredientsMatch(List<FoodProperty> recipeIngredients, List<FoodProperty> providedIngredients)
    {
        if (recipeIngredients.Count != providedIngredients.Count) {
            return false;
        }
        var sortedList1 = recipeIngredients.OrderBy(x => x.foodName).ToList();
        var sortedList2 = providedIngredients.OrderBy(x => x.foodName).ToList();

        for (int i = 0; i < sortedList1.Count; i++)
        {
            if (!sortedList1[i].EqualBaseProperty(sortedList2[i]))
                return false;
        }
        return true;
    }

    public bool AreIngredientsPerfect(List<FoodProperty> recipeIngredients, List<FoodProperty> providedIngredients)
    {
        if (recipeIngredients.Count != providedIngredients.Count) {
            return false;
        }
        var sortedList1 = recipeIngredients.OrderBy(x => x.foodName).ToList();
        var sortedList2 = providedIngredients.OrderBy(x => x.foodName).ToList();

        for (int i = 0; i < sortedList1.Count; i++)
        {
            if (!sortedList1[i].EqualProperty(sortedList2[i]))
                return false;
        }
        return true;
    }


    public string GetRandomDishName()
    {
        List<string> keys = new List<string>(recipeIDs.Keys);
        if (keys.Count == 0) {
            return null;
        }
        int randomIndex = UnityEngine.Random.Range(0, keys.Count);
        return keys[randomIndex];
    }


    public string GetLevelRandomDishName()
    {
        List<string> keys = new List<string>();
        foreach (Recipe recipe in recipeInfo.recipes) {
            keys.Add(recipe.dishName);
        }
        if (keys.Count == 0) {
            return null;
        }
        int dishNum = ChallengeLevelManager.Instance.GetLevelDishNum();
        if (dishNum == -1) {
            int randomIndex = UnityEngine.Random.Range(0, keys.Count);
            return keys[randomIndex];
        }
        else {
            dishNum = Math.Min(dishNum, keys.Count);
            int randomIndex = UnityEngine.Random.Range(0, dishNum);
            return keys[randomIndex];
        }
    }
}
