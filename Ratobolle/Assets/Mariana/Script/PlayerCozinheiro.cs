// Salve como PlayerCozinheiro.cs
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class PlayerCozinheiro : MonoBehaviour
{
    public static PlayerCozinheiro Instance { get; private set; }

    [Header("Configurações de Interação")]
    public float raioDeInteracao = 2.0f;
    public KeyCode teclaDeEntrega = KeyCode.E;
    public KeyCode teclaDoLivro = KeyCode.R;

    [Header("Lógica de Cozinha")]
    public Transform pontoPratoSegurado;

    // --- CONTROLE INTERNO ---
    private CollectibleItemData pratoAtual;
    private GameObject modeloPratoAtual;

    // Referência para o nosso novo Animator
    public BookAnimator bookAnimator;

    // Lógica do livro de receitas movida para cá
    private List<ReceitaSO> receitasDisponiveis;
    private int paginaAtual = 0;
    private bool livroAberto = false;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); } else { Instance = this; }
    }

    void Start()
    {

    }

    void Update()
    {
        // 1. Lógica para ABRIR/FECHAR o livro
        if (Input.GetKeyDown(teclaDoLivro))
        {
            TentarAbrirFecharLivro();
        }

        // 2. Se o livro está aberto, permite NAVEGAÇÃO e CRIAÇÃO
        if (livroAberto)
        {
            if (Input.GetKeyDown(KeyCode.X)) MudarPagina(-1);
            if (Input.GetKeyDown(KeyCode.C)) MudarPagina(1);
            if (Input.GetKeyDown(KeyCode.Z)) CriarReceitaAtual();
        }

        // 3. Lógica para ENTREGAR a comida
        if (Input.GetKeyDown(teclaDeEntrega))
        {
            TentarEntregarComida();
        }
    }

    // --- MÉTODOS DE CONTROLE DO LIVRO ---

    void TentarAbrirFecharLivro()
    {
        
        if (bookAnimator == null) return;

        if (livroAberto)
        {
            FecharLivro();
        }
        else
        {
            Collider[] colliders = Physics.OverlapSphere(transform.position, raioDeInteracao);
            foreach (var col in colliders)
            {
                if (col.gameObject.CompareTag("BancadaDaCozinha"))
                {
                    AbrirLivro();
                    return;
                }
            }
        }
    }

    void AbrirLivro()
    {
        livroAberto = true;
        bookAnimator.OpenBook();

        receitasDisponiveis = GerenciadorRestaurante.Instance.ObterReceitasDisponiveis();
        paginaAtual = 0;
        AtualizarPaginaVisual();
    }

    void FecharLivro()
    {
        livroAberto = false;
        bookAnimator.CloseBook();
    }

    void MudarPagina(int direcao)
    {
        int proximaPagina = paginaAtual + direcao;
        if (proximaPagina < 0 || proximaPagina >= receitasDisponiveis.Count) return;

        bookAnimator.AnimatePageTurn(() => {
            paginaAtual = proximaPagina;
            AtualizarPaginaVisual();
        });
    }

    // A FUNÇÃO QUE VOCÊ PRECISAVA
    void AtualizarPaginaVisual()
    {
        if (receitasDisponiveis == null || receitasDisponiveis.Count == 0) return;

        ReceitaSO receitaAtual = receitasDisponiveis[paginaAtual];
        bool temIngredientes = VerificarIngredientes(receitaAtual);

        // Manda o BookAnimator atualizar sua aparência
        // IMPORTANTE: Assumindo que a imagem da sua página está no campo 'emojiPrato' da sua ReceitaSO
        bookAnimator.UpdatePageContent(receitaAtual.paginaReceita, temIngredientes);
    }

    bool VerificarIngredientes(ReceitaSO receita)
    {
        foreach (var ingrediente in receita.ingredientesNecessarios)
        {
            if (SimplifiedPlayerInventory.Instance.GetItemCount(ingrediente.itemData.itemName) < ingrediente.quantity)
            {
                return false;
            }
        }
        return true;
    }

    void CriarReceitaAtual()
    {
        ReceitaSO receita = receitasDisponiveis[paginaAtual];
        if (!VerificarIngredientes(receita))
        {
            Debug.Log("Faltam ingredientes para esta receita!");
            return;
        }

        foreach (var ingrediente in receita.ingredientesNecessarios)
        {
            SimplifiedPlayerInventory.Instance.RemoveItem(ingrediente.itemData.itemName, ingrediente.quantity);
        }

        SegurarPrato(receita.pratoFinal);
        FecharLivro();
    }

    // --- MÉTODOS DE ENTREGA E MANIPULAÇÃO DE PRATOS ---

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