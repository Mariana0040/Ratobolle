using UnityEngine;
using TMPro;
using System.Collections.Generic;
using DG.Tweening; // <--- PASSO 1: IMPORTAR A BIBLIOTECA DO DOTWEEN

public class VovoGuiaController : MonoBehaviour
{
    // --- VARI�VEIS P�BLICAS (CONFIGURADAS NO INSPECTOR) ---
    [Header("Configura��es de Movimento")]
    public Transform alvoParaSeguir;
    public Vector3 offset = new Vector3(1.5f, 1.0f, 0);
    public float velocidadeDeSeguir = 2.0f; // Agora � uma velocidade mais direta

    [Header("Componentes do Di�logo")]
    public GameObject canvasDialogo;
    public TMP_Text textoDialogo;
    public TMP_Text textoPromptEnter;

    // --- Vari�veis Internas ---
    private Queue<string> frases;
    private bool emDialogo = false;
    private Vector3 posicaoOriginalEscala; // Para guardar a escala original

    void Awake() // Usamos Awake para garantir que a escala seja guardada antes de tudo
    {
        // Guarda a escala original da vov� para a anima��o de "surgir"
        posicaoOriginalEscala = transform.localScale;
        // Come�a com a vov� invis�vel para ela poder "aparecer"
        transform.localScale = Vector3.zero;
    }

    void Start()
    {
        frases = new Queue<string>();
        if (canvasDialogo != null)
        {
            canvasDialogo.SetActive(false);
        }

        // --- NOVA ANIMA��O DE SURGIR! ---
        // Anima a escala de 0 para a escala original em 0.5 segundos, com um efeito "el�stico" no final.
        transform.DOScale(posicaoOriginalEscala, 0.5f).SetEase(Ease.OutBack);
    }

    void Update()
    {
        // Se a vov� tem um alvo e n�o est� no meio de uma conversa...
        if (alvoParaSeguir != null && !emDialogo)
        {
            // --- MOVIMENTO DE SEGUIR COM DOTWEEN! ---
            Vector3 posicaoDesejada = alvoParaSeguir.position + offset;

            // Move a vov� para a posi��o desejada. DOTween cuida da suaviza��o.
            // Usamos DOMove em vez de ficar atualizando a cada frame.
            transform.DOMove(posicaoDesejada, 1f / velocidadeDeSeguir).SetEase(Ease.OutSine);

            // Usa o DOLookAt para fazer a vov� olhar suavemente para o jogador
            transform.DOLookAt(alvoParaSeguir.position, 0.5f);
        }

        // Se estamos em di�logo e o jogador aperta Enter...
        if (emDialogo && (Input.GetKeyDown(KeyCode.Return)))
        {
            MostrarProximaFrase();
        }
    }

    public void IniciarConversa(string[] conversa)
    {
        emDialogo = true;

        // --- NOVA ANIMA��O DE CONVERSA! ---
        // Para qualquer movimento que a vov� esteja fazendo para seguir
        transform.DOKill();

        // Move a vov� para uma posi��o de conversa, na frente do jogador
        Vector3 posicaoConversa = alvoParaSeguir.position + alvoParaSeguir.forward * 2f + Vector3.up;
        transform.DOMove(posicaoConversa, 0.5f).SetEase(Ease.OutCubic);

        // Vira a vov� para olhar diretamente para o jogador
        transform.DOLookAt(alvoParaSeguir.position, 0.3f);

        // Balan�o infinito estilo Aku Aku!
        transform.DOShakeRotation(100f, new Vector3(0, 5, 0), 10, 90, false, ShakeRandomnessMode.Harmonic).SetLoops(-1);

        // O resto � igual...
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

        // Para a anima��o de balan�o
        transform.DOKill();
        // Refaz a anima��o de escala para garantir que ela esteja no tamanho certo
        transform.DOScale(posicaoOriginalEscala, 0.2f);

        canvasDialogo.SetActive(false);
        if (textoPromptEnter != null) textoPromptEnter.gameObject.SetActive(false);
    }
}