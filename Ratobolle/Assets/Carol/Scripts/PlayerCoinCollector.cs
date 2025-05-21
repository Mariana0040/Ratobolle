using UnityEngine;
using TMPro; // Namespace para TextMeshPro

public class PlayerCoinCollector : MonoBehaviour
{
    [Header("UI")]
    [Tooltip("Refer�ncia ao objeto TextMeshPro UI que mostrar� a contagem de moedas.")]
    [SerializeField] private TextMeshProUGUI coinCountText;

    [Header("Contagem de Moedas")]
    [SerializeField] private int currentCoinCount = 0; // Come�a com 0 moedas

    [Header("Detec��o")]
    [Tooltip("Defina esta layer para os objetos de moeda para otimizar a detec��o.")]
    [SerializeField] private LayerMask coinLayer; // Configure no Inspetor

    private AudioSource audioSource; // Para tocar o som da moeda, se a moeda tiver um

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        // Se precisar adicionar um AudioSource se n�o existir:
        // if (audioSource == null && /* alguma condi��o para precisar de audioSource */)
        // {
        //     audioSource = gameObject.AddComponent<AudioSource>();
        // }
    }

    void Start()
    {
        // Atualiza a UI no in�cio do jogo
        UpdateCoinUI();
    }

    // Chamado automaticamente pela Unity quando o Collider deste GameObject (jogador)
    // entra em um outro Collider que est� marcado como 'Is Trigger'.
    void OnTriggerEnter(Collider other)
    {
        // Opcional: Verifica se o objeto que entrou est� na layer de moedas (otimiza��o)
        // (1 << other.gameObject.layer) cria uma m�scara para a layer do objeto 'other'
        // A opera��o '&' (E bit a bit) verifica se a layer do objeto 'other' est� inclu�da na nossa coinLayerMask
        // Se coinLayer n�o estiver definida (value == 0), ele tentar� pegar o componente Coin de qualquer trigger.
        if (coinLayer.value == 0 || (coinLayer.value & (1 << other.gameObject.layer)) > 0)
        {
            // Tenta pegar o componente Coin do objeto que entrou no trigger
            Coin coin = other.GetComponent<Coin>();

            // Se o objeto realmente for uma moeda (tem o script Coin)...
            if (coin != null)
            {
                // Adiciona o valor da moeda � contagem atual
                currentCoinCount += coin.value;
                Debug.Log($"Moeda COLETADA AUTOMATICAMENTE! Valor: {coin.value}. Total: {currentCoinCount}");

                // Atualiza o texto na UI
                UpdateCoinUI();

                // Chama a fun��o Collect da pr�pria moeda (para efeitos, som e destrui��o)
                coin.Collect(audioSource); // Passa o AudioSource do jogador, se houver
            }
        }
    }

    // Fun��o para atualizar o TextMeshPro UI
    void UpdateCoinUI()
    {
        if (coinCountText != null)
        {
            coinCountText.text = currentCoinCount.ToString();
            // Exemplo de formata��o para ter sempre 3 d�gitos (001, 015, 123):
            // coinCountText.text = "Moedas: " + currentCoinCount.ToString("D3");
        }
        else
        {
            // Este aviso � importante! Se ele aparecer, voc� n�o arrastou o objeto de texto para o script.
            Debug.LogWarning("PlayerCoinCollector: Refer�ncia do TextMeshPro para contagem de moedas (coinCountText) n�o definida no Inspetor!");
        }
    }

    // Fun��es p�blicas para que outros scripts possam interagir com a contagem de moedas
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
        return false; // N�o tem moedas suficientes
    }
}