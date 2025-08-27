// Importações necessárias para UI, TextMesh Pro e GERENCIAMENTO DE CENAS
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement; // ESSENCIAL PARA REINICIAR A CENA

public class TemporizadorDeQueijo : MonoBehaviour
{
    [Header("Configurações do Temporizador")]
    public float tempoEmMinutos = 1.5f;

    [Header("Referências de Componentes")]
    public Image imagemQueijo;
    public TextMeshProUGUI textoContagem;

    private float tempoRestanteEmSegundos;
    private float tempoTotalEmSegundos;
    private bool tempoAcabou = false; // Chave para garantir que a lógica de "perder" rode apenas uma vez

    void Start()
    {
        tempoTotalEmSegundos = tempoEmMinutos * 60f;
        tempoRestanteEmSegundos = tempoTotalEmSegundos;

        if (imagemQueijo.type != Image.Type.Filled)
        {
            imagemQueijo.type = Image.Type.Filled;
            imagemQueijo.fillMethod = Image.FillMethod.Radial360;
        }
        imagemQueijo.fillAmount = 1f;
    }

    void Update()
    {
        if (tempoRestanteEmSegundos > 0)
        {
            tempoRestanteEmSegundos -= Time.deltaTime;
            imagemQueijo.fillAmount = tempoRestanteEmSegundos / tempoTotalEmSegundos;
            AtualizarTextoDoTemporizador(tempoRestanteEmSegundos);
        }
        else if (!tempoAcabou) // Só entra aqui na primeira vez que o tempo acaba
        {
            // O tempo acabou!
            tempoAcabou = true; // Ativa a chave para não entrar aqui de novo
            tempoRestanteEmSegundos = 0;
            imagemQueijo.fillAmount = 0;
            textoContagem.text = "00:00";

            // --- NOVO: LÓGICA DE FIM DE JOGO ---
            FimDeJogo();
        }
    }

    void AtualizarTextoDoTemporizador(float tempoParaExibir)
    {
        if (tempoParaExibir < 0) tempoParaExibir = 0;
        float minutos = Mathf.FloorToInt(tempoParaExibir / 60);
        float segundos = Mathf.FloorToInt(tempoParaExibir % 60);
        textoContagem.text = string.Format("{0:00}:{1:00}", minutos, segundos);
    }

    // --- NOVA FUNÇÃO ---
    void FimDeJogo()
    {
        Debug.Log("O tempo acabou! Você perdeu! Reiniciando a fase...");

        // Recarrega a cena atual. O inventário será mantido pelo GerenciadorDeInventario.
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}