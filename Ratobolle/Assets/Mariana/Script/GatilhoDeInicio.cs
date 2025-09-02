using UnityEngine;
using TMPro;

// Este script vai em um objeto Trigger na porta da cozinha do chefe.
// Ele � de uso �nico.
[RequireComponent(typeof(Collider))]
public class GatilhoDeInicio : MonoBehaviour
{
    // --- NOVO: Define o "modo de opera��o" deste gatilho ---
    public enum TipoDeGatilho { Aviso, Inicio }
    [Header("Configura��o do Gatilho")]
    [Tooltip("Defina se este gatilho � para AVISAR o jogador ou para INICIAR a contagem.")]
    [SerializeField] private TipoDeGatilho tipoDeGatilho = TipoDeGatilho.Aviso;

    [Header("Refer�ncias")]
    [Tooltip("Arraste para c� o objeto da cena que tem o script 'TemporizadorDeQueijo'.")]
    [SerializeField] private TemporizadorDeQueijo temporizador;

    [Tooltip("Arraste para c� o texto da UI que ser� mostrado/escondido.")]
    [SerializeField] private TextMeshProUGUI textoDeAviso;

    [Header("Configura��es")]
    [Tooltip("Tag do objeto que pode ativar o gatilho (geralmente 'Player').")]
    [SerializeField] private string tagDoJogador = "Player";

    private bool jaFoiAtivado = false;

    void Start()
    {
        // Garante que o texto comece sempre desligado.
        if (textoDeAviso != null)
        {
            textoDeAviso.gameObject.SetActive(false);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (jaFoiAtivado || !other.CompareTag(tagDoJogador)) return;

        // --- L�GICA PRINCIPAL ---
        // O script age de forma diferente dependendo do seu tipo.
        switch (tipoDeGatilho)
        {
            // Se este gatilho for do tipo "Aviso"
            case TipoDeGatilho.Aviso:
                if (textoDeAviso != null)
                {
                    textoDeAviso.gameObject.SetActive(true);
                }
                break;

            // Se este gatilho for do tipo "In�cio"
            case TipoDeGatilho.Inicio:
                if (temporizador != null)
                {
                    temporizador.IniciarCronometro();
                    jaFoiAtivado = true;

                    if (textoDeAviso != null)
                    {
                        textoDeAviso.gameObject.SetActive(false);
                    }
                    gameObject.SetActive(false); // Desativa este gatilho para sempre
                }
                break;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag(tagDoJogador)) return;

        // Apenas o gatilho de "Aviso" precisa fazer algo quando o jogador sai.
        if (tipoDeGatilho == TipoDeGatilho.Aviso)
        {
            if (textoDeAviso != null)
            {
                textoDeAviso.gameObject.SetActive(false);
            }
        }
    }
}