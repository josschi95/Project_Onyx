using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Recipe Database", menuName = "Databases/Recipe Database")]
public class RecipeDatabase : ScriptableObject
{
    public Recipe[] recipes;
    public Dictionary<int, Recipe> GetRecipe = new Dictionary<int, Recipe>();

    public void AssignIDs()
    {
        GetRecipe = new Dictionary<int, Recipe>();
        for (int i = 0; i < recipes.Length; i++)
        {
            GetRecipe.Add(i, recipes[i]);
            recipes[i].recipeID = i;
        }
    }
}
