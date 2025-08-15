using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(CapsuleCollider))]
public class GorditoController : MonoBehaviour
{
    [Header("Movimento")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float turnSpeed = 180f;

    [Header("Pulo/Impulso Orientado")]
    [SerializeField] private float upwardImpulseForce = 10f;
    [SerializeField] private float forwardImpulseBoost = 5f;
    [SerializeField] private int maxJumps = 2;
    [Range(0f, 1f)][SerializeField] private float airControlFactor = 0.5f; // Aumentei um pouco o padrão

    [Header("Rolagem Visual")]
    [SerializeField] private Transform visualMeshTransform;
    [SerializeField] private float maxVisualRollSpeed = 450f;
    [SerializeField] private float visualRollDamping = 5f;

    [Header("Verificação de Chão")]
    [SerializeField] private float groundCheckDistance = 0.15f;
    [SerializeField] private LayerMask groundLayerMask;
    [SerializeField] private LayerMask movingPlatformSurfaceLayer;

    [Header("Configurações de Reset")]
    [SerializeField] private Transform respawnPoint;
    [SerializeField] private LayerMask resetZoneLayer;

    [Header("Interação com Plataforma Móvel")]
    [Tooltip("Tag do objeto Trigger da plataforma que faz o jogador 'grudar' (StickZone).")]
    [SerializeField] private string platformStickTag = "PlatformStickZone"; // Tag da StickZone

    // Componentes e Estado Interno
    private Rigidbody rb;
    private CapsuleCollider capsuleCollider;
    private bool isGrounded = false;
    private int jumpsRemaining = 0;
    private float currentVisualRollSpeed = 0f;

    // Input
    private float inputForward = 0f;
    private float inputTurn = 0f;
    private bool jumpInputFlag = false;

    // Estado da Plataforma
    private Transform currentAttachedPlatform = null;
    private bool isInsideStickZone = false; // O jogador está DENTRO do trigger da StickZone?

    // --- NOVO PARA SEGUIR PLATAFORMA MANUALMENTE ---
    private MovingPlatform_ManualFollow currentStandingPlatform = null;
    private Vector3 platformMovementDelta = Vector3.zero;


    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        capsuleCollider = GetComponent<CapsuleCollider>();
        if (visualMeshTransform == null) { Debug.LogError("Visual Mesh Transform não definido!", this); enabled = false; return; }
        if (groundLayerMask.value == 0) { Debug.LogWarning("Ground Layer Mask não definida."); }
        if (movingPlatformSurfaceLayer.value == 0) { Debug.LogWarning("Moving Platform Surface Layer não definida."); }
        jumpsRemaining = maxJumps;
    }

    void Update()
    {
        // Inputs são sempre lidos
        inputForward = Input.GetAxis("Vertical");
        inputTurn = Input.GetAxis("Horizontal");
        if (Input.GetButtonDown("Jump"))
        {
            jumpInputFlag = true;
        }

        LayerMask combinedGroundCheckLayers = groundLayerMask | movingPlatformSurfaceLayer;
        CheckIfGrounded(combinedGroundCheckLayers);

        if (isGrounded)
        {
            jumpsRemaining = maxJumps;
            // Se aterrissou e NÃO está mais na StickZone (pulou para fora)
            if (currentAttachedPlatform != null && !isInsideStickZone)
            {
                Debug.Log("Aterrissou FORA da StickZone da plataforma. Desgrudando.");
                DetachFromPlatform();
            }
        }
        else // Está no ar
        {
            // Se estava grudado, mas agora está no ar E fora da StickZone (pulou para longe ou caiu)
            if (currentAttachedPlatform != null && !isInsideStickZone)
            {
                Debug.Log("No ar e FORA da StickZone. Desgrudando.");
                DetachFromPlatform();
            }
        }

        HandleVisualRoll();
    }

    void FixedUpdate()
    {
        // Movimento e pulo são sempre processados.
        // O parentesco com a plataforma cuida de "carregar" o jogador.
        HandleRotation();
        HandleMovement(isGrounded ? 1f : airControlFactor);
        HandleJump();
        jumpInputFlag = false;
    }

    void HandleRotation()
    {
        Quaternion turnDelta = Quaternion.Euler(0f, inputTurn * turnSpeed * Time.fixedDeltaTime, 0f);
        rb.MoveRotation(rb.rotation * turnDelta);
    }

    void HandleMovement(float speedMultiplier)
    {
        Vector3 moveDirection = transform.forward * inputForward;
        Vector3 targetVelocityForMovement = moveDirection * moveSpeed * speedMultiplier;

        // Mantém a velocidade Y atual (para gravidade/pulo)
        // A velocidade da plataforma será adicionada implicitamente devido ao parentesco
        rb.linearVelocity = new Vector3(targetVelocityForMovement.x, rb.linearVelocity.y, targetVelocityForMovement.z);
    }

    void HandleJump()
    {
        if (jumpInputFlag && jumpsRemaining > 0)
        {
            Vector3 upwardForce = transform.up * upwardImpulseForce;
            Vector3 forwardBoost = (inputForward > 0.1f) ? transform.forward * forwardImpulseBoost : Vector3.zero;

            // Zera a velocidade Y para um pulo limpo, mas MANTÉM a velocidade XZ atual.
            // Se estiver em uma plataforma, a velocidade XZ da plataforma já estará em rb.velocity.
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
            rb.AddForce(upwardForce + forwardBoost, ForceMode.Impulse);

            jumpsRemaining--;
            isGrounded = false;
            // Debug.Log($"Pulo/Impulso! Jumps restantes: {jumpsRemaining}");
        }
    }

    void CheckIfGrounded(LayerMask layersToConsider)
    {
        // ... (código da checagem de chão como antes) ...
        Vector3 capsuleBottomCenter = transform.position + capsuleCollider.center - transform.up * (capsuleCollider.height * 0.5f - capsuleCollider.radius);
        Vector3 startPoint = capsuleBottomCenter + transform.up * (capsuleCollider.radius * 0.1f);
        float castRadius = capsuleCollider.radius * 0.9f;
        float castDistance = groundCheckDistance + capsuleCollider.radius * 0.1f;
        isGrounded = Physics.SphereCast(startPoint, castRadius, -transform.up, out RaycastHit hitInfo, castDistance, layersToConsider, QueryTriggerInteraction.Ignore);
    }

    void OnTriggerEnter(Collider other)
    {
        // Reset Zone
        if (resetZoneLayer.value != 0 && (resetZoneLayer.value & (1 << other.gameObject.layer)) > 0)
        {
            Debug.Log($"Jogador '{gameObject.name}' encostou na ResetZone: {other.gameObject.name}");
            DetachFromPlatform(); // Garante que desgruda
            RespawnPlayer();
            return;
        }

        // Entrou na StickZone da Plataforma
        if (other.CompareTag(platformStickTag))
        {
            Transform platform = other.transform.parent;
            if (platform != null && platform.GetComponent<MovingPlatform>() != null)
            {
                Debug.Log($"Jogador ENTROU na StickZone da plataforma '{platform.name}'.");
                AttachToPlatform(platform);
                isInsideStickZone = true;
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        // Saiu da StickZone da Plataforma
        if (other.CompareTag(platformStickTag))
        {
            Debug.Log($"Jogador SAIU da StickZone: {other.name}");
            isInsideStickZone = false;
            // A lógica de desgrudar agora está no Update, baseada em isGrounded e isInsideStickZone
        }
    }

    public void AttachToPlatform(Transform platform)
    {
        if (currentAttachedPlatform != platform)
        {
            transform.SetParent(platform, true); // O 'true' é crucial (worldPositionStays)
            currentAttachedPlatform = platform;
            // NÃO TORNAMOS O RIGIDBODY KINEMATIC AQUI
            // rb.isKinematic = true; // REMOVIDO/COMENTADO
            Debug.Log($"Jogador '{gameObject.name}' GRUDADO na plataforma '{platform.name}'.");
        }
    }

    void DetachFromPlatform()
    {
        if (currentAttachedPlatform != null)
        {
            Debug.Log($"Jogador '{gameObject.name}' DESGRUDADO da plataforma '{currentAttachedPlatform.name}'.");
            transform.SetParent(null);
            // NÃO PRECISAMOS MUDAR isKinematic se ele nunca foi mudado para true
            // rb.isKinematic = false; // REMOVIDO/COMENTADO
            currentAttachedPlatform = null;
        }
        isInsideStickZone = false;
    }

    void RespawnPlayer()
    {
        if (respawnPoint != null)
        {
            Debug.Log($"Jogador '{gameObject.name}' resetado para o ponto de respawn!");
            if (rb != null)
            {
                rb.position = respawnPoint.position; // Move a posição do Rigidbody
                rb.linearVelocity = Vector3.zero;

                rb.angularVelocity = Vector3.zero;   // Zera a velocidade angular
            }
            else
            {
                transform.position = respawnPoint.position;
            }
        }
        else
        {
            Debug.LogError($"RespawnPoint não definido no jogador '{gameObject.name}'! Não é possível resetar.", this);
        }
    }


    void HandleVisualRoll()
    {
        // ... (código da rolagem visual permanece o mesmo) ...
        if (visualMeshTransform == null) return;
        float targetVisualRollSpeed = inputForward * maxVisualRollSpeed;
        currentVisualRollSpeed = Mathf.Lerp(currentVisualRollSpeed, targetVisualRollSpeed, Time.deltaTime * visualRollDamping);
        if (Mathf.Abs(currentVisualRollSpeed) < 0.1f && Mathf.Abs(inputForward) < 0.01f)
        {
            currentVisualRollSpeed = 0f;
        }
        if (Mathf.Abs(currentVisualRollSpeed) > 0.01f)
        {
            float rollAngleDelta = currentVisualRollSpeed * Time.deltaTime;
            visualMeshTransform.Rotate(Vector3.right, rollAngleDelta, Space.Self);
        }
    }

#if UNITY_EDITOR
    void OnDrawGizmosSelected()
    {
        // ... (código dos Gizmos permanece o mesmo, incluindo o Gizmo para transform.up) ...
        if (capsuleCollider != null)
        {
            Vector3 capsuleBottomCenter = transform.position + capsuleCollider.center - transform.up * (capsuleCollider.height * 0.5f - capsuleCollider.radius);
            Vector3 startPoint = capsuleBottomCenter + transform.up * (capsuleCollider.radius * 0.1f);
            float castRadius = capsuleCollider.radius * 0.9f;
            float castDistance = groundCheckDistance + capsuleCollider.radius * 0.1f;
            Vector3 endPoint = startPoint - transform.up * castDistance;
            Gizmos.color = isGrounded ? Color.green : Color.red;
            Gizmos.DrawWireSphere(endPoint, castRadius);
            Gizmos.DrawLine(startPoint, endPoint);
            Gizmos.color = Color.blue;
            Gizmos.DrawRay(transform.position, transform.up * 2f);
        }
    }
#endif
}
