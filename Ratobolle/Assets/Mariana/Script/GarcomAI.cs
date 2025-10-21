using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Animator))] // Requerido para a lógica de animação
public class GarcomAI : MonoBehaviour
{
    // Chave que permite a perseguição após o jogador entrar no gatilho
    private bool podePerseguir = false;

    [Header("Patrulha")]
    public List<Transform> pontosPatrulha;
    public float velocidadePatrulha = 3.0f;

    [Header("Perseguição")]
    public Transform alvo;
    public float distanciaDeteccao = 10.0f;
    public float velocidadePerseguicao = 6.0f;

    [Header("Captura")]
    [Tooltip("A que distância o garçom captura o jogador, mesmo sem colisão.")]
    public float distanciaDeCaptura = 1.5f;

    [Header("Respawn")]
    [Tooltip("Ponto para onde o jogador retorna após ser capturado.")]
    public Transform playerRespawnPoint;

    // Componentes
    private NavMeshAgent agente;
    private Animator animator;

    // Controle de estado
    private enum Estado { Patrulhando, Perseguindo }
    private Estado estadoAtual;
    private int indicePontoAtual = 0;

    void Start()
    {
        agente = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();

        agente.updateRotation = false;
        agente.angularSpeed = 0;

        // Tenta encontrar o jogador automaticamente
        if (alvo == null && GameObject.FindGameObjectWithTag("Player") != null)
        {
            alvo = GameObject.FindGameObjectWithTag("Player").transform;
        }

        // Inicia o comportamento de patrulha
        estadoAtual = Estado.Patrulhando;
        if (pontosPatrulha.Count > 0)
        {
            MoverParaProximoPonto();
        }
    }

    void Update()
    {
        GerenciarEstados();

        switch (estadoAtual)
        {
            case Estado.Patrulhando:
                ExecutarPatrulha();
                break;
            case Estado.Perseguindo:
                ExecutarPerseguicao();
                break;
        }

        AtualizarAnimacao();
        RotacaoEmBlocos();
    }

    // Ativa a permissão de perseguição quando o jogador entra no trigger
    private void OnTriggerEnter(Collider other)
    {
        if (podePerseguir) return;

        if (other.CompareTag("Player"))
        {
            Debug.Log("GARÇOM ATIVADO! Agora ele pode perseguir!");
            podePerseguir = true;
        }
    }

    // Gerencia a transição entre os estados de patrulha e perseguição
    private void GerenciarEstados()
    {
        if (alvo == null) return;

        float distanciaParaAlvo = Vector3.Distance(transform.position, alvo.position);

        // A condição para perseguir é: ter permissão E o jogador estar dentro do raio de detecção
        if (podePerseguir && distanciaParaAlvo <= distanciaDeteccao)
        {
            estadoAtual = Estado.Perseguindo;
        }
        else
        {
            // Se o jogador fugir ou a permissão não for dada, ele patrulha
            estadoAtual = Estado.Patrulhando;
        }
    }

    // Lógica executada enquanto está patrulhando
    private void ExecutarPatrulha()
    {
        agente.speed = velocidadePatrulha;
        // Se chegou ao destino, vai para o próximo ponto
        if (!agente.pathPending && agente.remainingDistance < 0.5f)
        {
            MoverParaProximoPonto();
        }
    }

    // Lógica executada enquanto está perseguindo
    private void ExecutarPerseguicao()
    {
        agente.speed = velocidadePerseguicao;
        agente.SetDestination(alvo.position);

        // Verificação de captura por distância (Mecanismo Robusto)
        if (Vector3.Distance(transform.position, alvo.position) <= distanciaDeCaptura)
        {
            ProcessarCapturaDoJogador(alvo.gameObject);
        }
    }

    // Envia o agente para o próximo ponto de patrulha
    private void MoverParaProximoPonto()
    {
        if (pontosPatrulha.Count == 0) return;

        agente.speed = velocidadePatrulha;
        agente.SetDestination(pontosPatrulha[indicePontoAtual].position);
        indicePontoAtual = (indicePontoAtual + 1) % pontosPatrulha.Count;
    }

    // Lógica de captura por colisão física (Mecanismo Secundário)
    private void OnCollisionEnter(Collision collision)
    {
        if (estadoAtual == Estado.Perseguindo && collision.gameObject.CompareTag("Player"))
        {
            ProcessarCapturaDoJogador(collision.gameObject);
        }
    }

    // Ação central de capturar o jogador e voltar a patrulhar
    private void ProcessarCapturaDoJogador(GameObject jogador)
    {
        // Prevenção de chamada múltipla: se não está perseguindo, a captura já ocorreu.
        if (estadoAtual != Estado.Perseguindo) return;

        Debug.Log("Garçom capturou o jogador! Enviando para o respawn.");

        if (playerRespawnPoint == null)
        {
            Debug.LogError("Ponto de Respawn do Jogador não foi configurado!", this);
            return;
        }

        // 1. Move o jogador para o ponto de respawn
        jogador.transform.position = playerRespawnPoint.position;

        // 2. Muda o estado do Garçom de volta para Patrulhando
        estadoAtual = Estado.Patrulhando;

        // 3. Limpa o caminho atual (parar de seguir o jogador) e define um novo destino de patrulha
        agente.ResetPath();
        MoverParaProximoPonto();
    }

    // Atualiza as animações com base no estado e movimento
    private void AtualizarAnimacao()
    {
        bool estaSeMovendo = agente.velocity.magnitude > 0.1f;
        bool estaCorrendo = estadoAtual == Estado.Perseguindo;

        // O Animator deve ter os bools "isWalking" e "isRunning"
        animator.SetBool("isWalking", estaSeMovendo && !estaCorrendo);
        animator.SetBool("isRunning", estaSeMovendo && estaCorrendo);
    }

    // Controla a rotação para ser em ângulos de 90 graus
    private void RotacaoEmBlocos()
    {
        if (agente.velocity.sqrMagnitude > 0.1f)
        {
            Vector3 direcao = agente.velocity.normalized;

            if (Mathf.Abs(direcao.x) > Mathf.Abs(direcao.z)) { direcao.z = 0; }
            else { direcao.x = 0; }

            if (direcao != Vector3.zero)
            {
                transform.rotation = Quaternion.LookRotation(direcao);
            }
        }
    }

    // Desenha os raios de detecção e captura no Editor
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, distanciaDeteccao);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, distanciaDeCaptura);
    }
}