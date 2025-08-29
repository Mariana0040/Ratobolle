using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;

public class ComportamentoChefe : MonoBehaviour
{
    public enum Estado
    {
        Patrulhando,
        Perseguindo,
        Atacando,
        IndoFecharGeladeira // NOVO ESTADO!
    }

    public Estado estadoAtual = Estado.Patrulhando;

    [Header("Patrulha")]
    public List<Transform> pontosPatrulha;
    public float velocidadePatrulha = 3.0f;

    [Header("Perseguição")]
    public Transform alvo;
    public float distanciaDeteccao = 10.0f;
    public float velocidadePerseguicao = 6.0f;

    [Header("Geladeira")] // NOVO HEADER!
    public Transform geladeiraAlvo; // Referência para a geladeira que ele vai fechar
    public float distanciaParaFecharGeladeira = 1.0f; // Distância para considerar que chegou na geladeira

    // --- REFERÊNCIAS ---
    private NavMeshAgent agente;
    private Animator animator;
    private ControleDeAtaqueChefe sistemaDeAtaque; // Referência para o Sistema de Armas

    // --- CONTROLE INTERNO ---
    private int indicePontoAtual = 0;
    private bool geladeiraAberta = false; // Flag para controlar se a geladeira foi aberta

    void Start()
    {
        // Pega todos os componentes necessários
        agente = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        sistemaDeAtaque = GetComponent<ControleDeAtaqueChefe>(); // Pega a referência do script de ataque

        // Configurações do agente
        agente.updateRotation = false;
        agente.angularSpeed = 0;

        // Inicia na patrulha
        estadoAtual = Estado.Patrulhando;
        if (pontosPatrulha.Count > 0)
        {
            MoverParaProximoPonto();
        }
    }

    void Update()
    {
        GerenciarEstados(); // Decide o que fazer

        // Executa a ação do estado atual
        switch (estadoAtual)
        {
            case Estado.Patrulhando:
                ExecutarPatrulha();
                break;
            case Estado.Perseguindo:
                ExecutarPerseguicao();
                break;
            case Estado.Atacando:
                ExecutarAtaque();
                break;
            case Estado.IndoFecharGeladeira: // NOVO CASE!
                ExecutarIrFecharGeladeira();
                break;
        }

        AtualizarAnimacaoERotacao();
    }

    private void GerenciarEstados()
    {
        // Prioridade máxima: Geladeira Aberta!
        if (geladeiraAberta && geladeiraAlvo != null)
        {
            estadoAtual = Estado.IndoFecharGeladeira;
            return; // Sai daqui para não verificar outras coisas
        }

        if (alvo == null) return;

        float distanciaParaAlvo = Vector3.Distance(transform.position, alvo.position);

        // A distância de ataque tem a maior prioridade (se não estiver indo para geladeira)
        if (distanciaParaAlvo <= sistemaDeAtaque.distanciaDeAtaque)
        {
            estadoAtual = Estado.Atacando;
        }
        else if (distanciaParaAlvo <= distanciaDeteccao)
        {
            estadoAtual = Estado.Perseguindo;
        }
        else
        {
            estadoAtual = Estado.Patrulhando;
        }
    }

    private void ExecutarPatrulha()
    {
        agente.isStopped = false; // Garante que ele possa se mover
        agente.speed = velocidadePatrulha;

        if (!agente.pathPending && agente.remainingDistance < 0.5f)
        {
            MoverParaProximoPonto();
        }
    }

    private void ExecutarPerseguicao()
    {
        agente.isStopped = false; // Garante que ele possa se mover
        agente.speed = velocidadePerseguicao;
        MoverPara(alvo.position);
    }

    private void ExecutarAtaque()
    {
        // --- ORDENA QUE O CHEFE PARE IMEDIATAMENTE ---
        agente.isStopped = true;

        // Encarar o jogador para o ataque
        Vector3 direcao = (alvo.position - transform.position).normalized;
        transform.rotation = Quaternion.LookRotation(new Vector3(direcao.x, 0, direcao.z));

        // --- MANDA O "SISTEMA DE ARMAS" TENTAR ATACAR ---
        sistemaDeAtaque.TentarAtaque();
    }

    private void ExecutarIrFecharGeladeira() // NOVO MÉTODO!
    {
        agente.isStopped = false;
        agente.speed = velocidadePerseguicao; // Pode usar a velocidade de perseguição ou uma nova específica

        MoverPara(geladeiraAlvo.position);

        float distanciaParaGeladeira = Vector3.Distance(transform.position, geladeiraAlvo.position);

        if (distanciaParaGeladeira <= distanciaParaFecharGeladeira)
        {
            // Chegou perto da geladeira
            agente.isStopped = true;
            // AQUI VOCÊ PODE ADICIONAR UMA ANIMAÇÃO OU SOM DE FECHAR A GELADEIRA
            Debug.Log("Chefe fechou a geladeira!");
            // Voltar ao estado de patrulha ou perseguir o player se ele estiver perto
            geladeiraAberta = false; // Reseta a flag
            estadoAtual = Estado.Patrulhando; // Ou GerenciarEstados() para reavaliar
        }
    }


    // --- Funções Auxiliares ---

    private void MoverPara(Vector3 destino)
    {
        agente.SetDestination(destino);
    }

    private void MoverParaProximoPonto()
    {
        if (pontosPatrulha.Count == 0) return;
        Vector3 destino = pontosPatrulha[indicePontoAtual].position;
        MoverPara(destino);
        indicePontoAtual = (indicePontoAtual + 1) % pontosPatrulha.Count;
    }

    private void AtualizarAnimacaoERotacao()
    {
        // Só ativa a animação de andar se o agente não estiver parado
        bool estaAndando = !agente.isStopped && agente.velocity.magnitude > 0.1f;
        animator.SetBool("isWalking", estaAndando);

        // Só faz a rotação em blocos quando não está atacando OU indo fechar geladeira (se quiser que ele encare a geladeira)
        if (estadoAtual != Estado.Atacando && estadoAtual != Estado.IndoFecharGeladeira && estaAndando) // Adapte a condição se quiser rotação diferenciada
        {
            RotacaoEmBlocos();
        }
        else if (estadoAtual == Estado.IndoFecharGeladeira && !agente.isStopped && geladeiraAlvo != null)
        {
            // Opcional: fazer o chefe encarar a geladeira enquanto se aproxima
            Vector3 direcaoParaGeladeira = (geladeiraAlvo.position - transform.position).normalized;
            transform.rotation = Quaternion.LookRotation(new Vector3(direcaoParaGeladeira.x, 0, direcaoParaGeladeira.z));
        }
        else if (estadoAtual == Estado.Atacando)
        {
            // Já tem a lógica de encarar o alvo no ExecutarAtaque, mas é bom ter aqui para consistência
            Vector3 direcao = (alvo.position - transform.position).normalized;
            transform.rotation = Quaternion.LookRotation(new Vector3(direcao.x, 0, direcao.z));
        }
    }


    private void RotacaoEmBlocos()
    {
        Vector3 direcao = agente.velocity.normalized;
        if (direcao != Vector3.zero)
        {
            Quaternion lookRotation = Quaternion.LookRotation(direcao);
            transform.rotation = lookRotation;
        }
    }

    // NOVO MÉTODO PÚBLICO para ser chamado pela geladeira
    public void NotificarGeladeiraAberta(Transform geladeiraTransform)
    {
        geladeiraAberta = true;
        geladeiraAlvo = geladeiraTransform;
        Debug.Log("Chefe notificado: Geladeira aberta!");
    }
}