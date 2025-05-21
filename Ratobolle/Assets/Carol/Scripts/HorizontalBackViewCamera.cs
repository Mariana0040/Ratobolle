using UnityEngine;

public class HorizontalBackViewCamera : MonoBehaviour
{
    [Header("Alvo e Configurações")]
    [Tooltip("O Transform do objeto PAI do jogador (o que tem o Rigidbody).")]
    [SerializeField] private Transform target;
    [Tooltip("Distância fixa que a câmera ficará atrás do jogador.")]
    [SerializeField] private float distance = 4.0f; // Ajuste para quão longe quer ficar
    [Tooltip("Altura da câmera em relação ao PIVÔ do jogador. Abaixe este valor para uma visão mais horizontal.")]
    [SerializeField] private float height = 1.2f; // <<< TENTE VALORES MAIS BAIXOS (ex: 1.0, 1.2, 1.5)
    [Tooltip("Altura do PONTO no jogador para onde a câmera olha. Ajuste para ficar próximo de 'Height' para uma visão mais reta.")]
    [SerializeField] private float lookAtHeightOffset = 1.2f; // <<< TENTE VALORES PRÓXIMOS A 'Height'

    [Header("Suavização (Damping)")]
    [Tooltip("Suavização do movimento de POSIÇÃO da câmera.")]
    [SerializeField] private float positionDamping = 10.0f;
    [Tooltip("Suavização da ROTAÇÃO da câmera ao seguir o jogador.")]
    [SerializeField] private float rotationDamping = 10.0f;

    void LateUpdate()
    {
        if (!target) return;

        // --- Posição Desejada ---
        // Calcula a posição atrás do jogador, usando a rotação ATUAL do jogador e a altura definida.
        Vector3 wantedPosition = target.position - (target.forward * distance) + (target.up * height);

        // --- Rotação Desejada ---
        // Calcula a rotação necessária para olhar para o ponto alvo (lookAtHeightOffset)
        Vector3 lookAtPoint = target.position + (target.up * lookAtHeightOffset);
        Quaternion wantedRotation = Quaternion.LookRotation(lookAtPoint - wantedPosition, target.up); // Usar target.up como referência de "cima"

        // --- Aplica Suavização ---
        transform.position = Vector3.Lerp(transform.position, wantedPosition, positionDamping * Time.deltaTime);
        transform.rotation = Quaternion.Slerp(transform.rotation, wantedRotation, rotationDamping * Time.deltaTime);
    }

    // --- Gizmo ---
#if UNITY_EDITOR
    void OnDrawGizmosSelected()
    {
        if (target)
        {
            // Posição desejada
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