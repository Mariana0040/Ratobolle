// Salve como PageTurnAnimator.cs
using UnityEngine;
using DG.Tweening; // Não se esqueça de importar o DOTween

// Exige que um CanvasGroup esteja no mesmo objeto para a animação de fade
[RequireComponent(typeof(CanvasGroup))]
public class PageTurnAnimator : MonoBehaviour
{
    [Header("Configurações da Animação")]
    [Tooltip("Duração do fade out (ou fade in). A animação total levará o dobro do tempo.")]
    public float duracaoDoFade = 0.25f;

    private CanvasGroup canvasGroup;
    private bool estaAnimando = false;

    void Awake()
    {
        // Pega a referência do CanvasGroup automaticamente
        canvasGroup = GetComponent<CanvasGroup>();
    }

    /// <summary>
    /// Executa a animação de virada de página (fade out -> executa uma ação -> fade in).
    /// </summary>
    /// <param name="acaoNoMeioDaAnimacao">A tarefa a ser executada quando a tela está preta.</param>
    public void AnimarViradaDePagina(System.Action acaoNoMeioDaAnimacao)
    {
        // Impede que o jogador vire várias páginas de uma vez
        if (estaAnimando) return;
        estaAnimando = true;

        // 1. Fade Out: A página desaparece
        canvasGroup.DOFade(0, duracaoDoFade).SetEase(Ease.OutQuad)
            .OnComplete(() => {

                // 2. Ação Central: Executa a "tarefa" que o gerente nos deu
                // (Isso é onde o conteúdo da página será trocado)
                acaoNoMeioDaAnimacao?.Invoke();

                // 3. Fade In: A nova página aparece
                canvasGroup.DOFade(1, duracaoDoFade).SetEase(Ease.InQuad)
                    .OnComplete(() => {
                        // 4. Libera a trava da animação
                        estaAnimando = false;
                    });
            });
    }
}