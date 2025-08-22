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

    [Header("Configura��o do Cliente")]
    [Tooltip("Arraste para c� TODAS as cadeiras onde o cliente pode sentar.")]
    public List<Transform> cadeirasDisponiveis;

    // Precisamos guardar a refer�ncia da cadeira que ele escolheu
    private Transform cadeiraAlvoTransform;

    void Start()
    {
        agente = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();

        // --- L�GICA PRINCIPAL ---
        EncontrarCadeiraAleatoria();
    }

    void Update()
    {
        // Se o cliente ainda est� indo para a cadeira...
        if (estadoAtual == EstadoCliente.IndoParaCadeira)
        {
            // ... e se ele j� chegou perto do destino...
            if (!agente.pathPending && agente.remainingDistance < 0.2f)
            {
                // ... ent�o ele senta.
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

            // --- MUDAN�A: Guardamos a cadeira escolhida ---
            cadeiraAlvoTransform = cadeirasDisponiveis[indiceAleatorio];

            // Manda o cliente ir at� a posi��o da cadeira
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

        // --- C�DIGO NOVO: AJUSTE DE POSI��O E ROTA��O ---

        // 1. Teletransporta o cliente para a posi��o exata da cadeira.
        // Isso evita que ele pare um pouco antes ou um pouco depois.
        transform.position = cadeiraAlvoTransform.position;

        // 2. Iguala a rota��o do cliente � rota��o da cadeira.
        // O cliente vai instantaneamente virar para a mesma dire��o que a cadeira.
        transform.rotation = cadeiraAlvoTransform.rotation;

        // O resto da l�gica de anima��o continua a mesma
        // A fun��o AtualizarAnimacao() vai detectar que ele parou e vai setar "andando" para false.
    }

    void AtualizarAnimacao()
    {
        // A anima��o de andar s� toca se o agente estiver se movendo
        bool estaAndando = agente.velocity.magnitude > 0.1f;
        animator.SetBool("andando", estaAndando);
    }
}