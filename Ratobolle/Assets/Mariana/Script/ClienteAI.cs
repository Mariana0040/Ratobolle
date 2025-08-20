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

        // Verifica se existe alguma cadeira na lista
        if (cadeirasDisponiveis.Count > 0)
        {
            // Escolhe um número aleatório entre 0 e o total de cadeiras
            int indiceAleatorio = Random.Range(0, cadeirasDisponiveis.Count);

            // Pega a cadeira correspondente a esse número
            Transform cadeiraAlvo = cadeirasDisponiveis[indiceAleatorio];

            // Manda o cliente ir até a posição da cadeira
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

        // Dispara a animação de sentar
        animator.SetTrigger("sentar");
    }

    void AtualizarAnimacao()
    {
        // A animação de andar só toca se o agente estiver se movendo
        bool estaAndando = agente.velocity.magnitude > 0.1f;
        animator.SetBool("andando", estaAndando);
    }
}