using UnityEngine;

public class ControleDeAtaqueChefe : MonoBehaviour
{
    [Header("Refer�ncias do Ataque")]
    public GameObject tomatePrefab;
    public Transform pontoDeLancamento;
    public Transform pontoDeRespawnJogador;
    public Transform alvo;

    [Header("Par�metros do Ataque")]
    public float distanciaDeAtaque = 8.0f;
    public float intervaloAtaque = 2.5f;
    public float forcaLancamento = 20f;

    // <-- SINALIZADO: Nova se��o para o �udio
    [Header("�udio")]
    [Tooltip("O som que toca quando o chefe arremessa o tomate.")]
    [SerializeField] private AudioClip somDeArremesso;

    // Componentes e controle
    private Animator animator;
    private float proximoAtaquePermitido = 0f;
    private bool estaAnimandoAtaque = false;

    // <-- SINALIZADO: Refer�ncia para o "alto-falante" do chefe
    private AudioSource audioSource;

    void Start()
    {
        animator = GetComponent<Animator>();

        // <-- SINALIZADO: Pega o componente AudioSource no in�cio do jogo
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            Debug.LogWarning("Nenhum componente AudioSource encontrado no Chefe. O som de arremesso n�o funcionar�. Adicione um AudioSource.", this);
        }

        if (alvo == null)
        {
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

    public void TentarAtaque()
    {
        if (Time.time >= proximoAtaquePermitido && !estaAnimandoAtaque)
        {
            proximoAtaquePermitido = Time.time + intervaloAtaque;
            estaAnimandoAtaque = true;

            if (alvo != null)
            {
                Vector3 direcaoParaAlvo = (alvo.position - transform.position).normalized;
                transform.rotation = Quaternion.LookRotation(new Vector3(direcaoParaAlvo.x, 0, direcaoParaAlvo.z));
            }

            animator.SetTrigger("attackTrigger");
        }
    }

    public void EventoDeArremesso()
    {
        if (tomatePrefab == null || pontoDeLancamento == null || alvo == null) return;

        // --- L�GICA DE �UDIO ADICIONADA AQUI ---
        // Toca o som de arremesso, se ambos (o som e o "alto-falante") existirem.
        if (audioSource != null && somDeArremesso != null)
        {
            // PlayOneShot � ideal para efeitos sonoros, pois n�o interrompe outros sons.
            audioSource.PlayOneShot(somDeArremesso);
        }
        // ----------------------------------------

        Vector3 direcaoParaAlvo = (alvo.position - pontoDeLancamento.position).normalized;
        GameObject tomate = Instantiate(tomatePrefab, pontoDeLancamento.position, Quaternion.LookRotation(direcaoParaAlvo));

        ProjetilTomate projetilScript = tomate.GetComponent<ProjetilTomate>();
        if (projetilScript != null)
        {
            projetilScript.pontoDeRespawn = this.pontoDeRespawnJogador;
        }

        Rigidbody rb = tomate.GetComponent<Rigidbody>();
        rb.AddForce(direcaoParaAlvo * forcaLancamento, ForceMode.VelocityChange);
    }

    public void EventoTerminoAtaque()
    {
        estaAnimandoAtaque = false;
    }
}