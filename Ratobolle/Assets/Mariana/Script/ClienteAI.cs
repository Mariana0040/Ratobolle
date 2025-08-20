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

        // Verifica se existe alguma cadeira na lista
        if (cadeirasDisponiveis.Count > 0)
        {
            // Escolhe um n�mero aleat�rio entre 0 e o total de cadeiras
            int indiceAleatorio = Random.Range(0, cadeirasDisponiveis.Count);

            // Pega a cadeira correspondente a esse n�mero
            Transform cadeiraAlvo = cadeirasDisponiveis[indiceAleatorio];

            // Manda o cliente ir at� a posi��o da cadeira
            agente.SetDestination(cadeiraAlvo.position);
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

        // Dispara a anima��o de sentar
        animator.SetTrigger("sentar");
    }

    void AtualizarAnimacao()
    {
        // A anima��o de andar s� toca se o agente estiver se movendo
        bool estaAndando = agente.velocity.magnitude > 0.1f;
        animator.SetBool("andando", estaAndando);
    }
}