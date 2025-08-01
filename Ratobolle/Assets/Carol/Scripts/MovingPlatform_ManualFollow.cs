using UnityEngine;

public class MovingPlatform_ManualFollow : MonoBehaviour
{
    [Header("Pontos de Movimento")]
    [SerializeField] private Transform pointA;
    [SerializeField] private Transform pointB;
    [Header("Configurações de Movimento")]
    [SerializeField] private float speed = 2.0f;
    [SerializeField] private float waitTimeAtPoints = 1.0f;
    [SerializeField] private float arrivalThreshold = 0.05f;
    [Header("Debug")]
    [SerializeField] private bool logStateChanges = true;

    private Transform currentTarget;
    private bool isMovingToB = true;
    private bool isWaiting = false;
    private float waitTimer = 0f;
    private Rigidbody rbPlatform;
    private Vector3 lastPosition; // Para calcular o delta de movimento

    void Awake()
    {
        rbPlatform = GetComponent<Rigidbody>();
        if (rbPlatform == null) { Debug.LogError($"'{gameObject.name}' PRECISA de Rigidbody. IsKinematic=true.", this); enabled = false; return; }
        rbPlatform.isKinematic = true;
        rbPlatform.useGravity = false;
    }

    void Start()
    {
        if (!enabled) return;
        if (pointA == null || pointB == null) { Debug.LogError($"Pontos não definidos para '{gameObject.name}'!", this); enabled = false; return; }
        if (pointA == transform || pointB == transform) { Debug.LogError($"Pontos não podem ser a plataforma '{gameObject.name}'!", this); enabled = false; return; }

        rbPlatform.position = pointA.position;
        lastPosition = rbPlatform.position; // Inicializa lastPosition
        currentTarget = pointB;
        isMovingToB = true;
        if (logStateChanges) Debug.Log($"Plataforma '{gameObject.name}' iniciando. Alvo: {currentTarget.name}.");
    }

    void FixedUpdate()
    {
        if (!enabled || currentTarget == null) return;

        // Armazena a posição ANTES de mover
        lastPosition = rbPlatform.position;

        if (isWaiting)
        {
            waitTimer -= Time.fixedDeltaTime;
            if (waitTimer <= 0f) { isWaiting = false; SwitchTarget(); }
            // Mesmo esperando, lastPosition deve ser a posição atual para o jogador não se mover
            // No entanto, como a plataforma não se move, o delta será zero.
            return;
        }

        Vector3 newPosition = Vector3.MoveTowards(rbPlatform.position, currentTarget.position, speed * Time.fixedDeltaTime);
        rbPlatform.MovePosition(newPosition);

        if (Vector3.Distance(newPosition, currentTarget.position) < arrivalThreshold)
        {
            rbPlatform.MovePosition(currentTarget.position); // Garante posição exata
            lastPosition = rbPlatform.position; // Atualiza lastPosition para o ponto final
            isWaiting = true;
            waitTimer = waitTimeAtPoints;
            if (logStateChanges) Debug.Log($"Plataforma '{gameObject.name}' chegou em {currentTarget.name}. Esperando.");
        }
    }

    void SwitchTarget()
    {
        isMovingToB = !isMovingToB;
        currentTarget = isMovingToB ? pointB : pointA;
        if (logStateChanges) Debug.Log($"Plataforma '{gameObject.name}' trocou alvo para: {currentTarget.name}.");
    }

    // Função para o jogador pegar o delta de movimento da plataforma
    public Vector3 GetMovementDelta()
    {
        if (isWaiting) return Vector3.zero; // Se está parada, não há delta
        return rbPlatform.position - lastPosition;
    }

#if UNITY_EDITOR
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        float gizmoSphereRadius = 0.25f;
        if (pointA != null) { Gizmos.DrawWireSphere(pointA.position, gizmoSphereRadius); UnityEditor.Handles.Label(pointA.position + Vector3.up * 0.5f, "Ponto A"); }
        if (pointB != null) { Gizmos.DrawWireSphere(pointB.position, gizmoSphereRadius); UnityEditor.Handles.Label(pointB.position + Vector3.up * 0.5f, "Ponto B"); }
        if (pointA != null && pointB != null) { Gizmos.DrawLine(pointA.position, pointB.position); }

        if (Application.isPlaying && currentTarget != null && enabled)
        {
            Gizmos.color = Color.magenta;
            Vector3 platformPos = rbPlatform != null ? rbPlatform.position : transform.position;
            Gizmos.DrawLine(platformPos, currentTarget.position);
            Gizmos.DrawWireSphere(currentTarget.position, gizmoSphereRadius + 0.1f);
            UnityEditor.Handles.Label(currentTarget.position + Vector3.up * 0.7f, "ALVO ATUAL");
        }
    }
#endif
}