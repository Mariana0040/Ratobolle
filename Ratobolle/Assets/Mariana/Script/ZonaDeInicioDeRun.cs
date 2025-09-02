using UnityEngine;
using TMPro; // Necessário para controlar o texto de UI

// Este script vai em um objeto Trigger na cozinha do chefe
public class ZonaDeInicioDeRun : MonoBehaviour
{
    [Header("Referências")]
    [Tooltip("Arraste para cá o objeto da cena que tem o script 'TemporizadorDeQueijo'.")]
    [SerializeField] private TemporizadorDeQueijo temporizador;

    [Tooltip("Texto da UI que mostra a dica 'Aperte E para começar'.")]
    [SerializeField] private TextMeshProUGUI textoDeInteracao;

    // --- NOVO ---
    [Tooltip("A barreira física que impede o jogador de entrar na cozinha antes de iniciar.")]
    [SerializeField] private GameObject barreiraParaDesativar;

    [Header("Configurações")]
    [Tooltip("Tag do objeto que pode iniciar a run (geralmente 'Player').")]
    [SerializeField] private string tagDoJogador = "Player";

    private bool jogadorNaZona = false;

    void Start()
    {
        // Garante que o texto comece desligado
        if (textoDeInteracao != null)
        {
            textoDeInteracao.gameObject.SetActive(false);
        }

        // --- NOVO ---
        // Garante que a barreira comece LIGADA
        if (barreiraParaDesativar != null)
        {
            barreiraParaDesativar.SetActive(true);
        }
    }

    void Update()
    {
        // Se o jogador está na zona e aperta a tecla "E"
        if (jogadorNaZona && Input.GetKeyDown(KeyCode.E))
        {
            if (temporizador != null)
            {
                // Manda a ordem para o temporizador começar
                temporizador.IniciarCronometro();

                // Esconde o texto de interação
                if (textoDeInteracao != null)
                {
                    textoDeInteracao.gameObject.SetActive(false);
                }

                // --- NOVO ---
                // Desativa a barreira para liberar o caminho
                if (barreiraParaDesativar != null)
                {
                    barreiraParaDesativar.SetActive(false);
                }

                // Desativa este script para não poder ser usado novamente
                this.enabled = false;
            }
            else
            {
                Debug.LogError("Temporizador não foi definido no Inspector da ZonaDeInicioDeRun!", this);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(tagDoJogador))
        {
            jogadorNaZona = true;
            if (textoDeInteracao != null)
            {
                textoDeInteracao.gameObject.SetActive(true);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(tagDoJogador))
        {
            jogadorNaZona = false;
            if (textoDeInteracao != null)
            {
                textoDeInteracao.gameObject.SetActive(false);
            }
        }
    }
}