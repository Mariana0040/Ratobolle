using UnityEngine;

public class ControleDeAtaqueChefe : MonoBehaviour
{
    [Header("Refer�ncias do Ataque")]
    public GameObject tomatePrefab;
    public Transform pontoDeLancamento;
    public Transform pontoDeRespawnJogador;

    // --- NOVO: Refer�ncia para o alvo ---
    [Tooltip("Arraste o Transform do jogador para c�.")]
    public Transform alvo;

    [Header("Par�metros do Ataque")]
    public float distanciaDeAtaque = 8.0f;
    public float intervaloAtaque = 2.5f;
    public float forcaLancamento = 20f;

    // Componentes e controle
    private Animator animator;
    private float proximoAtaquePermitido = 0f;
    private bool estaAnimandoAtaque = false;

    void Start()
    {
        animator = GetComponent<Animator>();

        // --- NOVO: Tenta encontrar o alvo se n�o for definido ---
        if (alvo == null)
        {
            // Pega a refer�ncia do alvo a partir do script principal de IA
            ChefeDeCozinhaAI cerebro = GetComponent<ChefeDeCozinhaAI>();
            if (cerebro != null && cerebro.alvo != null)
            {
                alvo = cerebro.alvo;
            }
            else
            {
                Debug.LogError("Alvo n�o encontrado! O tomate n�o saber� para onde ir.", this);
            }
        }
    }

    // M�TODO P�BLICO CHAMADO PELO "C�REBRO PRINCIPAL"
    public void TentarAtaque()
    {
        if (Time.time >= proximoAtaquePermitido && !estaAnimandoAtaque)
        {
            proximoAtaquePermitido = Time.time + intervaloAtaque;
            estaAnimandoAtaque = true;

            // --- NOVO: Olhar para o jogador ANTES de disparar a anima��o ---
            // Isso garante que o chefe esteja virado na dire��o correta
            if (alvo != null)
            {
                Vector3 direcaoParaAlvo = (alvo.position - transform.position).normalized;
                transform.rotation = Quaternion.LookRotation(new Vector3(direcaoParaAlvo.x, 0, direcaoParaAlvo.z));
            }

            animator.SetTrigger("attackTrigger");
        }
    }

    // EVENTO CHAMADO PELA ANIMA��O NO MOMENTO DO ARREMESSO
    public void EventoDeArremesso()
    {
        if (tomatePrefab == null || pontoDeLancamento == null || alvo == null) return;

        // --- L�GICA DE LAN�AMENTO ATUALIZADA ---

        // 1. Calcula a dire��o exata do ponto de lan�amento at� o centro do jogador.
        Vector3 direcaoParaAlvo = (alvo.position - pontoDeLancamento.position).normalized;

        // 2. Cria o tomate. A rota��o inicial n�o importa tanto, pois a for�a vai ditar a trajet�ria.
        GameObject tomate = Instantiate(tomatePrefab, pontoDeLancamento.position, Quaternion.LookRotation(direcaoParaAlvo));

        ProjetilTomate projetilScript = tomate.GetComponent<ProjetilTomate>();
        if (projetilScript != null)
        {
            projetilScript.pontoDeRespawn = this.pontoDeRespawnJogador;
        }

        Rigidbody rb = tomate.GetComponent<Rigidbody>();

        // 3. Aplica a for�a NA DIRE��O CALCULADA, e n�o na dire��o "para frente" do ponto de lan�amento.
        rb.AddForce(direcaoParaAlvo * forcaLancamento, ForceMode.VelocityChange);
    }

    // EVENTO CHAMADO PELA ANIMA��O NO FIM DO ATAQUE
    public void EventoTerminoAtaque()
    {
        estaAnimandoAtaque = false;
    }
}