using UnityEngine;

public class ControleDeAtaqueChefe : MonoBehaviour
{
    [Header("Referências do Ataque")]
    public GameObject tomatePrefab;
    public Transform pontoDeLancamento;
    public Transform pontoDeRespawnJogador;

    // --- NOVO: Referência para o alvo ---
    [Tooltip("Arraste o Transform do jogador para cá.")]
    public Transform alvo;

    [Header("Parâmetros do Ataque")]
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

        // --- NOVO: Tenta encontrar o alvo se não for definido ---
        if (alvo == null)
        {
            // Pega a referência do alvo a partir do script principal de IA
            ChefeDeCozinhaAI cerebro = GetComponent<ChefeDeCozinhaAI>();
            if (cerebro != null && cerebro.alvo != null)
            {
                alvo = cerebro.alvo;
            }
            else
            {
                Debug.LogError("Alvo não encontrado! O tomate não saberá para onde ir.", this);
            }
        }
    }

    // MÉTODO PÚBLICO CHAMADO PELO "CÉREBRO PRINCIPAL"
    public void TentarAtaque()
    {
        if (Time.time >= proximoAtaquePermitido && !estaAnimandoAtaque)
        {
            proximoAtaquePermitido = Time.time + intervaloAtaque;
            estaAnimandoAtaque = true;

            // --- NOVO: Olhar para o jogador ANTES de disparar a animação ---
            // Isso garante que o chefe esteja virado na direção correta
            if (alvo != null)
            {
                Vector3 direcaoParaAlvo = (alvo.position - transform.position).normalized;
                transform.rotation = Quaternion.LookRotation(new Vector3(direcaoParaAlvo.x, 0, direcaoParaAlvo.z));
            }

            animator.SetTrigger("attackTrigger");
        }
    }

    // EVENTO CHAMADO PELA ANIMAÇÃO NO MOMENTO DO ARREMESSO
    public void EventoDeArremesso()
    {
        if (tomatePrefab == null || pontoDeLancamento == null || alvo == null) return;

        // --- LÓGICA DE LANÇAMENTO ATUALIZADA ---

        // 1. Calcula a direção exata do ponto de lançamento até o centro do jogador.
        Vector3 direcaoParaAlvo = (alvo.position - pontoDeLancamento.position).normalized;

        // 2. Cria o tomate. A rotação inicial não importa tanto, pois a força vai ditar a trajetória.
        GameObject tomate = Instantiate(tomatePrefab, pontoDeLancamento.position, Quaternion.LookRotation(direcaoParaAlvo));

        ProjetilTomate projetilScript = tomate.GetComponent<ProjetilTomate>();
        if (projetilScript != null)
        {
            projetilScript.pontoDeRespawn = this.pontoDeRespawnJogador;
        }

        Rigidbody rb = tomate.GetComponent<Rigidbody>();

        // 3. Aplica a força NA DIREÇÃO CALCULADA, e não na direção "para frente" do ponto de lançamento.
        rb.AddForce(direcaoParaAlvo * forcaLancamento, ForceMode.VelocityChange);
    }

    // EVENTO CHAMADO PELA ANIMAÇÃO NO FIM DO ATAQUE
    public void EventoTerminoAtaque()
    {
        estaAnimandoAtaque = false;
    }
}