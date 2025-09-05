using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Collider))]
public class TeleporterDoor : MonoBehaviour
{
    [Header("Configuração do Teleporte")]
    [SerializeField] private Transform destinationPoint;
    [SerializeField] private string playerTag = "Player";
    [SerializeField] private float cooldownTime = 1.0f;

    [Header("Configuração da Rodada")]
    [SerializeField] private TemporizadorDeQueijo temporizador;

    private bool isReady = true;

    void OnValidate()
    {
        Collider col = GetComponent<Collider>();
        if (col != null && !col.isTrigger)
        {
            col.isTrigger = true;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (isReady && other.CompareTag(playerTag) && destinationPoint != null)
        {
            if (temporizador != null)
            {
                temporizador.IniciarCronometro();
            }

            var playerController = other.GetComponent<FakeRollCapsuleController>();
            if (playerController != null)
            {
                StartCoroutine(TeleportPlayer(playerController));
            }
        }
    }

    private IEnumerator TeleportPlayer(FakeRollCapsuleController controller)
    {
        isReady = false;
        controller.enabled = false;

        Rigidbody playerRb = controller.GetComponent<Rigidbody>();
        if (playerRb != null)
        {
            playerRb.linearVelocity = Vector3.zero;
            playerRb.angularVelocity = Vector3.zero;
            playerRb.position = destinationPoint.position;
            playerRb.rotation = destinationPoint.rotation;
        }

        yield return new WaitForFixedUpdate();
        controller.enabled = true;
        yield return new WaitForSeconds(cooldownTime);
        isReady = true;
    }

    // --- FUNÇÃO DE RESET ADICIONADA ---
    /// <summary>
    /// Permite que a porta seja usada novamente para iniciar a próxima rodada.
    /// </summary>
    public void ResetForNewRound()
    {
        isReady = true;
    }

#if UNITY_EDITOR
    void OnDrawGizmos()
    {
        if (destinationPoint != null)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawLine(transform.position, destinationPoint.position);
            Gizmos.color = new Color(0, 1, 1, 0.25f);
            Gizmos.DrawSphere(destinationPoint.position, 0.5f);
            Gizmos.color = Color.green;
            Gizmos.DrawRay(destinationPoint.position, destinationPoint.forward * 1.5f);
        }
    }
#endif
}