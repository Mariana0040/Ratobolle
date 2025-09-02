using UnityEngine;

// Este script deve ser adicionado ao seu Prefab do Tomate
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Collider))]
public class ProjetilTomate : MonoBehaviour
{
    [Header("Configura��es do Impacto")]
    [Tooltip("Tag do objeto que o proj�til deve atingir (geralmente 'Player').")]
    [SerializeField] private string tagDoAlvo = "Player";

    [Tooltip("Quantidade de segundos a serem removidos do temporizador.")]
    [SerializeField] private float penalidadeDeTempo = 10f;

    [Tooltip("Part�culas ou efeito visual para tocar no ponto de impacto.")]
    [SerializeField] private GameObject efeitoDeImpacto;

    // Esta vari�vel ser� preenchida pelo script 'ControleDeAtaqueChefe'
    public Transform pontoDeRespawn;

    // Refer�ncia em cache para o temporizador para evitar procur�-lo repetidamente
    private TemporizadorDeQueijo temporizador;

    void Start()
    {
        // Encontra o temporizador na cena UMA VEZ e o armazena. � mais eficiente.
        temporizador = Object.FindFirstObjectByType<TemporizadorDeQueijo>();

        // Autodestrui��o para o caso de o tomate n�o atingir nada
        Destroy(gameObject, 8f);
    }

    // M�todo chamado pela engine de f�sica da Unity quando este objeto colide com outro
    private void OnCollisionEnter(Collision collision)
    {
        // 1. Verifica se o objeto com o qual colidimos tem a tag correta (ex: "Player")
        if (collision.gameObject.CompareTag(tagDoAlvo))
        {
            Debug.Log("Tomate atingiu o jogador!");

            // 2. Se encontramos um temporizador na cena, chama a fun��o para reduzir o tempo
            if (temporizador != null)
            {
                temporizador.ReduzirTempo(penalidadeDeTempo);
            }
            else
            {
                Debug.LogWarning("O proj�til atingiu o jogador, mas n�o encontrou o 'TemporizadorDeQueijo' na cena.");
            }

            // 3. Pega o controlador do jogador para for�ar o respawn
            FakeRollCapsuleController playerController = collision.gameObject.GetComponent<FakeRollCapsuleController>();
            if (playerController != null && pontoDeRespawn != null)
            {
                // Usa uma fun��o que j� existe no seu jogador para lidar com o hit
                playerController.HandleLaserHit(pontoDeRespawn, penalidadeDeTempo);
            }

            // 4. (Opcional) Instancia um efeito visual no local da colis�o
            if (efeitoDeImpacto != null)
            {
                Instantiate(efeitoDeImpacto, collision.contacts[0].point, Quaternion.identity);
            }
        }

        // 5. Destr�i o proj�til (tomate) ap�s a colis�o com qualquer objeto
        Destroy(gameObject);
    }
}