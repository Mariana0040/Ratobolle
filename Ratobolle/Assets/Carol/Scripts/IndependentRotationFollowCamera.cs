using UnityEngine;

public class IndependentRotationFollowCamera : MonoBehaviour
{
    [Header("Alvo e Configura��es")]
    [Tooltip("O Transform do objeto PAI do jogador (o que tem o Rigidbody).")]
    [SerializeField] private Transform target;
    [Tooltip("Dist�ncia que a c�mera tenta manter do jogador.")]
    [SerializeField] private float distance = 5.0f;
    [Tooltip("Altura da c�mera em rela��o ao jogador.")]
    [SerializeField] private float height = 2.0f;
    [Tooltip("Ajuste vertical no ponto de foco.")]
    [SerializeField] private float lookAtHeightOffset = 1.0f;

    [Header("Suaviza��o (Damping)")]
    [Tooltip("Suaviza��o do movimento de POSI��O da c�mera (menor = mais suave).")]
    [SerializeField] private float positionDamping = 5.0f;
    [Tooltip("Suaviza��o da ROTA��O da c�mera ao olhar para o alvo (menor = mais suave).")]
    [SerializeField] private float rotationDamping = 10.0f; // Para suavizar o LookAt

    // Rota��o atual da c�mera - N�O ser� afetada pelo input A/D do jogador
    private Quaternion currentCameraRotation;

    void Start()
    {
        // Inicializa a rota��o da c�mera com sua rota��o inicial na cena
        if (target != null)
        {
            // Calcula uma rota��o inicial para olhar para o target
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

        // --- Calcula a Posi��o Desejada ---
        // A posi��o � calculada usando a rota��o ATUAL da pr�pria c�mera,
        // e n�o a rota��o do target.
        Vector3 directionFromTarget = currentCameraRotation * Vector3.back; // Dire��o atr�s baseado na rota��o da c�mera
        Vector3 wantedPosition = target.position + directionFromTarget * distance + Vector3.up * height; // Adiciona altura globalmente

        // --- Aplica Suaviza��o na Posi��o ---
        transform.position = Vector3.Lerp(transform.position, wantedPosition, positionDamping * Time.deltaTime);

        // --- Calcula e Aplica Rota��o Suave para Olhar para o Alvo ---
        // A c�mera SEMPRE tenta olhar para o jogador, mas a sua posi��o relativa
        // � determinada pela 'currentCameraRotation' que N�O muda com A/D.
        Vector3 lookAtPoint = target.position + Vector3.up * lookAtHeightOffset;
        Quaternion targetLookRotation = Quaternion.LookRotation(lookAtPoint - transform.position);

        // Interpola suavemente a rota��o atual da c�mera para a rota��o 'targetLookRotation'
        currentCameraRotation = Quaternion.Slerp(transform.rotation, targetLookRotation, rotationDamping * Time.deltaTime);
        transform.rotation = currentCameraRotation; // Aplica a rota��o suavizada
    }

    // --- Gizmo Opcional ---
#if UNITY_EDITOR
    void OnDrawGizmosSelected()
    {
        if (target)
        {
            // ... (Gizmos anteriores para Posi��o Desejada e LookAt) ...
            Vector3 lookAtPoint = target.position + Vector3.up * lookAtHeightOffset;
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(lookAtPoint, 0.1f);
            Gizmos.DrawLine(transform.position, lookAtPoint);

            // Recalcula wantedPosition para o gizmo
            Vector3 directionFromTarget = transform.rotation * Vector3.back; // Usa rota��o atual da c�mera para gizmo
            Vector3 wantedPosition = target.position + directionFromTarget * distance + Vector3.up * height;
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(wantedPosition, 0.2f);
        }
    }
#endif
}