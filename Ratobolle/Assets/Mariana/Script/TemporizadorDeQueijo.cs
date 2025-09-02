using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class TemporizadorDeQueijo : MonoBehaviour
{
    [Header("Configura��es do Temporizador")]
    public float tempoEmMinutos = 1.5f;

    [Header("Refer�ncias de Componentes")]
    public Image imagemQueijo;
    public TextMeshProUGUI textoContagem;

    private float tempoRestanteEmSegundos;
    private float tempoTotalEmSegundos;
    private bool tempoAcabou = false;

    // --- NOVA TRAVA ---
    private bool cronometroRodando = false;

    void Start()
    {
        tempoTotalEmSegundos = tempoEmMinutos * 60f;
        tempoRestanteEmSegundos = tempoTotalEmSegundos;

        // Prepara a UI, mas n�o inicia o cron�metro
        if (imagemQueijo.type != Image.Type.Filled)
        {
            imagemQueijo.type = Image.Type.Filled;
            imagemQueijo.fillMethod = Image.FillMethod.Radial360;
        }
        imagemQueijo.fillAmount = 1f;
        AtualizarTextoDoTemporizador(tempoTotalEmSegundos); // Mostra o tempo inicial
    }

    void Update()
    {
        // --- CONDI��O ADICIONADA ---
        // S� executa a contagem se o cron�metro tiver sido iniciado
        if (!cronometroRodando)
        {
            return; // Sai do Update se a contagem n�o come�ou
        }

        if (tempoRestanteEmSegundos > 0)
        {
            tempoRestanteEmSegundos -= Time.deltaTime;
            imagemQueijo.fillAmount = tempoRestanteEmSegundos / tempoTotalEmSegundos;
            AtualizarTextoDoTemporizador(tempoRestanteEmSegundos);
        }
        else if (!tempoAcabou)
        {
            tempoAcabou = true;
            tempoRestanteEmSegundos = 0;
            imagemQueijo.fillAmount = 0;
            textoContagem.text = "00:00";
            FimDeJogo();
        }
    }

    // --- NOVA FUN��O P�BLICA ---
    /// <summary>
    /// Inicia a contagem regressiva do temporizador.
    /// </summary>
    public void IniciarCronometro()
    {
        if (cronometroRodando) return; // N�o deixa iniciar duas vezes

        Debug.Log("<color=green>A RUN COME�OU! O tempo est� correndo!</color>");
        cronometroRodando = true;
    }

    void AtualizarTextoDoTemporizador(float tempoParaExibir)
    {
        if (tempoParaExibir < 0) tempoParaExibir = 0;
        float minutos = Mathf.FloorToInt(tempoParaExibir / 60);
        float segundos = Mathf.FloorToInt(tempoParaExibir % 60);
        textoContagem.text = string.Format("{0:00}:{1:00}", minutos, segundos);
    }
    void FimDeJogo()
    {
        Debug.Log("O tempo acabou! Voc� perdeu! Reiniciando a fase...");
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void ReduzirTempo(float segundosAReduzir)
    {
        if (!tempoAcabou && cronometroRodando)
        {
            Debug.Log($"<color=red>TEMPO REDUZIDO em {segundosAReduzir} segundos!</color>");
            tempoRestanteEmSegundos -= segundosAReduzir;
            if (tempoRestanteEmSegundos < 0)
            {
                tempoRestanteEmSegundos = 0;
            }
        }
    }
}