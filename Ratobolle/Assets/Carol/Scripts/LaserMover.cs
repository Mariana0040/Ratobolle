using UnityEngine;
using static UnityEngine.UIElements.UxmlAttributeDescription;
public class LaserMover : MonoBehaviour
{
    [Header("Pontos de Movimento do Laser")]
    [Tooltip("Ponto A para onde o laser se move.")]
    [SerializeField] private Transform pointA;
    [Tooltip("Ponto B para onde o laser se move.")]
    [SerializeField] private Transform pointB;
    [Header("Configurações do Laser")]
    [Tooltip("Velocidade do laser.")]
    [SerializeField] private float speed = 3.0f;
    [Tooltip("Tempo de espera do laser em cada ponto (em segundos).")]
    [SerializeField] private float waitTimeAtPoints = 0.5f;
    [Tooltip("Quão perto o laser precisa chegar do ponto alvo para 'chegar'.")]
    [SerializeField] private float arrivalThreshold = 0.1f;

    [Header("Configurações de Respawn do Jogador")]
    [Tooltip("Ponto de respawn se o jogador for atingido enquanto o laser está indo de A para B (considerado 'frente' do laser).")]
    [SerializeField] private Transform respawnPointForwardDirection;
    [Tooltip("Ponto de respawn se o jogador for atingido enquanto o laser está indo de B para A (considerado 'costas' ou retorno do laser).")]
    [SerializeField] private Transform respawnPointBackwardDirection;
    [Tooltip("Tempo a ser perdido pelo jogador ao ser atingido (em segundos).")]
    [SerializeField] private float timePenalty = 5.0f; // Exemplo

    [Header("Debug")]
    [SerializeField] private bool logLaserState = false;

    // Estado interno
    private Transform currentTarget;
    private bool isMovingToB = true; // True se estiver indo de A para B (direção "frente")
    private bool isWaiting = false;
    private float waitTimer = 0f;

    // Rigidbody do laser (opcional, mas bom para movimento consistente se o laser tiver colisões físicas complexas)
    private Rigidbody rbLaser;

    void Awake()
    {
        rbLaser = GetComponent<Rigidbody>();
        if (rbLaser != null)
        {
            rbLaser.isKinematic = true; // Controlado por script
            rbLaser.useGravity = false;
        }
    }

    void Start()
    {
        if (pointA == null || pointB == null)
        {
            Debug.LogError($"Laser '{gameObject.name}': Pontos A e/ou B não definidos! Desativando.", this);
            enabled = false;
            return;
        }
        if (respawnPointForwardDirection == null || respawnPointBackwardDirection == null)
        {
            Debug.LogError($"Laser '{gameObject.name}': Pontos de Respawn não definidos! Desativando.", this);
            enabled = false;
            return;
        }

        // Começa no Ponto A, movendo para o Ponto B
        transform.position = pointA.position; // Posição inicial
        currentTarget = pointB;
        isMovingToB = true;

        if (logLaserState) Debug.Log($"Laser '{gameObject.name}' iniciado. Alvo: {currentTarget.name}. Direção: {(isMovingToB ? "A->B (Frente)" : "B->A (Costas)")}");
    }

    void FixedUpdate() // Usar FixedUpdate para movimento se tiver Rigidbody
    {
        if (!enabled || currentTarget == null) return;

        if (isWaiting)
        {
            waitTimer -= Time.fixedDeltaTime;
            if (waitTimer <= 0f)
            {
                isWaiting = false;
                SwitchTarget();
            }
            return;
        }

        // Movimento
        Vector3 currentPosition = (rbLaser != null) ? rbLaser.position : transform.position;
        Vector3 newPosition = Vector3.MoveTowards(currentPosition, currentTarget.position, speed * Time.fixedDeltaTime);

        if (rbLaser != null)
        {
            rbLaser.MovePosition(newPosition);
        }
        else
        {
            transform.position = newPosition;
        }

        // Checagem de chegada
        if (Vector3.Distance(newPosition, currentTarget.position) < arrivalThreshold)
        {
            if (rbLaser != null) rbLaser.MovePosition(currentTarget.position);
            else transform.position = currentTarget.position;

            isWaiting = true;
            waitTimer = waitTimeAtPoints;
            if (logLaserState) Debug.Log($"Laser '{gameObject.name}' chegou em {currentTarget.name}. Esperando.");
        }
    }

    void SwitchTarget()
    {
        isMovingToB = !isMovingToB; // Inverte a direção
        currentTarget = isMovingToB ? pointB : pointA;
        if (logLaserState) Debug.Log($"Laser '{gameObject.name}' trocou alvo para {currentTarget.name}. Nova direção: {(isMovingToB ? "A->B (Frente)" : "B->A (Costas)")}");
    }

    // Detecção de colisão com o jogador
    // O LASER precisa de um Collider (NÃO Trigger)
    // O JOGADOR precisa de um Rigidbody e um Collider (NÃO Trigger)
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player")) // Certifique-se que seu jogador tem a Tag "Player"
        {
            if (logLaserState) Debug.Log($"Laser '{gameObject.name}' colidiu com o Jogador '{collision.gameObject.name}'!");

            FakeRollCapsuleController playerController = collision.gameObject.GetComponent<FakeRollCapsuleController>();
            if (playerController != null)
            {
                Transform respawnTarget;
                // Determina para qual ponto de respawn enviar o jogador
                if (isMovingToB) // Laser indo de A para B (direção "frente" do laser)
                {
                    respawnTarget = respawnPointForwardDirection;
                    if (logLaserState) Debug.Log("Laser estava indo A->B. Usando Respawn 'Frente'.");
                }
                else // Laser indo de B para A (direção "costas" do laser)
                {
                    respawnTarget = respawnPointBackwardDirection;
                    if (logLaserState) Debug.Log("Laser estava indo B->A. Usando Respawn 'Costas'.");
                }

                // Chama uma função no script do jogador para lidar com o respawn e a penalidade
                playerController.HandleLaserHit(respawnTarget, timePenalty);
            }
            else
            {
                Debug.LogWarning($"Laser '{gameObject.name}' colidiu com objeto com tag 'Player', mas não encontrou o script 'FakeRollCapsuleController'.");
            }
        }
    }
#if UNITY_EDITOR
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        if (pointA != null) Gizmos.DrawWireSphere(pointA.position, 0.25f);
        if (pointB != null) Gizmos.DrawWireSphere(pointB.position, 0.25f);
        if (pointA != null && pointB != null) Gizmos.DrawLine(pointA.position, pointB.position);
        Gizmos.color = Color.cyan;
        if (respawnPointForwardDirection != null)
        {
            Gizmos.DrawWireCube(respawnPointForwardDirection.position, Vector3.one * 0.5f);
            UnityEditor.Handles.Label(respawnPointForwardDirection.position + Vector3.up * 0.3f, "Respawn Frente");
            if (pointA != null) Gizmos.DrawLine(pointA.position, respawnPointForwardDirection.position);
        }
        Gizmos.color = Color.magenta;
        if (respawnPointBackwardDirection != null)
        {
            Gizmos.DrawWireCube(respawnPointBackwardDirection.position, Vector3.one * 0.5f);
            UnityEditor.Handles.Label(respawnPointBackwardDirection.position + Vector3.up * 0.3f, "Respawn Costas");
            if (pointB != null) Gizmos.DrawLine(pointB.position, respawnPointBackwardDirection.position);
        }
    }
#endif
}