using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    [Tooltip("Ponto A para onde a plataforma se move.")]
    [SerializeField] private Transform pointA;
    [Tooltip("Ponto B para onde a plataforma se move.")]
    [SerializeField] private Transform pointB;
    [Tooltip("Velocidade da plataforma.")]
    [SerializeField] private float speed = 2.0f;
    [Tooltip("Tempo de espera em cada ponto (em segundos).")]
    [SerializeField] private float waitTime = 1.0f;

    [Header("Debug")]
    [SerializeField] private bool logStateChanges = false; // Para ajudar a debugar

    private Transform currentTarget;
    private float waitTimer;
    private bool isMovingToB = true; // True se estiver indo para B, False se estiver indo para A
    private bool isWaiting = false;

    void Start()
    {
        if (pointA == null || pointB == null)
        {
            Debug.LogError($"Pontos A ({pointA}) e/ou B ({pointB}) não definidos para a plataforma móvel '{gameObject.name}'! Desativando.", this);
            enabled = false;
            return;
        }

        // Garante que os pontos não são o mesmo objeto da plataforma
        if (pointA == transform || pointB == transform)
        {
            Debug.LogError($"Pontos A e/ou B não podem ser a própria plataforma '{gameObject.name}'! Use GameObjects vazios para os pontos. Desativando.", this);
            enabled = false;
            return;
        }


        // Define a posição inicial da plataforma para o Ponto A
        transform.position = pointA.position;
        currentTarget = pointB; // Começa movendo para o Ponto B
        isMovingToB = true;
        waitTimer = 0; // Não espera no início, a menos que queira
        isWaiting = false;

        if (logStateChanges) Debug.Log($"Plataforma '{gameObject.name}' iniciando em A, movendo para B.");
    }

    void Update()
    {
        if (pointA == null || pointB == null) return; // Segurança extra

        if (isWaiting)
        {
            waitTimer -= Time.deltaTime;
            if (waitTimer <= 0)
            {
                isWaiting = false;
                // Troca o alvo após esperar
                SwitchTarget();
            }
            return; // Não se move enquanto espera
        }

        // Move a plataforma em direção ao alvo atual
        // Garantir que currentTarget foi definido
        if (currentTarget == null)
        {
            // Isso não deveria acontecer se Start() rodou corretamente
            Debug.LogError($"currentTarget é nulo em Update para '{gameObject.name}'. Reiniciando alvo.", this);
            currentTarget = isMovingToB ? pointB : pointA;
            if (currentTarget == null) return; // Ainda nulo, problema maior.
        }

        transform.position = Vector3.MoveTowards(transform.position, currentTarget.position, speed * Time.deltaTime);

        // Verifica se chegou MUITO PERTO do alvo
        // Aumentar um pouco a tolerância pode ajudar se estiver "pulando" o ponto
        if (Vector3.Distance(transform.position, currentTarget.position) < 5f) // Tolerância de 5cm
        {

            isWaiting = true;
            waitTimer = waitTime;
            if (logStateChanges) Debug.Log($"Plataforma '{gameObject.name}' chegou em {currentTarget.name}. Esperando por {waitTime}s.");
        }
    }

    void SwitchTarget()
    {
        if (isMovingToB) // Estava indo para B, agora vai para A
        {
            currentTarget = pointA;
            isMovingToB = false;
            if (logStateChanges) Debug.Log($"Plataforma '{gameObject.name}' mudando alvo para A.");
        }
        else // Estava indo para A, agora vai para B
        {
            currentTarget = pointB;
            isMovingToB = true;
            if (logStateChanges) Debug.Log($"Plataforma '{gameObject.name}' mudando alvo para B.");
        }

        // Segurança extra para caso os pontos sejam nulos neste momento (não deveria acontecer)
        if (currentTarget == null)
        {
            Debug.LogError($"Falha ao trocar alvo para '{gameObject.name}', um dos pontos é nulo.", this);
            enabled = false;
        }
    }


    // Mantém a lógica de "grudar" o jogador
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.transform.SetParent(transform, true); // true para manter a posição/rotação mundial
        }
    }

    void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.transform.SetParent(null);
        }
    }

    // Gizmos permanecem úteis
#if UNITY_EDITOR
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan; // Cor geral para a plataforma
        if (pointA != null && pointB != null)
        {
            Gizmos.DrawLine(pointA.position, pointB.position);
        }

        if (pointA != null)
        {
            Gizmos.color = isMovingToB ? Color.gray : Color.green; // Verde se está indo para A
            Gizmos.DrawWireSphere(pointA.position, 0.3f);
            UnityEditor.Handles.Label(pointA.position + Vector3.up * 0.5f, "Ponto A");
        }
        if (pointB != null)
        {
            Gizmos.color = isMovingToB ? Color.red : Color.gray; // Vermelho se está indo para B
            Gizmos.DrawWireSphere(pointB.position, 0.3f);
            UnityEditor.Handles.Label(pointB.position + Vector3.up * 0.5f, "Ponto B");
        }

        // Mostra o alvo atual
        if (Application.isPlaying && currentTarget != null)
        {
            Gizmos.color = Color.magenta;
            Gizmos.DrawLine(transform.position, currentTarget.position);
            Gizmos.DrawWireSphere(currentTarget.position, 0.4f); // Maior para destacar
            UnityEditor.Handles.Label(currentTarget.position + Vector3.up * 0.7f, "ALVO ATUAL");
        }
    }
#endif
}