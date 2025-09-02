// Salve como PlayerInventoryManager.cs
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.Linq; // Essencial para a busca no banco de dados
using DG.Tweening;

// A classe InventoryUISlot permanece a mesma
[System.Serializable]
public class InventoryUISlot
{
    public string targetIngredientName;
    public Image iconImage;
    public TextMeshProUGUI quantityText;
}

public class PlayerInventoryManager : MonoBehaviour
{
    // --- NOVO: O BANCO DE DADOS DE ITENS ---
    [Header("Banco de Dados de Itens")]
    [Tooltip("Arraste TODOS os seus ScriptableObjects de 'CollectibleItemData' para esta lista.")]
    public List<CollectibleItemData> itemDatabase = new List<CollectibleItemData>();

    [Header("Gaveta do Inventário (Abre com Tecla I)")]
    public GameObject drawerPanelObject;
    public List<InventoryUISlot> allInventorySlots = new List<InventoryUISlot>();

    [Header("Animação da Gaveta (DOTween)")]
    [SerializeField] private RectTransform drawerRectTransform;
    [SerializeField] private float drawerHiddenXPosition = -500f;
    [SerializeField] private float drawerVisibleXPosition = 0f;
    [SerializeField] private float drawerAnimationDuration = 0.3f;
    [SerializeField] private Ease drawerEaseType = Ease.OutQuad;

    private Dictionary<string, int> ingredientCounts = new Dictionary<string, int>();
    private bool isDrawerOpen = false;

    // ... (Awake, Start, Update, ToggleDrawer e outras funções que não mudam podem ser omitidas para clareza) ...
    // O código completo está aqui para ser copiado e colado.

    void Awake()
    {
        if (drawerPanelObject == null) { Debug.LogError("Drawer Panel não definido!", this); enabled = false; return; }
        if (drawerRectTransform == null) drawerRectTransform = drawerPanelObject.GetComponent<RectTransform>();
        if (drawerRectTransform == null) { Debug.LogError("Drawer RectTransform não encontrado!", this); enabled = false; return; }
    }

    void Start()
    {
        drawerRectTransform.anchoredPosition = new Vector2(drawerHiddenXPosition, drawerRectTransform.anchoredPosition.y);
        drawerPanelObject.SetActive(false);
        isDrawerOpen = false;
        UpdateAllConfiguredVisualSlots();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            ToggleDrawer();
        }
    }

    // --- LÓGICA DE ATUALIZAÇÃO DE SLOT COMPLETAMENTE REFEITA ---
    private void UpdateSlotDisplay(InventoryUISlot slotUI)
    {
        if (slotUI == null) return;

        int currentCount = GetIngredientCount(slotUI.targetIngredientName);

        if (currentCount > 0)
        {
            // 1. Procura no nosso banco de dados pelo item com o nome correspondente.
            CollectibleItemData itemData = GetItemData(slotUI.targetIngredientName);

            if (itemData != null) // Se encontrou os dados do item...
            {
                // 2. USA O ÍCONE DIRETAMENTE DO BANCO DE DADOS!
                if (slotUI.iconImage != null)
                {
                    slotUI.iconImage.sprite = itemData.icon;
                    slotUI.iconImage.enabled = true;
                }
            }
            else // Se não encontrou, é um erro de configuração
            {
                Debug.LogWarning($"Nenhum ItemData encontrado no banco de dados para o nome '{slotUI.targetIngredientName}'. Verifique se ele foi adicionado à lista 'Item Database'.");
                if (slotUI.iconImage != null) slotUI.iconImage.enabled = false;
            }

            // A lógica da quantidade permanece a mesma
            if (slotUI.quantityText != null)
            {
                slotUI.quantityText.text = currentCount.ToString();
                slotUI.quantityText.enabled = true;
            }
        }
        else // Se a contagem do item for 0, limpa o slot
        {
            if (slotUI.iconImage != null) slotUI.iconImage.enabled = false;
            if (slotUI.quantityText != null) slotUI.quantityText.enabled = false;
        }
    }

    // --- NOVA FUNÇÃO AUXILIAR PARA BUSCAR NO BANCO DE DADOS ---
    /// <summary>
    /// Encontra e retorna o ScriptableObject de um item a partir do seu nome.
    /// </summary>
    private CollectibleItemData GetItemData(string itemName)
    {
        // Usa LINQ para encontrar o primeiro item na lista cujo nome corresponde.
        return itemDatabase.FirstOrDefault(item => item.itemName == itemName);
    }

    // O resto do script permanece o mesmo
    public void AddIngredientToInventory(string ingredientName, int quantity = 1)
    {
        if (ingredientCounts.ContainsKey(ingredientName))
        {
            ingredientCounts[ingredientName] += quantity;
        }
        else
        {
            ingredientCounts.Add(ingredientName, quantity);
        }
        UpdateVisualForIngredient(ingredientName);
    }

    private void UpdateVisualForIngredient(string ingredientName)
    {
        foreach (InventoryUISlot slotUI in allInventorySlots)
        {
            if (slotUI != null && slotUI.targetIngredientName == ingredientName)
            {
                UpdateSlotDisplay(slotUI);
                return;
            }
        }
    }

    public void UpdateAllConfiguredVisualSlots()
    {
        foreach (InventoryUISlot slotUI in allInventorySlots)
        {
            UpdateSlotDisplay(slotUI);
        }
    }

    public void ToggleDrawer()
    {
        isDrawerOpen = !isDrawerOpen;
        Vector2 targetPosition = new Vector2(isDrawerOpen ? drawerVisibleXPosition : drawerHiddenXPosition, drawerRectTransform.anchoredPosition.y);
        if (isDrawerOpen)
        {
            UpdateAllConfiguredVisualSlots();
            drawerPanelObject.SetActive(true);
        }
        drawerRectTransform.DOAnchorPos(targetPosition, drawerAnimationDuration)
            .SetEase(drawerEaseType)
            .OnComplete(() =>
            {
                if (!isDrawerOpen)
                {
                    drawerPanelObject.SetActive(false);
                }
            });
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
            UpdateVisualForIngredient(ingredientName);
            return true;
        }
        return false;
    }
}