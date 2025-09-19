using UnityEngine;
using DG.Tweening; // Precisamos do DOTween para a animação de UI
using System.Collections;

public class TutorialEntryController : MonoBehaviour
{
    [Header("Referências da Cena")]
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private Camera tutorialCamera;

    [Header("Referências da Transição de Entrada")]
    [Tooltip("Arraste aqui o objeto 'WipeCircle' do seu Canvas de Transição.")]
    [SerializeField] private RectTransform wipeCircle; // O círculo que vai escalar
    [SerializeField] private float fadeInDuration = 0.8f; // Duração da animação de "abertura"

    [Header("Parâmetros da Animação de Entrada do Player")]
    [SerializeField] private float entryRollDistance = 8f;
    [SerializeField] private float entryRollDuration = 1.5f;

    void Start()
    {
        // Garante que o círculo comece invisível (escala zero)
        if (wipeCircle != null)
        {
            wipeCircle.localScale = Vector3.zero;
        }

        if (SceneTransitionData.HasData)
        {
            // Posiciona a câmera e instancia o jogador invisivelmente,
            // enquanto a tela ainda está "preta".
            tutorialCamera.transform.position = SceneTransitionData.CameraPosition;
            tutorialCamera.transform.rotation = SceneTransitionData.CameraRotation;
            GameObject playerInstance = Instantiate(playerPrefab, SceneTransitionData.PlayerPosition, SceneTransitionData.PlayerRotation);

            SceneTransitionData.ClearData();

            // Inicia a sequência de entrada completa
            StartCoroutine(PerformEntryTransition(playerInstance));
        }
        else
        {
            // Se a cena for iniciada diretamente, apenas instancia o jogador e remove a UI de transição
            Instantiate(playerPrefab, transform.position, transform.rotation);
            if (wipeCircle != null) wipeCircle.transform.parent.gameObject.SetActive(false);
        }
    }

    private IEnumerator PerformEntryTransition(GameObject playerInstance)
    {
        // --- FASE 1: A Abertura da Íris ---
        // Calcula a escala final necessária para o círculo cobrir a tela inteira.
        // Uma escala de 2 é geralmente suficiente se o círculo estiver em um painel que preenche a tela.
        float finalScale = 2f;
        wipeCircle.DOScale(finalScale, fadeInDuration).SetEase(Ease.OutCubic);

        // --- FASE 2: A Rolagem do Rato ---
        // A animação de rolagem do rato começa AO MESMO TEMPO que a abertura da íris.
        FakeRollCapsuleController playerController = playerInstance.GetComponent<FakeRollCapsuleController>();
        if (playerController != null)
        {
            StartCoroutine(PerformEntryRoll(playerController));
        }

        // Espera a animação de fade in terminar
        yield return new WaitForSeconds(fadeInDuration);

        // --- FASE 3: Limpeza ---
        // Destrói o canvas de transição depois que ele fez seu trabalho
        if (wipeCircle != null)
        {
            Destroy(wipeCircle.transform.parent.gameObject);
        }
    }

    private IEnumerator PerformEntryRoll(FakeRollCapsuleController controller)
    {
        Rigidbody rb = controller.GetComponent<Rigidbody>();
        Vector3 targetVelocity = controller.transform.forward * (entryRollDistance / entryRollDuration);
        float elapsedTime = 0f;

        while (elapsedTime < entryRollDuration)
        {
            rb.linearVelocity = new Vector3(targetVelocity.x, rb.linearVelocity.y, targetVelocity.z);

            // Para isso funcionar, o método HandleVisualRoll no seu script FakeRollCapsuleController
            // precisa ser público. Se ainda não for, você precisará mudá-lo.
            // Ex: public void HandleVisualRoll(float forwardInput)
            // controller.HandleVisualRoll(1f); 

            elapsedTime += Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }

        rb.linearVelocity = Vector3.zero;
    }
}