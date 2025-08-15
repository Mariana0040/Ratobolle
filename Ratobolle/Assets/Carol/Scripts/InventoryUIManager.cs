using UnityEngine;
using UnityEngine.UI; // Para Image
using TMPro;      // Para TextMeshProUGUI
using System.Collections.Generic;
using System.Linq; // Para Distinct e outras operações de lista
using DG.Tweening; // Para animação da gaveta (opcional)

// Struct para facilitar a configuração dos slots no Inspetor
[System.Serializable]
public class UISlot
{
    public GameObject slotGameObject;
    public Image iconImage;
    public TextMeshProUGUI quantityText;
    // Opcional: para saber qual item está neste slot
    [HideInInspector] public CollectibleItemData currentItemData;
}

public class InventoryUIManager : MonoBehaviour
{
    [Header("Painéis da UI")]
    [SerializeField] private GameObject hotbarPanelObject; // O GameObject do HotbarPanel
    [SerializeField] private GameObject drawerPanelObject; // O GameObject do DrawerPanel

    [Header("Referências dos Slots da UI")]
    [Tooltip("Os 5 slots que estão sempre visíveis (Hotbar).")]
    public List<UISlot> hotbarSlots = new List<UISlot>(5);
    [Tooltip("Os 15 slots adicionais na gaveta.")]
    public List<UISlot> drawerSlots = new List<UISlot>(15);

    [Header("Animação da Gaveta (DOTween)")]
    [SerializeField] private RectTransform drawerRectTransform; // RectTransform do DrawerPanel
    [SerializeField] private float drawerAnimationDuration = 0.3f;
    [SerializeField] private Vector2 drawerHiddenPosition; // Posição Y quando escondida (ex: abaixo da hotbar)
    [SerializeField] private Vector2 drawerVisiblePosition; // Posição Y quando visível

    // Dados do Inventário: Armazena o ItemData e sua quantidade
    private Dictionary<CollectibleItemData, int> inventoryItems = new Dictionary<CollectibleItemData, int>();
    // Lista para manter a ordem de exibição dos tipos de itens
    private List<CollectibleItemData> displayedItemOrder = new List<CollectibleItemData>();

    private bool isDrawerOpen = false;

void Start()
    {
        inventoryItems.Clear();
        displayedItemOrder.Clear();

        // Garante que os painéis tenham o estado inicial correto
        if (hotbarPanelObject != null) hotbarPanelObject.SetActive(true); // Hotbar sempre visível
        if (drawerPanelObject != null)
        {
            if (drawerRectTransform == null) drawerRectTransform = drawerPanelObject.GetComponent<RectTransform>();
            drawerRectTransform.anchoredPosition = drawerHiddenPosition; // Começa escondida
            drawerPanelObject.SetActive(false); // Começa desativado para não interferir
        }
        isDrawerOpen = false;
        UpdateFullInventoryDisplay();

        // 2. **CORREÇÃO**: Em vez de atualizar, limpamos a UI para garantir que ela comece vazia.
        ClearAllInventoryUI();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            ToggleDrawer();
        }
    }

    public void AddItem(CollectibleItemData itemData, int quantity)
    {
        if (itemData == null) return;

        if (inventoryItems.ContainsKey(itemData))
        {
            inventoryItems[itemData] += quantity;
        }
        else
        {
            inventoryItems.Add(itemData, quantity);
            if (!displayedItemOrder.Contains(itemData)) // Adiciona à ordem de exibição se for um novo tipo
            {
                displayedItemOrder.Add(itemData);
            }
        }
        Debug.Log($"Adicionado {quantity}x {itemData.itemName}. Total: {inventoryItems[itemData]}");
        UpdateFullInventoryDisplay();
    }

    public void RemoveItem(CollectibleItemData itemData, int quantity)
    {
        if (itemData == null || !inventoryItems.ContainsKey(itemData)) return;

        inventoryItems[itemData] -= quantity;
        if (inventoryItems[itemData] <= 0)
        {
            inventoryItems.Remove(itemData);
            displayedItemOrder.Remove(itemData); // Remove da ordem de exibição se zerar
            Debug.Log($"{itemData.itemName} removido do inventário.");
        }
        else
        {
            Debug.Log($"Removido {quantity}x {itemData.itemName}. Restante: {inventoryItems[itemData]}");
        }
        UpdateFullInventoryDisplay();
    }


    public void ToggleDrawer()
    {
        if (drawerPanelObject == null || drawerRectTransform == null) return;

        isDrawerOpen = !isDrawerOpen;

        if (isDrawerOpen)
        {
            drawerPanelObject.SetActive(true); // Ativa o objeto antes de animar
            drawerRectTransform.DOAnchorPos(drawerVisiblePosition, drawerAnimationDuration).SetEase(Ease.OutBack);
            Debug.Log("Gaveta Aberta");
            UpdateFullInventoryDisplay(); // Atualiza os slots da gaveta
        }
        else
        {
            drawerRectTransform.DOAnchorPos(drawerHiddenPosition, drawerAnimationDuration).SetEase(Ease.InBack)
                .OnComplete(() => drawerPanelObject.SetActive(false)); // Desativa após a animação
            Debug.Log("Gaveta Fechada");
        }
    }

void UpdateFullInventoryDisplay()
    {
        List<UISlot> allSlots = new List<UISlot>();
        allSlots.AddRange(hotbarSlots);
        allSlots.AddRange(drawerSlots);

        foreach (var slot in allSlots)
        {
            ClearSlotUI(slot);
        }

        for (int i = 0; i < displayedItemOrder.Count; i++)
        {
            if (i < allSlots.Count) // Garante que não tentemos acessar um slot que não existe
            {
                CollectibleItemData item = displayedItemOrder[i];
                int quantity = inventoryItems[item];
                UpdateSlotUI(allSlots[i], item, quantity);
            }
        }
    }




    // Atualiza um único slot da UI
    void UpdateSlotUI(UISlot slot, CollectibleItemData itemData, int quantity)
    {
        if (slot == null || slot.iconImage == null || slot.quantityText == null) return;

        if (itemData != null && quantity > 0)
        {
            slot.iconImage.sprite = itemData.icon;
            slot.iconImage.color = Color.white;
            slot.iconImage.enabled = true;
            slot.quantityText.text = quantity.ToString();
            slot.quantityText.enabled = true;
            slot.currentItemData = itemData; // Guarda qual item está neste slot
        }
        else
        {
            ClearSlotUI(slot);
        }
    }

    // Limpa a aparência de um slot individual
    void ClearSlotUI(UISlot slot)
    {
        if (slot == null) return;

        // **MELHORIA**: A maneira mais eficaz de esconder um slot é desativar seu GameObject.
        slot.slotGameObject.SetActive(false);
        slot.currentItemData = null;
    }

    // Limpa toda a UI
    void ClearAllInventoryUI()
    {
        foreach (UISlot slot in hotbarSlots) ClearSlotUI(slot);
        foreach (UISlot slot in drawerSlots) ClearSlotUI(slot);
    }
}

    // Exemplo de como o jogador coletaria um item:
    // PlayerInventoryManager invManager = FindObjectOfType<PlayerInventoryManager>();
    // WorldItem worldItem = hit.collider.GetComponent<WorldItem>();
    // if (worldItem != null && worldItem.itemData != null) {
    //    invManager.AddItem(worldItem.itemData, worldItem.quantity);
    //    Destroy(worldItem.gameObject);
    // }
