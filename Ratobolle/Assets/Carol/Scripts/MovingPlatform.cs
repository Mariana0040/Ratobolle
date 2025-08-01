// MovingPlatform.cs
using UnityEngine;

public class MovingPlatform : MonoBehaviour
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
        currentTarget = pointB;
        isMovingToB = true;
    }

    void FixedUpdate()
    {
        if (!enabled || currentTarget == null) return;
        if (isWaiting)
        {
            waitTimer -= Time.fixedDeltaTime;
            if (waitTimer <= 0f) { isWaiting = false; SwitchTarget(); }
            return;
        }
        Vector3 newPosition = Vector3.MoveTowards(rbPlatform.position, currentTarget.position, speed * Time.fixedDeltaTime);
        rbPlatform.MovePosition(newPosition);
        if (Vector3.Distance(newPosition, currentTarget.position) < arrivalThreshold)
        {
            rbPlatform.MovePosition(currentTarget.position);
            isWaiting = true;
            waitTimer = waitTimeAtPoints;
            if (logStateChanges) Debug.Log($"Plataforma '{gameObject.name}' chegou em {currentTarget.name}.");
        }
    }

    void SwitchTarget()
    {
        isMovingToB = !isMovingToB;
        currentTarget = isMovingToB ? pointB : pointA;
        if (logStateChanges) Debug.Log($"Plataforma '{gameObject.name}' trocou alvo para: {currentTarget.name}.");
    }

    // Quando o jogador ENCOSTA na plataforma (colisão física)
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            ContactPoint contact = collision.contacts[0];
            // Verifica se o jogador colidiu com a parte de CIMA da plataforma
            if (Vector3.Dot(contact.normal, transform.up) > 0.5f) // Ajuste este valor se necessário (0.1 a 0.9)
            {
                FakeRollCapsuleController playerController = collision.gameObject.GetComponent<FakeRollCapsuleController>();
                if (playerController != null)
                {
                    // Avisa o jogador que ele deve grudar e parar de se mover por conta própria
                    playerController.AttachToPlatform(transform); // Passa o transform DESTA plataforma
                    if (logStateChanges) Debug.Log($"Jogador '{collision.gameObject.name}' PISOU EM CIMA. Avisando para grudar.");
                }
            }
        }
    }

    // Por enquanto, não vamos desgrudar automaticamente no OnCollisionExit para o teste de "estátua"
    // void OnCollisionExit(Collision collision) { /* ... */ }

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