// Importações necessárias para UI da Unity e TextMesh Pro
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TemporizadorDeQueijo : MonoBehaviour
{
    [Header("Configurações do Temporizador")]
    [Tooltip("Tempo total da contagem regressiva em MINUTOS.")]
    public float tempoEmMinutos = 1.5f; // Agora você pode usar valores como 1.5 para 1 minuto e 30 segundos

    [Header("Referências de Componentes")]
    [Tooltip("Arraste o componente de Imagem do queijo aqui.")]
    public Image imagemQueijo;

    [Tooltip("Arraste o componente TextMeshPro (UI) da contagem regressiva aqui.")]
    public TextMeshProUGUI textoContagem;

    private float tempoRestanteEmSegundos;
    private float tempoTotalEmSegundos;

    void Start()
    {
        // Converte o tempo de minutos para segundos para o cálculo interno
        tempoTotalEmSegundos = tempoEmMinutos * 60f;
        tempoRestanteEmSegundos = tempoTotalEmSegundos;

        // Validação inicial da imagem
        if (imagemQueijo.type != Image.Type.Filled)
        {
            Debug.LogWarning("O tipo da imagem do queijo não está como 'Filled'. Alterando automaticamente.");
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
            // Diminui o tempo restante (cálculo em segundos)
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

            // Opcional: Desativa este script para otimização
            this.enabled = false;
        }
    }

    // Função para formatar o tempo e atualizar o texto
    void AtualizarTextoDoTemporizador(float tempoParaExibir)
    {
        // Garante que não exiba tempo negativo
        if (tempoParaExibir < 0)
        {
            tempoParaExibir = 0;
        }

        // Calcula minutos e segundos
        float minutos = Mathf.FloorToInt(tempoParaExibir / 60);
        float segundos = Mathf.FloorToInt(tempoParaExibir % 60);

        // Formata o texto para o formato MM:SS, com zeros à esquerda
        // Ex: 1 minuto e 5 segundos será exibido como "01:05"
        textoContagem.text = string.Format("{0:00}:{1:00}", minutos, segundos);
    }
}