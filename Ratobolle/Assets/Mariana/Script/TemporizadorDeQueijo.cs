// Importa��es necess�rias para UI da Unity e TextMesh Pro
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TemporizadorDeQueijo : MonoBehaviour
{
    [Header("Configura��es do Temporizador")]
    [Tooltip("Tempo total da contagem regressiva em MINUTOS.")]
    public float tempoEmMinutos = 1.5f; // Agora voc� pode usar valores como 1.5 para 1 minuto e 30 segundos

    [Header("Refer�ncias de Componentes")]
    [Tooltip("Arraste o componente de Imagem do queijo aqui.")]
    public Image imagemQueijo;

    [Tooltip("Arraste o componente TextMeshPro (UI) da contagem regressiva aqui.")]
    public TextMeshProUGUI textoContagem;

    private float tempoRestanteEmSegundos;
    private float tempoTotalEmSegundos;

    void Start()
    {
        // Converte o tempo de minutos para segundos para o c�lculo interno
        tempoTotalEmSegundos = tempoEmMinutos * 60f;
        tempoRestanteEmSegundos = tempoTotalEmSegundos;

        // Valida��o inicial da imagem
        if (imagemQueijo.type != Image.Type.Filled)
        {
            Debug.LogWarning("O tipo da imagem do queijo n�o est� como 'Filled'. Alterando automaticamente.");
            imagemQueijo.type = Image.Type.Filled;
            imagemQueijo.fillMethod = Image.FillMethod.Radial360;
        }

        // Garante que o queijo comece cheio
        imagemQueijo.fillAmount = 1f;
    }

    void Update()
    {
        if (tempoRestanteEmSegundos > 0)
        {
            // Diminui o tempo restante (c�lculo em segundos)
            tempoRestanteEmSegundos -= Time.deltaTime;

            // Atualiza o preenchimento visual do queijo
            imagemQueijo.fillAmount = tempoRestanteEmSegundos / tempoTotalEmSegundos;

            // Atualiza o texto da contagem regressiva no formato MM:SS
            AtualizarTextoDoTemporizador(tempoRestanteEmSegundos);
        }
        else
        {
            // O tempo acabou
            tempoRestanteEmSegundos = 0;
            imagemQueijo.fillAmount = 0;
            textoContagem.text = "00:00"; // Exibe o final de forma consistente

            // Opcional: Desativa este script para otimiza��o
            this.enabled = false;
        }
    }

    // Fun��o para formatar o tempo e atualizar o texto
    void AtualizarTextoDoTemporizador(float tempoParaExibir)
    {
        // Garante que n�o exiba tempo negativo
        if (tempoParaExibir < 0)
        {
            tempoParaExibir = 0;
        }

        // Calcula minutos e segundos
        float minutos = Mathf.FloorToInt(tempoParaExibir / 60);
        float segundos = Mathf.FloorToInt(tempoParaExibir % 60);

        // Formata o texto para o formato MM:SS, com zeros � esquerda
        // Ex: 1 minuto e 5 segundos ser� exibido como "01:05"
        textoContagem.text = string.Format("{0:00}:{1:00}", minutos, segundos);
    }
}