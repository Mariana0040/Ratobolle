using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class PlayerCozinheiro : MonoBehaviour
{
    [Header("Configurações de Interação")]
    public float raioDeInteracao = 2.0f;
    public KeyCode teclaDeEntrega = KeyCode.E;
    public KeyCode teclaDoLivro = KeyCode.R;

    [Header("Lógica de Cozinha")]
    public Transform pontoPratoSegurado;
    public GameObject clochePrefab; // NOVO: Arraste aqui o Prefab do seu cloche
    public BookAnimator bookAnimator;

    private CollectibleItemData pratoAtual;
    private GameObject modeloSeguradoAtual; // Renomeado para maior clareza
    private List<ReceitaSO> receitasDisponiveis;
    private int paginaAtual = 0;
    private bool livroAberto = false;
    private SimplifiedPlayerInventory inventory;

    void Start()
    {
        inventory = Object.FindFirstObjectByType<SimplifiedPlayerInventory>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            TentarAbrirFecharLivro();
        }

        if (livroAberto)
        {
            if (Input.GetKeyDown(KeyCode.X)) MudarPagina(-1);
            if (Input.GetKeyDown(KeyCode.C)) MudarPagina(1);
            if (Input.GetKeyDown(KeyCode.Z)) CriarReceitaAtual(); // Pressionar Z para pegar o prato (com cloche)
        }

        if (Input.GetKeyDown(teclaDeEntrega))
        {
            TentarEntregarComida(); // Pressionar E para entregar
        }
    }

    // ALTERADO: Agora instancia o cloche, mas guarda a informação do prato.
    public void SegurarPrato(CollectibleItemData pratoData)
    {
        LimparPratoSegurado();
        pratoAtual = pratoData;

        if (clochePrefab == null || pontoPratoSegurado == null)
        {
            Debug.LogError("ERRO: O Prefab do Cloche não foi atribuído no Inspector!");
            Debug.LogError("ERRO: O 'Ponto Prato Segurado' não foi atribuído no Inspector!");
            return;


        }
        else
        {
            Debug.Log("Tudo certo! Criando o cloche...");
            modeloSeguradoAtual = Instantiate(clochePrefab, pontoPratoSegurado.position, pontoPratoSegurado.rotation, pontoPratoSegurado);
            // --- FIM DA MODIFICAÇÃO ---
        }
    }

    // Nenhuma alteração aqui. A lógica continua a mesma.
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
                    LimparPratoSegurado(); // Limpa o cloche da mão do jogador
                    return;
                }
            }
        }
    }

    // Renomeado para ficar mais claro que destrói qualquer modelo que o jogador estiver segurando.
    void LimparPratoSegurado()
    {
        pratoAtual = null;
        if (modeloSeguradoAtual != null)
        {
            Destroy(modeloSeguradoAtual);
        }
    }

    // --- O RESTO DO SEU CÓDIGO PERMANECE IGUAL ---
    #region Funções do Livro e Receitas
    void TentarAbrirFecharLivro()
    {
        if (bookAnimator == null) return;
        livroAberto = !livroAberto;

        if (livroAberto) AbrirLivro();
        else FecharLivro();
    }

    void AbrirLivro()
    {
        bookAnimator.OpenBook();
        receitasDisponiveis = GerenciadorRestaurante.Instance.ObterReceitasDisponiveis();
        paginaAtual = 0;
        AtualizarPaginaVisual();
    }

    void FecharLivro()
    {
        bookAnimator.CloseBook();
    }

    void MudarPagina(int direcao)
    {
        int proximaPagina = paginaAtual + direcao;
        if (proximaPagina < 0 || proximaPagina >= receitasDisponiveis.Count) return;

        bookAnimator.TurnPage(direcao, () => {
            paginaAtual = proximaPagina;
            AtualizarPaginaVisual();
        });
    }

    void AtualizarPaginaVisual()
    {
        if (receitasDisponiveis == null || receitasDisponiveis.Count == 0) return;

        ReceitaSO receitaAtual = receitasDisponiveis[paginaAtual];
        bool temIngredientes = VerificarIngredientes(receitaAtual);

        bookAnimator.UpdatePageContent(receitaAtual.paginaReceita, temIngredientes);
    }

    bool VerificarIngredientes(ReceitaSO receita)
    {
        if (inventory == null) return false;
        foreach (var ingrediente in receita.ingredientesNecessarios)
        {
            if (inventory.GetItemCount(ingrediente.itemData.itemName) < ingrediente.quantity)
            {
                return false;
            }
        }
        return true;
    }

    void CriarReceitaAtual()
    {
        if (receitasDisponiveis == null || paginaAtual < 0 || paginaAtual >= receitasDisponiveis.Count) return;

        ReceitaSO receita = receitasDisponiveis[paginaAtual];
        if (!VerificarIngredientes(receita))
        {
            Debug.Log("Faltam ingredientes!");
            return;
        }

        foreach (var ingrediente in receita.ingredientesNecessarios)
        {
            inventory.RemoveItem(ingrediente.itemData.itemName, ingrediente.quantity);
        }

        SegurarPrato(receita.pratoFinal);
        FecharLivro();
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, raioDeInteracao);
    }
    #endregion
}