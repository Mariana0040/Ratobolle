using UnityEngine;
using UnityEngine.AI;

public class ControleDeAtaqueChefe : MonoBehaviour
{
    [Header("Referências e Alvo")]
    [Tooltip("O alvo do ataque (o jogador).")]
    public Transform alvo;
    [Tooltip("O Prefab do tomate que será arremessado.")]
    public GameObject tomatePrefab;
    [Tooltip("Um objeto filho do chefe (ex: na mão) de onde o tomate será lançado.")]
    public Transform pontoDeLancamento;
    [Tooltip("O jogador será teletransportado para este ponto se for atingido.")]
    public Transform pontoDeRespawnJogador;

    [Header("Parâmetros do Ataque")]
    [Tooltip("A que distância o chefe para de perseguir e começa a atacar.")]
    public float distanciaDeAtaque = 8.0f;
    [Tooltip("Tempo (em segundos) entre cada arremesso de tomate.")]
    public float intervaloAtaque = 2.5f;
    [Tooltip("A força com que o tomate é arremessado.")]
    public float forcaLancamento = 20f;

    // Componentes internos para controle
    private NavMeshAgent agente;
    private Animator animator;
    private ChefeDeCozinhaAI scriptDeMovimento; // Referência ao seu script original

    private bool estaAtacando = false;
    private float proximoAtaquePermitido = 0f;

    void Start()
    {
        // Pega os componentes necessários no próprio GameObject
        agente = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        scriptDeMovimento = GetComponent<ChefeDeCozinhaAI>();

        // Tenta encontrar o jogador pela tag se não for definido no Inspector
        if (alvo == null)
        {
            alvo = GameObject.FindGameObjectWithTag("Player").transform;
        }
    }

    void Update()
    {
        if (alvo == null || estaAtacando)
        {
            return; // Não faz nada se não tiver alvo ou se já estiver no meio de um ataque
        }

        float distanciaParaAlvo = Vector3.Distance(transform.position, alvo.position);

        // Verifica se o jogador está na distância de ataque e se o cooldown já passou
        if (distanciaParaAlvo <= distanciaDeAtaque && Time.time >= proximoAtaquePermitido)
        {
            IniciarAtaque();
        }
    }

    private void IniciarAtaque()
    {
        estaAtacando = true;
        proximoAtaquePermitido = Time.time + intervaloAtaque;

        // --- PAUSA O SCRIPT DE MOVIMENTO PARA EVITAR CONFLITOS ---
        scriptDeMovimento.enabled = false;
        agente.isStopped = true;
        animator.SetBool("isWalking", false); // Garante que a animação de andar pare

        // Encarar o jogador para o ataque
        Vector3 direcao = (alvo.position - transform.position).normalized;
        transform.rotation = Quaternion.LookRotation(new Vector3(direcao.x, 0, direcao.z));

        // Dispara o gatilho da animação de ataque
        animator.SetTrigger("attackTrigger");
    }

    // !! IMPORTANTE !!
    // Este método deve ser chamado por um EVENTO na sua animação de ataque
    public void EventoDeArremesso()
    {
        if (tomatePrefab == null || pontoDeLancamento == null) return;

        // Cria o tomate
        GameObject tomate = Instantiate(tomatePrefab, pontoDeLancamento.position, pontoDeLancamento.rotation);

        // Configura o script do tomate
        ProjetilTomate projetilScript = tomate.GetComponent<ProjetilTomate>();
        if (projetilScript != null)
        {
            projetilScript.pontoDeRespawn = this.pontoDeRespawnJogador;
        }

        // Lança o tomate
        Rigidbody rb = tomate.GetComponent<Rigidbody>();
        rb.AddForce(pontoDeLancamento.forward * forcaLancamento, ForceMode.VelocityChange);
    }

    // !! IMPORTANTE !!
    // Este método deve ser chamado por um EVENTO no FINAL da sua animação de ataque
    public void EventoTerminoAtaque()
    {
        estaAtacando = false;

        // --- REATIVA O SCRIPT DE MOVIMENTO ---
        agente.isStopped = false;
        scriptDeMovimento.enabled = true;
    }
}