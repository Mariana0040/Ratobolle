using UnityEngine;

public class HorizontalBackViewCamera : MonoBehaviour
{
    [Header("Alvo e Configura��es")]
    [Tooltip("O Transform do objeto PAI do jogador (o que tem o Rigidbody).")]
    [SerializeField] private Transform target;
    [Tooltip("Dist�ncia fixa que a c�mera ficar� atr�s do jogador.")]
    [SerializeField] private float distance = 4.0f; // Ajuste para qu�o longe quer ficar
    [Tooltip("Altura da c�mera em rela��o ao PIV� do jogador. Abaixe este valor para uma vis�o mais horizontal.")]
    [SerializeField] private float height = 1.2f; // <<< TENTE VALORES MAIS BAIXOS (ex: 1.0, 1.2, 1.5)
    [Tooltip("Altura do PONTO no jogador para onde a c�mera olha. Ajuste para ficar pr�ximo de 'Height' para uma vis�o mais reta.")]
    [SerializeField] private float lookAtHeightOffset = 1.2f; // <<< TENTE VALORES PR�XIMOS A 'Height'

    [Header("Suaviza��o (Damping)")]
    [Tooltip("Suaviza��o do movimento de POSI��O da c�mera.")]
    [SerializeField] private float positionDamping = 10.0f;
    [Tooltip("Suaviza��o da ROTA��O da c�mera ao seguir o jogador.")]
    [SerializeField] private float rotationDamping = 10.0f;

    void LateUpdate()
    {
        if (!target) return;

        // --- Posi��o Desejada ---
        // Calcula a posi��o atr�s do jogador, usando a rota��o ATUAL do jogador e a altura definida.
        Vector3 wantedPosition = target.position - (target.forward * distance) + (target.up * height);

        // --- Rota��o Desejada ---
        // Calcula a rota��o necess�ria para olhar para o ponto alvo (lookAtHeightOffset)
        Vector3 lookAtPoint = target.position + (target.up * lookAtHeightOffset);
        Quaternion wantedRotation = Quaternion.LookRotation(lookAtPoint - wantedPosition, target.up); // Usar target.up como refer�ncia de "cima"

        // --- Aplica Suaviza��o ---
        transform.position = Vector3.Lerp(transform.position, wantedPosition, positionDamping * Time.deltaTime);
        transform.rotation = Quaternion.Slerp(transform.rotation, wantedRotation, rotationDamping * Time.deltaTime);
    }

    // --- Gizmo ---
#if UNITY_EDITOR
    void OnDrawGizmosSelected()
    {
        if (target)
        {
            // Posi��o desejada
            Vector3 wantedPosition = target.position - (target.forward * distance) + (target.up * height);
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(wantedPosition, 0.1f);
            Gizmos.DrawLine(transform.position, wantedPosition);

            // Ponto de LookAt
            Gizmos.color = Color.red;
            Vector3 lookAtPoint = target.position + (target.up * lookAtHeightOffset);
            Gizmos.DrawWireSphere(lookAtPoint, 0.1f);
            Gizmos.DrawLine(transform.position, lookAtPoint);
        }
    }
#endif
}