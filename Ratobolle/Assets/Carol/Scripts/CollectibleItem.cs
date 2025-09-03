using UnityEngine;

// O ScriptableObject permanece o mesmo
[CreateAssetMenu(fileName = "NewItemData", menuName = "Inventory/Item Data")]
public class CollectibleItemData : ScriptableObject
{
    public string itemName = "Novo Item";
    public Sprite icon;
}

// A classe do item no mundo do jogo, agora com a função de highlight de volta
public class CollectibleItem : MonoBehaviour
{
    [Header("Item Info")]
    [Tooltip("O nome único deste item, usado pelo inventário.")]
    public string itemName = "NomePadraoDoItem";

    [Tooltip("Quantidade que este item representa ao ser coletado.")]
    public int quantity = 1;

    [Header("Highlight Visual")]
    public Color highlightColor = Color.green;
    private Renderer objectRenderer;
    private Color originalColor;
    private bool isHighlighted = false;

    void Awake()
    {
        objectRenderer = GetComponent<Renderer>();
        if (objectRenderer == null) objectRenderer = GetComponentInChildren<Renderer>();

        if (objectRenderer != null)
        {
            originalColor = objectRenderer.material.color;
        }
        else
        {
            Debug.LogWarning($"Item colecionável '{gameObject.name}' não possui Renderer para highlight.", this);
        }
    }

    // --- A FUNÇÃO QUE ESTAVA FALTANDO ---
    // PlayerInteraction precisa desta função para destacar o objeto.
    public void SetHighlight(bool highlight)
    {
        if (objectRenderer == null || isHighlighted == highlight) return;

        isHighlighted = highlight;
        if (highlight)
        {
            objectRenderer.material.color = highlightColor;
        }
        else
        {
            objectRenderer.material.color = originalColor;
        }
    }

    // Esta função conversa com o novo inventário
    // PlayerInteraction NÃO chama mais esta função diretamente, mas é bom mantê-la
    // para outros possíveis usos.
    public void Coletar()
    {
        SimplifiedPlayerInventory inventory = FindFirstObjectByType<SimplifiedPlayerInventory>();

        if (inventory != null)
        {
            inventory.AddItem(this.itemName, this.quantity);
        }
        else
        {
            Debug.LogError("Nenhum 'SimplifiedPlayerInventory' encontrado na cena!");
        }
    }
}