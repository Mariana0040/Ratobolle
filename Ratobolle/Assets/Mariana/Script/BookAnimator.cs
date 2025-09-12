// Salve como BookAnimator.cs
using UnityEngine;
using UnityEngine.UI; // Precisamos disso para o componente Image
using System.Collections.Generic; // Precisamos disso para a Lista de Sprites
using DG.Tweening; // Precisamos disso para a animação

[RequireComponent(typeof(CanvasGroup))] // Garante que o fade funcione
public class BookAnimator : MonoBehaviour
{
    [Header("Referências Visuais")]
    [Tooltip("O painel principal do livro que será ativado/desativado.")]
    public GameObject bookPanel;
    [Tooltip("O componente Image que vai mostrar a página atual.")]
    public Image pageDisplay;
    [Tooltip("Arraste aqui TODAS as imagens (Sprites) das suas páginas, na ordem correta.")]
    public List<Sprite> pages;

    [Header("Configurações da Animação")]
    public float animationDuration = 0.25f;

    // Propriedade para que outros scripts saibam se o livro está aberto
    public bool IsOpen => bookPanel.activeSelf;

    // Controle Interno
    private int currentPageIndex = 0;
    private GorditoController playerController;
    private CanvasGroup canvasGroup;
    private bool isAnimating = false;

    void Awake()
    {
        // Encontra o jogador e o CanvasGroup automaticamente
        playerController = Object.FindFirstObjectByType<GorditoController>();
        canvasGroup = GetComponent<CanvasGroup>();
        // Garante que o livro comece fechado
        bookPanel.SetActive(false);
    }

    void Update()
    {
        // Se o livro não estiver aberto ou estiver no meio de uma animação, não faz nada
        if (!IsOpen || isAnimating) return;

        // Navegação de páginas
        if (Input.GetKeyDown(KeyCode.X))
        {
            TurnPage(-1); // Voltar página
        }
        if (Input.GetKeyDown(KeyCode.C))
        {
            TurnPage(1); // Avançar página
        }
    }

    // Chamado pelo Player para ABRIR o livro
    public void OpenBook()
    {
        if (IsOpen) return;

        currentPageIndex = 0; // Sempre abre na primeira página
        ShowPage(currentPageIndex); // Mostra a primeira página
        bookPanel.SetActive(true);
        playerController?.LockMovement(true); // Trava o jogador
    }

    // Chamado pelo Player para FECHAR o livro
    public void CloseBook()
    {
        if (!IsOpen) return;

        bookPanel.SetActive(false);
        playerController?.LockMovement(false); // Destrava o jogador
    }

    // Ação de virar a página com animação
    private void TurnPage(int direction)
    {
        int nextPageIndex = currentPageIndex + direction;

        // Impede de virar para uma página que não existe
        if (nextPageIndex < 0 || nextPageIndex >= pages.Count)
        {
            return;
        }
        currentPageIndex = nextPageIndex;

        // Animação de Fade Out -> Troca Imagem -> Fade In
        isAnimating = true;
        canvasGroup.DOFade(0, animationDuration / 2).OnComplete(() =>
        {
            ShowPage(currentPageIndex);
            canvasGroup.DOFade(1, animationDuration / 2).OnComplete(() =>
            {
                isAnimating = false;
            });
        });
    }

    // Ação simples de trocar a imagem no componente Image
    private void ShowPage(int index)
    {
        if (pages != null && pages.Count > 0)
        {
            pageDisplay.sprite = pages[index];
        }
    }
}