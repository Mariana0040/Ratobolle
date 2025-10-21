// Salve como BookAnimator.cs
using UnityEngine;
using UnityEngine.UI;
using System;
using DG.Tweening;

public class BookAnimator : MonoBehaviour
{
    [Header("Refer�ncias Visuais")]
    [Tooltip("O objeto principal do painel do livro, para escalar e animar.")]
    public GameObject bookPanel;

    [Tooltip("A IMAGEM DO LIVRO EM SI. Esta imagem ficar� por baixo de tudo.")]
    public Image bookImage; // Renomeado de 'backgroundImage' para mais clareza

    [Tooltip("A IMAGEM DA P�GINA DA RECEITA. � ESTA que vai mudar de cor e de sprite.")]
    public Image recipePageImage; // Renomeado de 'recipeImage' para mais clareza

    // --- CORRE��O #1: As cores agora se aplicam � p�gina da receita ---
    [Header("Cores da P�gina da Receita")]
    public Color canCreateColor = Color.white; // Cor normal
    public Color cannotCreateColor = new Color(1f, 0.6f, 0.6f, 1f); // Vermelho claro

    [Header("Configura��es de Anima��o (DOTween)")]
    public float animationDuration = 0.4f;
    public Ease animationEase = Ease.OutBack;
    public float pageTurnDuration = 0.5f;

    private Vector2 pageCenterPosition = Vector2.zero;
    private Vector2 pageOffscreenLeft = new Vector2(-1000, 0);
    private Vector2 pageOffscreenRight = new Vector2(1000, 0);
    private bool isAnimating = false;

    void Start()
    {
        if (bookPanel != null)
        {
            bookPanel.transform.localScale = Vector3.zero;
            bookPanel.SetActive(false);
        }
    }

    public void OpenBook()
    {
        if (isAnimating) return;
        isAnimating = true;
        bookPanel.SetActive(true);
        bookPanel.transform.DOScale(1f, animationDuration)
            .SetEase(animationEase)
            .OnComplete(() => isAnimating = false);
    }

    public void CloseBook()
    {
        if (isAnimating) return;
        isAnimating = true;
        bookPanel.transform.DOScale(0f, animationDuration)
            .SetEase(Ease.InBack)
            .OnComplete(() =>
            {
                isAnimating = false;
                bookPanel.SetActive(false);
            });
    }

    public void TurnPage(int direction, Action onMidpoint)
    {
        if (isAnimating || recipePageImage == null) return;
        isAnimating = true;

        Vector2 exitPosition = (direction > 0) ? pageOffscreenLeft : pageOffscreenRight;
        Vector2 entryPosition = (direction > 0) ? pageOffscreenRight : pageOffscreenLeft;

        Sequence pageTurnSequence = DOTween.Sequence();

        // --- CORRE��O #2: A anima��o agora move a 'recipePageImage' ---
        pageTurnSequence.Append(recipePageImage.rectTransform.DOAnchorPos(exitPosition, pageTurnDuration / 2).SetEase(Ease.InQuad));
        pageTurnSequence.AppendCallback(() =>
        {
            onMidpoint?.Invoke();
            recipePageImage.rectTransform.anchoredPosition = entryPosition;
        });
        pageTurnSequence.Append(recipePageImage.rectTransform.DOAnchorPos(pageCenterPosition, pageTurnDuration / 2).SetEase(Ease.OutQuad));
        pageTurnSequence.OnComplete(() => isAnimating = false);
    }

    public void UpdatePageContent(Sprite newRecipeSprite, bool canCreate)
    {
        if (recipePageImage != null)
        {
            recipePageImage.sprite = newRecipeSprite;
            // --- CORRE��O #3: A cor � aplicada na 'recipePageImage' ---
            recipePageImage.DOColor(canCreate ? canCreateColor : cannotCreateColor, 0.2f);
        }
    }
}