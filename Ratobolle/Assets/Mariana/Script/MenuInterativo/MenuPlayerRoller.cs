using UnityEngine;
using System.Collections;
using DG.Tweening;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(CapsuleCollider))]
public class MenuPlayerRoller : MonoBehaviour
{
    [Header("Referências Visuais")]
    [SerializeField] private Transform visualMeshTransform;

    [Header("Parâmetros de Movimento")]
    [SerializeField] private float moveSpeed = 6f;
    [SerializeField] private float turnSpeed = 8f;
    [SerializeField] private float stopDistance = 0.2f;

    [Header("Parâmetros de Rolagem Visual")]
    [SerializeField] private float visualRollSpeed = 500f;
    [SerializeField] private float idleRollSpeed = 100f; // Velocidade da rolagem "parado"

    private Rigidbody rb;
    private Coroutine currentAnimationCoroutine;
    private Coroutine idleRollCoroutine;
    private bool isIdleRolling = false;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        if (visualMeshTransform == null)
        {
            Debug.LogError("A referência para a malha visual (Visual Mesh Transform) não foi definida!", this);
        }
    }

    // --- NOVAS FUNÇÕES DE CONTROLE DE ESTADO ---

    public void StartIdleRoll()
    {
        if (isIdleRolling) return;
        isIdleRolling = true;
        idleRollCoroutine = StartCoroutine(IdleRollCoroutine());
    }

    public void StopIdleRoll()
    {
        if (!isIdleRolling) return;
        if (idleRollCoroutine != null)
        {
            StopCoroutine(idleRollCoroutine);
        }
        isIdleRolling = false;
    }

    private IEnumerator IdleRollCoroutine()
    {
        while (true)
        {
            HandleVisualRoll(idleRollSpeed / visualRollSpeed);
            yield return null;
        }
    }

    // ---------------------------------------------

    public void AnimateToPoint(Transform target, System.Action onComplete = null)
    {
        StopIdleRoll(); // Para o rolamento ocioso antes de se mover
        if (currentAnimationCoroutine != null)
        {
            StopCoroutine(currentAnimationCoroutine);
        }
        currentAnimationCoroutine = StartCoroutine(RollToTargetCoroutine(target, onComplete));
    }

    public void AnimateToCamera(Camera camera, System.Action onComplete = null)
    {
        StopIdleRoll();
        if (currentAnimationCoroutine != null)
        {
            StopCoroutine(currentAnimationCoroutine);
        }
        Vector3 targetPosition = camera.transform.position + camera.transform.forward * 0.5f;
        GameObject tempTarget = new GameObject("TempCameraTarget");
        tempTarget.transform.position = targetPosition;

        currentAnimationCoroutine = StartCoroutine(RollToTargetCoroutine(tempTarget.transform, () => {
            Destroy(tempTarget);
            onComplete?.Invoke();
        }));
    }

    private IEnumerator RollToTargetCoroutine(Transform target, System.Action onComplete)
    {
        Vector3 playerPositionXZ = new Vector3(transform.position.x, 0, transform.position.z);
        Vector3 targetPositionXZ = new Vector3(target.position.x, 0, target.position.z);

        while (Vector3.Distance(playerPositionXZ, targetPositionXZ) > stopDistance)
        {
            Vector3 direction = (target.position - transform.position).normalized;
            direction.y = 0;
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            rb.MoveRotation(Quaternion.Slerp(rb.rotation, targetRotation, Time.fixedDeltaTime * turnSpeed));
            Vector3 targetVelocity = transform.forward * moveSpeed;
            rb.linearVelocity = new Vector3(targetVelocity.x, rb.linearVelocity.y, targetVelocity.z);
            HandleVisualRoll(1f);

            playerPositionXZ.x = transform.position.x;
            playerPositionXZ.z = transform.position.z;
            targetPositionXZ.x = target.position.x;
            targetPositionXZ.z = target.position.z;

            yield return new WaitForFixedUpdate();
        }

        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        HandleVisualRoll(0f);
        onComplete?.Invoke();
    }

    private void HandleVisualRoll(float forwardInput)
    {
        if (visualMeshTransform == null) return;
        float rollAngleDelta = forwardInput * visualRollSpeed * Time.deltaTime;
        visualMeshTransform.Rotate(Vector3.right, rollAngleDelta, Space.Self);
    }

    public void AnimateRotationTo(Quaternion targetRotation, float duration)
    {
        rb.DORotate(targetRotation.eulerAngles, duration).SetEase(Ease.OutSine);
    }
}