using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class TemporizadorDeQueijo : MonoBehaviour
{
    [Header("Configura��es do Temporizador")]
    public float tempoEmMinutos = 1.5f;

    [Header("Refer�ncias de Componentes")]
    public Image imagemQueijo;
    public TextMeshProUGUI textoContagem;

    [Header("Refer�ncias da Cena")]
    public Transform playerTransform;
    public Transform respawnPoint;
    public IngredientSpawner ingredientSpawner;
    public TeleporterDoor portaDeInicio;

    private SimplifiedPlayerInventory playerInventory;
    private float tempoRestanteEmSegundos;
    private float tempoTotalEmSegundos;
    private bool cronometroRodando = false;
    private static bool primeiroInicio = true;
    private bool rodadaTerminou = false;

    void Awake()
    {
        playerInventory = SimplifiedPlayerInventory.Instance;
        if (playerInventory == null)
        {
            Debug.LogError("TEMPORIZADOR: N�o foi poss�vel encontrar a inst�ncia do SimplifiedPlayerInventory!", this);
        }
    }

    void Start()
    {
        tempoTotalEmSegundos = tempoEmMinutos * 60f;

        if (primeiroInicio)
        {
            playerInventory.ClearAllItems();
            primeiroInicio = false;
        }
        PrepararNovaRodada();
    }

    // --- L�GICA DO UPDATE SIMPLIFICADA E CORRIGIDA ---
    void Update()
    {
        // Se o cron�metro n�o estiver rodando, n�o h� nada a fazer.
        if (!cronometroRodando)
        {
            return;
        }

        // Se a contagem ainda est� ativa...
        if (tempoRestanteEmSegundos > 0)
        {
            tempoRestanteEmSegundos -= Time.deltaTime;
        }
        // Se o tempo acabou e a rodada ainda n�o foi marcada como "terminada"...
        else if (!rodadaTerminou)
        {
            // Aciona o processo de fim de rodada.
            StartCoroutine(FimDeRodadaCoroutine());
        }

        // A atualiza��o visual do texto acontece em todos os frames em que o cron�metro est� ativo.
        AtualizarTextoDoTemporizador(tempoRestanteEmSegundos);
    }

    private void PrepararNovaRodada()
    {
        Debug.Log("Preparando cen�rio para a pr�xima rodada... Aguardando jogador.");

        // Garante que o cron�metro esteja parado enquanto preparamos
        cronometroRodando = false;

        ingredientSpawner.RespawnAllIngredients();
        ResetarCronometroVisual();

        if (portaDeInicio != null)
        {
            portaDeInicio.ResetForNewRound();
        }

        // Reseta a trava da rodada, permitindo que a pr�xima possa terminar
        rodadaTerminou = false;
    }

    IEnumerator FimDeRodadaCoroutine()
    {
        // 1. Trava a rodada para que esta corrotina n�o seja chamada de novo
        rodadaTerminou = true;
        // 2. Para a contagem (embora o Update j� fa�a isso, � uma seguran�a extra)
        cronometroRodando = false;

        Debug.Log("O tempo acabou! O jogador perdeu a rodada!");
        playerInventory.LoseHalfOfAllItems();

        if (playerTransform != null && respawnPoint != null)
        {
            playerTransform.position = respawnPoint.position;
            playerTransform.rotation = respawnPoint.rotation;
        }

        yield return new WaitForSeconds(2f);

        // 3. Prepara tudo para a pr�xima tentativa
        PrepararNovaRodada();
    }

    public void IniciarCronometro()
    {
        // S� inicia se o cron�metro n�o estiver rodando E se a rodada n�o tiver terminado
        if (cronometroRodando || rodadaTerminou) return;

        Debug.Log("<color=green>PORTA ATRAVESSADA! O tempo est� correndo!</color>");
        cronometroRodando = true;
    }

    // Renomeado para maior clareza, pois apenas reseta a parte visual
    private void ResetarCronometroVisual()
    {
        tempoRestanteEmSegundos = tempoTotalEmSegundos;
        AtualizarTextoDoTemporizador(tempoRestanteEmSegundos);
    }

    void AtualizarTextoDoTemporizador(float tempoParaExibir)
    {
        if (tempoParaExibir < 0) tempoParaExibir = 0;

        imagemQueijo.fillAmount = tempoParaExibir / tempoTotalEmSegundos;

        float minutos = Mathf.FloorToInt(tempoParaExibir / 60);
        float segundos = Mathf.FloorToInt(tempoParaExibir % 60);
        textoContagem.text = string.Format("{0:00}:{1:00}", minutos, segundos);
    }

    public void ReduzirTempo(float segundosAReduzir)
    {
        if (cronometroRodando)
        {
            tempoRestanteEmSegundos -= segundosAReduzir;
        }
    }
}