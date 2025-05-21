using UnityEngine;

public class IngredientItem : MonoBehaviour // Ou CollectibleItem
{
    public string ingredientName = "Ingrediente Desconhecido";
    // public int amount = 1;
    public int quantity = 1;

    [Header("Highlight Visual")]
    public Color highlightColor = Color.green; // Cor diferente para colecionáveis
    private Renderer objectRenderer;
    private Color originalColor;
    private bool isHighlighted = false;

    void Awake() // Mudei de Start para Awake para garantir que a cor original seja pega antes
    {
        objectRenderer = GetComponent<Renderer>();
        if (objectRenderer != null)
        {
            originalColor = objectRenderer.material.color;
        }
        else
        {
            objectRenderer = GetComponentInChildren<Renderer>();
            if (objectRenderer != null)
            {
                originalColor = objectRenderer.material.color;
            }
            else
            {
                Debug.LogWarning($"Ingrediente '{gameObject.name}' não possui Renderer para highlight.", this);
            }
        }


        // Validações de Collider (podem permanecer)
        Collider col = GetComponent<Collider>();
        if (col == null) { Debug.LogError($"Ingrediente '{gameObject.name}' não possui Collider!", this); }
        // else if (!col.isTrigger) { Debug.LogWarning($"Collider do Ingrediente '{gameObject.name}' não é Trigger.", this); }
    }


    // --- NOVAS FUNÇÕES PARA HIGHLIGHT ---
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
}
    