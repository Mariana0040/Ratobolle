using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(CapsuleCollider))]
public class FakeRollCapsuleController : MonoBehaviour
{
    [Header("Movimento")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float turnSpeed = 180f;

    [Header("Pulo/Dash Frontal")] // Renomeado para clareza
    [Tooltip("For�a do impulso para frente ao apertar Pulo no ch�o.")]
    [SerializeField] private float forwardImpulseForce = 15f; // Renomeado de jumpForce/dashForce

    [Header("Rolagem Visual")]
    [SerializeField] private Transform visualMeshTransform;
    [SerializeField] private float maxVisualRollSpeed = 450f;
    [SerializeField] private float visualRollDamping = 5f;

    [Header("Verifica��o de Ch�o")]
    [SerializeField] private float groundCheckDistance = 0.15f;
    [SerializeField] private LayerMask groundLayerMask;

    public Transform cameraTransform;

    // Componentes e Estado
    private Rigidbody rb;
    private CapsuleCollider capsuleCollider;
    private bool isGrounded = false;
    private bool canDash = true;
    private float currentVisualRollSpeed = 0f;

    // Input
    private float inputForward = 0f;
    private float inputTurn = 0f;
    private float jumpInputDown = 0f; // Mudado de volta para bool

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        capsuleCollider = GetComponent<CapsuleCollider>();

        if (visualMeshTransform == null) { Debug.LogError("Visual Mesh Transform n�o definido!"); enabled = false; return; }
        if (groundLayerMask == 0) { Debug.LogWarning("Ground Layer Mask n�o definida."); }

        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
    }

    void Update()
    {
        // Leitura de Input
        inputForward = Input.GetAxis("Vertical");   // W/S
        inputTurn = Input.GetAxis("Horizontal");     // A/D
        jumpInputDown = Input.GetAxis("Jump"); // Espa�o 

        // Checa se est� no ch�o
        CheckIfGrounded();

        // Atualiza e Aplica Rota��o Visual
        HandleVisualRoll();
    }

    void FixedUpdate()
    {
        // Rota��o Y (Giro com A/D)
        HandleRotation();

        // Movimento (Andar com W/S)
        HandleMovement();

        // Pulo/Dash Frontal (L�gica modificada)
        HandleForwardImpulse();

    }

    void HandleRotation()
    {
        Quaternion turnDelta = Quaternion.Euler(0f, inputTurn * turnSpeed * Time.fixedDeltaTime, 0f);
        rb.MoveRotation(rb.rotation * turnDelta);
    }

    void HandleMovement()
    {
        Vector3 moveDelta = transform.forward * inputForward * moveSpeed * Time.fixedDeltaTime;
        rb.MovePosition(rb.position + moveDelta);
    }

    // Renomeado e l�gica alterada
    void HandleForwardImpulse()
    {
        // Verifica se o bot�o foi pressionado NESTE quadro E se est� no ch�o
        if (jumpInputDown != 0 && isGrounded && canDash)
        {
            // Garante que temos a refer�ncia visual
            if (visualMeshTransform != null)
            {
                // Pega a dire��o PARA FRENTE do OBJETO VISUAL
                Vector3 impulseDirection = visualMeshTransform.forward;

                // Aplica o impulso nessa dire��o usando a for�a definida
                rb.AddForce(impulseDirection * forwardImpulseForce, ForceMode.Impulse);

                // Importante: Sair do estado 'grounded' para n�o poder dar impulsos m�ltiplos no mesmo toque
                isGrounded = false;
                StartCoroutine(DelayDash());
            }
            else
            {
                Debug.LogError("Tentativa de Impulso sem Visual Mesh Transform definido!");
            }
        }
    }
    IEnumerator DelayDash()
    {
        canDash = false;
        yield return new WaitForSeconds(1f);
        canDash = true;
    }
    void HandleVisualRoll()
    {
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
        Vector3 capsuleBottomCenter = transform.position + capsuleCollider.center - transform.up * (capsuleCollider.height * 0.5f - capsuleCollider.radius);
        Vector3 startPoint = capsuleBottomCenter + transform.up * (capsuleCollider.radius * 0.1f);
        float castRadius = capsuleCollider.radius * 0.9f;
        float castDistance = groundCheckDistance + capsuleCollider.radius * 0.1f;
        isGrounded = Physics.SphereCast(startPoint, castRadius, -transform.up, out RaycastHit hitInfo, castDistance, groundLayerMask, QueryTriggerInteraction.Ignore);
    }

    // Gizmos (sem mudan�as)
#if UNITY_EDITOR
    void OnDrawGizmosSelected()
    {
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
        }
    }
#endif
}