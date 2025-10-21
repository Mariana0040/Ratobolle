using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Animator))] // Requerido para a l�gica de anima��o
public class GarcomAI : MonoBehaviour
{
    // Chave que permite a persegui��o ap�s o jogador entrar no gatilho
    private bool podePerseguir = false;

    [Header("Patrulha")]
    public List<Transform> pontosPatrulha;
    public float velocidadePatrulha = 3.0f;

    [Header("Persegui��o")]
    public Transform alvo;
    public float distanciaDeteccao = 10.0f;
    public float velocidadePerseguicao = 6.0f;

    [Header("Captura")]
    [Tooltip("A que dist�ncia o gar�om captura o jogador, mesmo sem colis�o.")]
    public float distanciaDeCaptura = 1.5f;

    [Header("Respawn")]
    [Tooltip("Ponto para onde o jogador retorna ap�s ser capturado.")]
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

    // Ativa a permiss�o de persegui��o quando o jogador entra no trigger
    private void OnTriggerEnter(Collider other)
    {
        if (podePerseguir) return;

        if (other.CompareTag("Player"))
        {
            Debug.Log("GAR�OM ATIVADO! Agora ele pode perseguir!");
            podePerseguir = true;
        }
    }

    // Gerencia a transi��o entre os estados de patrulha e persegui��o
    private void GerenciarEstados()
    {
        if (alvo == null) return;

        float distanciaParaAlvo = Vector3.Distance(transform.position, alvo.position);

        // A condi��o para perseguir �: ter permiss�o E o jogador estar dentro do raio de detec��o
        if (podePerseguir && distanciaParaAlvo <= distanciaDeteccao)
        {
            estadoAtual = Estado.Perseguindo;
        }
        else
        {
            // Se o jogador fugir ou a permiss�o n�o for dada, ele patrulha
            estadoAtual = Estado.Patrulhando;
        }
    }

    // L�gica executada enquanto est� patrulhando
    private void ExecutarPatrulha()
    {
        agente.speed = velocidadePatrulha;
        // Se chegou ao destino, vai para o pr�ximo ponto
        if (!agente.pathPending && agente.remainingDistance < 0.5f)
        {
            MoverParaProximoPonto();
        }
    }

    // L�gica executada enquanto est� perseguindo
    private void ExecutarPerseguicao()
    {
        agente.speed = velocidadePerseguicao;
        agente.SetDestination(alvo.position);

        // Verifica��o de captura por dist�ncia (Mecanismo Robusto)
        if (Vector3.Distance(transform.position, alvo.position) <= distanciaDeCaptura)
        {
            ProcessarCapturaDoJogador(alvo.gameObject);
        }
    }

    // Envia o agente para o pr�ximo ponto de patrulha
    private void MoverParaProximoPonto()
    {
        if (pontosPatrulha.Count == 0) return;

        agente.speed = velocidadePatrulha;
        agente.SetDestination(pontosPatrulha[indicePontoAtual].position);
        indicePontoAtual = (indicePontoAtual + 1) % pontosPatrulha.Count;
    }

    // L�gica de captura por colis�o f�sica (Mecanismo Secund�rio)
    private void OnCollisionEnter(Collision collision)
    {
        if (estadoAtual == Estado.Perseguindo && collision.gameObject.CompareTag("Player"))
        {
            ProcessarCapturaDoJogador(collision.gameObject);
        }
    }

    // A��o central de capturar o jogador e voltar a patrulhar
    private void ProcessarCapturaDoJogador(GameObject jogador)
    {
        // Preven��o de chamada m�ltipla: se n�o est� perseguindo, a captura j� ocorreu.
        if (estadoAtual != Estado.Perseguindo) return;

        Debug.Log("Gar�om capturou o jogador! Enviando para o respawn.");

        if (playerRespawnPoint == null)
        {
            Debug.LogError("Ponto de Respawn do Jogador n�o foi configurado!", this);
            return;
        }

        // 1. Move o jogador para o ponto de respawn
        jogador.transform.position = playerRespawnPoint.position;

        // 2. Muda o estado do Gar�om de volta para Patrulhando
        estadoAtual = Estado.Patrulhando;

        // 3. Limpa o caminho atual (parar de seguir o jogador) e define um novo destino de patrulha
        agente.ResetPath();
        MoverParaProximoPonto();
    }

    // Atualiza as anima��es com base no estado e movimento
    private void AtualizarAnimacao()
    {
        bool estaSeMovendo = agente.velocity.magnitude > 0.1f;
        bool estaCorrendo = estadoAtual == Estado.Perseguindo;

        // O Animator deve ter os bools "isWalking" e "isRunning"
        animator.SetBool("isWalking", estaSeMovendo && !estaCorrendo);
        animator.SetBool("isRunning", estaSeMovendo && estaCorrendo);
    }

    // Controla a rota��o para ser em �ngulos de 90 graus
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

    // Desenha os raios de detec��o e captura no Editor
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, distanciaDeteccao);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, distanciaDeCaptura);
    }
}