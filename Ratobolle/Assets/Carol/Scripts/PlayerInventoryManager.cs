// Salve como PlayerInventoryManager.cs
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using DG.Tweening; // ESSENCIAL: Certifique-se de que DOTween est� importado!

// Esta classe pode ser a mesma que voc� j� tem.
// Garante que voc� possa configurar cada slot no Inspector.
[System.Serializable]
public class InventoryUISlot
{
    [Tooltip("O NOME EXATO do ingrediente que este slot deve exibir. Deve corresponder ao 'itemName' do CollectibleItemData.")]
    public string targetIngredientName;
    [Tooltip("Refer�ncia para a IMAGEM do �cone do slot.")]
    public Image iconImage;
    [Tooltip("Refer�ncia para o TEXTO da quantidade do slot.")]
    public TextMeshProUGUI quantityText;
}

public class PlayerInventoryManager : MonoBehaviour
{
    [Header("Gaveta do Invent�rio (Abre com Tecla I)")]
    [Tooltip("O GameObject do painel da gaveta que cont�m todos os slots e ser� animado.")]
    public GameObject drawerPanelObject;
    [Tooltip("TODOS os slots visuais que estar�o dentro da gaveta.")]
    public List<InventoryUISlot> allInventorySlots = new List<InventoryUISlot>(); // Configure com seus 18+ slots

    [Header("Anima��o da Gaveta (DOTween)")]
    [Tooltip("RectTransform do DrawerPanel para anima��o.")]
    [SerializeField] private RectTransform drawerRectTransform;
    [Tooltip("Posi��o X ANCORADA do RectTransform da gaveta quando ESCONDIDA.")]
    [SerializeField] private float drawerHiddenXPosition = -500f;
    [Tooltip("Posi��o X ANCORADA do RectTransform da gaveta quando VIS�VEL.")]
    [SerializeField] private float drawerVisibleXPosition = 0f;
    [Tooltip("Dura��o da anima��o de abrir/fechar a gaveta.")]
    [SerializeField] private float drawerAnimationDuration = 0.3f;
    [SerializeField] private Ease drawerEaseType = Ease.OutQuad;

    // Dicion�rio para armazenar a CONTAGEM de cada ingrediente
    private Dictionary<string, int> ingredientCounts = new Dictionary<string, int>();
    private bool isDrawerOpen = false;

    void Awake()
    {
        // Valida��es essenciais para a gaveta funcionar
        if (drawerPanelObject == null)
        {
            Debug.LogError("PlayerInventoryManager: 'Drawer Panel Object' n�o foi definido no Inspector! O invent�rio n�o funcionar�.", this);
            enabled = false; // Desativa o script se a refer�ncia principal estiver faltando
            return;
        }

        if (drawerRectTransform == null)
        {
            drawerRectTransform = drawerPanelObject.GetComponent<RectTransform>();
            if (drawerRectTransform == null)
            {
                Debug.LogError("PlayerInventoryManager: 'Drawer Rect Transform' n�o foi encontrado! A anima��o da gaveta falhar�.", this);
                enabled = false;
                return;
            }
        }
    }

    void Start()
    {
        // Configura o estado inicial da gaveta: escondida e inativa
        drawerRectTransform.anchoredPosition = new Vector2(drawerHiddenXPosition, drawerRectTransform.anchoredPosition.y);
        drawerPanelObject.SetActive(false);
        isDrawerOpen = false;

        // Limpa/inicializa todos os slots visuais no in�cio do jogo
        UpdateAllConfiguredVisualSlots();
    }

    void Update()
    {
        // Verifica a tecla 'I' para abrir/fechar a gaveta
        if (Input.GetKeyDown(KeyCode.I))
        {
            ToggleDrawer();
        }
    }

    public void AddIngredientToInventory(string ingredientName, int quantity = 1)
    {

        // Atualiza a contagem no dicion�rio
        if (ingredientCounts.ContainsKey(ingredientName))
        {
            ingredientCounts[ingredientName] += quantity;
        }
        else
        {
            ingredientCounts.Add(ingredientName, quantity);
        }
        Debug.Log($"Invent�rio: Adicionado {quantity}x '{ingredientName}'. Total: {GetIngredientCount(ingredientName)}");

        // Atualiza o slot visual correspondente
        UpdateVisualForIngredient(ingredientName);
    }

    // Procura e atualiza o slot espec�fico para um ingrediente
    private void UpdateVisualForIngredient(string ingredientName)
    {
        foreach (InventoryUISlot slotUI in allInventorySlots)
        {
            // Se o nome do slot corresponde ao nome do ingrediente...
            if (slotUI != null && slotUI.targetIngredientName == ingredientName)
            {
                UpdateSlotDisplay(slotUI); // ...atualiza sua apar�ncia.
                return; // Para de procurar, pois j� encontrou o slot correto.
            }
        }
    }

    // Atualiza a apar�ncia de um �nico slot baseado na contagem atual do item
    private void UpdateSlotDisplay(InventoryUISlot slotUI)
    {
        if (slotUI == null) return;

        int currentCount = GetIngredientCount(slotUI.targetIngredientName);

        if (currentCount > 0)
        {
            // Tenta carregar o �cone da pasta "Resources/Icons/"
            Sprite iconSprite = Resources.Load<Sprite>("Icons/" + slotUI.targetIngredientName + "_Icon");

            if (slotUI.iconImage != null)
            {
                if (iconSprite != null)
                {
                    slotUI.iconImage.sprite = iconSprite;
                    slotUI.iconImage.enabled = true; // Mostra o �cone
                }
                else
                {
                    Debug.LogWarning($"�cone n�o encontrado em 'Resources/Icons/' para '{slotUI.targetIngredientName}_Icon'");
                    slotUI.iconImage.enabled = false; // Esconde se n�o encontrar o sprite
                }
            }

            if (slotUI.quantityText != null)
            {
                slotUI.quantityText.text = currentCount.ToString();
                slotUI.quantityText.enabled = true; // Mostra o texto da quantidade
            }
        }
        else // Se a contagem do item for 0, limpa o slot
        {
            if (slotUI.iconImage != null) slotUI.iconImage.enabled = false;
            if (slotUI.quantityText != null) slotUI.quantityText.enabled = false;
        }
    }

    // For�a uma atualiza��o em TODOS os slots configurados
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
            // Antes de mostrar, garante que todos os slots est�o com a informa��o mais recente
            UpdateAllConfiguredVisualSlots();
            drawerPanelObject.SetActive(true); // Ativa o painel ANTES de come�ar a anima��o
        }

        // Anima a posi��o do painel
        drawerRectTransform.DOAnchorPos(targetPosition, drawerAnimationDuration)
            .SetEase(drawerEaseType)
            .OnComplete(() =>
            {
                // Quando a anima��o de FECHAR terminar, desativa o GameObject para otimiza��o
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
            UpdateVisualForIngredient(ingredientName); // Atualiza o slot visual ap�s usar os itens
            return true;
        }
        return false; // N�o tem ingredientes suficientes
    }
}