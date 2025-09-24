using UnityEngine;

using UnityEngine.AI;

using System.Collections.Generic;
 
[RequireComponent(typeof(NavMeshAgent))]

[RequireComponent(typeof(Animator))]

public class GatoHumanoideAI : MonoBehaviour

{

    // --- NOVO: Chave que permite a persegui��o ---

    private bool podePerseguir = false;
 
    [Header("Patrulha")]

    public List<Transform> pontosPatrulha;

    public float velocidadePatrulha = 3.0f; // Velocidade para andar
 
    [Header("Persegui��o")]

    public Transform alvo;

    public float distanciaDeteccao = 10.0f;

    public float velocidadePerseguicao = 6.0f; // Velocidade para correr
 
    [Header("Respawn")]

    [Tooltip("Ponto para onde o gato retorna ap�s capturar o jogador.")]

    public Transform playerRespawnPoint;
 
    private NavMeshAgent agente;

    private Animator animator;
 
    private enum Estado { Patrulhando, Perseguindo }

    private Estado estadoAtual;

    private int indicePontoAtual = 0;
 
    void Start()

    {

        agente = GetComponent<NavMeshAgent>();

        animator = GetComponent<Animator>();
 
        agente.updateRotation = false;

        agente.angularSpeed = 0;
 
        // Inicia o comportamento de patrulha normalmente

        estadoAtual = Estado.Patrulhando;

        if (pontosPatrulha.Count > 0)

        {

            MoverParaProximoPonto();

        }
 
        if (alvo == null && GameObject.FindGameObjectWithTag("Player") != null)

        {

            alvo = GameObject.FindGameObjectWithTag("Player").transform;

        }

    }
 
    void Update()

    {

        GerenciarEstados(); // Decide o que fazer
 
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
 
    // --- NOVO: Detecta quando o jogador encosta no gatilho ---

    private void OnTriggerEnter(Collider other)

    {

        // Se o modo de persegui��o j� foi liberado, n�o faz nada

        if (podePerseguir) return;
 
        // Verifica se quem encostou tem a tag "Player"

        if (other.CompareTag("Player"))

        {

            Debug.Log("GATO ATIVADO! Agora ele pode perseguir o Gordito!");

            podePerseguir = true; // Liga a chave

        }

    }
 
    private void GerenciarEstados()

    {

        if (alvo == null) return;
 
        float distanciaParaAlvo = Vector3.Distance(transform.position, alvo.position);
 
        // --- L�GICA MODIFICADA ---

        // A condi��o para perseguir agora precisa que a chave 'podePerseguir' esteja ligada

        if (podePerseguir && distanciaParaAlvo <= distanciaDeteccao)

        {

            estadoAtual = Estado.Perseguindo;

        }

        else

        {

            // Em todos os outros casos (jogador longe OU a chave 'podePerseguir' est� desligada),

            // ele continua patrulhando.

            estadoAtual = Estado.Patrulhando;

        }

    }
 
    private void ExecutarPatrulha()

    {

        agente.speed = velocidadePatrulha; // Define a velocidade de andar
 
        if (!agente.pathPending && agente.remainingDistance < 0.5f)

        {

            MoverParaProximoPonto();

        }

    }
 
    private void ExecutarPerseguicao()

    {

        agente.speed = velocidadePerseguicao; // Define a velocidade de correr

        MoverPara(alvo.position);

    }
 
    private void MoverPara(Vector3 destino)

    {

        agente.SetDestination(destino);

    }
 
    private void MoverParaProximoPonto()

    {

        if (pontosPatrulha.Count == 0) return;

        Vector3 destino = pontosPatrulha[indicePontoAtual].position;

        agente.speed = velocidadePatrulha; // Garante que a velocidade de patrulha seja usada

        MoverPara(destino);

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
 
            if (direcao != Vector3.zero)

            {

                Quaternion lookRotation = Quaternion.LookRotation(direcao);

                transform.rotation = lookRotation;

            }

        }

    }
 
    private void AtualizarAnimacao()

    {

        // Verifica se o agente est� se movendo

        bool estaSeMovendo = agente.velocity.magnitude > 0.1f;
 
        // Verifica se o estado atual � de persegui��o (correndo)

        bool estaCorrendo = estadoAtual == Estado.Perseguindo;
 
        // Atualiza os par�metros no Animator

        animator.SetBool("isWalking", estaSeMovendo && !estaCorrendo); // S� anda se estiver se movendo E N�O correndo

        animator.SetBool("isRunning", estaSeMovendo && estaCorrendo);  // S� corre se estiver se movendo E no estado de persegui��o

    }
 
    void OnDrawGizmosSelected()

    {

        Gizmos.color = Color.yellow;

        Gizmos.DrawWireSphere(transform.position, distanciaDeteccao);

    }
 
    // --- L�GICA DE CAPTURA E RESPAWN DO JOGADOR (ATUALIZADA) ---
 
    private void OnCollisionEnter(Collision collision)

    {

        if (estadoAtual == Estado.Perseguindo && collision.gameObject.CompareTag("Player"))

        {

            Debug.Log("Gato capturou o jogador! Enviando jogador para o respawn.");

            ProcessarCapturaDoJogador(collision.gameObject);

        }

    }
 
    private void ProcessarCapturaDoJogador(GameObject jogador)

    {

        // Verifica��o de seguran�a

        if (playerRespawnPoint == null)

        {

            Debug.LogError("Ponto de Respawn do Jogador n�o foi configurado no Gato! N�o � poss�vel mover o jogador.", this);

            return;

        }
 
        // 1. Teleporta o JOGADOR para a posi��o de respawn.

        jogador.transform.position = playerRespawnPoint.position;
 
        // NOTA: Se o seu jogador usa um CharacterController, voc� pode precisar desativ�-lo

        // e reativ�-lo para que o teleporte funcione corretamente.

        // Ex:

        // CharacterController cc = jogador.GetComponent<CharacterController>();

        // if(cc != null) cc.enabled = false;

        // jogador.transform.position = playerRespawnPoint.position;

        // if(cc != null) cc.enabled = true;
 
        // 2. Muda o estado do GATO de volta para Patrulhando.

        estadoAtual = Estado.Patrulhando;
 
        // 3. Para o movimento atual do GATO e o manda para o pr�ximo ponto de patrulha.

        agente.ResetPath();

        MoverParaProximoPonto();

    }

}
 