using UnityEngine;
using TMPro;

public class PontoDeCorrida : MonoBehaviour
{
    // Enum para definir no Inspector se este ponto � o de in�cio ou de fim.
    public enum TipoDePonto { IniciarCorrida, TerminarCorrida }

    [Header("Configura��o do Ponto")]
    [Tooltip("Define se este ponto INICIA ou TERMINA a corrida.")]
    [SerializeField] private TipoDePonto tipoDoPonto = TipoDePonto.IniciarCorrida;

    [Header("Refer�ncias Essenciais")]
    [Tooltip("Arraste o OUTRO ponto (o de destino) para c�.")]
    [SerializeField] private Transform pontoDeDestino;

    [Tooltip("Arraste o objeto do Jogador para c�.")]
    [SerializeField] private Transform jogador;

    [Tooltip("Arraste o objeto da cena que cont�m o script 'TemporizadorDeQueijo'.")]
    [SerializeField] private TemporizadorDeQueijo temporizador;

    [Header("Interface do Usu�rio (UI)")]
    [Tooltip("O objeto de UI que mostra a mensagem para interagir (Ex: um painel com texto).")]
    [SerializeField] private GameObject promptDeInteracaoUI;

    // Flag para saber se o jogador est� dentro do collider.
    private bool jogadorEstaNaArea = false;

    void Awake()
    {
        // Garante que o prompt de intera��o comece desligado.
        if (promptDeInteracaoUI != null)
        {
            promptDeInteracaoUI.SetActive(false);
        }
    }

    void Update()
    {
        // Se o jogador est� na �rea e aperta a tecla E, executa a a��o.
        if (jogadorEstaNaArea && Input.GetKeyDown(KeyCode.E))
        {
            ExecutarAcao();
        }
    }

    private void ExecutarAcao()
    {
        // Esconde a UI de intera��o assim que o jogador interage.
        promptDeInteracaoUI.SetActive(false);

        // Executa a l�gica baseada no tipo deste ponto.
        switch (tipoDoPonto)
        {
            case TipoDePonto.IniciarCorrida:
                Debug.Log("RUN INICIADA!");
                TeleportarJogador(pontoDeDestino);
                temporizador.IniciarCronometro();
                break;

            case TipoDePonto.TerminarCorrida:
                Debug.Log("RUN TERMINADA COM SUCESSO!");
                temporizador.StopTimer(); // Para o cron�metro para evitar a derrota.
                TeleportarJogador(pontoDeDestino);
                // Chama a fun��o para preparar a pr�xima rodada ap�s um pequeno delay.
                Invoke(nameof(PrepararProximaRodada), 0.5f);
                break;
        }
    }

    // Fun��o que ser� chamada com delay para resetar o cen�rio.
    private void PrepararProximaRodada()
    {
        temporizador.PrepararNovaRodada();
    }

    private void TeleportarJogador(Transform destino)
    {
        // � importante mover o Rigidbody diretamente para evitar bugs de f�sica.
        Rigidbody rbDoJogador = jogador.GetComponent<Rigidbody>();
        if (rbDoJogador != null)
        {
            rbDoJogador.position = destino.position;
            rbDoJogador.linearVelocity = Vector3.zero;      // Zera a velocidade para n�o sair voando.
            rbDoJogador.angularVelocity = Vector3.zero; // Zera a rota��o.
        }
        else
        {
            // Fallback caso o jogador n�o tenha um Rigidbody.
            jogador.position = destino.position;
        }
        jogador.rotation = destino.rotation;
    }

    // Ativado quando o jogador entra no Collider (que deve estar marcado como "Is Trigger").
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            jogadorEstaNaArea = true;
            MostrarPromptDeInteracao();
        }
    }

    // Ativado quando o jogador sai do Collider.
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            jogadorEstaNaArea = false;
            if (promptDeInteracaoUI != null)
            {
                promptDeInteracaoUI.SetActive(false);
            }
        }
    }

    // Mostra e atualiza o texto do prompt de intera��o.
    private void MostrarPromptDeInteracao()
    {
        if (promptDeInteracaoUI != null)
        {
            TextMeshProUGUI textoDoPrompt = promptDeInteracaoUI.GetComponentInChildren<TextMeshProUGUI>();
            if (textoDoPrompt != null)
            {
                if (tipoDoPonto == TipoDePonto.IniciarCorrida)
                {
                    textoDoPrompt.text = "Aperte E para iniciar a RUN";
                }
                else
                {
                    textoDoPrompt.text = "Aperte E para terminar a RUN";
                }
            }
            promptDeInteracaoUI.SetActive(true);
        }
    }
}