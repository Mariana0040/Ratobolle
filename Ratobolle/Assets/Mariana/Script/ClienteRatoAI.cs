using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using Mono.Cecil.Cil;
using System.Collections.Generic;
using System.Linq;
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
    public CanvasInteracao canvasInteracao; // Arraste o canvas do balão aqui

    private List<ReceitaSO> pedidoAtual = new List<ReceitaSO>();
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
        FazerPedido(); // << NOVO
    }

    void FazerPedido()
    {
        List<ReceitaSO> receitasDisponiveis = GerenciadorRestaurante.Instance.ObterReceitasDisponiveis();
        int nivelRestaurante = GerenciadorRestaurante.Instance.nivelAtualRestaurante;

        // Nível 1 pede 1 comida, Nível 2 pede 2, etc. (limitado pelas receitas disponíveis)
        int quantidadeDePedidos = Mathf.Min(nivelRestaurante, receitasDisponiveis.Count);

        for (int i = 0; i < quantidadeDePedidos; i++)
        {
            pedidoAtual.Add(receitasDisponiveis[Random.Range(0, receitasDisponiveis.Count)]);
        }

        canvasInteracao.MostrarPedido(pedidoAtual);
    }

    // Chamado pelo PlayerCozinheiro
    public bool TentarEntregar(CollectibleItemData prato)
    {
        ReceitaSO receitaCorrespondente = pedidoAtual.FirstOrDefault(r => r.pratoFinal == prato);

        if (receitaCorrespondente != null)
        {
            Debug.Log("Pedido correto entregue!");
            pedidoAtual.Remove(receitaCorrespondente);
            canvasInteracao.MostrarPedido(pedidoAtual); // Atualiza o balão

            if (pedidoAtual.Count == 0)
            {
                StartCoroutine(ComerComida(5f)); // Cliente come e vai embora feliz
            }
            return true;
        }

        Debug.Log("Pedido errado!");
        return false;
    }

    public void IrEmbora()
    {
        canvasInteracao.EsconderBalao();
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
        animator.SetBool("Comendo",true);
        Debug.Log("Cliente Rato começou a comer.");
        yield return new WaitForSeconds(duracao);
        animator.SetBool("Comendo", false);
        Debug.Log("Cliente Rato terminou de comer e vai embora satisfeito.");
        IrEmbora();
    }

    void AtualizarAnimacao()
    {
        bool estaAndando = !agente.isStopped && agente.velocity.magnitude > 0.1f;
        animator.SetBool("andando", estaAndando);
    }
}