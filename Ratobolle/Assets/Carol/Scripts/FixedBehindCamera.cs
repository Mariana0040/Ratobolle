using UnityEngine;

public class FixedBehindCamera : MonoBehaviour
{
    [Header("Alvo e Configurações")]
    [Tooltip("O Transform do objeto PAI do jogador (o que tem o Rigidbody e o Controller).")]
    [SerializeField] private Transform target;
    [Tooltip("Distância fixa que a câmera ficará atrás do jogador.")]
    [SerializeField] private float distance = 4.0f; // Ajuste conforme necessário
    [Tooltip("Altura fixa da câmera em relação à base do jogador.")]
    [SerializeField] private float height = 1.8f; // Ajuste para a altura do ombro/cabeça
    [Tooltip("Ajuste vertical no ponto para onde a câmera olha (centralizar o jogador).")]
    [SerializeField] private float lookAtHeightOffset = 1.2f; // Ajuste fino

    [Header("Suavização (Damping)")]
    [Tooltip("Suavização do movimento de POSIÇÃO da câmera (menor = mais suave). 0 para sem suavização.")]
    [SerializeField] private float positionDamping = 10.0f;
    [Tooltip("Suavização da ROTAÇÃO da câmera ao seguir o jogador (menor = mais suave). 0 para sem suavização.")]
    [SerializeField] private float rotationDamping = 10.0f;


    void LateUpdate() // Usar LateUpdate para câmeras
    {
        if (!target)
        {
            Debug.LogWarning("Alvo da câmera não definido!");
            return; // Sai se não houver alvo
        }

        // --- Calcula a Posição Desejada ---
        // Posição diretamente atrás do jogador, usando a rotação ATUAL do jogador.
        // target.forward é a direção para onde o jogador está olhando.
        // -target.forward é a direção diretamente atrás.
        // target.up é a direção para cima relativa ao jogador.
        Vector3 wantedPosition = target.position - (target.forward * distance) + (target.up * height);

        // --- Calcula a Rotação Desejada ---
        // Queremos que a câmera olhe na mesma direção GERAL que o jogador,
        // mas focando em um ponto ligeiramente acima da base dele.
        Vector3 lookAtPoint = target.position + (target.up * lookAtHeightOffset);
        // Calcula a rotação necessária para olhar para esse ponto a partir da 'wantedPosition'
        // No entanto, para garantir que fique *exatamente* atrás, vamos forçar a rotação Y
        // a ser a mesma do jogador e calcular o X a partir do LookAt.

        // Alternativa mais simples e direta para manter a rotação alinhada:
        // Define a rotação da câmera para olhar na mesma direção que o jogador.
        Quaternion wantedRotation = Quaternion.LookRotation(target.forward, target.up);


        // --- Aplica Suavização (Lerp/Slerp) ---

        // Posição
        if (positionDamping > 0)
        {
            transform.position = Vector3.Lerp(transform.position, wantedPosition, positionDamping * Time.deltaTime);
        }
        else
        {
            transform.position = wantedPosition; // Movimento instantâneo
        }

        // Rotação
        if (rotationDamping > 0)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, wantedRotation, rotationDamping * Time.deltaTime);
        }
        else
        {
            transform.rotation = wantedRotation; // Rotação instantânea
        }

        // // Alternativa usando LookAt (pode gerar leve oscilação lateral se a altura for diferente)
        // transform.LookAt(lookAtPoint);

    }

    // --- Gizmo Opcional ---
#if UNITY_EDITOR
    void OnDrawGizmosSelected()
    {
        if (target)
        {
            // Desenha a posição desejada
            Vector3 wantedPosition = target.position - (target.forward * distance) + (target.up * height);
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(wantedPosition, 0.2f);

            // Desenha uma linha da câmera para a posição desejada
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
