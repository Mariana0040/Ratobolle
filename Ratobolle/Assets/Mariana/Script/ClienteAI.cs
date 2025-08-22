using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic; // Usaremos uma Lista (Array) de cadeiras

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Animator))]
public class ClienteAI : MonoBehaviour
{
    private enum EstadoCliente { IndoParaCadeira, Sentado }
    private EstadoCliente estadoAtual;

    private NavMeshAgent agente;
    private Animator animator;

    [Header("Configuração do Cliente")]
    [Tooltip("Arraste para cá TODAS as cadeiras onde o cliente pode sentar.")]
    public List<Transform> cadeirasDisponiveis;

    // Precisamos guardar a referência da cadeira que ele escolheu
    private Transform cadeiraAlvoTransform;

    void Start()
    {
        agente = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();

        // --- LÓGICA PRINCIPAL ---
        EncontrarCadeiraAleatoria();
    }

    void Update()
    {
        // Se o cliente ainda está indo para a cadeira...
        if (estadoAtual == EstadoCliente.IndoParaCadeira)
        {
            // ... e se ele já chegou perto do destino...
            if (!agente.pathPending && agente.remainingDistance < 0.2f)
            {
                // ... então ele senta.
                Sentar();
            }
        }

        AtualizarAnimacao();
    }

    void EncontrarCadeiraAleatoria()
    {
        estadoAtual = EstadoCliente.IndoParaCadeira;

        if (cadeirasDisponiveis.Count > 0)
        {
            int indiceAleatorio = Random.Range(0, cadeirasDisponiveis.Count);

            // --- MUDANÇA: Guardamos a cadeira escolhida ---
            cadeiraAlvoTransform = cadeirasDisponiveis[indiceAleatorio];

            // Manda o cliente ir até a posição da cadeira
            agente.SetDestination(cadeiraAlvoTransform.position);
        }
        else
        {
            Debug.LogError("Nenhuma cadeira foi definida na lista 'cadeirasDisponiveis'!", this);
        }
    }

    void Sentar()
    {
        estadoAtual = EstadoCliente.Sentado;
        agente.isStopped = true; // Para de se mover

        // --- CÓDIGO NOVO: AJUSTE DE POSIÇÃO E ROTAÇÃO ---

        // 1. Teletransporta o cliente para a posição exata da cadeira.
        // Isso evita que ele pare um pouco antes ou um pouco depois.
        transform.position = cadeiraAlvoTransform.position;

        // 2. Iguala a rotação do cliente à rotação da cadeira.
        // O cliente vai instantaneamente virar para a mesma direção que a cadeira.
        transform.rotation = cadeiraAlvoTransform.rotation;

        // O resto da lógica de animação continua a mesma
        // A função AtualizarAnimacao() vai detectar que ele parou e vai setar "andando" para false.
    }

    void AtualizarAnimacao()
    {
        // A animação de andar só toca se o agente estiver se movendo
        bool estaAndando = agente.velocity.magnitude > 0.1f;
        animator.SetBool("andando", estaAndando);
    }
}