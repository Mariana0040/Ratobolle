using UnityEngine;
using System.Collections.Generic;

public class PlayerInteraction : MonoBehaviour
{
    [Header("Detecção Geral")]
    [Tooltip("Ponto de origem para a detecção.")]
    [SerializeField] private Transform detectionOrigin;
    [Tooltip("Raio da esfera de detecção.")]
    [SerializeField] private float detectionRadius = 3.0f;

    [Header("Interação (Tecla E)")]
    [SerializeField] private LayerMask interactableLayer;

    [Header("Coleta (Tecla C)")]
    [SerializeField] private LayerMask collectibleLayer;

    [Header("Feedback")]
    [SerializeField] private GameObject interactionPromptUI;
    [SerializeField] private AudioClip collectSound;
    private AudioSource audioSource;

    [Header("Inventário")]
    [Tooltip("Referência ao script de inventário no jogador.")]
    [SerializeField] private SimplifiedPlayerInventory playerInventory;

    private InteractableObject currentFocusedInteractable = null;
    private CollectibleItem currentFocusedCollectible = null;

    void Awake()
    {
        if (detectionOrigin == null) detectionOrigin = transform;
        if (interactionPromptUI != null) interactionPromptUI.SetActive(false);

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null && collectSound != null) audioSource = gameObject.AddComponent<AudioSource>();

        if (playerInventory == null)
        {
            playerInventory = GetComponent<SimplifiedPlayerInventory>();
            if (playerInventory == null)
            {
                Debug.LogError("SimplifiedPlayerInventory não definido ou não encontrado! Coleta não funcionará.", this);
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
        InteractableObject closestInteractable = FindClosestObject<InteractableObject>(interactableLayer);
        if (closestInteractable != currentFocusedInteractable)
        {
            if (currentFocusedInteractable != null) currentFocusedInteractable.SetHighlight(false);
            if (closestInteractable != null) closestInteractable.SetHighlight(true);
            currentFocusedInteractable = closestInteractable;
        }
        if (interactionPromptUI != null) interactionPromptUI.SetActive(currentFocusedInteractable != null);

        CollectibleItem closestCollectible = FindClosestObject<CollectibleItem>(collectibleLayer);

        if (closestCollectible != currentFocusedCollectible)
        {
            if (currentFocusedCollectible != null) currentFocusedCollectible.SetHighlight(false);
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
        if (currentFocusedCollectible != null)
        {
            float distanceToCollectible = Vector3.Distance(detectionOrigin.position, currentFocusedCollectible.transform.position);
            if (distanceToCollectible <= detectionRadius)
            {
                if (playerInventory != null)
                {
                    playerInventory.AddItem(currentFocusedCollectible.itemName, currentFocusedCollectible.quantity);
                    Debug.Log($"Item '{currentFocusedCollectible.itemName}' adicionado ao inventário.");
                }
                else
                {
                    Debug.LogError("SimplifiedPlayerInventory não está referenciado.");
                }

                if (audioSource != null && collectSound != null)
                {
                    audioSource.PlayOneShot(collectSound);
                }

                currentFocusedCollectible.SetHighlight(false);

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
            if (currentFocusedCollectible != null)
            {
                Gizmos.color = Color.cyan;
                Gizmos.DrawLine(detectionOrigin.position, currentFocusedCollectible.transform.position);
            }
        }
    }
#endif
}