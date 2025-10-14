using UnityEngine;
using TMPro; // Para controlar o texto
using System.Collections.Generic; // Para usar o sistema de Fila (Queue)

public class VovoGuiaController : MonoBehaviour
{
    // --- VARI�VEIS P�BLICAS (CONFIGURADAS NO INSPECTOR) ---
    public Transform alvoParaSeguir;
    public Vector3 offset = new Vector3(1.5f, 1.0f, 0);
    public float velocidadeDeSuavizacao = 5.0f;

    // --- VARI�VEIS DO SISTEMA DE DI�LOGO ---
    private GameObject canvasDialogo; // A refer�ncia para o Canvas do di�logo
    private TMP_Text textoDialogo; // A refer�ncia para o campo de texto
    private Queue<string> frases; // Uma "fila" para guardar as frases da conversa
    private bool emDialogo = false;
    public TMP_Text textoPromptEnter; // <-- NOSSA NOVA LINHA!


    void Start()
    {
        // Encontra o Canvas e o Texto pelo nome e os guarda.
        // IMPORTANTE: Seus objetos devem ter EXATAMENTE esses nomes na Hierarchy!
        canvasDialogo = GameObject.Find("CanvasDialogo");
        textoDialogo = GameObject.Find("TextoDialogo").GetComponent<TMP_Text>();

        // Inicia com a fila de frases vazia
        frases = new Queue<string>();

        // Garante que o di�logo comece escondido
        if (canvasDialogo != null)
        {
            canvasDialogo.SetActive(false);
        }
    }

    void Update()
    {
        // Se a vov� tem um alvo e n�o est� no meio de uma conversa...
        if (alvoParaSeguir != null && !emDialogo)
        {
            // ...ela segue o jogador.
            Vector3 posicaoDesejada = alvoParaSeguir.position + offset;
            transform.position = Vector3.Lerp(transform.position, posicaoDesejada, velocidadeDeSuavizacao * Time.deltaTime);
            transform.LookAt(alvoParaSeguir);
        }

        // Se estamos em di�logo e o jogador aperta Enter... (Mudei para a tecla Enter!)
        if (emDialogo && (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter)))
        {
            // ...mostra a pr�xima frase.
            MostrarProximaFrase();
        }
    }

    // Fun��o que o gatilho chama. Agora ela recebe um ARRAY de strings.
    public void IniciarConversa(string[] conversa)
    {
        emDialogo = true;
        canvasDialogo.SetActive(true); // Mostra a caixa de di�logo
        textoPromptEnter.gameObject.SetActive(true); // <-- MOSTRA o texto do "Enter"


        frases.Clear(); // Limpa qualquer conversa antiga

        // Adiciona cada frase da conversa na nossa fila
        foreach (string frase in conversa)
        {
            frases.Enqueue(frase);
        }

        // Mostra a PRIMEIRA frase da fila
        MostrarProximaFrase();
    }

    public void MostrarProximaFrase()
    {
        // Se a fila de frases acabou...
        if (frases.Count == 0)
        {
            // ...encerra a conversa.
            EncerrarConversa();
            return; // Sai da fun��o
        }

        // Pega a pr�xima frase da fila
        string fraseAtual = frases.Dequeue();
        // Coloca a frase no campo de texto da tela
        textoDialogo.text = fraseAtual;
    }

    void EncerrarConversa()
    {
        emDialogo = false;
        canvasDialogo.SetActive(false); // Esconde a caixa de di�logo
        textoPromptEnter.gameObject.SetActive(false); // <-- ESCONDE o texto do "Enter"
    }
}