// Salve como BookAnimator.cs
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using DG.Tweening;

[RequireComponent(typeof(CanvasGroup))]
public class BookAnimator : MonoBehaviour
{
    [Header("Referências Visuais Essenciais")]
    public GameObject bookPanel;
    [Tooltip("O componente Image que vai mostrar a imagem da página atual.")]
    public Image pageDisplay;

    [Header("Feedback de Ingredientes")]
    public Color colorNormal = Color.white;
    public Color colorSemIngredientes = new Color(1f, 0.8f, 0.8f);

    [Header("Configurações da Animação")]
    public float animationDuration = 0.25f;

    public bool IsOpen => bookPanel.activeSelf;

    // Controle Interno
    private GorditoController playerController;
    private CanvasGroup canvasGroup;
    private bool isAnimating = false;

    void Awake()
    {
        playerController = FindFirstObjectByType<GorditoController>();
        canvasGroup = GetComponent<CanvasGroup>();
        bookPanel.SetActive(false);
    }

    public void OpenBook()
    {
        if (IsOpen) return;
        bookPanel.SetActive(true);
        playerController?.LockMovement(true);
    }

    public void CloseBook()
    {
        if (!IsOpen) return;
        bookPanel.SetActive(false);
        playerController?.LockMovement(false);
    }

    public void UpdatePageContent(Sprite paginaSprite, bool temIngredientes)
    {
        pageDisplay.sprite = paginaSprite;
        pageDisplay.color = temIngredientes ? colorNormal : colorSemIngredientes;
    }

    public void AnimatePageTurn(System.Action acaoNoMeio)
    {
        if (isAnimating) return;
        isAnimating = true;
        canvasGroup.DOFade(0, animationDuration / 2).OnComplete(() =>
        {
            acaoNoMeio?.Invoke();
            canvasGroup.DOFade(1, animationDuration / 2).OnComplete(() =>
            {
                isAnimating = false;
            });
        });
    }
}