using UnityEngine;

public class IndependentRotationFollowCamera : MonoBehaviour
{
    [Header("Alvo e Configurações")]
    [Tooltip("O Transform do objeto PAI do jogador (o que tem o Rigidbody).")]
    [SerializeField] private Transform target;
    [Tooltip("Distância que a câmera tenta manter do jogador.")]
    [SerializeField] private float distance = 5.0f;
    [Tooltip("Altura da câmera em relação ao jogador.")]
    [SerializeField] private float height = 2.0f;
    [Tooltip("Ajuste vertical no ponto de foco.")]
    [SerializeField] private float lookAtHeightOffset = 1.0f;

    [Header("Suavização (Damping)")]
    [Tooltip("Suavização do movimento de POSIÇÃO da câmera (menor = mais suave).")]
    [SerializeField] private float positionDamping = 5.0f;
    [Tooltip("Suavização da ROTAÇÃO da câmera ao olhar para o alvo (menor = mais suave).")]
    [SerializeField] private float rotationDamping = 10.0f; // Para suavizar o LookAt

    // Rotação atual da câmera - NÃO será afetada pelo input A/D do jogador
    private Quaternion currentCameraRotation;

    void Start()
    {
        // Inicializa a rotação da câmera com sua rotação inicial na cena
        if (target != null)
        {
            // Calcula uma rotação inicial para olhar para o target
            Vector3 lookAtPoint = target.position + Vector3.up * lookAtHeightOffset;
            currentCameraRotation = Quaternion.LookRotation(lookAtPoint - transform.position);
            // Aplica imediatamente para evitar um pulo no primeiro frame
            transform.rotation = currentCameraRotation;
        }
        else
        {
            currentCameraRotation = transform.rotation;
        }
    }


    void LateUpdate()
    {
        if (!target) return;

        // --- Calcula a Posição Desejada ---
        // A posição é calculada usando a rotação ATUAL da própria câmera,
        // e não a rotação do target.
        Vector3 directionFromTarget = currentCameraRotation * Vector3.back; // Direção atrás baseado na rotação da câmera
        Vector3 wantedPosition = target.position + directionFromTarget * distance + Vector3.up * height; // Adiciona altura globalmente

        // --- Aplica Suavização na Posição ---
        transform.position = Vector3.Lerp(transform.position, wantedPosition, positionDamping * Time.deltaTime);

        // --- Calcula e Aplica Rotação Suave para Olhar para o Alvo ---
        // A câmera SEMPRE tenta olhar para o jogador, mas a sua posição relativa
        // é determinada pela 'currentCameraRotation' que NÃO muda com A/D.
        Vector3 lookAtPoint = target.position + Vector3.up * lookAtHeightOffset;
        Quaternion targetLookRotation = Quaternion.LookRotation(lookAtPoint - transform.position);

        // Interpola suavemente a rotação atual da câmera para a rotação 'targetLookRotation'
        currentCameraRotation = Quaternion.Slerp(transform.rotation, targetLookRotation, rotationDamping * Time.deltaTime);
        transform.rotation = currentCameraRotation; // Aplica a rotação suavizada
    }

    // --- Gizmo Opcional ---
#if UNITY_EDITOR
    void OnDrawGizmosSelected()
    {
        if (target)
        {
            // ... (Gizmos anteriores para Posição Desejada e LookAt) ...
            Vector3 lookAtPoint = target.position + Vector3.up * lookAtHeightOffset;
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(lookAtPoint, 0.1f);
            Gizmos.DrawLine(transform.position, lookAtPoint);

            // Recalcula wantedPosition para o gizmo
            Vector3 directionFromTarget = transform.rotation * Vector3.back; // Usa rotação atual da câmera para gizmo
            Vector3 wantedPosition = target.position + directionFromTarget * distance + Vector3.up * height;
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(wantedPosition, 0.2f);
        }
    }
#endif
}