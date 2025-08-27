// Dentro do seu script CollectibleItem.cs
using UnityEngine;

[CreateAssetMenu(fileName = "NewItemData", menuName = "Inventory/Item Data")]
public class CollectibleItemData : ScriptableObject
{
    public string itemName = "Novo Item";
    public Sprite icon; // Ícone 2D para a UI
    // Outras propriedades: descrição, tipo, etc.
}
public class CollectibleItem : MonoBehaviour // Certifique-se que o nome da classe é este
{
    [Header("Item Info")]
    [Tooltip("O nome único deste item, usado pelo inventário.")]
    public string itemName = "NomePadraoDoItem"; // Defina no Inspetor para cada item!

    [Tooltip("Quantidade que este item representa ao ser coletado.")]
    public int quantity = 1; // Defina no Inspetor se um item pode valer mais

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

        if (string.IsNullOrEmpty(itemName) || itemName == "NomePadraoDoItem")
        {
            Debug.LogError($"Item colecionável '{gameObject.name}' não tem um 'itemName' válido definido no Inspetor!", this);
        }
        // Validação do Collider (pode ser mantida ou ajustada)
        Collider col = GetComponent<Collider>();
        if (col == null) { Debug.LogError($"Item '{gameObject.name}' não possui Collider!", this); }
    }
    // Dentro do seu script de coleta...
    public void ColetarIngrediente(string nomeDoItem)
    {
        // Acessa o PlayerInventoryManager da cena para adicionar o item (como já faz)
         FindFirstObjectByType<PlayerInventoryManager>().AddIngredientToInventory(nomeDoItem);
    }

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

    // Se você tiver uma função Collect na moeda/item, ela pode ser simplificada
    // já que a lógica principal de adicionar ao inventário e destruir está no PlayerInteraction
    public void OnCollected()
    {
        // Você pode tocar um som específico do item aqui ou instanciar um efeito
        Debug.Log($"{itemName} foi sinalizado como coletado (lógica de destruição no PlayerInteraction).");
    }

}
    