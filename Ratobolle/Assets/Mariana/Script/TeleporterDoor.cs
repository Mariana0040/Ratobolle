using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Collider))]
public class TeleporterDoor : MonoBehaviour
{
    [Header("Configuração do Teleporte")]
    [Tooltip("O ponto exato para onde o jogador será movido. Arraste um objeto para cá.")]
    [SerializeField] private Transform destinationPoint;

    [Tooltip("Tag do objeto que pode ativar este teleporte (geralmente 'Player').")]
    [SerializeField] private string playerTag = "Player";

    [Tooltip("Tempo em segundos que o teletransporte fica desativado após o uso para evitar loops.")]
    [SerializeField] private float cooldownTime = 1.0f;

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
        // Adicionamos um Log aqui para ter certeza absoluta que o trigger está funcionando.
        // Verifique no Console da Unity se esta mensagem aparece quando você encosta na porta.
        Debug.Log($"OnTriggerEnter ativado por: {other.name} com a tag: {other.tag}");

        if (isReady && other.CompareTag(playerTag) && destinationPoint != null)
        {
            // Tenta pegar o script do controlador do jogador.
            // O nome deve ser EXATAMENTE o mesmo do seu arquivo de script.
            var playerController = other.GetComponent<FakeRollCapsuleController>();

            if (playerController != null)
            {
                StartCoroutine(TeleportPlayer(playerController));
            }
            else
            {
                Debug.LogError($"O objeto '{other.name}' tem a tag 'Player', mas não foi encontrado o script 'FakeRollCapsuleController' nele!", other);
            }
        }
    }

    private IEnumerator TeleportPlayer(FakeRollCapsuleController controller)
    {
        // 1. Desativa o teletransporte e o controle do jogador.
        isReady = false;
        controller.enabled = false; // <-- PASSO CRUCIAL: Impede que o FixedUpdate do jogador seja executado.

        Debug.Log($"Teleportando {controller.name}. Controle do jogador desativado.");

        Rigidbody playerRb = controller.GetComponent<Rigidbody>();

        // 2. Zera as forças e move o jogador.
        if (playerRb != null)
        {
            playerRb.linearVelocity = Vector3.zero;
            playerRb.angularVelocity = Vector3.zero;
            playerRb.position = destinationPoint.position;
            playerRb.rotation = destinationPoint.rotation;
        }

        // 3. Espera um frame de física para garantir que a mudança de posição seja "assentada".
        // Este é o segundo passo mais importante.
        yield return new WaitForFixedUpdate();

        // 4. Reativa o controle do jogador.
        controller.enabled = true;
        Debug.Log($"Teleporte completo. Controle de {controller.name} reativado.");

        // 5. Espera o cooldown do teleporte.
        yield return new WaitForSeconds(cooldownTime);
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