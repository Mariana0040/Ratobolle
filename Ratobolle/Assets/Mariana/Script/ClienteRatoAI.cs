using UnityEngine;
using UnityEngine.AI;
using System.Collections;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Animator))]
public class ClienteRatoAI : MonoBehaviour
{
    private enum EstadoCliente { IndoParaCadeira, EsperandoPedido, Comendo, IndoEmbora }
    private EstadoCliente estadoAtual;

    private NavMeshAgent agente;
    private Animator animator;

    // MODIFICADO: A referência agora é para o componente 'Chair'
    public Chair cadeiraAlvo;

    void Start()
    {
        agente = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();

        GerenciadorRestaurante.Instance.RegistrarNovoCliente(this);
        EncontrarCadeiraLivre();
    }

    void Update()
    {
        switch (estadoAtual)
        {
            case EstadoCliente.IndoParaCadeira:
                if (!agente.pathPending && agente.remainingDistance < 0.2f)
                {
                    Sentar();
                }
                break;
            case EstadoCliente.IndoEmbora:
                if (!agente.pathPending && agente.remainingDistance < 0.5f)
                {
                    Destroy(gameObject);
                }
                break;
        }
        AtualizarAnimacao();
    }

    void EncontrarCadeiraLivre()
    {
        estadoAtual = EstadoCliente.IndoParaCadeira;
        cadeiraAlvo = GerenciadorRestaurante.Instance.SolicitarCadeiraLivre();

        if (cadeiraAlvo != null)
        {
            // A linha "cadeiraAlvo.Ocupar()" foi REMOVIDA daqui, pois o Gerente já faz isso.
            agente.SetDestination(cadeiraAlvo.transform.position);
        }
        else
        {
            // Se não há cadeiras, vai embora
            IrEmbora();
        }
    }

    void Sentar()
    {
        estadoAtual = EstadoCliente.EsperandoPedido;
        agente.isStopped = true;
        // MODIFICADO: Usa a transform do componente 'Chair'
        transform.position = cadeiraAlvo.transform.position;
        transform.rotation = cadeiraAlvo.transform.rotation;
        Debug.Log("Cliente Rato sentado e esperando o pedido.");
    }

    public void IrEmbora()
    {
        estadoAtual = EstadoCliente.IndoEmbora;
        agente.isStopped = false;

        // MODIFICADO: Libera o objeto 'Chair' que estava usando
        if (cadeiraAlvo != null)
        {
            cadeiraAlvo.Liberar();
            cadeiraAlvo = null;
        }

        agente.SetDestination(GerenciadorRestaurante.Instance.pontoDeSaida.position);
    }

    // O resto do script não precisou de mudanças
    public void ReceberComida(float tempoDeComer)
    {
        if (estadoAtual == EstadoCliente.EsperandoPedido)
        {
            StartCoroutine(ComerComida(tempoDeComer));
        }
    }

    private IEnumerator ComerComida(float duracao)
    {
        estadoAtual = EstadoCliente.Comendo;
        Debug.Log("Cliente Rato começou a comer.");
        yield return new WaitForSeconds(duracao);
        Debug.Log("Cliente Rato terminou de comer e vai embora satisfeito.");
        IrEmbora();
    }

    void AtualizarAnimacao()
    {
        bool estaAndando = !agente.isStopped && agente.velocity.magnitude > 0.1f;
        animator.SetBool("andando", estaAndando);
    }
}