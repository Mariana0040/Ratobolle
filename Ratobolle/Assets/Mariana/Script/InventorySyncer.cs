using UnityEngine;

// Garante que este script s� funcione se o PlayerInventoryManager estiver presente.
[RequireComponent(typeof(PlayerInventoryManager))]
public class InventorySyncer : MonoBehaviour
{
    private PlayerInventoryManager visualInventory;

    void Awake()
    {
        // Pega a refer�ncia do seu script de invent�rio visual.
        visualInventory = GetComponent<PlayerInventoryManager>();

        // --- ETAPA DE CARREGAMENTO (LOAD) ---
        // Pega todos os dados guardados no "c�rebro imortal".
        var savedData = InventoryData.GetAllIngredientCounts();

        // Para cada item que estava salvo...
        foreach (var item in savedData)
        {
            string ingredientName = item.Key;
            int quantity = item.Value;

            // ...adiciona ele ao seu invent�rio visual.
            visualInventory.AddIngredientToInventory(ingredientName, quantity);
        }
    }

    void OnDestroy()
    {
        // --- ETAPA DE SALVAMENTO (SAVE) ---
        // Este m�todo � chamado um pouco antes de a cena ser recarregada e o objeto destru�do.

        // Para cada slot visual configurado no seu invent�rio...
        foreach (var slot in visualInventory.allInventorySlots)
        {
            // ...pegamos a quantidade atual do invent�rio visual...
            int currentCount = visualInventory.GetIngredientCount(slot.targetIngredientName);

            // ...e salvamos no "c�rebro imortal".
            InventoryData.SetIngredientCount(slot.targetIngredientName, currentCount);
        }

        Debug.Log("Dados do invent�rio salvos no c�rebro de dados antes de recarregar a cena!");
    }
}