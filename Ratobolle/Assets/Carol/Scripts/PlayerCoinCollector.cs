using UnityEngine;
using TMPro; // Namespace para TextMeshPro

public class PlayerCoinCollector : MonoBehaviour
{
    [Header("UI")]
    [Tooltip("Referência ao objeto TextMeshPro UI que mostrará a contagem de moedas.")]
    [SerializeField] private TextMeshProUGUI coinCountText;

    [Header("Contagem de Moedas")]
    [SerializeField] private int currentCoinCount = 0; // Começa com 0 moedas

    [Header("Detecção")]
    [Tooltip("Defina esta layer para os objetos de moeda para otimizar a detecção.")]
    [SerializeField] private LayerMask coinLayer; // Configure no Inspetor

    private AudioSource audioSource; // Para tocar o som da moeda, se a moeda tiver um

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        // Se precisar adicionar um AudioSource se não existir:
        // if (audioSource == null && /* alguma condição para precisar de audioSource */)
        // {
        //     audioSource = gameObject.AddComponent<AudioSource>();
        // }
    }

    void Start()
    {
        // Atualiza a UI no início do jogo
        UpdateCoinUI();
    }

    // Chamado automaticamente pela Unity quando o Collider deste GameObject (jogador)
    // entra em um outro Collider que está marcado como 'Is Trigger'.
    void OnTriggerEnter(Collider other)
    {
        // Opcional: Verifica se o objeto que entrou está na layer de moedas (otimização)
        // (1 << other.gameObject.layer) cria uma máscara para a layer do objeto 'other'
        // A operação '&' (E bit a bit) verifica se a layer do objeto 'other' está incluída na nossa coinLayerMask
        // Se coinLayer não estiver definida (value == 0), ele tentará pegar o componente Coin de qualquer trigger.
        if (coinLayer.value == 0 || (coinLayer.value & (1 << other.gameObject.layer)) > 0)
        {
            // Tenta pegar o componente Coin do objeto que entrou no trigger
            Coin coin = other.GetComponent<Coin>();

            // Se o objeto realmente for uma moeda (tem o script Coin)...
            if (coin != null)
            {
                // Adiciona o valor da moeda à contagem atual
                currentCoinCount += coin.value;
                Debug.Log($"Moeda COLETADA AUTOMATICAMENTE! Valor: {coin.value}. Total: {currentCoinCount}");

                // Atualiza o texto na UI
                UpdateCoinUI();

                // Chama a função Collect da própria moeda (para efeitos, som e destruição)
                coin.Collect(audioSource); // Passa o AudioSource do jogador, se houver
            }
        }
    }

    // Função para atualizar o TextMeshPro UI
    void UpdateCoinUI()
    {
        if (coinCountText != null)
        {
            coinCountText.text = currentCoinCount.ToString();
            // Exemplo de formatação para ter sempre 3 dígitos (001, 015, 123):
            // coinCountText.text = "Moedas: " + currentCoinCount.ToString("D3");
        }
        else
        {
            // Este aviso é importante! Se ele aparecer, você não arrastou o objeto de texto para o script.
            Debug.LogWarning("PlayerCoinCollector: Referência do TextMeshPro para contagem de moedas (coinCountText) não definida no Inspetor!");
        }
    }

    // Funções públicas para que outros scripts possam interagir com a contagem de moedas
    public int GetCoinCount()
    {
        return currentCoinCount;
    }

    public void AddCoins(int amount) // Se algo externo der moedas
    {
        currentCoinCount += amount;
        UpdateCoinUI();
    }

    public bool SpendCoins(int amount) // Se o jogador gastar moedas
    {
        if (currentCoinCount >= amount)
        {
            currentCoinCount -= amount;
            UpdateCoinUI();
            return true; // Conseguiu gastar
        }
        return false; // Não tem moedas suficientes
    }
}