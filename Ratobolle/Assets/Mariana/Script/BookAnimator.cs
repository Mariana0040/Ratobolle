// Salve como BookAnimator.cs
using UnityEngine;
using UnityEngine.UI; // Precisamos disso para o componente Image
using System.Collections.Generic; // Precisamos disso para a Lista de Sprites
using DG.Tweening; // Precisamos disso para a anima��o

[RequireComponent(typeof(CanvasGroup))] // Garante que o fade funcione
public class BookAnimator : MonoBehaviour
{
    [Header("Refer�ncias Visuais")]
    [Tooltip("O painel principal do livro que ser� ativado/desativado.")]
    public GameObject bookPanel;
    [Tooltip("O componente Image que vai mostrar a p�gina atual.")]
    public Image pageDisplay;
    [Tooltip("Arraste aqui TODAS as imagens (Sprites) das suas p�ginas, na ordem correta.")]
    public List<Sprite> pages;

    [Header("Configura��es da Anima��o")]
    public float animationDuration = 0.25f;

    // Propriedade para que outros scripts saibam se o livro est� aberto
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
        // Se o livro n�o estiver aberto ou estiver no meio de uma anima��o, n�o faz nada
        if (!IsOpen || isAnimating) return;

        // Navega��o de p�ginas
        if (Input.GetKeyDown(KeyCode.X))
        {
            TurnPage(-1); // Voltar p�gina
        }
        if (Input.GetKeyDown(KeyCode.C))
        {
            TurnPage(1); // Avan�ar p�gina
        }
    }

    // Chamado pelo Player para ABRIR o livro
    public void OpenBook()
    {
        if (IsOpen) return;

        currentPageIndex = 0; // Sempre abre na primeira p�gina
        ShowPage(currentPageIndex); // Mostra a primeira p�gina
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

    // A��o de virar a p�gina com anima��o
    private void TurnPage(int direction)
    {
        int nextPageIndex = currentPageIndex + direction;

        // Impede de virar para uma p�gina que n�o existe
        if (nextPageIndex < 0 || nextPageIndex >= pages.Count)
        {
            return;
        }
        currentPageIndex = nextPageIndex;

        // Anima��o de Fade Out -> Troca Imagem -> Fade In
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

    // A��o simples de trocar a imagem no componente Image
    private void ShowPage(int index)
    {
        if (pages != null && pages.Count > 0)
        {
            pageDisplay.sprite = pages[index];
        }
    }
}