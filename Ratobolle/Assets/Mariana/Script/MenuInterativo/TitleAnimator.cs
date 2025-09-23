using UnityEngine;
using DG.Tweening;

/// <summary>
/// Anima o título do jogo. O objeto entra (movimento + rotação + escala) na tela e,
/// ao chegar na posição final, fica em um loop de pulso (escala).
/// </summary>
public class TitleAnimator : MonoBehaviour
{
    [Header("Animação de Movimento e Rotação")]
    [Tooltip("Posição inicial do título, fora da tela. Será a 'anchoredPosition'.")]
    public Vector3 initialPosition = new Vector3(0, 800f, 0); // Exemplo: começa bem acima
    [Tooltip("Posição final onde o título deve ficar. Será a 'anchoredPosition'.")]
    public Vector3 finalPosition = new Vector3(0, 300f, 0); // Sua posição atual
    [Tooltip("Duração em segundos para o título chegar ao seu destino.")]
    public float moveDuration = 1.5f;
    [Tooltip("Efeito de suavização (easing) para o movimento.")]
    public Ease moveEase = Ease.OutBounce; // Um efeito "quicando" ao chegar

    [Tooltip("Ângulo total que o título irá girar durante a entrada.")]
    public float rollAngle = 360f; // Ex: 360f para uma volta completa
    [Tooltip("Duração da rotação (pode ser diferente da duração do movimento).")]
    public float rollDuration = 1.2f; // Pode ser um pouco mais rápido que o movimento

    [Header("Animação de Pulso (Idle)")]
    [Tooltip("O quanto a escala deve aumentar no pulso. 1.05 = 5% maior.")]
    public float pulseScaleMultiplier = 1.05f;
    [Tooltip("Duração de cada pulso (ida ou volta).")]
    public float pulseDuration = 1.0f;

    private RectTransform rectTransform;
    private Vector3 originalScale; // Guarda a escala que o objeto tem no Inspector
    private Quaternion originalRotation; // Guarda a rotação original

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        if (rectTransform == null)
        {
            Debug.LogError("Este script precisa ser anexado a um objeto de UI com um RectTransform.");
            return;
        }

        // Salva a escala e rotação originais ANTES de qualquer animação
        originalScale = rectTransform.localScale;
        originalRotation = rectTransform.localRotation;

        // Inicia a animação de entrada
        PlayEntryAnimation();
    }

    private void PlayEntryAnimation()
    {
        // 1. Define a posição inicial do logo (fora da tela)
        // Usamos anchoredPosition para UI
        rectTransform.anchoredPosition = initialPosition;
        // Começa com escala zero ou bem pequena para um efeito de "crescer"
        rectTransform.localScale = Vector3.zero;
        // Começa com uma rotação para simular o início do "rolar"
        rectTransform.localRotation = Quaternion.Euler(0, 0, -rollAngle); // Começa "girado" para trás

        // 2. Cria uma sequência para controlar múltiplas animações
        Sequence entrySequence = DOTween.Sequence();

        // 3. Anima o movimento para a posição final
        // DOLocalMove é para anchoredPosition em RectTransform
        entrySequence.Append(rectTransform.DOLocalMove(finalPosition, moveDuration).SetEase(moveEase));

        // 4. Anima a rotação ao mesmo tempo (Join)
        entrySequence.Join(rectTransform.DORotate(originalRotation.eulerAngles, rollDuration, RotateMode.FastBeyond360).SetEase(moveEase));

        // 5. Anima a escala para o tamanho original ao mesmo tempo (Join)
        entrySequence.Join(rectTransform.DOScale(originalScale, moveDuration / 2).SetEase(Ease.OutBack)); // Cresce rapidamente

        // 6. Define o que acontece QUANDO a sequência terminar
        entrySequence.OnComplete(StartPulsingIdleAnimation);
    }

    private void StartPulsingIdleAnimation()
    {
        // Anima a escala para o valor multiplicado e de volta ao original (LoopType.Yoyo)
        rectTransform.DOScale(originalScale * pulseScaleMultiplier, pulseDuration)
            .SetEase(Ease.InOutSine)       // Suaviza o início e o fim do pulso
            .SetLoops(-1, LoopType.Yoyo);  // -1 significa loop infinito
    }
}