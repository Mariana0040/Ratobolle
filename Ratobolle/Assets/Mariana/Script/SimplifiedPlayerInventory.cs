// Salve como SimplifiedPlayerInventory.cs
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;

// --- Estruturas de Dados Simplificadas ---

// Representa um item que o jogador possui. Cont�m os dados e a quantidade.
[System.Serializable]
public class InventoryItem
{
    public CollectibleItemData itemData;
    public int quantity;

    // Construtor para facilitar a cria��o de novos itens
    public InventoryItem(CollectibleItemData data, int amount)
    {
        itemData = data;
        quantity = amount;
    }
}

// Apenas para organizar as refer�ncias da UI no Inspector. N�o cont�m dados do jogo.
[System.Serializable]
public class UI_InventorySlot
{
    public Image iconImage;
    public TextMeshProUGUI quantityText;
}


public class SimplifiedPlayerInventory : MonoBehaviour
{
    [Header("Banco de Dados de Itens")]
    [Tooltip("Arraste TODOS os seus ScriptableObjects de 'CollectibleItemData' para esta lista.")]
    public List<CollectibleItemData> itemDatabase;

    [Header("Configura��o da UI")]
    [Tooltip("Arraste os GameObjects dos seus slots de UI para esta lista, em ordem.")]
    public List<UI_InventorySlot> uiSlots; // Lista com as refer�ncias da UI

    [Header("Gaveta do Invent�rio (Abre com Tecla I)")]
    public GameObject drawerPanelObject;
    [SerializeField] private RectTransform drawerRectTransform;
    [SerializeField] private float drawerHiddenXPosition = -500f;
    [SerializeField] private float drawerVisibleXPosition = 0f;
    [SerializeField] private float drawerAnimationDuration = 0.3f;
    [SerializeField] private Ease drawerEaseType = Ease.OutQuad;

    // A lista principal que guarda os itens do jogador. Esta � a fonte da verdade.
    private List<InventoryItem> playerItems = new List<InventoryItem>();
    private bool isDrawerOpen = false;

    #region Fun��es da Engine (Start, Update)

    void Awake()
    {
        if (drawerPanelObject == null) { Debug.LogError("O painel da gaveta (Drawer Panel) n�o foi definido!", this); enabled = false; return; }
        if (drawerRectTransform == null) drawerRectTransform = drawerPanelObject.GetComponent<RectTransform>();
        if (drawerRectTransform == null) { Debug.LogError("RectTransform da gaveta n�o encontrado!", this); enabled = false; return; }
    }

    void Start()
    {
        // Garante que o invent�rio comece fechado e com a UI limpa
        drawerRectTransform.anchoredPosition = new Vector2(drawerHiddenXPosition, drawerRectTransform.anchoredPosition.y);
        drawerPanelObject.SetActive(false);
        isDrawerOpen = false;
        UpdateInventoryUI(); // Atualiza a UI para garantir que todos os slots comecem vazios
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            ToggleDrawer();
        }
    }

    #endregion

    #region L�gica Principal do Invent�rio (Adicionar, Remover, Usar)

    /// <summary>
    /// Adiciona um item ao invent�rio. Empilha se o item j� existir, ou adiciona a um novo slot.
    /// </summary>
    /// <param name="itemName">O nome do item a ser adicionado (deve corresponder ao itemName no ScriptableObject).</param>
    /// <param name="quantity">A quantidade a ser adicionada.</param>
    public void AddItem(string itemName, int quantity = 1)
    {
        // 1. Encontrar os dados do item no banco de dados
        CollectibleItemData data = GetItemData(itemName);
        if (data == null)
        {
            Debug.LogWarning($"Item '{itemName}' n�o encontrado no banco de dados. Verifique se o nome est� correto e se foi adicionado � lista.");
            return;
        }

        // 2. Verificar se o item j� existe no invent�rio (para empilhar)
        InventoryItem existingItem = playerItems.FirstOrDefault(item => item.itemData.itemName == itemName);

        if (existingItem != null)
        {
            // Se j� existe, apenas aumenta a quantidade
            existingItem.quantity += quantity;
        }
        else
        {
            // Se n�o existe, verifica se h� espa�o para um novo item
            if (playerItems.Count < uiSlots.Count)
            {
                // Adiciona o novo item � lista
                playerItems.Add(new InventoryItem(data, quantity));
            }
            else
            {
                Debug.Log("Invent�rio cheio! N�o � poss�vel adicionar " + itemName);
                // Opcional: Adicione um som ou feedback visual para o jogador aqui
            }
        }

        // 3. Atualizar a UI para refletir a mudan�a
        UpdateInventoryUI();
    }

    /// <summary>
    /// Remove uma certa quantidade de um item do invent�rio.
    /// </summary>
    /// <param name="itemName">O nome do item a ser removido.</param>
    /// <param name="quantity">A quantidade a ser removida.</param>
    public bool RemoveItem(string itemName, int quantity = 1)
    {
        InventoryItem itemToRemove = playerItems.FirstOrDefault(item => item.itemData.itemName == itemName);

        if (itemToRemove != null)
        {
            itemToRemove.quantity -= quantity;

            // Se a quantidade zerar ou ficar negativa, remove o item da lista
            if (itemToRemove.quantity <= 0)
            {
                playerItems.Remove(itemToRemove);
            }

            UpdateInventoryUI();
            return true; // Remo��o bem-sucedida
        }

        Debug.LogWarning($"Tentativa de remover o item '{itemName}', mas ele n�o foi encontrado no invent�rio.");
        return false; // Item n�o encontrado
    }

    /// <summary>
    /// Verifica se o jogador tem uma quantidade suficiente de um item e, se tiver, usa-o.
    /// </summary>
    public bool UseItem(string itemName, int quantityNeeded = 1)
    {
        InventoryItem item = playerItems.FirstOrDefault(i => i.itemData.itemName == itemName);
        if (item != null && item.quantity >= quantityNeeded)
        {
            return RemoveItem(itemName, quantityNeeded);
        }
        return false; // N�o tem o item ou a quantidade necess�ria
    }


    /// <summary>
    /// Simplesmente verifica a quantidade de um item que o jogador possui.
    /// </summary>
    public int GetItemCount(string itemName)
    {
        InventoryItem item = playerItems.FirstOrDefault(i => i.itemData.itemName == itemName);
        return item?.quantity ?? 0; // Retorna a quantidade se o item existir, sen�o retorna 0
    }

    #endregion

    #region Atualiza��o da UI e Fun��es Auxiliares

    /// <summary>
    /// A fun��o central que redesenha toda a UI do invent�rio com base na lista 'playerItems'.
    /// </summary>
    private void UpdateInventoryUI()
    {
        for (int i = 0; i < uiSlots.Count; i++)
        {
            if (i < playerItems.Count)
            {
                // Se existe um item para este slot, exibe suas informa��es
                uiSlots[i].iconImage.sprite = playerItems[i].itemData.icon;
                uiSlots[i].quantityText.text = playerItems[i].quantity.ToString();
                uiSlots[i].iconImage.enabled = true;
                uiSlots[i].quantityText.enabled = true;
            }
            else
            {
                // Se n�o h� item para este slot, limpa e desativa
                uiSlots[i].iconImage.sprite = null;
                uiSlots[i].quantityText.text = "";
                uiSlots[i].iconImage.enabled = false;
                uiSlots[i].quantityText.enabled = false;
            }
        }
    }

    /// <summary>
    /// Encontra e retorna os dados de um item (ScriptableObject) a partir de seu nome.
    /// </summary>
    private CollectibleItemData GetItemData(string itemName)
    {
        return itemDatabase.FirstOrDefault(item => item.itemName == itemName);
    }

    #endregion

    #region Anima��o da Gaveta (DOTween) - Sem altera��es

    public void ToggleDrawer()
    {
        isDrawerOpen = !isDrawerOpen;
        Vector2 targetPosition = new Vector2(isDrawerOpen ? drawerVisibleXPosition : drawerHiddenXPosition, drawerRectTransform.anchoredPosition.y);

        if (isDrawerOpen)
        {
            UpdateInventoryUI(); // Garante que a UI est� atualizada ao abrir
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

    #endregion
}