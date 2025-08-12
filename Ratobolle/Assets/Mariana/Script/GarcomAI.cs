using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Animator))]
public class GarcomAI : MonoBehaviour
{
    // --- ESTADOS DO GAR�OM ---
    private enum Estado { Patrulhando, Perseguindo, Capturado }
    private Estado estadoAtual;

    [Header("Patrulha e Persegui��o")]
    public List<Transform> pontosPatrulha;
    public float velocidadePatrulha = 3.0f;
    public float velocidadePerseguicao = 7.0f; // Gar�ons s�o r�pidos!
    public Transform alvo;

    [Header("Efeito de Raiva")]
    [Tooltip("O material vermelho que ser� aplicado quando ele capturar o jogador.")]
    public Material materialVermelho;
    [Tooltip("Arraste para c� o componente Skinned Mesh Renderer do modelo do gar�om.")]
    public Renderer modeloDoPersonagem;

    // --- COMPONENTES E CONTROLE ---
    private NavMeshAgent agente;
    private Animator animator;
    private int indicePontoAtual = 0;
    private bool foiAtivado = false; // Chave para iniciar a persegui��o

    void Start()
    {
        agente = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();

        // Come�a patrulhando normalmente
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
        // Roda a l�gica baseada no estado atual
        switch (estadoAtual)
        {
            case Estado.Patrulhando:
                ExecutarPatrulha();
                break;
            case Estado.Perseguindo:
                ExecutarPerseguicao();
                break;
            case Estado.Capturado:
                // N�o faz nada, fica parado e com raiva.
                break;
        }
        AtualizarAnimacao();
    }

    // --- DETEC��O E CAPTURA ---

    // Chamado quando o jogador entra na �REA DE VIS�O (Trigger)
    private void OnTriggerEnter(Collider other)
    {
        // Se j� foi ativado ou j� capturou o jogador, n�o faz nada
        if (foiAtivado || estadoAtual == Estado.Capturado) return;

        if (other.CompareTag("Player"))
        {
            Debug.Log("Gar�om viu o Gordito! Iniciando persegui��o!");
            foiAtivado = true;
            estadoAtual = Estado.Perseguindo;
        }
    }

    // Chamado quando o gar�om ENCOSTA FISICAMENTE no jogador (Collision)
    private void OnCollisionEnter(Collision collision)
    {
        // S� captura se estiver no estado de persegui��o
        if (estadoAtual != Estado.Perseguindo) return;

        if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("Gar�om capturou o Gordito! F�RIA!");
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

    // --- FUN��ES AUXILIARES ---

    private void MoverParaProximoPonto()
    {
        if (pontosPatrulha.Count == 0) return;
        agente.SetDestination(pontosPatrulha[indicePontoAtual].position);
        indicePontoAtual = (indicePontoAtual + 1) % pontosPatrulha.Count;
    }

    private void AtualizarAnimacao()
    {
        // Anima��o de andar s� se estiver patrulhando
        animator.SetBool("isWalking", estadoAtual == Estado.Patrulhando);
        // Anima��o de correr s� se estiver perseguindo
        animator.SetBool("isRunning", estadoAtual == Estado.Perseguindo);
    }
}