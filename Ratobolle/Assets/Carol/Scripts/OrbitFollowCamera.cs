using UnityEngine;

public class OrbitFollowCamera : MonoBehaviour
{
    [Header("Alvo e Configurações")]
    [Tooltip("O Transform do objeto PAI do jogador (o que tem o Rigidbody).")]
    [SerializeField] private Transform target;
    [Tooltip("Distância fixa da câmera para o jogador.")]
    [SerializeField] private float distance = 5.0f; // 5 unidades Unity
    [Tooltip("Altura fixa da câmera em relação ao jogador.")]
    [SerializeField] private float height = 1.5f; // Ajuste para visão mais horizontal
    [Tooltip("Ajuste vertical no ponto para onde a câmera olha.")]
    [SerializeField] private float lookAtHeightOffset = 0.8f; // Ajuste para centralizar o jogador

    [Header("Órbita com A/D")]
    [Tooltip("Velocidade com que a câmera orbita ao redor do jogador com A/D (graus por segundo).")]
    [SerializeField] private float orbitSpeed = 100f;

    [Header("Suavização (Damping)")]
    [Tooltip("Suavização do movimento de POSIÇÃO da câmera (menor = mais suave).")]
    [SerializeField] private float positionDamping = 8.0f;
    [Tooltip("Suavização da ROTAÇÃO da câmera ao olhar para o alvo (menor = mais suave).")]
    [SerializeField] private float rotationDamping = 10.0f; // Para suavizar o LookAt

    // Estado interno da câmera
    private float currentYRotation = 0f; // Rastreia o ângulo horizontal atual da câmera

    void Start()
    {
        // Inicializa a rotação Y baseado na posição inicial relativa ao target
        if (target != null)
        {
            Vector3 initialOffset = transform.position - target.position;
            // Calcula o ângulo Y inicial baseado no offset
            // Atan2 é bom para obter ângulos de vetores 2D (ignoramos Y aqui)
            currentYRotation = Mathf.Atan2(initialOffset.x, initialOffset.z) * Mathf.Rad2Deg;
        }
        else
        {
            currentYRotation = transform.eulerAngles.y; // Usa rotação atual se não houver target no Start
        }
    }


    void LateUpdate()
    {
        if (!target) return; // Sai se não houver alvo

        // --- Atualiza a Rotação de Órbita (baseado no input A/D) ---
        float horizontalInput = Input.GetAxis("Horizontal"); // Lê A/D
        // Adiciona a rotação de órbita ao ângulo atual
        currentYRotation += horizontalInput * orbitSpeed * Time.deltaTime;

        // --- Calcula a Rotação e Posição Desejadas ---
        // Cria a rotação desejada da câmera em torno do jogador
        Quaternion desiredRotation = Quaternion.Euler(0, currentYRotation, 0);
        // Calcula a posição desejada atrás e acima do jogador, usando a rotação de órbita
        // Multiplicamos a rotação pelo offset base (Vector3.forward * distance)
        Vector3 desiredPositionOffset = desiredRotation * Vector3.forward * distance;
        Vector3 wantedPosition = target.position - desiredPositionOffset + Vector3.up * height;

        // --- Aplica Suavização na Posição ---
        transform.position = Vector3.Lerp(transform.position, wantedPosition, positionDamping * Time.deltaTime);

        // --- Aplica Rotação Suave para Olhar para o Alvo ---
        // Calcula a rotação necessária para olhar para o ponto alvo
        Vector3 lookAtPoint = target.position + Vector3.up * lookAtHeightOffset;
        Quaternion lookRotation = Quaternion.LookRotation(lookAtPoint - transform.position);
        // Interpola suavemente a rotação atual da câmera para a rotação 'lookRotation'
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, rotationDamping * Time.deltaTime);
    }

    // --- Gizmo Opcional ---
#if UNITY_EDITOR
    void OnDrawGizmosSelected()
    {
        if (target)
        {
            // Desenha o ponto para onde a câmera está olhando
            Gizmos.color = Color.green;
            Vector3 lookAtPoint = target.position + Vector3.up * lookAtHeightOffset;
            Gizmos.DrawWireSphere(lookAtPoint, 0.1f);
            Gizmos.DrawLine(transform.position, lookAtPoint);

            // Desenha a posição desejada (calculada antes do Lerp)
            // Recalcula aqui apenas para o Gizmo
            Quaternion desiredRotation = Quaternion.Euler(0, currentYRotation, 0);
            Vector3 desiredPositionOffset = desiredRotation * Vector3.forward * distance;
            Vector3 wantedPosition = target.position - desiredPositionOffset + Vector3.up * height;
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(wantedPosition, 0.2f);
        }
    }
#endif
}