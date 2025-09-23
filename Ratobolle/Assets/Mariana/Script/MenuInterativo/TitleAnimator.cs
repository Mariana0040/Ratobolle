using UnityEngine;
using DG.Tweening;

/// <summary>
/// Anima o t�tulo do jogo. O objeto entra (movimento + rota��o + escala) na tela e,
/// ao chegar na posi��o final, fica em um loop de pulso (escala).
/// </summary>
public class TitleAnimator : MonoBehaviour
{
    [Header("Anima��o de Movimento e Rota��o")]
    [Tooltip("Posi��o inicial do t�tulo, fora da tela. Ser� a 'anchoredPosition'.")]
    public Vector3 initialPosition = new Vector3(0, 800f, 0); // Exemplo: come�a bem acima
    [Tooltip("Posi��o final onde o t�tulo deve ficar. Ser� a 'anchoredPosition'.")]
    public Vector3 finalPosition = new Vector3(0, 300f, 0); // Sua posi��o atual
    [Tooltip("Dura��o em segundos para o t�tulo chegar ao seu destino.")]
    public float moveDuration = 1.5f;
    [Tooltip("Efeito de suaviza��o (easing) para o movimento.")]
    public Ease moveEase = Ease.OutBounce; // Um efeito "quicando" ao chegar

    [Tooltip("�ngulo total que o t�tulo ir� girar durante a entrada.")]
    public float rollAngle = 360f; // Ex: 360f para uma volta completa
    [Tooltip("Dura��o da rota��o (pode ser diferente da dura��o do movimento).")]
    public float rollDuration = 1.2f; // Pode ser um pouco mais r�pido que o movimento

    [Header("Anima��o de Pulso (Idle)")]
    [Tooltip("O quanto a escala deve aumentar no pulso. 1.05 = 5% maior.")]
    public float pulseScaleMultiplier = 1.05f;
    [Tooltip("Dura��o de cada pulso (ida ou volta).")]
    public float pulseDuration = 1.0f;

    private RectTransform rectTransform;
    private Vector3 originalScale; // Guarda a escala que o objeto tem no Inspector
    private Quaternion originalRotation; // Guarda a rota��o original

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        if (rectTransform == null)
        {
            Debug.LogError("Este script precisa ser anexado a um objeto de UI com um RectTransform.");
            return;
        }

        // Salva a escala e rota��o originais ANTES de qualquer anima��o
        originalScale = rectTransform.localScale;
        originalRotation = rectTransform.localRotation;

        // Inicia a anima��o de entrada
        PlayEntryAnimation();
    }

    private void PlayEntryAnimation()
    {
        // 1. Define a posi��o inicial do logo (fora da tela)
        // Usamos anchoredPosition para UI
        rectTransform.anchoredPosition = initialPosition;
        // Come�a com escala zero ou bem pequena para um efeito de "crescer"
        rectTransform.localScale = Vector3.zero;
        // Come�a com uma rota��o para simular o in�cio do "rolar"
        rectTransform.localRotation = Quaternion.Euler(0, 0, -rollAngle); // Come�a "girado" para tr�s

        // 2. Cria uma sequ�ncia para controlar m�ltiplas anima��es
        Sequence entrySequence = DOTween.Sequence();

        // 3. Anima o movimento para a posi��o final
        // DOLocalMove � para anchoredPosition em RectTransform
        entrySequence.Append(rectTransform.DOLocalMove(finalPosition, moveDuration).SetEase(moveEase));

        // 4. Anima a rota��o ao mesmo tempo (Join)
        entrySequence.Join(rectTransform.DORotate(originalRotation.eulerAngles, rollDuration, RotateMode.FastBeyond360).SetEase(moveEase));

        // 5. Anima a escala para o tamanho original ao mesmo tempo (Join)
        entrySequence.Join(rectTransform.DOScale(originalScale, moveDuration / 2).SetEase(Ease.OutBack)); // Cresce rapidamente

        // 6. Define o que acontece QUANDO a sequ�ncia terminar
        entrySequence.OnComplete(StartPulsingIdleAnimation);
    }

    private void StartPulsingIdleAnimation()
    {
        // Anima a escala para o valor multiplicado e de volta ao original (LoopType.Yoyo)
        rectTransform.DOScale(originalScale * pulseScaleMultiplier, pulseDuration)
            .SetEase(Ease.InOutSine)       // Suaviza o in�cio e o fim do pulso
            .SetLoops(-1, LoopType.Yoyo);  // -1 significa loop infinito
    }
}