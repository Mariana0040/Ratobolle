using UnityEngine;
using System.Collections.Generic;
// using System.Linq; // N�o estamos usando Linq explicitamente aqui, pode remover se quiser

public class PlayerInteraction : MonoBehaviour
{
    [Header("Detec��o Geral")]
    [Tooltip("Ponto de origem para a detec��o.")]
    [SerializeField] private Transform detectionOrigin;
    [Tooltip("Raio da esfera de detec��o.")]
    [SerializeField] private float detectionRadius = 3.0f;

    [Header("Intera��o (Tecla E)")]
    [SerializeField] private LayerMask interactableLayer;

    [Header("Coleta (Tecla C)")]
    [SerializeField] private LayerMask collectibleLayer; // Certifique-se que seus CollectibleItem est�o nesta layer

    [Header("Feedback")]
    [SerializeField] private GameObject interactionPromptUI;
    [SerializeField] private AudioClip collectSound;
    private AudioSource audioSource;

    [Header("Invent�rio")]
    [Tooltip("Refer�ncia ao script PlayerInventoryManager no jogador.")]
    [SerializeField] private PlayerInventoryManager playerInventoryManager;

    // Rastrear o objeto atualmente em foco
    private InteractableObject currentFocusedInteractable = null;
    // ***** ESTA LINHA DEVE USAR CollectibleItem *****
    private CollectibleItem currentFocusedCollectible = null;

    void Awake()
    {
        if (detectionOrigin == null) detectionOrigin = transform;
        if (interactionPromptUI != null) interactionPromptUI.SetActive(false);

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null && collectSound != null) audioSource = gameObject.AddComponent<AudioSource>();

        if (playerInventoryManager == null)
        {
            playerInventoryManager = GetComponent<PlayerInventoryManager>();
            if (playerInventoryManager == null)
            {
                Debug.LogError("PlayerInventoryManager n�o definido ou n�o encontrado! Coleta n�o funcionar�.", this);
            }
        }
    }
    void Update()
    {
        DetectObjectsInRange();

        if (Input.GetKeyDown(KeyCode.E))
        {
            TryInteract();
        }

        if (Input.GetKeyDown(KeyCode.C))
        {
            TryCollectWithKey();
        }
    }

    void DetectObjectsInRange()
    {
        // Interativos
        InteractableObject closestInteractable = FindClosestObject<InteractableObject>(interactableLayer);
        if (closestInteractable != currentFocusedInteractable)
        {
            if (currentFocusedInteractable != null) currentFocusedInteractable.SetHighlight(false);
            if (closestInteractable != null) closestInteractable.SetHighlight(true);
            currentFocusedInteractable = closestInteractable;
        }
        if (interactionPromptUI != null) interactionPromptUI.SetActive(currentFocusedInteractable != null);

        // Colecion�veis
        // ***** ESTA LINHA DEVE USAR CollectibleItem *****
        CollectibleItem closestCollectible = FindClosestObject<CollectibleItem>(collectibleLayer);

        if (closestCollectible != currentFocusedCollectible)
        {
            // Chama SetHighlight de CollectibleItem
            if (currentFocusedCollectible != null) currentFocusedCollectible.SetHighlight(false);
            // Chama SetHighlight de CollectibleItem
            if (closestCollectible != null) closestCollectible.SetHighlight(true);
            currentFocusedCollectible = closestCollectible;
        }
    }

    T FindClosestObject<T>(LayerMask layer) where T : MonoBehaviour
    {
        Collider[] hits = Physics.OverlapSphere(detectionOrigin.position, detectionRadius, layer);
        T closestObject = null;
        float minDistance = float.MaxValue;
        foreach (Collider hitCollider in hits)
        {
            T component = hitCollider.GetComponent<T>();
            if (component != null)
            {
                float distanceToHit = Vector3.Distance(detectionOrigin.position, hitCollider.transform.position);
                if (distanceToHit < minDistance)
                {
                    minDistance = distanceToHit;
                    closestObject = component;
                }
            }
        }
        return closestObject;
    }

    void TryInteract()
    {
        if (currentFocusedInteractable != null)
        {
            float distanceToInteractable = Vector3.Distance(detectionOrigin.position, currentFocusedInteractable.transform.position);
            if (distanceToInteractable <= detectionRadius)
            {
                currentFocusedInteractable.Interact();
            }
        }
    }

    void TryCollectWithKey()
    {
        // currentFocusedCollectible agora � do tipo CollectibleItem
        if (currentFocusedCollectible != null)
        {
            float distanceToCollectible = Vector3.Distance(detectionOrigin.position, currentFocusedCollectible.transform.position);
            if (distanceToCollectible <= detectionRadius)
            {
                if (playerInventoryManager != null)
                {
                    // Acessa as propriedades p�blicas 'itemName' e 'quantity' da classe CollectibleItem
                    playerInventoryManager.AddIngredientToInventory(currentFocusedCollectible.itemName, currentFocusedCollectible.quantity);
                    Debug.Log($"Item '{currentFocusedCollectible.itemName}' adicionado ao invent�rio.");
                }
                else
                {
                    Debug.LogError("PlayerInventoryManager n�o est� referenciado.");
                }

                if (audioSource != null && collectSound != null)
                {
                    audioSource.PlayOneShot(collectSound);
                }

                currentFocusedCollectible.SetHighlight(false); // Chama SetHighlight de CollectibleItem

                // Opcional: Chamar uma fun��o no item antes de destruir
                // currentFocusedCollectible.OnCollected(); 

                Destroy(currentFocusedCollectible.gameObject);
                currentFocusedCollectible = null;
            }
        }
    }

#if UNITY_EDITOR
    void OnDrawGizmos()
    {
        if (detectionOrigin != null)
        {
            Gizmos.color = new Color(0.5f, 0.5f, 1f, 0.3f);
            Gizmos.DrawSphere(detectionOrigin.position, detectionRadius);
            if (currentFocusedInteractable != null)
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawLine(detectionOrigin.position, currentFocusedInteractable.transform.position);
            }
            // ***** ESTA LINHA DEVE USAR CollectibleItem (se o tipo da vari�vel mudou) *****
            if (currentFocusedCollectible != null) // currentFocusedCollectible � do tipo CollectibleItem
            {
                Gizmos.color = Color.cyan;
                Gizmos.DrawLine(detectionOrigin.position, currentFocusedCollectible.transform.position);
            }
        }
    }
#endif
}