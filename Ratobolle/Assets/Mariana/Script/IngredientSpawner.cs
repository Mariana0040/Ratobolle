using UnityEngine;
using System.Collections.Generic; // Necess�rio para usar Listas
using System.Linq; // Necess�rio para o "OrderBy"

// Este script gerencia o surgimento aleat�rio dos ingredientes.
public class IngredientSpawner : MonoBehaviour
{
    [Header("Itens para Spawnar")]
    [Tooltip("Arraste todos os PREFABS dos ingredientes que podem aparecer aqui.")]
    [SerializeField] private List<GameObject> ingredientPrefabs;

    [Header("Locais de Spawn")]
    [Tooltip("Arraste todos os PONTOS DE SPAWN (Transforms) para c�.")]
    [SerializeField] private List<Transform> spawnPoints;

    [Header("Configura��es")]
    [Tooltip("Quantos ingredientes devem aparecer a cada vez?")]
    [SerializeField] private int numberOfIngredientsToSpawn = 4;

    void Start()
    {
        SpawnIngredients();
    }

    public void SpawnIngredients()
    {
        // --- Valida��es para evitar erros ---
        if (ingredientPrefabs == null || ingredientPrefabs.Count == 0)
        {
            Debug.LogError("A lista de prefabs de ingredientes est� vazia!", this);
            return;
        }
        if (spawnPoints == null || spawnPoints.Count == 0)
        {
            Debug.LogError("A lista de pontos de spawn est� vazia!", this);
            return;
        }
        if (numberOfIngredientsToSpawn > spawnPoints.Count)
        {
            Debug.LogWarning("Voc� est� tentando spawnar mais ingredientes do que o n�mero de pontos dispon�veis. Reduzindo para o m�ximo poss�vel.", this);
            numberOfIngredientsToSpawn = spawnPoints.Count;
        }

        // --- L�gica Principal do Sorteio ---

        // 1. Embaralha a lista de pontos de spawn para que os locais sejam aleat�rios
        List<Transform> shuffledSpawnPoints = spawnPoints.OrderBy(x => Random.value).ToList();

        // 2. Itera pelo n�mero de ingredientes que queremos criar
        for (int i = 0; i < numberOfIngredientsToSpawn; i++)
        {
            // 3. Sorteia um ingrediente aleat�rio da nossa lista de prefabs
            int randomIngredientIndex = Random.Range(0, ingredientPrefabs.Count);
            GameObject randomIngredientPrefab = ingredientPrefabs[randomIngredientIndex];

            // 4. Pega o pr�ximo ponto de spawn dispon�vel da lista embaralhada
            Transform spawnPoint = shuffledSpawnPoints[i];

            // 5. Cria (instancia) o ingrediente sorteado no local sorteado
            Instantiate(randomIngredientPrefab, spawnPoint.position, spawnPoint.rotation);
        }
    }
}