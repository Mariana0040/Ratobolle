using UnityEngine;
using UnityEngine.Events;
using DG.Tweening; // Certifique-se de que DOTween est� importado e configurado

public enum InteractableType { Door, Drawer, Refrigerator, Generic }

public class InteractableObject : MonoBehaviour
{
    public InteractableType type = InteractableType.Generic;
    public bool isOpen = false;
    public UnityEvent onInteract;

    [Header("Configura��es Espec�ficas (Porta/Geladeira)")]
    [Tooltip("Para portas/geladeiras, este � o objeto que realmente gira (a porta).")]
    public Transform pivotPoint;
    [Tooltip("Eixo em torno do qual a porta/gaveta gira/move. Para porta de geladeira, geralmente Vector3.up.")]
    public Vector3 rotationAxis = Vector3.up;
    [Tooltip("�ngulo para abrir a porta/geladeira.")]
    public float openAngle = 90f;
    [Tooltip("Dura��o da anima��o de abrir/fechar com DOTween.")]
    public float tweenDuration = 0.5f;

    [Header("Highlight Visual")] // <--- SE��O DE HIGHLIGHT PRECISA ESTAR AQUI
    public Color highlightColor = Color.yellow;
    public Renderer objectRenderer;
    private Color originalColor;
    private bool isHighlighted = false;

    private Animator animator;
    private Quaternion initialRotation;
    private Vector3 initialPosition;

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
            else
            {
                Debug.LogWarning($"Objeto interativo '{gameObject.name}' n�o possui Renderer para highlight.", this);
            }
        }

        // L�gica para o pivotPoint
        if (pivotPoint == null && (type == InteractableType.Door || type == InteractableType.Refrigerator))
        {
            Debug.LogWarning($"Objeto interativo '{gameObject.name}' do tipo {type} n�o tem um 'Pivot Point' definido. Tentando usar 'transform'.", this);
            pivotPoint = transform;
        }

        if (pivotPoint != null)
        {
            initialRotation = pivotPoint.localRotation;
            initialPosition = pivotPoint.localPosition;
        }
    }

    public void Interact()
    {
        isOpen = !isOpen;
        Debug.Log($"{gameObject.name} interagido! Novo estado: {(isOpen ? "Aberto" : "Fechado")}");
        if (onInteract != null) // Boa pr�tica checar se n�o � nulo
            onInteract.Invoke();


        switch (type)
        {
            case InteractableType.Refrigerator:
            case InteractableType.Door:
                HandleDoorTween();
                break;
            // case InteractableType.Drawer: // Se voc� implementar gaveta com DOTween
            // HandleDrawerTween();
            // break;
            default:
                HandleAnimatorOrGeneric();
                break;
        }
    }

    void HandleDoorTween()
    {
        if (pivotPoint == null) return;
        DOTween.Kill(pivotPoint); // Garante que tweens anteriores sejam parados

        if (isOpen)
        {
            // Abre a porta
            // DORotate gira em rela��o ao espa�o do mundo por padr�o
            // DOLocalRotate gira em rela��o ao espa�o local do pai
            Quaternion targetRotation = initialRotation * Quaternion.Euler(rotationAxis * openAngle);
            pivotPoint.DOLocalRotateQuaternion(targetRotation, tweenDuration).SetEase(Ease.OutQuad);
        }
        else
        {
            // Fecha a porta (volta para a rota��o inicial)
            pivotPoint.DOLocalRotateQuaternion(initialRotation, tweenDuration).SetEase(Ease.OutQuad);
        }
    }

    /*
    void HandleDrawerTween() {
        // ... (l�gica da gaveta se precisar) ...
    }
    */
    /* Exemplo para gaveta com DOTween:
void HandleDrawerTween() {
    if (pivotPoint == null) return;
    DOTween.Kill(pivotPoint);
    if (isOpen) {
        // Abre a gaveta - ajuste 'openOffset' conforme necess�rio
        Vector3 openOffset = transform.forward * 0.5f; // Move 0.5 unidades para frente (local)
        pivotPoint.DOLocalMove(initialPosition + openOffset, tweenDuration).SetEase(Ease.OutQuad);
    } else {
        // Fecha a gaveta
        pivotPoint.DOLocalMove(initialPosition, tweenDuration).SetEase(Ease.OutQuad);
    }
}
*/

    void HandleAnimatorOrGeneric()
    {
        if (animator != null)
        {
            animator.SetBool("IsOpen", isOpen); // Assumindo que seu Animator tem um bool "IsOpen"
        }
        else
        {
            Debug.Log($"Objeto '{gameObject.name}' (Tipo: {type}) interagido, mas n�o tem Animator nem l�gica DOTween espec�fica (al�m de porta/geladeira).");
        }
    }

    // Fun��o para ser chamada de fora, se necess�rio
    public void TriggerInteraction()
    {
        Interact();
    }

    // --- FUN��O DE HIGHLIGHT PRECISA ESTAR AQUI ---
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