using UnityEngine;

// O ScriptableObject permanece o mesmo
[CreateAssetMenu(fileName = "NewItemData", menuName = "Inventory/Item Data")]
public class CollectibleItemData : ScriptableObject
{
    public string itemName = "Novo Item";
    public Sprite icon;
}

// A classe do item no mundo do jogo, agora com a fun��o de highlight de volta
public class CollectibleItem : MonoBehaviour
{
    [Header("Item Info")]
    [Tooltip("O nome �nico deste item, usado pelo invent�rio.")]
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
            Debug.LogWarning($"Item colecion�vel '{gameObject.name}' n�o possui Renderer para highlight.", this);
        }
    }

    // --- A FUN��O QUE ESTAVA FALTANDO ---
    // PlayerInteraction precisa desta fun��o para destacar o objeto.
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

    // Esta fun��o conversa com o novo invent�rio
    // PlayerInteraction N�O chama mais esta fun��o diretamente, mas � bom mant�-la
    // para outros poss�veis usos.
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