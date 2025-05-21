using UnityEngine;

public class FixedBehindCamera : MonoBehaviour
{
    [Header("Alvo e Configura��es")]
    [Tooltip("O Transform do objeto PAI do jogador (o que tem o Rigidbody e o Controller).")]
    [SerializeField] private Transform target;
    [Tooltip("Dist�ncia fixa que a c�mera ficar� atr�s do jogador.")]
    [SerializeField] private float distance = 4.0f; // Ajuste conforme necess�rio
    [Tooltip("Altura fixa da c�mera em rela��o � base do jogador.")]
    [SerializeField] private float height = 1.8f; // Ajuste para a altura do ombro/cabe�a
    [Tooltip("Ajuste vertical no ponto para onde a c�mera olha (centralizar o jogador).")]
    [SerializeField] private float lookAtHeightOffset = 1.2f; // Ajuste fino

    [Header("Suaviza��o (Damping)")]
    [Tooltip("Suaviza��o do movimento de POSI��O da c�mera (menor = mais suave). 0 para sem suaviza��o.")]
    [SerializeField] private float positionDamping = 10.0f;
    [Tooltip("Suaviza��o da ROTA��O da c�mera ao seguir o jogador (menor = mais suave). 0 para sem suaviza��o.")]
    [SerializeField] private float rotationDamping = 10.0f;


    void LateUpdate() // Usar LateUpdate para c�meras
    {
        if (!target)
        {
            Debug.LogWarning("Alvo da c�mera n�o definido!");
            return; // Sai se n�o houver alvo
        }

        // --- Calcula a Posi��o Desejada ---
        // Posi��o diretamente atr�s do jogador, usando a rota��o ATUAL do jogador.
        // target.forward � a dire��o para onde o jogador est� olhando.
        // -target.forward � a dire��o diretamente atr�s.
        // target.up � a dire��o para cima relativa ao jogador.
        Vector3 wantedPosition = target.position - (target.forward * distance) + (target.up * height);

        // --- Calcula a Rota��o Desejada ---
        // Queremos que a c�mera olhe na mesma dire��o GERAL que o jogador,
        // mas focando em um ponto ligeiramente acima da base dele.
        Vector3 lookAtPoint = target.position + (target.up * lookAtHeightOffset);
        // Calcula a rota��o necess�ria para olhar para esse ponto a partir da 'wantedPosition'
        // No entanto, para garantir que fique *exatamente* atr�s, vamos for�ar a rota��o Y
        // a ser a mesma do jogador e calcular o X a partir do LookAt.

        // Alternativa mais simples e direta para manter a rota��o alinhada:
        // Define a rota��o da c�mera para olhar na mesma dire��o que o jogador.
        Quaternion wantedRotation = Quaternion.LookRotation(target.forward, target.up);


        // --- Aplica Suaviza��o (Lerp/Slerp) ---

        // Posi��o
        if (positionDamping > 0)
        {
            transform.position = Vector3.Lerp(transform.position, wantedPosition, positionDamping * Time.deltaTime);
        }
        else
        {
            transform.position = wantedPosition; // Movimento instant�neo
        }

        // Rota��o
        if (rotationDamping > 0)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, wantedRotation, rotationDamping * Time.deltaTime);
        }
        else
        {
            transform.rotation = wantedRotation; // Rota��o instant�nea
        }

        // // Alternativa usando LookAt (pode gerar leve oscila��o lateral se a altura for diferente)
        // transform.LookAt(lookAtPoint);

    }

    // --- Gizmo Opcional ---
#if UNITY_EDITOR
    void OnDrawGizmosSelected()
    {
        if (target)
        {
            // Desenha a posi��o desejada
            Vector3 wantedPosition = target.position - (target.forward * distance) + (target.up * height);
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(wantedPosition, 0.2f);

            // Desenha uma linha da c�mera para a posi��o desejada
            Gizmos.DrawLine(transform.position, wantedPosition);

            // Desenha o ponto de LookAt
            Gizmos.color = Color.red;
            Vector3 lookAtPoint = target.position + (target.up * lookAtHeightOffset);
            Gizmos.DrawWireSphere(lookAtPoint, 0.1f);
            Gizmos.DrawLine(transform.position, lookAtPoint);

        }
    }
#endif
}
