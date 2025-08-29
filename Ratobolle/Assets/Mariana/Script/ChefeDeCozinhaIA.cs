using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(ControleDeAtaqueChefe))]
public class ChefeDeCozinhaAI : MonoBehaviour
{
    private enum Estado { Patrulhando, Perseguindo, Atacando, IndoFecharGeladeira }
    private Estado estadoAtual;

    [Header("Patrulha")]
    public List<Transform> pontosPatrulha;
    public float velocidadePatrulha = 3.0f;

    [Header("Perseguição")]
    public Transform alvo;
    public float distanciaDeteccao = 10.0f;
    public float velocidadePerseguicao = 6.0f;

    [Header("Geladeira")]
    [Tooltip("A que distância o chefe para e fecha a geladeira.")]
    public float distanciaParaFecharGeladeira = 2.0f;
    private InteractableObject geladeiraInterativa;

    private NavMeshAgent agente;
    private Animator animator;
    private ControleDeAtaqueChefe sistemaDeAtaque;

    private int indicePontoAtual = 0;
    private bool estaOcupado = false;

    void Start()
    {
        agente = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        sistemaDeAtaque = GetComponent<ControleDeAtaqueChefe>();
        agente.updateRotation = false;
        agente.angularSpeed = 0;
        MudarEstado(Estado.Patrulhando);
    }

    void Update()
    {
        if (estaOcupado) return;
        GerenciarEstados();
        ExecutarEstadoAtual();
        AtualizarAnimacaoERotacao();
    }

    // Método centralizado para mudar de estado e registrar a mudança
    private void MudarEstado(Estado novoEstado)
    {
        if (estadoAtual == novoEstado) return;

        estadoAtual = novoEstado;
        Debug.Log($"Chefe mudou para o estado: <color=yellow>{novoEstado}</color>");

        // Ação imediata ao entrar no estado
        switch (estadoAtual)
        {
            case Estado.Patrulhando:
                if (pontosPatrulha.Count > 0) MoverParaProximoPonto();
                break;
            case Estado.IndoFecharGeladeira:
                agente.isStopped = false; // Garante que ele se mova
                break;
        }
    }

    private void GerenciarEstados()
    {
        // PRIORIDADE MÁXIMA
        if (geladeiraInterativa != null && geladeiraInterativa.isOpen)
        {
            MudarEstado(Estado.IndoFecharGeladeira);
            return;
        }

        if (alvo == null)
        {
            MudarEstado(Estado.Patrulhando);
            return;
        }

        float distanciaParaAlvo = Vector3.Distance(transform.position, alvo.position);
        if (distanciaParaAlvo <= sistemaDeAtaque.distanciaDeAtaque)
        {
            MudarEstado(Estado.Atacando);
        }
        else if (distanciaParaAlvo <= distanciaDeteccao)
        {
            MudarEstado(Estado.Perseguindo);
        }
        else
        {
            MudarEstado(Estado.Patrulhando);
        }
    }

    private void ExecutarEstadoAtual()
    {
        switch (estadoAtual)
        {
            case Estado.Patrulhando: ExecutarPatrulha(); break;
            case Estado.Perseguindo: ExecutarPerseguicao(); break;
            case Estado.Atacando: ExecutarAtaque(); break;
            case Estado.IndoFecharGeladeira: ExecutarIrFecharGeladeira(); break;
        }
    }

    public void NotificarGeladeiraAberta(InteractableObject geladeira)
    {
        if (estaOcupado) return;
        Debug.Log("<color=green>CHEFE RECEBEU A NOTIFICAÇÃO!</color>");
        geladeiraInterativa = geladeira;
    }

    private void ExecutarIrFecharGeladeira()
    {
        agente.speed = velocidadePerseguicao;
        MoverPara(geladeiraInterativa.transform.position);

        if (!agente.pathPending && agente.remainingDistance <= distanciaParaFecharGeladeira)
        {
            StartCoroutine(FecharGeladeira());
        }
    }

    private IEnumerator FecharGeladeira()
    {
        estaOcupado = true;
        agente.isStopped = true;

        Vector3 direcao = (geladeiraInterativa.transform.position - transform.position).normalized;
        if (direcao != Vector3.zero)
            transform.rotation = Quaternion.LookRotation(new Vector3(direcao.x, 0, direcao.z));

        geladeiraInterativa.Interact();
        yield return new WaitForSeconds(geladeiraInterativa.tweenDuration + 0.2f);

        geladeiraInterativa = null;
        estaOcupado = false;
    }

    // --- Funções de Estado ---
    private void ExecutarPatrulha()
    {
        agente.isStopped = false;
        agente.speed = velocidadePatrulha;
        if (!agente.pathPending && agente.remainingDistance < 0.5f)
        {
            MoverParaProximoPonto();
        }
    }

    private void ExecutarPerseguicao()
    {
        agente.isStopped = false;
        agente.speed = velocidadePerseguicao;
        MoverPara(alvo.position);
    }

    private void ExecutarAtaque()
    {
        agente.isStopped = true;
        Vector3 direcao = (alvo.position - transform.position).normalized;
        if (direcao != Vector3.zero)
            transform.rotation = Quaternion.LookRotation(new Vector3(direcao.x, 0, direcao.z));
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
        bool estaAndando = !agente.isStopped && agente.velocity.magnitude > 0.1f;
        animator.SetBool("isWalking", estaAndando);
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
            transform.rotation = Quaternion.LookRotation(direcao);
        }
    }
}