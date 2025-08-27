using UnityEngine;
using System.Collections.Generic;

public static class InventoryData
{
    // O dicion�rio "imortal" que realmente guarda a contagem de ingredientes.
    private static Dictionary<string, int> persistentIngredientCounts = new Dictionary<string, int>();

    // Fun��o para registrar a contagem de um item.
    public static void SetIngredientCount(string ingredientName, int count)
    {
        if (persistentIngredientCounts.ContainsKey(ingredientName))
        {
            persistentIngredientCounts[ingredientName] = count;
        }
        else
        {
            persistentIngredientCounts.Add(ingredientName, count);
        }
    }

    // Fun��o para pegar a contagem de um item.
    public static int GetIngredientCount(string ingredientName)
    {
        persistentIngredientCounts.TryGetValue(ingredientName, out int count);
        return count;
    }

    // Fun��o para pegar TODOS os dados de uma vez.
    public static Dictionary<string, int> GetAllIngredientCounts()
    {
        return new Dictionary<string, int>(persistentIngredientCounts);
    }
}