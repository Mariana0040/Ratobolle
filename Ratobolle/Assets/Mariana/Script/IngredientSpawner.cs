using UnityEngine;
using System.Collections.Generic; // Necessário para usar Listas
using System.Linq; // Necessário para o "OrderBy"

// Este script gerencia o surgimento aleatório dos ingredientes.
public class IngredientSpawner : MonoBehaviour
{
    [Header("Itens para Spawnar")]
    [Tooltip("Arraste todos os PREFABS dos ingredientes que podem aparecer aqui.")]
    [SerializeField] private List<GameObject> ingredientPrefabs;

    [Header("Locais de Spawn")]
    [Tooltip("Arraste todos os PONTOS DE SPAWN (Transforms) para cá.")]
    [SerializeField] private List<Transform> spawnPoints;

    [Header("Configurações")]
    [Tooltip("Quantos ingredientes devem aparecer a cada vez?")]
    [SerializeField] private int numberOfIngredientsToSpawn = 4;

    void Start()
    {
        SpawnIngredients();
    }

    public void SpawnIngredients()
    {
        // --- Validações para evitar erros ---
        if (ingredientPrefabs == null || ingredientPrefabs.Count == 0)
        {
            Debug.LogError("A lista de prefabs de ingredientes está vazia!", this);
            return;
        }
        if (spawnPoints == null || spawnPoints.Count == 0)
        {
            Debug.LogError("A lista de pontos de spawn está vazia!", this);
            return;
        }
        if (numberOfIngredientsToSpawn > spawnPoints.Count)
        {
            Debug.LogWarning("Você está tentando spawnar mais ingredientes do que o número de pontos disponíveis. Reduzindo para o máximo possível.", this);
            numberOfIngredientsToSpawn = spawnPoints.Count;
        }

        // --- Lógica Principal do Sorteio ---

        // 1. Embaralha a lista de pontos de spawn para que os locais sejam aleatórios
        List<Transform> shuffledSpawnPoints = spawnPoints.OrderBy(x => Random.value).ToList();

        // 2. Itera pelo número de ingredientes que queremos criar
        for (int i = 0; i < numberOfIngredientsToSpawn; i++)
        {
            // 3. Sorteia um ingrediente aleatório da nossa lista de prefabs
            int randomIngredientIndex = Random.Range(0, ingredientPrefabs.Count);
            GameObject randomIngredientPrefab = ingredientPrefabs[randomIngredientIndex];

            // 4. Pega o próximo ponto de spawn disponível da lista embaralhada
            Transform spawnPoint = shuffledSpawnPoints[i];

            // 5. Cria (instancia) o ingrediente sorteado no local sorteado
            Instantiate(randomIngredientPrefab, spawnPoint.position, spawnPoint.rotation);
        }
    }
}