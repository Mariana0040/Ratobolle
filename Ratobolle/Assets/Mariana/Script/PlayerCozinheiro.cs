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
    public BookAnimator bookAnimator; // Arraste aqui o objeto com o script BookAnimator

    private CollectibleItemData pratoAtual;
    private GameObject modeloPratoAtual;
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
        if (Input.GetKeyDown(teclaDoLivro))
        {
            TentarAbrirFecharLivro();
        }

        if (livroAberto)
        {
            if (Input.GetKeyDown(KeyCode.X)) MudarPagina(-1); // Esquerda
            if (Input.GetKeyDown(KeyCode.C)) MudarPagina(1);  // Direita
            if (Input.GetKeyDown(KeyCode.Z)) CriarReceitaAtual();
        }

        if (Input.GetKeyDown(teclaDeEntrega))
        {
            TentarEntregarComida();
        }
    }

    void TentarAbrirFecharLivro()
    {
        if (bookAnimator == null) return;
        livroAberto = !livroAberto;

        if (livroAberto)
        {
            AbrirLivro();
        }
        else
        {
            FecharLivro();
        }
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

        // A MÁGICA: Passamos a lógica de atualização como um "presente" para a animação
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

        // O PlayerCozinheiro apenas MANDA o BookAnimator se atualizar.
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