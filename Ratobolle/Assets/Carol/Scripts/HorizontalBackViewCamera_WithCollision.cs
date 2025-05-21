using UnityEngine;

public class HorizontalBackViewCamera_WithCollision : MonoBehaviour
{
    [Header("Alvo e Configura��es")]
    [Tooltip("O Transform do objeto PAI do jogador.")]
    [SerializeField] private Transform target;
    [Tooltip("Dist�ncia M�XIMA desejada da c�mera para o jogador.")]
    [SerializeField] private float desiredDistance = 4.0f;
    [Tooltip("Altura da c�mera em rela��o ao PIV� do jogador.")]
    [SerializeField] private float height = 1.2f;
    [Tooltip("Altura do PONTO no jogador para onde a c�mera olha.")]
    [SerializeField] private float lookAtHeightOffset = 1.2f;

    [Header("Colis�o da C�mera")]
    [Tooltip("Quais Layers devem ser consideradas obst�culos para a c�mera.")]
    [SerializeField] private LayerMask collisionLayers;
    [Tooltip("Offset para afastar a c�mera do ponto de colis�o.")]
    [SerializeField] private float collisionOffset = 0.2f;
    [Tooltip("Raio da esfera usada para checar colis�o (0 para Raycast simples).")]
    [SerializeField] private float collisionSphereRadius = 0.1f;

    [Header("Suaviza��o (Damping)")]
    [Tooltip("Suaviza��o do movimento de POSI��O da c�mera.")]
    [SerializeField] private float positionDamping = 10.0f;
    [Tooltip("Suaviza��o da ROTA��O da c�mera ao seguir o jogador.")]
    [SerializeField] private float rotationDamping = 10.0f;

    private Vector3 smoothedPosition; // Para suavizar a posi��o final

    void Start()
    {
        // Inicializa a posi��o suavizada para evitar um salto no in�cio
        if (target != null)
        {
            // Calcula a posi��o inicial ideal e define a smoothedPosition
            Vector3 initialWantedPosition = target.position - (target.forward * desiredDistance) + (target.up * height);
            smoothedPosition = initialWantedPosition; // Come�a na posi��o ideal
            transform.position = smoothedPosition; // Coloca a c�mera l� imediatamente

            // Define a rota��o inicial para olhar para o alvo
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

        // --- Posi��o Ideal da C�mera (sem colis�o) ---
        // Esta posi��o � calculada usando a rota��o ATUAL do target (jogador).
        Vector3 idealPosition = target.position - (target.forward * desiredDistance) + (target.up * height);

        // --- Checagem de Colis�o ---
        Vector3 rayOrigin = target.position + (target.up * lookAtHeightOffset); // Origem um pouco acima do piv� do jogador
        Vector3 rayDirection = idealPosition - rayOrigin;
        float maxRayDistance = rayDirection.magnitude; // Dist�ncia at� a posi��o ideal

        RaycastHit hit;
        Vector3 targetCameraPosition = idealPosition; // Come�a com a posi��o ideal

        if (maxRayDistance > 0.01f) // S� faz o cast se houver uma dist�ncia a ser percorrida
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
        else // Se a dist�ncia ideal j� � muito pequena, usa a posi��o ideal (ou a posi��o do target + offset m�nimo)
        {
            targetCameraPosition = idealPosition;
        }


        // --- Aplica Suaviza��o na Posi��o ---
        // Suaviza o movimento da c�mera para a targetCameraPosition (que foi ajustada pela colis�o)
        smoothedPosition = Vector3.Lerp(smoothedPosition, targetCameraPosition, positionDamping * Time.deltaTime);
        transform.position = smoothedPosition;


        // --- Rota��o Desejada (para olhar para o jogador) ---
        // A c�mera sempre tenta olhar para o ponto alvo no jogador.
        // A rota��o Y da c�mera ser� influenciada pela rota��o do jogador porque
        // a 'targetCameraPosition' � calculada usando 'target.forward'.
        Vector3 lookAtPoint = target.position + (target.up * lookAtHeightOffset);
        Quaternion wantedRotation;
        if (Vector3.Distance(transform.position, lookAtPoint) > 0.01f) // Evita LookRotation para o mesmo ponto
        {
            wantedRotation = Quaternion.LookRotation(lookAtPoint - transform.position, target.up);
        }
        else
        {
            wantedRotation = transform.rotation; // Mant�m rota��o atual se estiver muito perto
        }


        // --- Aplica Suaviza��o na Rota��o ---
        transform.rotation = Quaternion.Slerp(transform.rotation, wantedRotation, rotationDamping * Time.deltaTime);
    }

#if UNITY_EDITOR
    void OnDrawGizmosSelected()
    {
        if (target != null)
        {
            // Posi��o ideal (sem colis�o)
            Vector3 idealPosition = target.position - (target.forward * desiredDistance) + (target.up * height);
            Gizmos.color = Color.gray;
            Gizmos.DrawWireSphere(idealPosition, 0.15f);

            // Linha do target para a posi��o ideal
            Vector3 rayOrigin = target.position + (target.up * lookAtHeightOffset);
            Gizmos.DrawLine(rayOrigin, idealPosition);

            // Posi��o atual da c�mera (ap�s colis�o e suaviza��o)
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position, 0.2f);

            // Raio de colis�o (se estiver sendo usado)
            Gizmos.color = Color.red;
            Vector3 rayDirection = idealPosition - rayOrigin;
            float maxRayDistance = rayDirection.magnitude;
            if (maxRayDistance > 0.01f)
            {
                Gizmos.DrawRay(rayOrigin, rayDirection.normalized * maxRayDistance);
                if (collisionSphereRadius > 0.01f)
                {
                    // Desenha a esfera no final do cast se n�o houver colis�o
                    // ou no ponto de colis�o se houver (mais complexo de desenhar precisamente aqui sem o hit.point)
                    // Simplificando: desenha a esfera no final da dist�ncia m�xima do cast
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