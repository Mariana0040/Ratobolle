// Dentro do seu script CollectibleItem.cs
using UnityEngine;

[CreateAssetMenu(fileName = "NewItemData", menuName = "Inventory/Item Data")]
public class CollectibleItemData : ScriptableObject
{
    public string itemName = "Novo Item";
    public Sprite icon; // �cone 2D para a UI
    // Outras propriedades: descri��o, tipo, etc.
}
public class CollectibleItem : MonoBehaviour // Certifique-se que o nome da classe � este
{
    [Header("Item Info")]
    [Tooltip("O nome �nico deste item, usado pelo invent�rio.")]
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
            Debug.LogWarning($"Item colecion�vel '{gameObject.name}' n�o possui Renderer para highlight.", this);
        }

        if (string.IsNullOrEmpty(itemName) || itemName == "NomePadraoDoItem")
        {
            Debug.LogError($"Item colecion�vel '{gameObject.name}' n�o tem um 'itemName' v�lido definido no Inspetor!", this);
        }
        // Valida��o do Collider (pode ser mantida ou ajustada)
        Collider col = GetComponent<Collider>();
        if (col == null) { Debug.LogError($"Item '{gameObject.name}' n�o possui Collider!", this); }
    }
    // Dentro do seu script de coleta...
    public void ColetarIngrediente(string nomeDoItem)
    {
        // Acessa o PlayerInventoryManager da cena para adicionar o item (como j� faz)
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

    // Se voc� tiver uma fun��o Collect na moeda/item, ela pode ser simplificada
    // j� que a l�gica principal de adicionar ao invent�rio e destruir est� no PlayerInteraction
    public void OnCollected()
    {
        // Voc� pode tocar um som espec�fico do item aqui ou instanciar um efeito
        Debug.Log($"{itemName} foi sinalizado como coletado (l�gica de destrui��o no PlayerInteraction).");
    }

}
    