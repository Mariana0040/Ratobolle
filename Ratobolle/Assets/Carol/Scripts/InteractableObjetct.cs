using UnityEngine;
using UnityEngine.Events;
using DG.Tweening;

public enum InteractableType { Door, Drawer, Refrigerator, Generic }

public class InteractableObject : MonoBehaviour
{
    public InteractableType type = InteractableType.Generic;
    public bool isOpen = false;
    public UnityEvent onInteract;

    [Header("Refer�ncia para o Chefe")]
    [Tooltip("Arraste o objeto do Chefe aqui se este objeto for a geladeira.")]
    public ChefeDeCozinhaAI chefe; // <-- CORRIGIDO para o novo nome do script

    [Header("Configura��es Espec�ficas (Porta/Geladeira)")]
    public Transform pivotPoint;
    public Vector3 rotationAxis = Vector3.up;
    public float openAngle = 90f;
    public float tweenDuration = 0.5f;

    [Header("Highlight Visual")]
    public Color highlightColor = Color.yellow;
    public Renderer objectRenderer;
    private Color originalColor;
    private bool isHighlighted = false;

    [Header("Physics")]
    [Tooltip("O collider f�sico da porta que deve ser desativado quando aberta.")]
    public Collider doorCollider;

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

        // --- L�GICA DE NOTIFICA��O DO CHEFE ---
        // Se for uma geladeira E ela foi ABERTA, notifica o chefe.
        if (type == InteractableType.Refrigerator && isOpen)
        {
            if (chefe != null)
            {
                // Passa a refer�ncia do pr�prio objeto interativo
                chefe.NotificarGeladeiraAberta(this);
            }
            else
            {
                Debug.LogWarning("Geladeira aberta, mas n�o h� refer�ncia do Chefe para notificar!");
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

    // Dentro de InteractableObject.cs

    void HandleDoorTween()
    {
        if (pivotPoint == null) return;

        // Mata qualquer anima��o anterior para evitar conflitos
        DOTween.Kill(pivotPoint);

        if (isOpen)
        {
            // ABRINDO
            // Se houver um collider, desative-o. Se n�o, n�o fa�a nada.
            if (doorCollider != null)
            {
                doorCollider.enabled = false;
            }

            Quaternion targetRotation = initialRotation * Quaternion.Euler(rotationAxis * openAngle);
            pivotPoint.DOLocalRotateQuaternion(targetRotation, tweenDuration).SetEase(Ease.OutQuad);
        }
        else
        {
            // FECHANDO
            // Se houver um collider, reative-o AP�S a anima��o terminar.
            if (doorCollider != null)
            {
                pivotPoint.DOLocalRotateQuaternion(initialRotation, tweenDuration)
                    .SetEase(Ease.OutQuad)
                    .OnComplete(() => {
                        doorCollider.enabled = true;
                    });
            }
            else // Se n�o houver collider para se preocupar, apenas feche a porta.
            {
                pivotPoint.DOLocalRotateQuaternion(initialRotation, tweenDuration).SetEase(Ease.OutQuad);
            }
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