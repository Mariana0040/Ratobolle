using UnityEngine;

// Garante que este script só funcione se o PlayerInventoryManager estiver presente.
[RequireComponent(typeof(PlayerInventoryManager))]
public class InventorySyncer : MonoBehaviour
{
    private PlayerInventoryManager visualInventory;

    void Awake()
    {
        // Pega a referência do seu script de inventário visual.
        visualInventory = GetComponent<PlayerInventoryManager>();

        // --- ETAPA DE CARREGAMENTO (LOAD) ---
        // Pega todos os dados guardados no "cérebro imortal".
        var savedData = InventoryData.GetAllIngredientCounts();

        // Para cada item que estava salvo...
        foreach (var item in savedData)
        {
            string ingredientName = item.Key;
            int quantity = item.Value;

            // ...adiciona ele ao seu inventário visual.
            visualInventory.AddIngredientToInventory(ingredientName, quantity);
        }
    }

    void OnDestroy()
    {
        // --- ETAPA DE SALVAMENTO (SAVE) ---
        // Este método é chamado um pouco antes de a cena ser recarregada e o objeto destruído.

        // Para cada slot visual configurado no seu inventário...
        foreach (var slot in visualInventory.allInventorySlots)
        {
            // ...pegamos a quantidade atual do inventário visual...
            int currentCount = visualInventory.GetIngredientCount(slot.targetIngredientName);

            // ...e salvamos no "cérebro imortal".
            InventoryData.SetIngredientCount(slot.targetIngredientName, currentCount);
        }

        Debug.Log("Dados do inventário salvos no cérebro de dados antes de recarregar a cena!");
    }
}