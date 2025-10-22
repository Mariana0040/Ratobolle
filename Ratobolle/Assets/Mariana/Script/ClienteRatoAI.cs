using UnityEngine;
using UnityEngine.AI;
using System.Collections;
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

    public Chair cadeiraAlvo;
    public CanvasInteracao canvasInteracao;

    [Header("Configuração da Entrega")]
    public Transform pontoPratoNaMesa; // NOVO: Arraste o objeto que marca onde o prato ficará na mesa

    private List<ReceitaSO> pedidoAtual = new List<ReceitaSO>();
    private List<GameObject> pratosInstanciados = new List<GameObject>(); // NOVO: Guarda os pratos na mesa para limpá-los depois

    void Start()
    {
        agente = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();

        GerenciadorRestaurante.Instance.RegistrarNovoCliente(this);
        EncontrarCadeiraLivre();
    }

    // ALTERADO: Adicionamos a lógica para instanciar o prato na mesa.
    public bool TentarEntregar(CollectibleItemData prato)
    {
        ReceitaSO receitaCorrespondente = pedidoAtual.FirstOrDefault(r => r.pratoFinal == prato);

        if (receitaCorrespondente != null)
        {
            Debug.Log("Pedido correto entregue: " + prato.itemName);

            // NOVO: Faz o prato aparecer na mesa.
            if (pontoPratoNaMesa != null && prato.modelPrefab != null)
            {
                // Instancia o prato e o adiciona à lista para ser destruído depois.
                GameObject pratoNaMesa = Instantiate(prato.modelPrefab, pontoPratoNaMesa.position, pontoPratoNaMesa.rotation);
                pratosInstanciados.Add(pratoNaMesa);
                pratoNaMesa.transform.parent = pontoPratoNaMesa.transform;
            }

            pedidoAtual.Remove(receitaCorrespondente);
            canvasInteracao.MostrarPedido(pedidoAtual);

            // Se este foi o último item do pedido, começa a comer.
            if (pedidoAtual.Count == 0)
            {
                estadoAtual = EstadoCliente.Comendo;
                StartCoroutine(ComerComida(5f)); // Cliente come e vai embora feliz
            }
            return true; // Entrega bem-sucedida
        }

        Debug.Log("Pedido errado!");
        return false; // Entrega falhou
    }

    // ALTERADO: Adicionamos a limpeza dos pratos que estão na mesa.
    public void IrEmbora()
    {
        // NOVO: Destrói os pratos que foram instanciados na mesa.
        foreach (var prato in pratosInstanciados)
        {
            Destroy(prato);
        }
        pratosInstanciados.Clear();

        canvasInteracao.EsconderBalao();
        estadoAtual = EstadoCliente.IndoEmbora;
        agente.isStopped = false;

        if (cadeiraAlvo != null)
        {
            cadeiraAlvo.Liberar();
            cadeiraAlvo = null;
        }

        agente.SetDestination(GerenciadorRestaurante.Instance.pontoDeSaida.position);
    }

    private IEnumerator ComerComida(float duracao)
    {
        // A animação de comer agora é controlada pelo estado.
        // O estado já foi mudado para Comendo em TentarEntregar
        Debug.Log("Cliente Rato começou a comer.");
        yield return new WaitForSeconds(duracao);
        Debug.Log("Cliente Rato terminou de comer e vai embora satisfeito.");
        IrEmbora();
    }

    // ALTERADO: A animação de comer agora é baseada no estado.
    void AtualizarAnimacao()
    {
        if (agente == null || !agente.isOnNavMesh) return;

        bool estaAndando = !agente.isStopped && agente.velocity.magnitude > 0.1f;
        animator.SetBool("andando", estaAndando);

        // Define a animação de comer baseada no estado atual
        animator.SetBool("Comendo", estadoAtual == EstadoCliente.Comendo);
    }

    // --- O RESTO DO SEU CÓDIGO PERMANECE IGUAL ---
    #region Funções Originais
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
            agente.SetDestination(cadeiraAlvo.transform.position);
        }
        else
        {
            IrEmbora();
        }
    }

    void Sentar()
    {
        estadoAtual = EstadoCliente.EsperandoPedido;
        agente.isStopped = true;
        transform.position = cadeiraAlvo.transform.position;
        transform.rotation = cadeiraAlvo.transform.rotation;
        Debug.Log("Cliente Rato sentado e esperando o pedido.");
        FazerPedido();
    }

    void FazerPedido()
    {
        List<ReceitaSO> receitasDisponiveis = GerenciadorRestaurante.Instance.ObterReceitasDisponiveis();
        int nivelRestaurante = GerenciadorRestaurante.Instance.nivelAtualRestaurante;
        int quantidadeDePedidos = Mathf.Min(nivelRestaurante, receitasDisponiveis.Count);

        for (int i = 0; i < quantidadeDePedidos; i++)
        {
            pedidoAtual.Add(receitasDisponiveis[Random.Range(0, receitasDisponiveis.Count)]);
        }

        canvasInteracao.MostrarPedido(pedidoAtual);
    }
    #endregion
}