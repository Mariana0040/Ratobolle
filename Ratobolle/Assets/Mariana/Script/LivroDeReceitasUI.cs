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
    [Header("Refer�ncias da UI")]
    public GameObject painelLivro;
    public TextMeshProUGUI nomeReceitaTexto;
    public TextMeshProUGUI textoPagina;
    public Transform containerIngredientes;
    public GameObject prefabIconeIngrediente;
    public Button botaoCriar;

    [Header("Feedback Visual da P�gina")]
    [Tooltip("A imagem de fundo da p�gina que mudar� de cor.")]
    public Image paginaBackground;
    public Color corNormal = Color.white;
    public Color corSemIngredientes = new Color(1f, 0.8f, 0.8f); // Um vermelho claro

    [Header("Anima��o")]
    public float duracaoAnimacaoPagina = 0.25f;

    // Propriedade p�blica para que o PlayerCozinheiro possa saber se o livro est� aberto
    public bool IsOpen => painelLivro.activeSelf;

    // --- CONTROLE INTERNO ---
    private List<ReceitaSO> receitasDisponiveis;
    private int paginaAtual = 0;
    private GorditoController playerController;
    private CanvasGroup painelCanvasGroup;
    private bool isPageTurning = false;

    void Awake()
    {
        // Pega as refer�ncias essenciais no in�cio
        playerController = Object.FindFirstObjectByType<GorditoController>(); // Encontra o controlador do jogador na cena
        painelCanvasGroup = GetComponent<CanvasGroup>();
        painelLivro.SetActive(false); // Garante que o livro comece fechado
    }

    void Update()
    {
        // Se o livro n�o estiver aberto, este script n�o faz nada
        if (!IsOpen) return;

        // Se a p�gina n�o estiver no meio de uma anima��o de virada...
        if (!isPageTurning)
        {
            // ...permite a navega��o com as teclas X e C e a cria��o com Z
            if (Input.GetKeyDown(KeyCode.X)) MudarPagina(-1);
            if (Input.GetKeyDown(KeyCode.C)) MudarPagina(1);
            if (Input.GetKeyDown(KeyCode.Z)) CriarReceitaAtual();
        }
    }

    // Chamado pelo PlayerCozinheiro para ABRIR o livro
    public void AbrirLivro(List<ReceitaSO> receitas)
    {
        if (IsOpen) return; // Se j� est� aberto, n�o faz nada

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
        if (!IsOpen) return; // Se j� est� fechado, n�o faz nada

        painelLivro.SetActive(false);
        // Destrava o movimento do jogador
        playerController?.LockMovement(false);
    }

    void MudarPagina(int direcao)
    {
        int proximaPagina = paginaAtual + direcao;

        // Impede que o jogador vire para uma p�gina que n�o existe
        if (proximaPagina < 0 || proximaPagina >= receitasDisponiveis.Count)
        {
            return;
        }

        paginaAtual = proximaPagina;

        // --- ANIMA��O COM DOTWEEN ---
        isPageTurning = true; // Trava o input de virar a p�gina
        // 1. Fade out da p�gina atual
        painelCanvasGroup.DOFade(0, duracaoAnimacaoPagina / 2).SetEase(Ease.OutQuad)
            .OnComplete(() => {
                // 2. Quando a tela est� preta, atualiza o conte�do da p�gina
                MostrarPagina(paginaAtual);
                // 3. E ent�o faz o fade in com o novo conte�do
                painelCanvasGroup.DOFade(1, duracaoAnimacaoPagina / 2).SetEase(Ease.InQuad)
                    .OnComplete(() => {
                        // 4. Libera o input quando a anima��o terminar
                        isPageTurning = false;
                    });
            });
    }

    void MostrarPagina(int index)
    {
        // Limpa os �cones de ingredientes da p�gina anterior
        foreach (Transform child in containerIngredientes) Destroy(child.gameObject);

        ReceitaSO receita = receitasDisponiveis[index];
        nomeReceitaTexto.text = receita.nomeReceita;
        textoPagina.text = $"P�gina {index + 1}/{receitasDisponiveis.Count}";

        bool podeCriar = true;
        foreach (var ingrediente in receita.ingredientesNecessarios)
        {
            // (Seu c�digo para instanciar e preencher o prefab do �cone do ingrediente vai aqui)

            int quantidadeQueTem = SimplifiedPlayerInventory.Instance.GetItemCount(ingrediente.itemData.itemName);
            if (quantidadeQueTem < ingrediente.quantity)
            {
                podeCriar = false;
            }
        }

        // Muda a cor do fundo da p�gina com base na disponibilidade de ingredientes
        if (paginaBackground != null)
        {
            paginaBackground.color = podeCriar ? corNormal : corSemIngredientes;
        }

        // Ativa ou desativa o bot�o de criar
        botaoCriar.interactable = podeCriar;
    }

    public void CriarReceitaAtual()
    {
        if (!botaoCriar.interactable) return; // N�o faz nada se o bot�o estiver desativado

        ReceitaSO receita = receitasDisponiveis[paginaAtual];

        // Remove os ingredientes do invent�rio
        foreach (var ingrediente in receita.ingredientesNecessarios)
        {
            SimplifiedPlayerInventory.Instance.RemoveItem(ingrediente.itemData.itemName, ingrediente.quantity);
        }

        // Manda o jogador segurar o prato pronto
        PlayerCozinheiro.Instance.SegurarPrato(receita.pratoFinal);

        // Fecha o livro automaticamente ap�s criar a receita
        FecharLivro();
    }
}