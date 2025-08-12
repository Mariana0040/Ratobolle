using UnityEngine;

public class ControleDeAtaqueChefe : MonoBehaviour
{
    [Header("Refer�ncias do Ataque")]
    public GameObject tomatePrefab;
    public Transform pontoDeLancamento;
    public Transform pontoDeRespawnJogador;

    [Header("Par�metros do Ataque")]
    public float distanciaDeAtaque = 8.0f; // O C�rebro Principal usa este valor para decidir
    public float intervaloAtaque = 2.5f;
    public float forcaLancamento = 20f;

    // Componentes e controle
    private Animator animator;
    private float proximoAtaquePermitido = 0f;
    private bool estaAnimandoAtaque = false;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    // M�TODO P�BLICO CHAMADO PELO "C�REBRO PRINCIPAL"
    public void TentarAtaque()
    {
        // S� ataca se o tempo de recarga passou e se n�o est� no meio de uma anima��o
        if (Time.time >= proximoAtaquePermitido && !estaAnimandoAtaque)
        {
            proximoAtaquePermitido = Time.time + intervaloAtaque;
            estaAnimandoAtaque = true; // Impede que o trigger seja chamado v�rias vezes

            // Dispara o gatilho da anima��o de ataque
            animator.SetTrigger("attackTrigger");
        }
    }

    // EVENTO CHAMADO PELA ANIMA��O NO MOMENTO DO ARREMESSO
    public void EventoDeArremesso()
    {
        if (tomatePrefab == null || pontoDeLancamento == null) return;

        GameObject tomate = Instantiate(tomatePrefab, pontoDeLancamento.position, pontoDeLancamento.rotation);

        ProjetilTomate projetilScript = tomate.GetComponent<ProjetilTomate>();
        if (projetilScript != null)
        {
            projetilScript.pontoDeRespawn = this.pontoDeRespawnJogador;
        }

        Rigidbody rb = tomate.GetComponent<Rigidbody>();
        rb.AddForce(pontoDeLancamento.forward * forcaLancamento, ForceMode.VelocityChange);
    }

    // EVENTO CHAMADO PELA ANIMA��O NO FIM DO ATAQUE
    public void EventoTerminoAtaque()
    {
        estaAnimandoAtaque = false; // Libera para um novo ataque
    }
}