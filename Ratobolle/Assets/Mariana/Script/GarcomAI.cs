using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Animator))]
public class GarcomAI : MonoBehaviour
{
    // --- ESTADOS DO GARÇOM ---
    private enum Estado { Patrulhando, Perseguindo, Capturado }
    private Estado estadoAtual;

    [Header("Patrulha e Perseguição")]
    public List<Transform> pontosPatrulha;
    public float velocidadePatrulha = 3.0f;
    public float velocidadePerseguicao = 7.0f; // Garçons são rápidos!
    public Transform alvo;

    [Header("Efeito de Raiva")]
    [Tooltip("O material vermelho que será aplicado quando ele capturar o jogador.")]
    public Material materialVermelho;
    [Tooltip("Arraste para cá o componente Skinned Mesh Renderer do modelo do garçom.")]
    public Renderer modeloDoPersonagem;

    // --- COMPONENTES E CONTROLE ---
    private NavMeshAgent agente;
    private Animator animator;
    private int indicePontoAtual = 0;
    private bool foiAtivado = false; // Chave para iniciar a perseguição

    void Start()
    {
        agente = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();

        // Começa patrulhando normalmente
        estadoAtual = Estado.Patrulhando;
        if (pontosPatrulha.Count > 0)
        {
            MoverParaProximoPonto();
        }

        if (alvo == null && GameObject.FindGameObjectWithTag("Player") != null)
        {
            alvo = GameObject.FindGameObjectWithTag("Player").transform;
        }
    }

    void Update()
    {
        // Roda a lógica baseada no estado atual
        switch (estadoAtual)
        {
            case Estado.Patrulhando:
                ExecutarPatrulha();
                break;
            case Estado.Perseguindo:
                ExecutarPerseguicao();
                break;
            case Estado.Capturado:
                // Não faz nada, fica parado e com raiva.
                break;
        }
        AtualizarAnimacao();
    }

    // --- DETECÇÃO E CAPTURA ---

    // Chamado quando o jogador entra na ÁREA DE VISÃO (Trigger)
    private void OnTriggerEnter(Collider other)
    {
        // Se já foi ativado ou já capturou o jogador, não faz nada
        if (foiAtivado || estadoAtual == Estado.Capturado) return;

        if (other.CompareTag("Player"))
        {
            Debug.Log("Garçom viu o Gordito! Iniciando perseguição!");
            foiAtivado = true;
            estadoAtual = Estado.Perseguindo;
        }
    }

    // Chamado quando o garçom ENCOSTA FISICAMENTE no jogador (Collision)
    private void OnCollisionEnter(Collision collision)
    {
        // Só captura se estiver no estado de perseguição
        if (estadoAtual != Estado.Perseguindo) return;

        if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("Garçom capturou o Gordito! FÚRIA!");
            CapturarJogador();
        }
    }

    // --- COMPORTAMENTOS ---

    private void ExecutarPatrulha()
    {
        agente.speed = velocidadePatrulha;
        agente.isStopped = false;

        if (!agente.pathPending && agente.remainingDistance < 0.5f)
        {
            MoverParaProximoPonto();
        }
    }

    private void ExecutarPerseguicao()
    {
        agente.speed = velocidadePerseguicao;
        agente.isStopped = false;
        agente.SetDestination(alvo.position);
    }

    private void CapturarJogador()
    {
        estadoAtual = Estado.Capturado;
        agente.isStopped = true; // PARA O MOVIMENTO

        // Troca o material para vermelho
        if (modeloDoPersonagem != null && materialVermelho != null)
        {
            modeloDoPersonagem.material = materialVermelho;
        }
    }

    // --- FUNÇÕES AUXILIARES ---

    private void MoverParaProximoPonto()
    {
        if (pontosPatrulha.Count == 0) return;
        agente.SetDestination(pontosPatrulha[indicePontoAtual].position);
        indicePontoAtual = (indicePontoAtual + 1) % pontosPatrulha.Count;
    }

    private void AtualizarAnimacao()
    {
        // Animação de andar só se estiver patrulhando
        animator.SetBool("isWalking", estadoAtual == Estado.Patrulhando);
        // Animação de correr só se estiver perseguindo
        animator.SetBool("isRunning", estadoAtual == Estado.Perseguindo);
    }
}