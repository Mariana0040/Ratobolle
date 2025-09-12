// Salve como LivroDeReceitasUI.cs
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening; // Importa a biblioteca do DOTween

[RequireComponent(typeof(CanvasGroup))] // Garante que o CanvasGroup sempre exista para o fade
public class LivroDeReceitasUI : MonoBehaviour
{
    [Header("Referências da UI")]
    public GameObject painelLivro;
    public TextMeshProUGUI nomeReceitaTexto;
    public TextMeshProUGUI textoPagina;
    public Transform containerIngredientes;
    public GameObject prefabIconeIngrediente;
    public Button botaoCriar;

    [Header("Feedback Visual da Página")]
    [Tooltip("A imagem de fundo da página que mudará de cor.")]
    public Image paginaBackground;
    public Color corNormal = Color.white;
    public Color corSemIngredientes = new Color(1f, 0.8f, 0.8f); // Um vermelho claro

    [Header("Animação")]
    public float duracaoAnimacaoPagina = 0.25f;

    // Propriedade pública para que o PlayerCozinheiro possa saber se o livro está aberto
    public bool IsOpen => painelLivro.activeSelf;

    // --- CONTROLE INTERNO ---
    private List<ReceitaSO> receitasDisponiveis;
    private int paginaAtual = 0;
    private GorditoController playerController;
    private CanvasGroup painelCanvasGroup;
    private bool isPageTurning = false;

    void Awake()
    {
        // Pega as referências essenciais no início
        playerController = Object.FindFirstObjectByType<GorditoController>(); // Encontra o controlador do jogador na cena
        painelCanvasGroup = GetComponent<CanvasGroup>();
        painelLivro.SetActive(false); // Garante que o livro comece fechado
    }

    void Update()
    {
        // Se o livro não estiver aberto, este script não faz nada
        if (!IsOpen) return;

        // Se a página não estiver no meio de uma animação de virada...
        if (!isPageTurning)
        {
            // ...permite a navegação com as teclas X e C e a criação com Z
            if (Input.GetKeyDown(KeyCode.X)) MudarPagina(-1);
            if (Input.GetKeyDown(KeyCode.C)) MudarPagina(1);
            if (Input.GetKeyDown(KeyCode.Z)) CriarReceitaAtual();
        }
    }

    // Chamado pelo PlayerCozinheiro para ABRIR o livro
    public void AbrirLivro(List<ReceitaSO> receitas)
    {
        if (IsOpen) return; // Se já está aberto, não faz nada

        receitasDisponiveis = receitas;
        paginaAtual = 0;
        painelLivro.SetActive(true);
        MostrarPagina(paginaAtual);

        // Trava o movimento do jogador
        playerController?.LockMovement(true);
    }

    // Chamado pelo PlayerCozinheiro (ou internamente) para FECHAR o livro
    public void FecharLivro()
    {
        if (!IsOpen) return; // Se já está fechado, não faz nada

        painelLivro.SetActive(false);
        // Destrava o movimento do jogador
        playerController?.LockMovement(false);
    }

    void MudarPagina(int direcao)
    {
        int proximaPagina = paginaAtual + direcao;

        // Impede que o jogador vire para uma página que não existe
        if (proximaPagina < 0 || proximaPagina >= receitasDisponiveis.Count)
        {
            return;
        }

        paginaAtual = proximaPagina;

        // --- ANIMAÇÃO COM DOTWEEN ---
        isPageTurning = true; // Trava o input de virar a página
        // 1. Fade out da página atual
        painelCanvasGroup.DOFade(0, duracaoAnimacaoPagina / 2).SetEase(Ease.OutQuad)
            .OnComplete(() => {
                // 2. Quando a tela está preta, atualiza o conteúdo da página
                MostrarPagina(paginaAtual);
                // 3. E então faz o fade in com o novo conteúdo
                painelCanvasGroup.DOFade(1, duracaoAnimacaoPagina / 2).SetEase(Ease.InQuad)
                    .OnComplete(() => {
                        // 4. Libera o input quando a animação terminar
                        isPageTurning = false;
                    });
            });
    }

    void MostrarPagina(int index)
    {
        // Limpa os ícones de ingredientes da página anterior
        foreach (Transform child in containerIngredientes) Destroy(child.gameObject);

        ReceitaSO receita = receitasDisponiveis[index];
        nomeReceitaTexto.text = receita.nomeReceita;
        textoPagina.text = $"Página {index + 1}/{receitasDisponiveis.Count}";

        bool podeCriar = true;
        foreach (var ingrediente in receita.ingredientesNecessarios)
        {
            // (Seu código para instanciar e preencher o prefab do ícone do ingrediente vai aqui)

            int quantidadeQueTem = SimplifiedPlayerInventory.Instance.GetItemCount(ingrediente.itemData.itemName);
            if (quantidadeQueTem < ingrediente.quantity)
            {
                podeCriar = false;
            }
        }

        // Muda a cor do fundo da página com base na disponibilidade de ingredientes
        if (paginaBackground != null)
        {
            paginaBackground.color = podeCriar ? corNormal : corSemIngredientes;
        }

        // Ativa ou desativa o botão de criar
        botaoCriar.interactable = podeCriar;
    }

    public void CriarReceitaAtual()
    {
        if (!botaoCriar.interactable) return; // Não faz nada se o botão estiver desativado

        ReceitaSO receita = receitasDisponiveis[paginaAtual];

        // Remove os ingredientes do inventário
        foreach (var ingrediente in receita.ingredientesNecessarios)
        {
            SimplifiedPlayerInventory.Instance.RemoveItem(ingrediente.itemData.itemName, ingrediente.quantity);
        }

        // Manda o jogador segurar o prato pronto
        PlayerCozinheiro.Instance.SegurarPrato(receita.pratoFinal);

        // Fecha o livro automaticamente após criar a receita
        FecharLivro();
    }
}