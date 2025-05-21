using UnityEngine;

public class OrbitFollowCamera : MonoBehaviour
{
    [Header("Alvo e Configura��es")]
    [Tooltip("O Transform do objeto PAI do jogador (o que tem o Rigidbody).")]
    [SerializeField] private Transform target;
    [Tooltip("Dist�ncia fixa da c�mera para o jogador.")]
    [SerializeField] private float distance = 5.0f; // 5 unidades Unity
    [Tooltip("Altura fixa da c�mera em rela��o ao jogador.")]
    [SerializeField] private float height = 1.5f; // Ajuste para vis�o mais horizontal
    [Tooltip("Ajuste vertical no ponto para onde a c�mera olha.")]
    [SerializeField] private float lookAtHeightOffset = 0.8f; // Ajuste para centralizar o jogador

    [Header("�rbita com A/D")]
    [Tooltip("Velocidade com que a c�mera orbita ao redor do jogador com A/D (graus por segundo).")]
    [SerializeField] private float orbitSpeed = 100f;

    [Header("Suaviza��o (Damping)")]
    [Tooltip("Suaviza��o do movimento de POSI��O da c�mera (menor = mais suave).")]
    [SerializeField] private float positionDamping = 8.0f;
    [Tooltip("Suaviza��o da ROTA��O da c�mera ao olhar para o alvo (menor = mais suave).")]
    [SerializeField] private float rotationDamping = 10.0f; // Para suavizar o LookAt

    // Estado interno da c�mera
    private float currentYRotation = 0f; // Rastreia o �ngulo horizontal atual da c�mera

    void Start()
    {
        // Inicializa a rota��o Y baseado na posi��o inicial relativa ao target
        if (target != null)
        {
            Vector3 initialOffset = transform.position - target.position;
            // Calcula o �ngulo Y inicial baseado no offset
            // Atan2 � bom para obter �ngulos de vetores 2D (ignoramos Y aqui)
            currentYRotation = Mathf.Atan2(initialOffset.x, initialOffset.z) * Mathf.Rad2Deg;
        }
        else
        {
            currentYRotation = transform.eulerAngles.y; // Usa rota��o atual se n�o houver target no Start
        }
    }


    void LateUpdate()
    {
        if (!target) return; // Sai se n�o houver alvo

        // --- Atualiza a Rota��o de �rbita (baseado no input A/D) ---
        float horizontalInput = Input.GetAxis("Horizontal"); // L� A/D
        // Adiciona a rota��o de �rbita ao �ngulo atual
        currentYRotation += horizontalInput * orbitSpeed * Time.deltaTime;

        // --- Calcula a Rota��o e Posi��o Desejadas ---
        // Cria a rota��o desejada da c�mera em torno do jogador
        Quaternion desiredRotation = Quaternion.Euler(0, currentYRotation, 0);
        // Calcula a posi��o desejada atr�s e acima do jogador, usando a rota��o de �rbita
        // Multiplicamos a rota��o pelo offset base (Vector3.forward * distance)
        Vector3 desiredPositionOffset = desiredRotation * Vector3.forward * distance;
        Vector3 wantedPosition = target.position - desiredPositionOffset + Vector3.up * height;

        // --- Aplica Suaviza��o na Posi��o ---
        transform.position = Vector3.Lerp(transform.position, wantedPosition, positionDamping * Time.deltaTime);

        // --- Aplica Rota��o Suave para Olhar para o Alvo ---
        // Calcula a rota��o necess�ria para olhar para o ponto alvo
        Vector3 lookAtPoint = target.position + Vector3.up * lookAtHeightOffset;
        Quaternion lookRotation = Quaternion.LookRotation(lookAtPoint - transform.position);
        // Interpola suavemente a rota��o atual da c�mera para a rota��o 'lookRotation'
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, rotationDamping * Time.deltaTime);
    }

    // --- Gizmo Opcional ---
#if UNITY_EDITOR
    void OnDrawGizmosSelected()
    {
        if (target)
        {
            // Desenha o ponto para onde a c�mera est� olhando
            Gizmos.color = Color.green;
            Vector3 lookAtPoint = target.position + Vector3.up * lookAtHeightOffset;
            Gizmos.DrawWireSphere(lookAtPoint, 0.1f);
            Gizmos.DrawLine(transform.position, lookAtPoint);

            // Desenha a posi��o desejada (calculada antes do Lerp)
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