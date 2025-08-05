using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic; // Importante para usar Listas

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Animator))]
public class ChefeDeCozinhaAI : MonoBehaviour
{
    [Header("Patrulha")]
    [Tooltip("Lista de pontos de patrulha.")]
    public List<Transform> pontosPatrulha;
    [Tooltip("Velocidade do chefe durante a patrulha.")]
    public float velocidadePatrulha = 3.0f;

    [Header("Persegui��o")]
    [Tooltip("O alvo a ser perseguido (o jogador).")]
    public Transform alvo;
    [Tooltip("A que dist�ncia o chefe detecta o jogador.")]
    public float distanciaDeteccao = 10.0f;
    [Tooltip("Velocidade do chefe ao perseguir o jogador.")]
    public float velocidadePerseguicao = 6.0f;

    private NavMeshAgent agente;
    private Animator animator;

    private enum Estado { Patrulhando, Perseguindo }
    private Estado estadoAtual;
    private int indicePontoAtual = 0; // �ndice do ponto atual na lista de patrulha

    void Start()
    {
        agente = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();

        agente.updateRotation = false; // Desativa a rota��o autom�tica do NavMeshAgent
        agente.angularSpeed = 0; // Impede rota��o suave

        if (pontosPatrulha.Count > 0)
        {
            estadoAtual = Estado.Patrulhando;
            MoverParaProximoPonto();
        }
        else
        {
            Debug.LogError("Nenhum ponto de patrulha definido para o Chefe de Cozinha!");
            estadoAtual = Estado.Patrulhando; // Fica no estado de patrulha, mas sem destino
        }

        // Tenta encontrar o jogador pela tag se n�o for definido no Inspector
        if (alvo == null)
        {
            GameObject jogador = GameObject.FindGameObjectWithTag("Player");
            if (jogador != null)
            {
                alvo = jogador.transform;
            }
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

    private void GerenciarEstados()
    {
        if (alvo == null) return; // N�o faz nada se n�o houver um alvo

        float distanciaParaAlvo = Vector3.Distance(transform.position, alvo.position);

        if (distanciaParaAlvo <= distanciaDeteccao && estadoAtual == Estado.Patrulhando)
        {
            estadoAtual = Estado.Perseguindo;
        }
        else if (distanciaParaAlvo > distanciaDeteccao && estadoAtual == Estado.Perseguindo)
        {
            estadoAtual = Estado.Patrulhando;
        }
    }

    private void ExecutarPatrulha()
    {
        // Verifica se chegou ao destino de patrulha
        if (!agente.pathPending && agente.remainingDistance < 0.5f)
        {
            MoverParaProximoPonto();
        }
    }

    private void ExecutarPerseguicao()
    {
        agente.speed = velocidadePerseguicao;
        MoverPara(alvo.position, velocidadePerseguicao);
    }

    private void MoverPara(Vector3 destino, float velocidade)
    {
        agente.speed = velocidade;
        agente.SetDestination(destino);
    }

    private void MoverParaProximoPonto()
    {
        if (pontosPatrulha.Count == 0) return; // N�o faz nada se n�o houver pontos

        // Define o destino para o pr�ximo ponto na lista
        Vector3 destino = pontosPatrulha[indicePontoAtual].position;
        MoverPara(destino, velocidadePatrulha);

        // Incrementa o �ndice para o pr�ximo ponto, voltando ao in�cio se chegar ao fim da lista
        indicePontoAtual = (indicePontoAtual + 1) % pontosPatrulha.Count;
    }

    private void RotacaoEmBlocos()
    {
        if (agente.velocity.sqrMagnitude > 0.1f)
        {
            Vector3 direcao = agente.velocity.normalized;

            if (Mathf.Abs(direcao.x) > Mathf.Abs(direcao.z))
            {
                direcao.z = 0;
            }
            else
            {
                direcao.x = 0;
            }

            Quaternion lookRotation = Quaternion.LookRotation(direcao);
            transform.rotation = lookRotation;
        }
    }

    private void AtualizarAnimacao()
    {
        bool estaAndando = agente.velocity.magnitude > 0.1f;
        animator.SetBool("isWalking", estaAndando);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, distanciaDeteccao);
    }
}