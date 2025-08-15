// Salve como DoorController.cs
using UnityEngine;
using DG.Tweening; // Namespace para DOTween (instale via Asset Store ou Package Manager)
using UnityEngine.Events; // Para UnityEvent se quiser ações extras ao interagir

public class DoorController : MonoBehaviour
{
    [Header("Configurações da Porta")]
    [Tooltip("O Transform que representa a parte da porta que realmente gira (a folha da porta). O pivô deste Transform deve estar na dobradiça.")]
    [SerializeField] private Transform doorPivot;

    [Tooltip("Ângulo em graus para abrir a porta. Positivo para um lado, negativo para o outro.")]
    [SerializeField] private float openAngle = 90.0f;

    [Tooltip("Eixo em torno do qual a porta gira (geralmente Vector3.up para o eixo Y).")]
    [SerializeField] private Vector3 rotationAxis = Vector3.up;

    [Tooltip("Duração da animação de abrir/fechar em segundos.")]
    [SerializeField] private float animationDuration = 0.5f;

    [Tooltip("Tipo de suavização da animação DOTween.")]
    [SerializeField] private Ease easeType = Ease.OutQuad;

    [Header("Estado")]
    [SerializeField] private bool isOpen = false; // Começa fechada

    [Header("Eventos (Opcional)")]
    public UnityEvent onDoorOpen;
    public UnityEvent onDoorClose;

    private Quaternion closedRotation; // Rotação inicial (fechada)
    private Quaternion openRotation;   // Rotação quando aberta

    void Awake()
    {
        // Se doorPivot não for definido, tenta usar o próprio transform deste GameObject.
        // Isso funciona se este script estiver no objeto que deve girar e o pivô estiver correto.
        if (doorPivot == null)
        {
            Debug.LogWarning($"Door Pivot não definido para '{gameObject.name}'. Usando o próprio transform. Certifique-se de que o pivô está na dobradiça.", this);
            doorPivot = transform;
        }

        // Armazena a rotação inicial (fechada)
        closedRotation = doorPivot.localRotation;
        // Calcula a rotação quando a porta está aberta
        // Multiplicamos a rotação fechada por uma nova rotação em torno do eixo especificado
        openRotation = closedRotation * Quaternion.Euler(rotationAxis * openAngle);
    }

    // Função pública para ser chamada para interagir com a porta
    public void InteractDoor()
    {
        isOpen = !isOpen; // Inverte o estado da porta

        // Mata qualquer animação DOTween anterior neste pivot para evitar conflitos
        DOTween.Kill(doorPivot);

        if (isOpen)
        {
            // Anima para a rotação aberta
            doorPivot.DOLocalRotateQuaternion(openRotation, animationDuration).SetEase(easeType);
            Debug.Log($"Porta '{gameObject.name}' abrindo.");
            onDoorOpen?.Invoke(); // Chama o evento de abrir, se houver algo configurado
        }
        else
        {
            // Anima para a rotação fechada
            doorPivot.DOLocalRotateQuaternion(closedRotation, animationDuration).SetEase(easeType);
            Debug.Log($"Porta '{gameObject.name}' fechando.");
            onDoorClose?.Invoke(); // Chama o evento de fechar
        }
    }

    // Para testes no editor (opcional)
    [ContextMenu("Test Interact Door")] // Adiciona um botão no menu de contexto do componente
    void TestInteract()
    {
        InteractDoor();
    }
}