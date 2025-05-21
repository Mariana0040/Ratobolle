using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

// Uma pequena classe ou struct para ajudar a organizar os slots no Inspetor
[System.Serializable] // Isso faz com que apare�a no Inspetor
public class InventoryUISlot
{
    [Tooltip("O NOME EXATO do ingrediente que este slot deve exibir (ex: 'Queijo', 'Cebola'). Case-sensitive!")]
    public string targetIngredientName;
    [Tooltip("O componente Image para o �cone deste slot.")]
    public Image iconImage;
    [Tooltip("O componente TextMeshProUGUI para a quantidade deste slot.")]
    public TextMeshProUGUI quantityText;
}

public class PlayerInventoryManager : MonoBehaviour
{
    [Header("Configura��o dos Slots Visuais do Invent�rio")]
    [Tooltip("Configure cada slot visual aqui, definindo qual ingrediente ele representa.")]
    public List<InventoryUISlot> visualSlots = new List<InventoryUISlot>();

    // Dicion�rio para armazenar a CONTAGEM de cada ingrediente
    private Dictionary<string, int> ingredientCounts = new Dictionary<string, int>();

    void Start()
    {
        // Inicializa a UI no come�o
        UpdateAllVisualSlots();
    }

    // Fun��o chamada quando um ingrediente � coletado
    public void AddIngredientToInventory(string ingredientName, int quantity = 1)
    {
        if (string.IsNullOrEmpty(ingredientName))
        {
            Debug.LogError("Tentativa de adicionar ingrediente com nome nulo ou vazio!");
            return;
        }

        // Atualiza a contagem do ingrediente
        if (ingredientCounts.ContainsKey(ingredientName))
        {
            ingredientCounts[ingredientName] += quantity;
        }
        else
        {
            ingredientCounts.Add(ingredientName, quantity);
        }
        Debug.Log($"Invent�rio: Adicionado {quantity}x '{ingredientName}'. Total de '{ingredientName}': {GetIngredientCount(ingredientName)}");

        // Atualiza o slot visual espec�fico para este ingrediente
        UpdateSpecificVisualSlot(ingredientName);
    }

    // Atualiza UM slot visual espec�fico baseado no nome do ingrediente
    void UpdateSpecificVisualSlot(string ingredientName)
    {
        foreach (InventoryUISlot slotUI in visualSlots)
        {
            // Verifica se este slot da UI � para o ingrediente que foi atualizado
            if (slotUI.targetIngredientName == ingredientName)
            {
                int currentCount = GetIngredientCount(ingredientName);

                // Atualiza �CONE
                if (slotUI.iconImage != null)
                {
                    if (currentCount > 0) // S� mostra o �cone se tiver pelo menos 1
                    {
                        Sprite iconSprite = Resources.Load<Sprite>("Icons/" + ingredientName + "_Icon"); // Caminho para seus �cones
                        if (iconSprite != null)
                        {
                            slotUI.iconImage.sprite = iconSprite;
                            slotUI.iconImage.color = Color.white; // Garante visibilidade
                            slotUI.iconImage.enabled = true;
                        }
                        else
                        {
                            Debug.LogWarning($"Sprite do �cone n�o encontrado para '{ingredientName}_Icon' em Resources/Icons/");
                            slotUI.iconImage.sprite = null;
                            slotUI.iconImage.color = Color.clear;
                            slotUI.iconImage.enabled = false;
                        }
                    }
                    else // Se a contagem for 0, esconde o �cone
                    {
                        slotUI.iconImage.sprite = null;
                        slotUI.iconImage.color = Color.clear;
                        slotUI.iconImage.enabled = false;
                    }
                }

                // Atualiza TEXTO DA QUANTIDADE
                if (slotUI.quantityText != null)
                {
                    if (currentCount > 0) // S� mostra a quantidade se for maior que 0
                    {
                        slotUI.quantityText.text = currentCount.ToString();
                        slotUI.quantityText.enabled = true;
                    }
                    else // Se a contagem for 0, limpa e esconde o texto
                    {
                        slotUI.quantityText.text = "";
                        slotUI.quantityText.enabled = false;
                    }
                }
                return; // Encontrou o slot para este ingrediente, pode sair do loop
            }
        }
        // Se chegou aqui, significa que n�o h� um slot visual configurado para este ingredientName
        // Debug.LogWarning($"Nenhum slot visual configurado para o ingrediente: {ingredientName}");
    }

    // Atualiza todos os slots visuais (�til no Start ou ao carregar o jogo)
    public void UpdateAllVisualSlots()
    {
        foreach (InventoryUISlot slotUI in visualSlots)
        {
            if (!string.IsNullOrEmpty(slotUI.targetIngredientName))
            {
                UpdateSpecificVisualSlot(slotUI.targetIngredientName);
            }
            else // Se um slot n�o tem targetIngredientName, limpa ele
            {
                if (slotUI.iconImage != null)
                {
                    slotUI.iconImage.sprite = null;
                    slotUI.iconImage.color = Color.clear;
                    slotUI.iconImage.enabled = false;
                }
                if (slotUI.quantityText != null)
                {
                    slotUI.quantityText.text = "";
                    slotUI.quantityText.enabled = false;
                }
            }
        }
    }

    // Limpa todos os slots visuais (n�o mexe na contagem de ingredientes, s� na UI)
    void ClearAllInventoryUI() // Pode ser usado se quiser resetar a UI visualmente
    {
        foreach (InventoryUISlot slotUI in visualSlots)
        {
            if (slotUI.iconImage != null)
            {
                slotUI.iconImage.sprite = null;
                slotUI.iconImage.color = Color.clear;
                slotUI.iconImage.enabled = false;
            }
            if (slotUI.quantityText != null)
            {
                slotUI.quantityText.text = "";
                slotUI.quantityText.enabled = false;
            }
        }
    }

    public int GetIngredientCount(string ingredientName)
    {
        ingredientCounts.TryGetValue(ingredientName, out int count);
        return count;
    }

    public bool UseIngredients(string ingredientName, int quantityNeeded)
    {
        if (GetIngredientCount(ingredientName) >= quantityNeeded)
        {
            ingredientCounts[ingredientName] -= quantityNeeded;
            Debug.Log($"Usado {quantityNeeded}x {ingredientName}. Restante: {GetIngredientCount(ingredientName)}");
            UpdateSpecificVisualSlot(ingredientName); // Atualiza o slot do ingrediente usado
            return true;
        }
        Debug.LogWarning($"N�o h� {ingredientName} suficiente. Necess�rio: {quantityNeeded}, Possui: {GetIngredientCount(ingredientName)}");
        return false;
    }
}