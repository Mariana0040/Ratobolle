using UnityEngine;
using System.Collections.Generic; // Necess�rio para usar Dicion�rios

public class IngredientCollector : MonoBehaviour
{
    //public string IngredientItem;
    // Usamos um Dicion�rio para armazenar os ingredientes coletados e suas quantidades
    // Chave: Nome do Ingrediente (string)
    // Valor: Quantidade coletada (int)
    private Dictionary<string, int> inventory = new Dictionary<string, int>();

    [Header("Feedback (Opcional)")]
    [SerializeField] private AudioClip collectSound; // Som a tocar ao coletar
    private AudioSource audioSource;

    void Awake()
    {
        // Tenta pegar um AudioSource no mesmo objeto para o som
        audioSource = GetComponent<AudioSource>();
        // Adiciona um se n�o existir e um som foi definido
        if (audioSource == null && collectSound != null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    // Este m�todo � chamado automaticamente pela Unity quando outro Collider entra no Trigger deste objeto
    // Requer que PELO MENOS UM dos objetos (jogador ou ingrediente) tenha um Rigidbody
    // Requer que o Collider do OUTRO objeto (o ingrediente) seja 'Is Trigger = true'
    void OnTriggerEnter(Collider other)
    {
        // Tenta pegar o componente IngredientItem do objeto que entrou no trigger
        IngredientItem ingredient = other.GetComponent<IngredientItem>();

        // Verifica se o objeto que entrou � realmente um ingrediente
        // (Alternativa: if (other.CompareTag("Ingredient"))) // Se voc� configurou a Tag
        if (ingredient != null)
        {
            Collect(ingredient);

            // Destroi o objeto do ingrediente no mundo ap�s ser coletado
            Destroy(other.gameObject);
        }
    }

    // Fun��o para adicionar o ingrediente ao invent�rio
    void Collect(IngredientItem itemToCollect)
    {
        string itemName = itemToCollect.ingredientName;
        // int amount = itemToCollect.amount; // Se voc� adicionou quantidade no IngredientItem

        Debug.Log($"Coletou: {itemName}"); // Feedback no Console

        // Toca o som de coleta, se houver
        if (audioSource != null && collectSound != null)
        {
            audioSource.PlayOneShot(collectSound);
        }

        // Adiciona ao invent�rio
        if (inventory.ContainsKey(itemName))
        {
            // Se j� tem esse ingrediente, aumenta a quantidade
            inventory[itemName]++; // ou inventory[itemName] += amount;
        }
        else
        {
            // Se � a primeira vez coletando, adiciona ao dicion�rio
            inventory.Add(itemName, 1); // ou inventory.Add(itemName, amount);
        }

        // Opcional: Exibe o invent�rio atual no console para debug
        PrintInventory();
        // Opcional: Chamar um evento ou atualizar a UI
        // OnIngredientCollected?.Invoke(itemName, inventory[itemName]);
    }

    // Fun��o de exemplo para mostrar o invent�rio no console
    void PrintInventory()
    {
        Debug.Log("--- Invent�rio Atual ---");
        if (inventory.Count == 0)
        {
            Debug.Log("Vazio");
        }
        else
        {
            foreach (KeyValuePair<string, int> item in inventory)
            {
                Debug.Log($"{item.Key}: {item.Value}");
            }
        }
        Debug.Log("-----------------------");
    }

    // Fun��o p�blica para que outros scripts possam verificar a quantidade de um ingrediente
    public int GetIngredientCount(string ingredientName)
    {
        if (inventory.ContainsKey(ingredientName))
        {
            return inventory[ingredientName];
        }
        else
        {
            return 0; // N�o tem esse ingrediente
        }
    }

    // Fun��o p�blica para verificar se possui uma certa quantidade
    public bool HasEnoughIngredients(string ingredientName, int amountNeeded)
    {
        return GetIngredientCount(ingredientName) >= amountNeeded;
    }

    // Fun��o para remover ingredientes (ex: ao cozinhar)
    public bool RemoveIngredients(string ingredientName, int amountToRemove)
    {
        if (HasEnoughIngredients(ingredientName, amountToRemove))
        {
            inventory[ingredientName] -= amountToRemove;
            // Opcional: remover a chave se a quantidade chegar a zero
            if (inventory[ingredientName] <= 0)
            {
                inventory.Remove(ingredientName);
            }
            PrintInventory(); // Atualiza o log
            return true; // Remo��o bem-sucedida
        }
        return false; // N�o tinha ingredientes suficientes
    }
}
