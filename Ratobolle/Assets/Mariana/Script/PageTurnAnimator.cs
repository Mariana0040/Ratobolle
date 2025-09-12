// Salve como PageTurnAnimator.cs
using UnityEngine;
using DG.Tweening; // N�o se esque�a de importar o DOTween

// Exige que um CanvasGroup esteja no mesmo objeto para a anima��o de fade
[RequireComponent(typeof(CanvasGroup))]
public class PageTurnAnimator : MonoBehaviour
{
    [Header("Configura��es da Anima��o")]
    [Tooltip("Dura��o do fade out (ou fade in). A anima��o total levar� o dobro do tempo.")]
    public float duracaoDoFade = 0.25f;

    private CanvasGroup canvasGroup;
    private bool estaAnimando = false;

    void Awake()
    {
        // Pega a refer�ncia do CanvasGroup automaticamente
        canvasGroup = GetComponent<CanvasGroup>();
    }

    /// <summary>
    /// Executa a anima��o de virada de p�gina (fade out -> executa uma a��o -> fade in).
    /// </summary>
    /// <param name="acaoNoMeioDaAnimacao">A tarefa a ser executada quando a tela est� preta.</param>
    public void AnimarViradaDePagina(System.Action acaoNoMeioDaAnimacao)
    {
        // Impede que o jogador vire v�rias p�ginas de uma vez
        if (estaAnimando) return;
        estaAnimando = true;

        // 1. Fade Out: A p�gina desaparece
        canvasGroup.DOFade(0, duracaoDoFade).SetEase(Ease.OutQuad)
            .OnComplete(() => {

                // 2. A��o Central: Executa a "tarefa" que o gerente nos deu
                // (Isso � onde o conte�do da p�gina ser� trocado)
                acaoNoMeioDaAnimacao?.Invoke();

                // 3. Fade In: A nova p�gina aparece
                canvasGroup.DOFade(1, duracaoDoFade).SetEase(Ease.InQuad)
                    .OnComplete(() => {
                        // 4. Libera a trava da anima��o
                        estaAnimando = false;
                    });
            });
    }
}