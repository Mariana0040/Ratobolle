// Salve como FinishZone.cs
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class FinishZone : MonoBehaviour
{
    [Header("Referências")]
    [Tooltip("Arraste o objeto da sua cena que tem o script 'TemporizadorDeQueijo'.")]
    [SerializeField] private TemporizadorDeQueijo temporizador;

    // Opcional: UI para dizer "Pressione E para Finalizar"
    [SerializeField] private GameObject promptDeInteracaoUI;

    // Controla se o jogador está dentro da zona de gatilho
    private bool isPlayerInRange = false;

    void Awake()
    {
        // Garante que o collider seja um trigger, uma prática segura
        GetComponent<Collider>().isTrigger = true;

        if (promptDeInteracaoUI != null)
        {
            promptDeInteracaoUI.SetActive(false);
        }
    }

    void Update()
    {
        // Se o jogador estiver dentro da área E apertar a tecla 'E'...
        if (isPlayerInRange && Input.GetKeyDown(KeyCode.E))
        {
            // Verifica se a referência ao temporizador existe para evitar erros
            if (temporizador != null)
            {
                Debug.Log("<color=cyan>Jogador apertou 'E' na zona segura! A PROVA TERMINOU!</color>");

                // Manda o comando para o temporizador parar
                temporizador.StopTimer();

                // Desativa a si mesmo para não ser usado de novo na mesma rodada
                this.enabled = false;
                if (promptDeInteracaoUI != null)
                {
                    promptDeInteracaoUI.SetActive(false);
                }
            }
        }
    }

    // Chamado pela Unity quando um objeto entra no trigger
    private void OnTriggerEnter(Collider other)
    {
        // Verifica se o objeto que entrou tem a tag "Player"
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = true;
            if (promptDeInteracaoUI != null)
            {
                promptDeInteracaoUI.SetActive(true);
            }
        }
    }

    // Chamado pela Unity quando um objeto sai do trigger
    private void OnTriggerExit(Collider other)
    {
        // Verifica se o objeto que saiu tem a tag "Player"
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = false;
            if (promptDeInteracaoUI != null)
            {
                promptDeInteracaoUI.SetActive(false);
            }
        }
    }
}