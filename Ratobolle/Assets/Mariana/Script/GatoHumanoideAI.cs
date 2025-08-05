using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class GatoHumanoideAI : MonoBehaviour
{
    // --- Variáveis de Patrulha ---
    [Header("Patrulha")]
    public Transform[] pontosDePatrulha;
    private int indicePontoAtual = 0;

    // --- Variáveis de Perseguição ---
    [Header("Perseguição")]
    public Transform jogador;
    public float raioDeVisao = 15f;
    public float raioDeAtaque = 2f;

    // --- Componentes e Estado ---
    private NavMeshAgent agente;
    private enum Estado { Patrulhando, Perseguindo }
    private Estado estadoAtual;

    void Start()
    {
        agente = GetComponent<NavMeshAgent>();

        // Tenta encontrar o jogador pela tag "Player" se não for atribuído no Inspector
        if (jogador == null)
        {
            GameObject jogadorObj = GameObject.FindGameObjectWithTag("Player");
            if (jogadorObj != null)
            {
                jogador = jogadorObj.transform;
            }
            else
            {
                Debug.LogError("ERRO: Jogador não encontrado! Verifique se o objeto do jogador tem a tag 'Player' ou atribua-o manualmente no Inspector.");
                // Desativa o script se não houver jogador para evitar mais erros
                this.enabled = false;
                return;
            }
        }

        // Configurações iniciais do agente
        agente.autoBraking = false;
        estadoAtual = Estado.Patrulhando;
        IrParaProximoPonto();
    }

    void Update()
    {
        // Se o jogador ou o agente não existirem, não faz nada.
        if (jogador == null || agente == null) return;

        // Lógica de mudança de estado
        float distanciaDoJogador = Vector3.Distance(jogador.position, transform.position);

        if (distanciaDoJogador <= raioDeVisao)
        {
            estadoAtual = Estado.Perseguindo;
        }
        else
        {
            estadoAtual = Estado.Patrulhando;
        }

        // Executa o comportamento com base no estado atual
        if (estadoAtual == Estado.Perseguindo)
        {
            PerseguirJogador();
        }
        else // Patrulhando
        {
            Patrulhar();
        }
    }

    void Patrulhar()
    {
        // Se o agente estiver perto do destino, vai para o próximo ponto
        if (!agente.pathPending && agente.remainingDistance < 0.5f)
        {
            IrParaProximoPonto();
        }
    }

    void PerseguirJogador()
    {
        // Verifica novamente para garantir (embora já verificado no Update)
        if (jogador == null) return;

        float distancia = Vector3.Distance(jogador.position, transform.position);

        // Para de seguir se estiver dentro do raio de "ataque"
        if (distancia <= raioDeAtaque)
        {
            agente.isStopped = true;
        }
        else
        {
            agente.isStopped = false;
            // ESTA É A LINHA CRÍTICA - agora está segura pelas verificações acima
            agente.SetDestination(jogador.position);
        }
    }

    void IrParaProximoPonto()
    {
        if (pontosDePatrulha.Length == 0) return;

        agente.destination = pontosDePatrulha[indicePontoAtual].position;
        indicePontoAtual = (indicePontoAtual + 1) % pontosDePatrulha.Length;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, raioDeVisao);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, raioDeAtaque);
    }
}
