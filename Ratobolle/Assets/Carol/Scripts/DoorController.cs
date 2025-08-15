// Salve como DoorController.cs
using UnityEngine;
using DG.Tweening; // Namespace para DOTween (instale via Asset Store ou Package Manager)
using UnityEngine.Events; // Para UnityEvent se quiser a��es extras ao interagir

public class DoorController : MonoBehaviour
{
    [Header("Configura��es da Porta")]
    [Tooltip("O Transform que representa a parte da porta que realmente gira (a folha da porta). O piv� deste Transform deve estar na dobradi�a.")]
    [SerializeField] private Transform doorPivot;

    [Tooltip("�ngulo em graus para abrir a porta. Positivo para um lado, negativo para o outro.")]
    [SerializeField] private float openAngle = 90.0f;

    [Tooltip("Eixo em torno do qual a porta gira (geralmente Vector3.up para o eixo Y).")]
    [SerializeField] private Vector3 rotationAxis = Vector3.up;

    [Tooltip("Dura��o da anima��o de abrir/fechar em segundos.")]
    [SerializeField] private float animationDuration = 0.5f;

    [Tooltip("Tipo de suaviza��o da anima��o DOTween.")]
    [SerializeField] private Ease easeType = Ease.OutQuad;

    [Header("Estado")]
    [SerializeField] private bool isOpen = false; // Come�a fechada

    [Header("Eventos (Opcional)")]
    public UnityEvent onDoorOpen;
    public UnityEvent onDoorClose;

    private Quaternion closedRotation; // Rota��o inicial (fechada)
    private Quaternion openRotation;   // Rota��o quando aberta

    void Awake()
    {
        // Se doorPivot n�o for definido, tenta usar o pr�prio transform deste GameObject.
        // Isso funciona se este script estiver no objeto que deve girar e o piv� estiver correto.
        if (doorPivot == null)
        {
            Debug.LogWarning($"Door Pivot n�o definido para '{gameObject.name}'. Usando o pr�prio transform. Certifique-se de que o piv� est� na dobradi�a.", this);
            doorPivot = transform;
        }

        // Armazena a rota��o inicial (fechada)
        closedRotation = doorPivot.localRotation;
        // Calcula a rota��o quando a porta est� aberta
        // Multiplicamos a rota��o fechada por uma nova rota��o em torno do eixo especificado
        openRotation = closedRotation * Quaternion.Euler(rotationAxis * openAngle);
    }

    // Fun��o p�blica para ser chamada para interagir com a porta
    public void InteractDoor()
    {
        isOpen = !isOpen; // Inverte o estado da porta

        // Mata qualquer anima��o DOTween anterior neste pivot para evitar conflitos
        DOTween.Kill(doorPivot);

        if (isOpen)
        {
            // Anima para a rota��o aberta
            doorPivot.DOLocalRotateQuaternion(openRotation, animationDuration).SetEase(easeType);
            Debug.Log($"Porta '{gameObject.name}' abrindo.");
            onDoorOpen?.Invoke(); // Chama o evento de abrir, se houver algo configurado
        }
        else
        {
            // Anima para a rota��o fechada
            doorPivot.DOLocalRotateQuaternion(closedRotation, animationDuration).SetEase(easeType);
            Debug.Log($"Porta '{gameObject.name}' fechando.");
            onDoorClose?.Invoke(); // Chama o evento de fechar
        }
    }

    // Para testes no editor (opcional)
    [ContextMenu("Test Interact Door")] // Adiciona um bot�o no menu de contexto do componente
    void TestInteract()
    {
        InteractDoor();
    }
}