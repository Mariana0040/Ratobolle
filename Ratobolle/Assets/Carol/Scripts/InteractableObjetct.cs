using UnityEngine;
using UnityEngine.Events;
using DG.Tweening;

public enum InteractableType { Door, Drawer, Refrigerator, Generic }

public class InteractableObject : MonoBehaviour
{
    public InteractableType type = InteractableType.Generic;
    public bool isOpen = false;
    public UnityEvent onInteract;

    [Header("Referência para o Chefe")]
    [Tooltip("Arraste o objeto do Chefe aqui se este objeto for a geladeira.")]
    public ChefeDeCozinhaAI chefe; // <-- CORRIGIDO para o novo nome do script

    [Header("Configurações Específicas (Porta/Geladeira)")]
    public Transform pivotPoint;
    public Vector3 rotationAxis = Vector3.up;
    public float openAngle = 90f;
    public float tweenDuration = 0.5f;

    [Header("Highlight Visual")]
    public Color highlightColor = Color.yellow;
    public Renderer objectRenderer;
    private Color originalColor;
    private bool isHighlighted = false;

    private Animator animator;
    private Quaternion initialRotation;

    void Awake()
    {
        animator = GetComponent<Animator>();

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
        }

        if (pivotPoint == null && (type == InteractableType.Door || type == InteractableType.Refrigerator))
        {
            pivotPoint = transform;
        }

        if (pivotPoint != null)
        {
            initialRotation = pivotPoint.localRotation;
        }
    }

    public void Interact()
    {
        isOpen = !isOpen;
        onInteract?.Invoke();

        // --- LÓGICA DE NOTIFICAÇÃO DO CHEFE ---
        // Se for uma geladeira E ela foi ABERTA, notifica o chefe.
        if (type == InteractableType.Refrigerator && isOpen)
        {
            if (chefe != null)
            {
                // Passa a referência do próprio objeto interativo
                chefe.NotificarGeladeiraAberta(this);
            }
            else
            {
                Debug.LogWarning("Geladeira aberta, mas não há referência do Chefe para notificar!");
            }
        }
        // ------------------------------------

        switch (type)
        {
            case InteractableType.Refrigerator:
                HandleDoorTween();
                break;
            case InteractableType.Door:
                HandleDoorTween();
                break;
            default:
                HandleAnimatorOrGeneric();
                break;
        }
    }

    void HandleDoorTween()
    {
        if (pivotPoint == null) return;
        DOTween.Kill(pivotPoint);

        if (isOpen)
        {
            Quaternion targetRotation = initialRotation * Quaternion.Euler(rotationAxis * openAngle);
            pivotPoint.DOLocalRotateQuaternion(targetRotation, tweenDuration).SetEase(Ease.OutQuad);
        }
        else
        {
            pivotPoint.DOLocalRotateQuaternion(initialRotation, tweenDuration).SetEase(Ease.OutQuad);
        }
    }

    void HandleAnimatorOrGeneric()
    {
        if (animator != null)
        {
            animator.SetBool("IsOpen", isOpen);
        }
    }

    public void TriggerInteraction()
    {
        Interact();
    }

    public void SetHighlight(bool highlight)
    {
        if (objectRenderer == null || isHighlighted == highlight) return;
        isHighlighted = highlight;
        objectRenderer.material.color = highlight ? highlightColor : originalColor;
    }
}