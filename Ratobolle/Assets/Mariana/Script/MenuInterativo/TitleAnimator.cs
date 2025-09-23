using UnityEngine;
using DG.Tweening;

/// <summary>
/// Anima o título do jogo. O objeto entra (movimento + rotação + escala) na tela e,
/// ao chegar na posição final, fica em um loop de pulso (escala).
/// </summary>
public class TitleAnimator : MonoBehaviour
{
    [Header("Animação de Entrada (Rolagem/Aparição)")]
    [Tooltip("Posição inicial do título, fora da tela. Será a 'anchoredPosition'.")]
    public Vector3 startOffset = new Vector3(-200, -100, 0); // Exemplo: um pouco abaixo e à esquerda da posição final
    [Tooltip("Duração em segundos para o título chegar ao seu destino.")]
    public float entryDuration = 1.5f;
    [Tooltip("Efeito de suavização (easing) para a entrada.")]
    public Ease entryEase = Ease.OutBack; // Um efeito que "exagera" e volta

    [Tooltip("Ângulo total que o título irá girar durante a entrada (para simular rolagem).")]
    public float rollAngleEntry = 360f; // Ex: 360f para uma volta completa no eixo Z

    [Header("Animação de Pulso (Idle)")]
    [Tooltip("O quanto a escala deve aumentar no pulso. 1.05 = 5% maior.")]
    public float pulseScaleMultiplier = 1.05f;
    [Tooltip("Duração de cada pulso (ida ou volta).")]
    public float pulseDuration = 1.0f;

    private RectTransform rectTransform;
    private Vector3 originalScale; // Guarda a escala que o objeto tem no Inspector
    private Vector3 originalAnchoredPosition; // Guarda a posição final que o designer configurou
    private Quaternion originalRotation; // Guarda a rotação original (geralmente Quaternion.identity para UI)

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        if (rectTransform == null)
        {
            Debug.LogError("Este script precisa ser anexado a um objeto de UI com um RectTransform.");
            return;
        }

        // Salva a escala, posição e rotação originais ANTES de qualquer animação
        // A posição atual no Inspector será a 'posição final' para a entrada.
        originalAnchoredPosition = rectTransform.anchoredPosition;
        originalScale = rectTransform.localScale;
        originalRotation = rectTransform.localRotation; // Para UI, geralmente será Quaternion.identity

        // Inicia a animação de entrada
        PlayEntryAnimation();
    }

    private void PlayEntryAnimation()
    {
        // 1. Define o estado inicial para a animação de entrada
        rectTransform.localScale = Vector3.zero; // Começa invisível
        rectTransform.anchoredPosition = originalAnchoredPosition + startOffset; // Começa com um offset
        rectTransform.localRotation = Quaternion.Euler(0, 0, -rollAngleEntry); // Começa "girado" no eixo Z

        // 2. Cria uma sequência para controlar múltiplas animações
        Sequence entrySequence = DOTween.Sequence();

        // 3. Anima a escala para o tamanho original
        entrySequence.Append(rectTransform.DOScale(originalScale, entryDuration).SetEase(entryEase));

        // 4. Anima o movimento para a posição final (originalAnchoredPosition)
        // Usa Join para que isso aconteça ao mesmo tempo que a escala
        entrySequence.Join(rectTransform.DOLocalMove(originalAnchoredPosition, entryDuration).SetEase(entryEase));

        // 5. Anima a rotação para a rotação original (simulando a rolagem)
        // Usa Join para que isso aconteça ao mesmo tempo
        entrySequence.Join(rectTransform.DORotate(originalRotation.eulerAngles, entryDuration, RotateMode.FastBeyond360).SetEase(entryEase));

        // 6. Define o que acontece QUANDO a sequência terminar
        entrySequence.OnComplete(StartPulsingIdleAnimation);
    }

    private void StartPulsingIdleAnimation()
    {
        // Garante que a rotação e escala estejam exatamente nas originais após a entrada
        rectTransform.localRotation = originalRotation;
        rectTransform.localScale = originalScale;

        // Anima a escala para o valor multiplicado e de volta ao original (LoopType.Yoyo)
        rectTransform.DOScale(originalScale * pulseScaleMultiplier, pulseDuration)
            .SetEase(Ease.InOutSine)       // Suaviza o início e o fim do pulso
            .SetLoops(-1, LoopType.Yoyo);  // -1 significa loop infinito
    }
}