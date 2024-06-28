using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "RecipeInfo_SO", menuName = "Cooking/RecipeInfo_SO")]
public class RecipeInfo_SO : ScriptableObject
{
    public List<Recipe> recipes;
}
