using UnityEngine;

public class ControleDeAtaqueChefe : MonoBehaviour
{
    [Header("Referências do Ataque")]
    public GameObject tomatePrefab;
    public Transform pontoDeLancamento;
    public Transform pontoDeRespawnJogador;

    [Header("Parâmetros do Ataque")]
    public float distanciaDeAtaque = 8.0f; // O Cérebro Principal usa este valor para decidir
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

    // MÉTODO PÚBLICO CHAMADO PELO "CÉREBRO PRINCIPAL"
    public void TentarAtaque()
    {
        // Só ataca se o tempo de recarga passou e se não está no meio de uma animação
        if (Time.time >= proximoAtaquePermitido && !estaAnimandoAtaque)
        {
            proximoAtaquePermitido = Time.time + intervaloAtaque;
            estaAnimandoAtaque = true; // Impede que o trigger seja chamado várias vezes

            // Dispara o gatilho da animação de ataque
            animator.SetTrigger("attackTrigger");
        }
    }

    // EVENTO CHAMADO PELA ANIMAÇÃO NO MOMENTO DO ARREMESSO
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

    // EVENTO CHAMADO PELA ANIMAÇÃO NO FIM DO ATAQUE
    public void EventoTerminoAtaque()
    {
        estaAnimandoAtaque = false; // Libera para um novo ataque
    }
}