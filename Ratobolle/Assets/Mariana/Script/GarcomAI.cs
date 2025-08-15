using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;

[RequireComponent(typeof(NavMeshAgent))]
public class GarcomAI : MonoBehaviour
{
    // Chave que permite a persegui��o
    private bool podePerseguir = false;

    [Header("Patrulha")]
    public List<Transform> pontosPatrulha;
    public float velocidadePatrulha = 3.0f; // Velocidade de andar

    [Header("Persegui��o")]
    public Transform alvo;
    public float distanciaDeteccao = 10.0f;
    public float velocidadePerseguicao = 6.0f; // Velocidade de correr

    // Componentes
    private NavMeshAgent agente;
    public Animator animator;

    // Controle de estado
    private enum Estado { Patrulhando, Perseguindo }
    private Estado estadoAtual;
    private int indicePontoAtual = 0;

    void Start()
    {
        agente = GetComponent<NavMeshAgent>();

        agente.updateRotation = false;
        agente.angularSpeed = 0;

        // Come�a a patrulhar imediatamente
        estadoAtual = Estado.Patrulhando;
        if (pontosPatrulha.Count > 0)
        {
            MoverParaProximoPonto();
        }

        // Encontra o jogador se n�o for definido
        if (alvo == null && GameObject.FindGameObjectWithTag("Player") != null)
        {
            alvo = GameObject.FindGameObjectWithTag("Player").transform;
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

    // Detecta quando o jogador encosta no gatilho para ativar a persegui��o
    private void OnTriggerEnter(Collider other)
    {
        if (podePerseguir) return; // Se j� pode perseguir, n�o faz nada

        if (other.CompareTag("Player"))
        {
            Debug.Log("GAR�OM ATIVADO! Agora ele pode perseguir!");
            podePerseguir = true;
        }
    }

    private void GerenciarEstados()
    {
        if (alvo == null) return;

        float distanciaParaAlvo = Vector3.Distance(transform.position, alvo.position);

        // A persegui��o s� � uma op��o se a chave 'podePerseguir' estiver ligada
        if (podePerseguir && distanciaParaAlvo <= distanciaDeteccao)
        {
            estadoAtual = Estado.Perseguindo;
        }
        else
        {
            // Sen�o, ele sempre patrulha
            estadoAtual = Estado.Patrulhando;
        }
    }

    private void ExecutarPatrulha()
    {
        agente.speed = velocidadePatrulha;
        if (!agente.pathPending && agente.remainingDistance < 0.5f)
        {
            MoverParaProximoPonto();
        }
    }

    private void ExecutarPerseguicao()
    {
        agente.speed = velocidadePerseguicao;
        agente.SetDestination(alvo.position);
    }

    private void MoverParaProximoPonto()
    {
        if (pontosPatrulha.Count == 0) return;
        agente.speed = velocidadePatrulha;
        agente.SetDestination(pontosPatrulha[indicePontoAtual].position);
        indicePontoAtual = (indicePontoAtual + 1) % pontosPatrulha.Count;
    }

    private void RotacaoEmBlocos()
    {
        if (agente.velocity.sqrMagnitude > 0.1f)
        {
            Vector3 direcao = agente.velocity.normalized;

            if (Mathf.Abs(direcao.x) > Mathf.Abs(direcao.z)) { direcao.z = 0; }
            else { direcao.x = 0; }

            if (direcao != Vector3.zero)
            {
                transform.rotation = Quaternion.LookRotation(direcao);
            }
        }
    }

    private void AtualizarAnimacao()
    {
        // Se o estado � 'Perseguindo', ativa a anima��o de correr.
        // Sen�o, desativa (o que far� ele voltar para a anima��o de andar).
        bool estaCorrendo = (estadoAtual == Estado.Perseguindo);

        animator.SetBool("isRunning", estaCorrendo);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, distanciaDeteccao);
    }
}