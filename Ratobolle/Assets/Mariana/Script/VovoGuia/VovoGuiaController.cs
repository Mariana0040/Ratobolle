using UnityEngine;
using TMPro;
using System.Collections.Generic;
using DG.Tweening; // <--- PASSO 1: IMPORTAR A BIBLIOTECA DO DOTWEEN

public class VovoGuiaController : MonoBehaviour
{
    // --- VARIÁVEIS PÚBLICAS (CONFIGURADAS NO INSPECTOR) ---
    [Header("Configurações de Movimento")]
    public Transform alvoParaSeguir;
    public Vector3 offset = new Vector3(1.5f, 1.0f, 0);
    public float velocidadeDeSeguir = 2.0f; // Agora é uma velocidade mais direta

    [Header("Componentes do Diálogo")]
    public GameObject canvasDialogo;
    public TMP_Text textoDialogo;
    public TMP_Text textoPromptEnter;

    // --- Variáveis Internas ---
    private Queue<string> frases;
    private bool emDialogo = false;
    private Vector3 posicaoOriginalEscala; // Para guardar a escala original

    void Awake() // Usamos Awake para garantir que a escala seja guardada antes de tudo
    {
        // Guarda a escala original da vovó para a animação de "surgir"
        posicaoOriginalEscala = transform.localScale;
        // Começa com a vovó invisível para ela poder "aparecer"
        transform.localScale = Vector3.zero;
    }

    void Start()
    {
        frases = new Queue<string>();
        if (canvasDialogo != null)
        {
            canvasDialogo.SetActive(false);
        }

        // --- NOVA ANIMAÇÃO DE SURGIR! ---
        // Anima a escala de 0 para a escala original em 0.5 segundos, com um efeito "elástico" no final.
        transform.DOScale(posicaoOriginalEscala, 0.5f).SetEase(Ease.OutBack);
    }

    void Update()
    {
        // Se a vovó tem um alvo e não está no meio de uma conversa...
        if (alvoParaSeguir != null && !emDialogo)
        {
            // --- MOVIMENTO DE SEGUIR COM DOTWEEN! ---
            Vector3 posicaoDesejada = alvoParaSeguir.position + offset;

            // Move a vovó para a posição desejada. DOTween cuida da suavização.
            // Usamos DOMove em vez de ficar atualizando a cada frame.
            transform.DOMove(posicaoDesejada, 1f / velocidadeDeSeguir).SetEase(Ease.OutSine);

            // Usa o DOLookAt para fazer a vovó olhar suavemente para o jogador
            transform.DOLookAt(alvoParaSeguir.position, 0.5f);
        }

        // Se estamos em diálogo e o jogador aperta Enter...
        if (emDialogo && (Input.GetKeyDown(KeyCode.Return)))
        {
            MostrarProximaFrase();
        }
    }

    public void IniciarConversa(string[] conversa)
    {
        emDialogo = true;

        // --- NOVA ANIMAÇÃO DE CONVERSA! ---
        // Para qualquer movimento que a vovó esteja fazendo para seguir
        transform.DOKill();

        // Move a vovó para uma posição de conversa, na frente do jogador
        Vector3 posicaoConversa = alvoParaSeguir.position + alvoParaSeguir.forward * 2f + Vector3.up;
        transform.DOMove(posicaoConversa, 0.5f).SetEase(Ease.OutCubic);

        // Vira a vovó para olhar diretamente para o jogador
        transform.DOLookAt(alvoParaSeguir.position, 0.3f);

        // Balanço infinito estilo Aku Aku!
        transform.DOShakeRotation(100f, new Vector3(0, 5, 0), 10, 90, false, ShakeRandomnessMode.Harmonic).SetLoops(-1);

        // O resto é igual...
        canvasDialogo.SetActive(true);
        if (textoPromptEnter != null) textoPromptEnter.gameObject.SetActive(true);

        frases.Clear();
        foreach (string frase in conversa)
        {
            frases.Enqueue(frase);
        }
        MostrarProximaFrase();
    }

    public void MostrarProximaFrase()
    {
        if (frases.Count == 0)
        {
            EncerrarConversa();
            return;
        }
        string fraseAtual = frases.Dequeue();
        textoDialogo.text = fraseAtual;
    }

    void EncerrarConversa()
    {
        emDialogo = false;

        // Para a animação de balanço
        transform.DOKill();
        // Refaz a animação de escala para garantir que ela esteja no tamanho certo
        transform.DOScale(posicaoOriginalEscala, 0.2f);

        canvasDialogo.SetActive(false);
        if (textoPromptEnter != null) textoPromptEnter.gameObject.SetActive(false);
    }
}