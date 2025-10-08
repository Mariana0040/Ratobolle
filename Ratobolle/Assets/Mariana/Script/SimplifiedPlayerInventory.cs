// Salve como SimplifiedPlayerInventory.cs
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine.SceneManagement;
using Mono.Cecil.Cil;
// As definições de classe permanecem as mesmas
[System.Serializable]
public class InventoryItem
{
    public CollectibleItemData itemData;
    public int quantity;
    public InventoryItem(CollectibleItemData data, int amount) { itemData = data; quantity = amount; }
}
[System.Serializable]
public class UI_InventorySlot
{
    public Image iconImage;
    public TextMeshProUGUI quantityText;
}
public class SimplifiedPlayerInventory : MonoBehaviour
{
    public static SimplifiedPlayerInventory Instance { get; private set; }
    [Header("Banco de Dados de Itens")]
    public List<CollectibleItemData> itemDatabase;

    [Header("Referências da UI (Encontradas Automaticamente)")]
    public List<UI_InventorySlot> uiSlots;
    public GameObject drawerPanelObject;
    [SerializeField] private RectTransform drawerRectTransform;


    [Header("Configurações de Animação")]
    [SerializeField] private float drawerHiddenXPosition = -500f;
    [SerializeField] private float drawerVisibleXPosition = 0f;
    [SerializeField] private float drawerAnimationDuration = 0.3f;
    [SerializeField] private Ease drawerEaseType = Ease.OutQuad;

    private List<InventoryItem> playerItems = new List<InventoryItem>();
    private bool isDrawerOpen = false;

    #region Singleton e Gerenciamento de Cena

    void Awake()
    {

        Instance = this;

    }
    // --- FIM DA CORREÇÃO ---
    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    // --- LÓGICA DE RECONEXÃO DA UI ATUALIZADA E ROBUSTA ---
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log("Nova cena carregada. Procurando pela UI do inventário...");

        if (drawerPanelObject != null)
        {
            drawerRectTransform = drawerPanelObject.GetComponent<RectTransform>();

            uiSlots.Clear();

            // 1. Encontra todos os componentes "etiqueta" que estão nos filhos do painel
            UI_Slot_Reference[] slotReferences = drawerPanelObject.GetComponentsInChildren<UI_Slot_Reference>(true);

            // 2. Para cada etiqueta encontrada, pega os componentes de Imagem e Texto do mesmo objeto
            foreach (UI_Slot_Reference slotRef in slotReferences)
            {
                Image icon = slotRef.GetComponentInChildren<Image>(true);
                TextMeshProUGUI text = slotRef.GetComponentInChildren<TextMeshProUGUI>(true);

                if (icon != null && text != null)
                {
                    UI_InventorySlot newSlot = new UI_InventorySlot { iconImage = icon, quantityText = text };
                    uiSlots.Add(newSlot);
                }
            }

            Debug.Log($"UI do inventário encontrada! {uiSlots.Count} slots foram reconectados.");

            // 3. Agora que temos a lista de slots correta e completa, atualizamos a UI!
            UpdateInventoryUI();

            drawerRectTransform.anchoredPosition = new Vector2(drawerHiddenXPosition, drawerRectTransform.anchoredPosition.y);
            drawerPanelObject.SetActive(false);
            isDrawerOpen = false;
        }
        else
        {
            Debug.LogWarning("Não foi possível encontrar a UI do inventário na nova cena. Verifique se o painel principal tem a Tag 'InventoryUI'.");
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            ToggleDrawer();
        }
    }

    #endregion

    // O resto do seu código (AddItem, LoseHalfOfAllItems, etc.) não precisa de NENHUMA alteração.
    #region Lógica Principal do Inventário
    public void AddItem(string itemName, int quantity = 1)
    {
        CollectibleItemData data = GetItemData(itemName);
        if (data == null) return;
        InventoryItem existingItem = playerItems.FirstOrDefault(item => item.itemData.itemName == itemName);
        if (existingItem != null)
        {
            existingItem.quantity += quantity;
        }
        else
        {
            if (playerItems.Count < uiSlots.Count)
            {
                playerItems.Add(new InventoryItem(data, quantity));
            }
        }
        UpdateInventoryUI();
    }
    public bool RemoveItem(string itemName, int quantity = 1)
    {
        InventoryItem itemToRemove = playerItems.FirstOrDefault(item => item.itemData.itemName == itemName);
        if (itemToRemove != null)
        {
            itemToRemove.quantity -= quantity;
            if (itemToRemove.quantity <= 0)
            {
                playerItems.Remove(itemToRemove);
            }
            UpdateInventoryUI();
            return true;
        }
        Debug.LogWarning($"Tentativa de remover o item '{itemName}', mas ele não foi encontrado no inventário.");
        return false;
    }

    public bool UseItem(string itemName, int quantityNeeded = 1)
    {
        InventoryItem item = playerItems.FirstOrDefault(i => i.itemData.itemName == itemName);
        if (item != null && item.quantity >= quantityNeeded)
        {
            return RemoveItem(itemName, quantityNeeded);
        }
        return false;
    }

    public int GetItemCount(string itemName)
    {
        InventoryItem item = playerItems.FirstOrDefault(i => i.itemData.itemName == itemName);
        return item?.quantity ?? 0;
    }
    #endregion

    #region Atualização da UI e Funções Auxiliares
    private void UpdateInventoryUI()
    {
        if (uiSlots == null || uiSlots.Count == 0) return;
        for (int i = 0; i < uiSlots.Count; i++)
        {
            if (i < playerItems.Count)
            {
                uiSlots[i].iconImage.sprite = playerItems[i].itemData.icon;
                uiSlots[i].quantityText.text = playerItems[i].quantity.ToString();
                uiSlots[i].iconImage.enabled = true;
                uiSlots[i].quantityText.enabled = true;
            }
            else
            {
                uiSlots[i].iconImage.sprite = null;
                uiSlots[i].quantityText.text = "";
                uiSlots[i].iconImage.enabled = false;
                uiSlots[i].quantityText.enabled = false;
            }
        }
    }

    private CollectibleItemData GetItemData(string itemName)
    {
        return itemDatabase.FirstOrDefault(item => item.itemName == itemName);
    }
    #endregion

    #region Animação da Gaveta e Lógica de Jogo
    public void ToggleDrawer()
    {
        if (drawerPanelObject == null) return;
        isDrawerOpen = !isDrawerOpen;
        Vector2 targetPosition = new Vector2(isDrawerOpen ? drawerVisibleXPosition : drawerHiddenXPosition, drawerRectTransform.anchoredPosition.y);
        if (isDrawerOpen)
        {
            UpdateInventoryUI();
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

    // --- ADICIONE ESTA NOVA FUNÇÃO AQUI ---
    /// <summary>
    /// Retorna a contagem de quantos tipos de itens únicos o jogador possui.
    /// </summary>
    public int GetUniqueItemCount()
    {
        return playerItems.Count;
    }
    // -----------------------------------


    public void LoseHalfOfAllItems()
    {
        Debug.Log("<color=orange>TEMPO ESGOTADO! Reduzindo ingredientes.</color>");
        for (int i = playerItems.Count - 1; i >= 0; i--)
        {
            InventoryItem item = playerItems[i];

            // Nova lógica: usa Mathf.CeilToInt para arredondar para CIMA.
            // Ex: 5 / 2.0f = 2.5f -> CeilToInt = 3 itens perdidos.
            // Ex: 1 / 2.0f = 0.5f -> CeilToInt = 1 item perdido.
            int quantityToLose = Mathf.CeilToInt(item.quantity / 2.0f);

            item.quantity -= quantityToLose;
        }
        playerItems.RemoveAll(item => item.quantity <= 0);
        UpdateInventoryUI();
    }


    public void ClearAllItems()
    {
        playerItems.Clear();
        UpdateInventoryUI();
    }
    #endregion
}