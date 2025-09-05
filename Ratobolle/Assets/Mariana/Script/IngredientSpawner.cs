using UnityEngine;
using System.Collections.Generic;
using System.Linq;

// ESTE SCRIPT AGORA � PASSIVO. S� AGE QUANDO O TEMPORIZADOR MANDA.
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

    // A fun��o Start() foi REMOVIDA.

    public void SpawnIngredients()
    {
        if (ingredientPrefabs == null || ingredientPrefabs.Count == 0 || spawnPoints == null || spawnPoints.Count == 0)
        {
            Debug.LogError("Listas de prefabs ou pontos de spawn n�o configuradas no IngredientSpawner!", this);
            return;
        }

        int spawnCount = Mathf.Min(numberOfIngredientsToSpawn, spawnPoints.Count);
        List<Transform> shuffledSpawnPoints = spawnPoints.OrderBy(x => Random.value).ToList();

        for (int i = 0; i < spawnCount; i++)
        {
            int randomIngredientIndex = Random.Range(0, ingredientPrefabs.Count);
            GameObject randomIngredientPrefab = ingredientPrefabs[randomIngredientIndex];
            Transform spawnPoint = shuffledSpawnPoints[i];
            Instantiate(randomIngredientPrefab, spawnPoint.position, spawnPoint.rotation);
        }
    }

    public void RespawnAllIngredients()
    {
        Debug.Log("Limpando ingredientes antigos da cozinha...");

        GameObject[] existingIngredients = GameObject.FindGameObjectsWithTag("Ingredient");
        foreach (GameObject ingredient in existingIngredients)
        {
            Destroy(ingredient);
        }

        Debug.Log("Spawning novos ingredientes...");
        SpawnIngredients();
    }
}