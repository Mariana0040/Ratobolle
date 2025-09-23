using UnityEngine;
using DG.Tweening;

/// <summary>
/// Anima o t�tulo do jogo. O objeto entra (movimento + rota��o + escala) na tela e,
/// ao chegar na posi��o final, fica em um loop de pulso (escala).
/// </summary>
public class TitleAnimator : MonoBehaviour
{
    [Header("Anima��o de Entrada (Rolagem/Apari��o)")]
    [Tooltip("Posi��o inicial do t�tulo, fora da tela. Ser� a 'anchoredPosition'.")]
    public Vector3 startOffset = new Vector3(-200, -100, 0); // Exemplo: um pouco abaixo e � esquerda da posi��o final
    [Tooltip("Dura��o em segundos para o t�tulo chegar ao seu destino.")]
    public float entryDuration = 1.5f;
    [Tooltip("Efeito de suaviza��o (easing) para a entrada.")]
    public Ease entryEase = Ease.OutBack; // Um efeito que "exagera" e volta

    [Tooltip("�ngulo total que o t�tulo ir� girar durante a entrada (para simular rolagem).")]
    public float rollAngleEntry = 360f; // Ex: 360f para uma volta completa no eixo Z

    [Header("Anima��o de Pulso (Idle)")]
    [Tooltip("O quanto a escala deve aumentar no pulso. 1.05 = 5% maior.")]
    public float pulseScaleMultiplier = 1.05f;
    [Tooltip("Dura��o de cada pulso (ida ou volta).")]
    public float pulseDuration = 1.0f;

    private RectTransform rectTransform;
    private Vector3 originalScale; // Guarda a escala que o objeto tem no Inspector
    private Vector3 originalAnchoredPosition; // Guarda a posi��o final que o designer configurou
    private Quaternion originalRotation; // Guarda a rota��o original (geralmente Quaternion.identity para UI)

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        if (rectTransform == null)
        {
            Debug.LogError("Este script precisa ser anexado a um objeto de UI com um RectTransform.");
            return;
        }

        // Salva a escala, posi��o e rota��o originais ANTES de qualquer anima��o
        // A posi��o atual no Inspector ser� a 'posi��o final' para a entrada.
        originalAnchoredPosition = rectTransform.anchoredPosition;
        originalScale = rectTransform.localScale;
        originalRotation = rectTransform.localRotation; // Para UI, geralmente ser� Quaternion.identity

        // Inicia a anima��o de entrada
        PlayEntryAnimation();
    }

    private void PlayEntryAnimation()
    {
        // 1. Define o estado inicial para a anima��o de entrada
        rectTransform.localScale = Vector3.zero; // Come�a invis�vel
        rectTransform.anchoredPosition = originalAnchoredPosition + startOffset; // Come�a com um offset
        rectTransform.localRotation = Quaternion.Euler(0, 0, -rollAngleEntry); // Come�a "girado" no eixo Z

        // 2. Cria uma sequ�ncia para controlar m�ltiplas anima��es
        Sequence entrySequence = DOTween.Sequence();

        // 3. Anima a escala para o tamanho original
        entrySequence.Append(rectTransform.DOScale(originalScale, entryDuration).SetEase(entryEase));

        // 4. Anima o movimento para a posi��o final (originalAnchoredPosition)
        // Usa Join para que isso aconte�a ao mesmo tempo que a escala
        entrySequence.Join(rectTransform.DOLocalMove(originalAnchoredPosition, entryDuration).SetEase(entryEase));

        // 5. Anima a rota��o para a rota��o original (simulando a rolagem)
        // Usa Join para que isso aconte�a ao mesmo tempo
        entrySequence.Join(rectTransform.DORotate(originalRotation.eulerAngles, entryDuration, RotateMode.FastBeyond360).SetEase(entryEase));

        // 6. Define o que acontece QUANDO a sequ�ncia terminar
        entrySequence.OnComplete(StartPulsingIdleAnimation);
    }

    private void StartPulsingIdleAnimation()
    {
        // Garante que a rota��o e escala estejam exatamente nas originais ap�s a entrada
        rectTransform.localRotation = originalRotation;
        rectTransform.localScale = originalScale;

        // Anima a escala para o valor multiplicado e de volta ao original (LoopType.Yoyo)
        rectTransform.DOScale(originalScale * pulseScaleMultiplier, pulseDuration)
            .SetEase(Ease.InOutSine)       // Suaviza o in�cio e o fim do pulso
            .SetLoops(-1, LoopType.Yoyo);  // -1 significa loop infinito
    }
}