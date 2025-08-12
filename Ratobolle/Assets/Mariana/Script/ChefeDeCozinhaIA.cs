using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(ControleDeAtaqueChefe))] // Garante que o script de ataque também esteja presente
public class ChefeDeCozinhaAI : MonoBehaviour
{
    // --- ESTADO ATUAL ---
    private enum Estado { Patrulhando, Perseguindo, Atacando }
    private Estado estadoAtual;

    [Header("Patrulha")]
    public List<Transform> pontosPatrulha;
    public float velocidadePatrulha = 3.0f;

    [Header("Perseguição")]
    public Transform alvo;
    public float distanciaDeteccao = 10.0f;
    public float velocidadePerseguicao = 6.0f;

    // --- REFERÊNCIAS ---
    private NavMeshAgent agente;
    private Animator animator;
    private ControleDeAtaqueChefe sistemaDeAtaque; // Referência para o Sistema de Armas

    // --- CONTROLE INTERNO ---
    private int indicePontoAtual = 0;

    void Start()
    {
        // Pega todos os componentes necessários
        agente = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        sistemaDeAtaque = GetComponent<ControleDeAtaqueChefe>(); // Pega a referência do script de ataque

        // Configurações do agente
        agente.updateRotation = false;
        agente.angularSpeed = 0;

        // Inicia na patrulha
        estadoAtual = Estado.Patrulhando;
        if (pontosPatrulha.Count > 0)
        {
            MoverParaProximoPonto();
        }
    }

    void Update()
    {
        GerenciarEstados(); // Decide o que fazer

        // Executa a ação do estado atual
        switch (estadoAtual)
        {
            case Estado.Patrulhando:
                ExecutarPatrulha();
                break;
            case Estado.Perseguindo:
                ExecutarPerseguicao();
                break;
            case Estado.Atacando:
                ExecutarAtaque();
                break;
        }

        AtualizarAnimacaoERotacao();
    }

    private void GerenciarEstados()
    {
        if (alvo == null) return;

        float distanciaParaAlvo = Vector3.Distance(transform.position, alvo.position);

        // A distância de ataque tem a maior prioridade
        if (distanciaParaAlvo <= sistemaDeAtaque.distanciaDeAtaque)
        {
            estadoAtual = Estado.Atacando;
        }
        else if (distanciaParaAlvo <= distanciaDeteccao)
        {
            estadoAtual = Estado.Perseguindo;
        }
        else
        {
            estadoAtual = Estado.Patrulhando;
        }
    }

    private void ExecutarPatrulha()
    {
        agente.isStopped = false; // Garante que ele possa se mover
        agente.speed = velocidadePatrulha;

        if (!agente.pathPending && agente.remainingDistance < 0.5f)
        {
            MoverParaProximoPonto();
        }
    }

    private void ExecutarPerseguicao()
    {
        agente.isStopped = false; // Garante que ele possa se mover
        agente.speed = velocidadePerseguicao;
        MoverPara(alvo.position);
    }

    private void ExecutarAtaque()
    {
        // --- ORDENA QUE O CHEFE PARE IMEDIATAMENTE ---
        agente.isStopped = true;

        // Encarar o jogador para o ataque
        Vector3 direcao = (alvo.position - transform.position).normalized;
        transform.rotation = Quaternion.LookRotation(new Vector3(direcao.x, 0, direcao.z));

        // --- MANDA O "SISTEMA DE ARMAS" TENTAR ATACAR ---
        sistemaDeAtaque.TentarAtaque();
    }

    // --- Funções Auxiliares ---

    private void MoverPara(Vector3 destino)
    {
        agente.SetDestination(destino);
    }

    private void MoverParaProximoPonto()
    {
        if (pontosPatrulha.Count == 0) return;
        Vector3 destino = pontosPatrulha[indicePontoAtual].position;
        MoverPara(destino);
        indicePontoAtual = (indicePontoAtual + 1) % pontosPatrulha.Count;
    }

    private void AtualizarAnimacaoERotacao()
    {
        // Só ativa a animação de andar se o agente não estiver parado
        bool estaAndando = !agente.isStopped && agente.velocity.magnitude > 0.1f;
        animator.SetBool("isWalking", estaAndando);

        // Só faz a rotação em blocos quando não está atacando
        if (estadoAtual != Estado.Atacando && estaAndando)
        {
            RotacaoEmBlocos();
        }
    }

    private void RotacaoEmBlocos()
    {
        Vector3 direcao = agente.velocity.normalized;
        if (direcao != Vector3.zero)
        {
            Quaternion lookRotation = Quaternion.LookRotation(direcao);
            transform.rotation = lookRotation;
        }
    }
}