using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(CapsuleCollider))]
public class FakeRollCapsuleController : MonoBehaviour
{
    [Header("Movimento")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float turnSpeed = 180f;

    [Header("Pulo/Impulso Orientado")]
    [Tooltip("Força do impulso na direção 'para cima' LOCAL do jogador.")]
    [SerializeField] private float upwardImpulseForce = 10f; // Aumentei um pouco o valor padrão
    [Tooltip("Força do impulso na direção 'para frente' LOCAL do jogador, adicionada ao pulo se W estiver pressionado.")]
    [SerializeField] private float forwardImpulseBoost = 5f; // Força específica para o impulso frontal no pulo
    [Tooltip("Quantos impulsos/pulos o jogador pode dar antes de tocar o chão.")]
    [SerializeField] private int maxJumps = 2;
    [Tooltip("Pequeno controle no ar: quanta da velocidade de movimento é aplicada APÓS o pulo inicial.")]
    [Range(0f, 1f)]
    [SerializeField] private float airControlFactor = 0.3f;

    [Header("Rolagem Visual")]
    [SerializeField] private Transform visualMeshTransform;
    [SerializeField] private float maxVisualRollSpeed = 450f;
    [SerializeField] private float visualRollDamping = 5f;

    [Header("Verificação de Chão")]
    [SerializeField] private float groundCheckDistance = 0.15f;
    [SerializeField] private LayerMask groundLayerMask;

    // Componentes e Estado Interno
    private Rigidbody rb;
    private CapsuleCollider capsuleCollider;
    private bool isGrounded = false;
    private int jumpsRemaining = 0;
    private float currentVisualRollSpeed = 0f;

    // Input
    private float inputForward = 0f; // Valor de W/S
    private float inputTurn = 0f;    // Valor de A/D
    private bool jumpInputFlag = false; // Flag para registrar o aperto da tecla Espaço

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        capsuleCollider = GetComponent<CapsuleCollider>();
        if (visualMeshTransform == null) { Debug.LogError("Visual Mesh Transform não definido!"); enabled = false; return; }
        if (groundLayerMask == 0) { Debug.LogWarning("Ground Layer Mask não definida."); }
        jumpsRemaining = maxJumps;
    }

    void Update()
    {
        inputForward = Input.GetAxis("Vertical");
        inputTurn = Input.GetAxis("Horizontal");

        // Se o botão de pulo for pressionado, ativa a flag.
        // Fazemos isso no Update para não perder o input se o FixedUpdate rodar menos frequentemente.
        if (Input.GetButtonDown("Jump"))
        {
            jumpInputFlag = true;
        }

        CheckIfGrounded();
        if (isGrounded)
        {
            jumpsRemaining = maxJumps;
        }
        HandleVisualRoll();
    }

    void FixedUpdate()
    {
        HandleRotation();
        HandleMovement(isGrounded ? 1f : airControlFactor);
        HandleJump(); // Lida com o pulo

        // Reseta a flag do pulo DEPOIS de ser processada em HandleJump
        jumpInputFlag = false;
    }

    void HandleRotation()
    {
        Quaternion turnDelta = Quaternion.Euler(0f, inputTurn * turnSpeed * Time.fixedDeltaTime, 0f);
        rb.MoveRotation(rb.rotation * turnDelta);
    }

    void HandleMovement(float speedMultiplier)
    {
        if (isGrounded || speedMultiplier > 0)
        {
            Vector3 moveDelta = transform.forward * inputForward * moveSpeed * speedMultiplier * Time.fixedDeltaTime;
            rb.MovePosition(rb.position + moveDelta);
        }
    }

    void HandleJump()
    {
        // Verifica se a flag do pulo está ativa E se ainda há pulos restantes.
        if (jumpInputFlag && jumpsRemaining > 0)
        {
            // 1. Força base do pulo na direção "para cima" LOCAL do jogador
            Vector3 upwardForce = transform.up * upwardImpulseForce;

            // 2. Força adicional para frente se W estiver pressionado (inputForward > 0)
            Vector3 forwardBoostForce = Vector3.zero;
            if (inputForward > 0.1f) // Se estiver pressionando W (ou input positivo)
            {
                forwardBoostForce = transform.forward * forwardImpulseBoost;
            }
            // Se estiver pressionando S (inputForward < -0.1f), poderia adicionar um pequeno impulso para trás
            // else if (inputForward < -0.1f) {
            //     forwardBoostForce = transform.forward * inputForward * forwardImpulseBoost * 0.5f; // Menor impulso para trás
            // }


            // Zera a velocidade vertical atual para um pulo mais consistente
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);

            // Combina as forças e aplica como UM ÚNICO impulso
            Vector3 totalImpulse = upwardForce + forwardBoostForce;
            rb.AddForce(totalImpulse, ForceMode.Impulse);

            jumpsRemaining--;
            isGrounded = false; // Importante para que o próximo CheckIfGrounded não resete jumpsRemaining imediatamente
        }
        // A flag jumpInputFlag é resetada no final do FixedUpdate
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

    void CheckIfGrounded()
    {
        // ... (código da checagem de chão permanece o mesmo) ...
        Vector3 capsuleBottomCenter = transform.position + capsuleCollider.center - transform.up * (capsuleCollider.height * 0.5f - capsuleCollider.radius);
        Vector3 startPoint = capsuleBottomCenter + transform.up * (capsuleCollider.radius * 0.1f);
        float castRadius = capsuleCollider.radius * 0.9f;
        float castDistance = groundCheckDistance + capsuleCollider.radius * 0.1f;
        isGrounded = Physics.SphereCast(startPoint, castRadius, -transform.up, out RaycastHit hitInfo, castDistance, groundLayerMask, QueryTriggerInteraction.Ignore);
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