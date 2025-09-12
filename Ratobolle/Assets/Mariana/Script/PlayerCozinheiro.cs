// Salve como PlayerCozinheiro.cs
using UnityEngine;

public class PlayerCozinheiro : MonoBehaviour
{
    // --- O CÓDIGO DO SINGLETON QUE ESTAVA FALTANDO ---
    public static PlayerCozinheiro Instance { get; private set; }
    // ---------------------------------------------------

    [Header("Configurações de Interação")]
    public float raioDeInteracao = 2.0f;
    public KeyCode teclaDeEntrega = KeyCode.E;
    public KeyCode teclaDoLivro = KeyCode.R;

    [Header("Lógica de Cozinha")]
    public Transform pontoPratoSegurado;

    // Controle Interno
    private CollectibleItemData pratoAtual;
    private GameObject modeloPratoAtual;
    private LivroDeReceitasUI livroDeReceitas;

    void Awake()
    {
        // --- CONFIGURAÇÃO DO SINGLETON ---
        // Garante que só exista uma instância do PlayerCozinheiro.
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
        // ---------------------------------
    }

    void Start()
    {
        livroDeReceitas = Object.FindFirstObjectByType<LivroDeReceitasUI>();
    }

    void Update()
    {
        if (Input.GetKeyDown(teclaDoLivro))
        {
            TentarAbrirFecharLivro();
        }

        if (Input.GetKeyDown(teclaDeEntrega))
        {
            TentarEntregarComida();
        }
    }

    void TentarAbrirFecharLivro()
    {
        if (livroDeReceitas == null) return;

        if (livroDeReceitas.IsOpen)
        {
            livroDeReceitas.FecharLivro();
            return;
        }

        Collider[] colliders = Physics.OverlapSphere(transform.position, raioDeInteracao);
        foreach (var col in colliders)
        {
            if (col.CompareTag("BancadaDeCozinha"))
            {
                livroDeReceitas.AbrirLivro(GerenciadorRestaurante.Instance.ObterReceitasDisponiveis());
                return;
            }
        }
    }

    void TentarEntregarComida()
    {
        if (pratoAtual == null) return;

        Collider[] colliders = Physics.OverlapSphere(transform.position, raioDeInteracao);
        foreach (var col in colliders)
        {
            if (col.TryGetComponent(out ClienteRatoAI cliente))
            {
                if (cliente.TentarEntregar(pratoAtual))
                {
                    GerenciadorRestaurante.Instance.RegistrarEntregaSucedida();
                    LimparPratoSegurado();
                    return;
                }
            }
        }
    }

    public void SegurarPrato(CollectibleItemData pratoData)
    {
        LimparPratoSegurado();
        pratoAtual = pratoData;

        if (pratoData.modelPrefab != null && pontoPratoSegurado != null)
        {
            modeloPratoAtual = Instantiate(pratoData.modelPrefab, pontoPratoSegurado);
        }
    }

    void LimparPratoSegurado()
    {
        pratoAtual = null;
        if (modeloPratoAtual != null)
        {
            Destroy(modeloPratoAtual);
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, raioDeInteracao);
    }
}