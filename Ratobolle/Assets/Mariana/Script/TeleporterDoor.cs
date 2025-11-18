using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Collider))]
public class TeleporterDoor : MonoBehaviour
{
    [Header("Configuração do Teleporte")]
    [SerializeField] private Transform destinationPoint;
    [SerializeField] private string playerTag = "Player";
    [SerializeField] private float cooldownTime = 1.0f;

    [Header("Interação")]
    [Tooltip("O objeto de texto/UI que mostra a dica 'Pressione E'")]
    [SerializeField] private GameObject interactionPrompt; // <-- NOVO: Referência para o texto de interação

    [Header("Configuração da Rodada")]
    [SerializeField] private TemporizadorDeQueijo temporizador;

    private bool isReady = true;
    private FakeRollCapsuleController playerControllerInRange; // <-- NOVO: Armazena o jogador que está na área

    void Start()
    {
        // Garante que o aviso de interação comece desligado
        if (interactionPrompt != null)
        {
            interactionPrompt.SetActive(false);
        }
    }

    void OnValidate()
    {
        Collider col = GetComponent<Collider>();
        if (col != null && !col.isTrigger)
        {
            col.isTrigger = true;
        }
    }

    private void Update()
    {
        // Se o jogador estiver na área, a porta estiver pronta e a tecla 'E' for pressionada...
        if (playerControllerInRange != null && isReady && Input.GetKeyDown(KeyCode.E))
        {
            // ...inicia o teletransporte.
            Teleport();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Verifica se é o jogador
        if (other.CompareTag(playerTag))
        {
            // Armazena a referência do controller do jogador
            playerControllerInRange = other.GetComponent<FakeRollCapsuleController>();

            // Mostra o aviso para pressionar 'E'
            if (interactionPrompt != null)
            {
                interactionPrompt.SetActive(true);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // Verifica se o jogador que está saindo é o mesmo que entrou
        if (other.CompareTag(playerTag) && other.GetComponent<FakeRollCapsuleController>() == playerControllerInRange)
        {
            // Limpa a referência do jogador
            playerControllerInRange = null;

            // Esconde o aviso
            if (interactionPrompt != null)
            {
                interactionPrompt.SetActive(false);
            }
        }
    }

    private void Teleport()
    {
        // Verifica se o ponto de destino e o jogador são válidos
        if (destinationPoint != null && playerControllerInRange != null)
        {
            if (temporizador != null)
            {
                temporizador.IniciarCronometro();
            }

            // Inicia a rotina de teleporte
            StartCoroutine(TeleportPlayer(playerControllerInRange));
        }
    }

    private IEnumerator TeleportPlayer(FakeRollCapsuleController controller)
    {
        isReady = false;
        if (interactionPrompt != null) interactionPrompt.SetActive(false); // Esconde o prompt durante o teleporte

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

        // Se o jogador ainda estiver dentro do trigger após o cooldown, mostra o prompt novamente
        if (playerControllerInRange != null && interactionPrompt != null)
        {
            interactionPrompt.SetActive(true);
        }
    }

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