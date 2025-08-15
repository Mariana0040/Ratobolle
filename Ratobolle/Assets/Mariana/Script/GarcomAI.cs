using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;

[RequireComponent(typeof(NavMeshAgent))]
public class GarcomAI : MonoBehaviour
{
    // Chave que permite a perseguição
    private bool podePerseguir = false;

    [Header("Patrulha")]
    public List<Transform> pontosPatrulha;
    public float velocidadePatrulha = 3.0f; // Velocidade de andar

    [Header("Perseguição")]
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

        // Começa a patrulhar imediatamente
        estadoAtual = Estado.Patrulhando;
        if (pontosPatrulha.Count > 0)
        {
            MoverParaProximoPonto();
        }

        // Encontra o jogador se não for definido
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

    // Detecta quando o jogador encosta no gatilho para ativar a perseguição
    private void OnTriggerEnter(Collider other)
    {
        if (podePerseguir) return; // Se já pode perseguir, não faz nada

        if (other.CompareTag("Player"))
        {
            Debug.Log("GARÇOM ATIVADO! Agora ele pode perseguir!");
            podePerseguir = true;
        }
    }

    private void GerenciarEstados()
    {
        if (alvo == null) return;

        float distanciaParaAlvo = Vector3.Distance(transform.position, alvo.position);

        // A perseguição só é uma opção se a chave 'podePerseguir' estiver ligada
        if (podePerseguir && distanciaParaAlvo <= distanciaDeteccao)
        {
            estadoAtual = Estado.Perseguindo;
        }
        else
        {
            // Senão, ele sempre patrulha
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
        // Se o estado é 'Perseguindo', ativa a animação de correr.
        // Senão, desativa (o que fará ele voltar para a animação de andar).
        bool estaCorrendo = (estadoAtual == Estado.Perseguindo);

        animator.SetBool("isRunning", estaCorrendo);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, distanciaDeteccao);
    }
}