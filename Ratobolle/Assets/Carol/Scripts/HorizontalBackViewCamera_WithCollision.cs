using UnityEngine;

public class HorizontalBackViewCamera_WithCollision : MonoBehaviour
{
    [Header("Alvo e Configurações")]
    [Tooltip("O Transform do objeto PAI do jogador.")]
    [SerializeField] private Transform target;
    [Tooltip("Distância MÁXIMA desejada da câmera para o jogador.")]
    [SerializeField] private float desiredDistance = 4.0f;
    [Tooltip("Altura da câmera em relação ao PIVÔ do jogador.")]
    [SerializeField] private float height = 1.2f;
    [Tooltip("Altura do PONTO no jogador para onde a câmera olha.")]
    [SerializeField] private float lookAtHeightOffset = 1.2f;

    [Header("Colisão da Câmera")]
    [Tooltip("Quais Layers devem ser consideradas obstáculos para a câmera.")]
    [SerializeField] private LayerMask collisionLayers;
    [Tooltip("Offset para afastar a câmera do ponto de colisão.")]
    [SerializeField] private float collisionOffset = 0.2f;
    [Tooltip("Raio da esfera usada para checar colisão (0 para Raycast simples).")]
    [SerializeField] private float collisionSphereRadius = 0.1f;

    [Header("Suavização (Damping)")]
    [Tooltip("Suavização do movimento de POSIÇÃO da câmera.")]
    [SerializeField] private float positionDamping = 10.0f;
    [Tooltip("Suavização da ROTAÇÃO da câmera ao seguir o jogador.")]
    [SerializeField] private float rotationDamping = 10.0f;

    private Vector3 smoothedPosition; // Para suavizar a posição final

    void Start()
    {
        // Inicializa a posição suavizada para evitar um salto no início
        if (target != null)
        {
            // Calcula a posição inicial ideal e define a smoothedPosition
            Vector3 initialWantedPosition = target.position - (target.forward * desiredDistance) + (target.up * height);
            smoothedPosition = initialWantedPosition; // Começa na posição ideal
            transform.position = smoothedPosition; // Coloca a câmera lá imediatamente

            // Define a rotação inicial para olhar para o alvo
            Vector3 lookAtPoint = target.position + (target.up * lookAtHeightOffset);
            if (initialWantedPosition != lookAtPoint) // Evita LookRotation de zero vector
            {
                transform.rotation = Quaternion.LookRotation(lookAtPoint - initialWantedPosition, target.up);
            }
        }
        else
        {
            smoothedPosition = transform.position;
        }
    }


    void LateUpdate()
    {
        if (!target) return;

        // --- Posição Ideal da Câmera (sem colisão) ---
        // Esta posição é calculada usando a rotação ATUAL do target (jogador).
        Vector3 idealPosition = target.position - (target.forward * desiredDistance) + (target.up * height);

        // --- Checagem de Colisão ---
        Vector3 rayOrigin = target.position + (target.up * lookAtHeightOffset); // Origem um pouco acima do pivô do jogador
        Vector3 rayDirection = idealPosition - rayOrigin;
        float maxRayDistance = rayDirection.magnitude; // Distância até a posição ideal

        RaycastHit hit;
        Vector3 targetCameraPosition = idealPosition; // Começa com a posição ideal

        if (maxRayDistance > 0.01f) // Só faz o cast se houver uma distância a ser percorrida
        {
            if (collisionSphereRadius > 0.01f)
            {
                if (Physics.SphereCast(rayOrigin, collisionSphereRadius, rayDirection.normalized, out hit, maxRayDistance, collisionLayers, QueryTriggerInteraction.Ignore))
                {
                    targetCameraPosition = rayOrigin + rayDirection.normalized * (hit.distance - collisionOffset);
                }
            }
            else
            {
                if (Physics.Raycast(rayOrigin, rayDirection.normalized, out hit, maxRayDistance, collisionLayers, QueryTriggerInteraction.Ignore))
                {
                    targetCameraPosition = rayOrigin + rayDirection.normalized * (hit.distance - collisionOffset);
                }
            }
        }
        else // Se a distância ideal já é muito pequena, usa a posição ideal (ou a posição do target + offset mínimo)
        {
            targetCameraPosition = idealPosition;
        }


        // --- Aplica Suavização na Posição ---
        // Suaviza o movimento da câmera para a targetCameraPosition (que foi ajustada pela colisão)
        smoothedPosition = Vector3.Lerp(smoothedPosition, targetCameraPosition, positionDamping * Time.deltaTime);
        transform.position = smoothedPosition;


        // --- Rotação Desejada (para olhar para o jogador) ---
        // A câmera sempre tenta olhar para o ponto alvo no jogador.
        // A rotação Y da câmera será influenciada pela rotação do jogador porque
        // a 'targetCameraPosition' é calculada usando 'target.forward'.
        Vector3 lookAtPoint = target.position + (target.up * lookAtHeightOffset);
        Quaternion wantedRotation;
        if (Vector3.Distance(transform.position, lookAtPoint) > 0.01f) // Evita LookRotation para o mesmo ponto
        {
            wantedRotation = Quaternion.LookRotation(lookAtPoint - transform.position, target.up);
        }
        else
        {
            wantedRotation = transform.rotation; // Mantém rotação atual se estiver muito perto
        }


        // --- Aplica Suavização na Rotação ---
        transform.rotation = Quaternion.Slerp(transform.rotation, wantedRotation, rotationDamping * Time.deltaTime);
    }

#if UNITY_EDITOR
    void OnDrawGizmosSelected()
    {
        if (target != null)
        {
            // Posição ideal (sem colisão)
            Vector3 idealPosition = target.position - (target.forward * desiredDistance) + (target.up * height);
            Gizmos.color = Color.gray;
            Gizmos.DrawWireSphere(idealPosition, 0.15f);

            // Linha do target para a posição ideal
            Vector3 rayOrigin = target.position + (target.up * lookAtHeightOffset);
            Gizmos.DrawLine(rayOrigin, idealPosition);

            // Posição atual da câmera (após colisão e suavização)
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position, 0.2f);

            // Raio de colisão (se estiver sendo usado)
            Gizmos.color = Color.red;
            Vector3 rayDirection = idealPosition - rayOrigin;
            float maxRayDistance = rayDirection.magnitude;
            if (maxRayDistance > 0.01f)
            {
                Gizmos.DrawRay(rayOrigin, rayDirection.normalized * maxRayDistance);
                if (collisionSphereRadius > 0.01f)
                {
                    // Desenha a esfera no final do cast se não houver colisão
                    // ou no ponto de colisão se houver (mais complexo de desenhar precisamente aqui sem o hit.point)
                    // Simplificando: desenha a esfera no final da distância máxima do cast
                    Gizmos.DrawWireSphere(rayOrigin + rayDirection.normalized * maxRayDistance, collisionSphereRadius);
                }
            }


            // Ponto de LookAt
            Gizmos.color = Color.green;
            Vector3 lookAtPoint = target.position + (target.up * lookAtHeightOffset);
            Gizmos.DrawWireSphere(lookAtPoint, 0.1f);
            Gizmos.DrawLine(transform.position, lookAtPoint);
        }
    }
#endif
}