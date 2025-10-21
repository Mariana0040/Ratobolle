using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro; // Precisamos para o texto de UI
using System.Collections; // Precisamos para a Corrotina

public class PhaseTransitionDoor : MonoBehaviour
{
    [Header("Configuração da Transição")]
    [Tooltip("O nome exato da cena para a qual esta porta levará.")]
    [SerializeField] private string targetSceneName = "FASE UM";

    [Header("Requisitos de Ingredientes")]
    [Tooltip("Quantos ingredientes únicos o jogador precisa ter para poder passar.")]
    [SerializeField] private int requiredIngredientsCount = 3;

    [Header("Feedback Visual e de Texto")]
    [SerializeField] private Renderer doorRenderer; // Arraste o Renderer da porta aqui
    [SerializeField] private Color lockedColor = Color.red;
    private Color originalColor; // Cor original será guardada aqui

    [Header("Referências de UI")]
    [Tooltip("Texto que aparece quando o jogador pode interagir (ex: 'Apertar E').")]
    [SerializeField] private TextMeshProUGUI interactionPromptText;
    [Tooltip("Texto que aparece se o jogador não tiver os ingredientes (ex: 'Sem ingredientes suficientes').")]
    [SerializeField] private TextMeshProUGUI statusText;

    [Header("Referências do Jogador")]
    [Tooltip("Arraste o objeto que contém o inventário do jogador aqui.")]
    [SerializeField] private SimplifiedPlayerInventory playerInventory;

    private bool playerIsInRange = false;
    private bool canTransition = false;

    void Awake()
    {
        if (doorRenderer != null)
        {
            originalColor = doorRenderer.material.color; // Guarda a cor normal da porta
        }

        // Garante que os textos comecem desativados
        if (interactionPromptText != null) interactionPromptText.gameObject.SetActive(false);
        if (statusText != null) statusText.gameObject.SetActive(false);
    }

    void Update()
    {
        // Se o jogador estiver no alcance e apertar a tecla "E"
        if (playerIsInRange && Input.GetKeyDown(KeyCode.E))
        {
            CheckTransitionCondition(); // Re-verifica a condição no momento do clique

            if (canTransition)
            {
                Debug.Log("Condição atendida! Carregando a próxima fase...");
                SceneManager.LoadScene(targetSceneName);
            }
            else
            {
                // Se não puder passar, mostra o texto de status por um tempo
                StartCoroutine(ShowStatusText());
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerIsInRange = true;
            CheckTransitionCondition(); // Verifica os ingredientes assim que o jogador entra
            UpdateVisuals(); // Atualiza a cor e o texto de prompt
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerIsInRange = false;
            UpdateVisuals(); // Esconde os textos e restaura a cor
        }
    }

    // Verifica se o jogador tem os ingredientes necessários
    private void CheckTransitionCondition()
    {
        if (playerInventory == null)
        {
            Debug.LogError("Referência do inventário não definida na porta!");
            canTransition = false;
            return;
        }

        canTransition = playerInventory.GetUniqueItemCount() >= requiredIngredientsCount;
    }

    // Atualiza a cor da porta e mostra/esconde o texto de interação
    private void UpdateVisuals()
    {
        if (playerIsInRange)
        {
            // Mostra o prompt "Aperte E"
            if (interactionPromptText != null) interactionPromptText.gameObject.SetActive(true);

            // Muda a cor da porta com base na condição
            if (doorRenderer != null)
            {
                doorRenderer.material.color = canTransition ? originalColor : lockedColor;
            }
        }
        else
        {
            // Esconde os prompts e restaura a cor original
            if (interactionPromptText != null) interactionPromptText.gameObject.SetActive(false);
            if (statusText != null) statusText.gameObject.SetActive(false);
            if (doorRenderer != null)
            {
                doorRenderer.material.color = originalColor;
            }
        }
    }

    // Mostra o texto "Sem ingredientes..." por alguns segundos
    private IEnumerator ShowStatusText()
    {
        if (statusText != null)
        {
            statusText.text = "Sem ingredientes suficientes";
            statusText.gameObject.SetActive(true);
            yield return new WaitForSeconds(2.5f); // Mostra por 2.5 segundos
            statusText.gameObject.SetActive(false);
        }
    }
}