using UnityEngine;

// Este script deve ser adicionado ao seu Prefab do Tomate
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Collider))]
public class ProjetilTomate : MonoBehaviour
{
    [Header("Configurações do Impacto")]
    [Tooltip("Tag do objeto que o projétil deve atingir (geralmente 'Player').")]
    [SerializeField] private string tagDoAlvo = "Player";

    [Tooltip("Quantidade de segundos a serem removidos do temporizador.")]
    [SerializeField] private float penalidadeDeTempo = 10f;

    [Tooltip("Partículas ou efeito visual para tocar no ponto de impacto.")]
    [SerializeField] private GameObject efeitoDeImpacto;

    // Esta variável será preenchida pelo script 'ControleDeAtaqueChefe'
    public Transform pontoDeRespawn;

    // Referência em cache para o temporizador para evitar procurá-lo repetidamente
    private TemporizadorDeQueijo temporizador;

    void Start()
    {
        // Encontra o temporizador na cena UMA VEZ e o armazena. É mais eficiente.
        temporizador = Object.FindFirstObjectByType<TemporizadorDeQueijo>();

        // Autodestruição para o caso de o tomate não atingir nada
        Destroy(gameObject, 8f);
    }

    // Método chamado pela engine de física da Unity quando este objeto colide com outro
    private void OnCollisionEnter(Collision collision)
    {
        // 1. Verifica se o objeto com o qual colidimos tem a tag correta (ex: "Player")
        if (collision.gameObject.CompareTag(tagDoAlvo))
        {
            Debug.Log("Tomate atingiu o jogador!");

            // 2. Se encontramos um temporizador na cena, chama a função para reduzir o tempo
            if (temporizador != null)
            {
                temporizador.ReduzirTempo(penalidadeDeTempo);
            }
            else
            {
                Debug.LogWarning("O projétil atingiu o jogador, mas não encontrou o 'TemporizadorDeQueijo' na cena.");
            }

            // 3. Pega o controlador do jogador para forçar o respawn
            FakeRollCapsuleController playerController = collision.gameObject.GetComponent<FakeRollCapsuleController>();
            if (playerController != null && pontoDeRespawn != null)
            {
                // Usa uma função que já existe no seu jogador para lidar com o hit
                playerController.HandleLaserHit(pontoDeRespawn, penalidadeDeTempo);
            }

            // 4. (Opcional) Instancia um efeito visual no local da colisão
            if (efeitoDeImpacto != null)
            {
                Instantiate(efeitoDeImpacto, collision.contacts[0].point, Quaternion.identity);
            }
        }

        // 5. Destrói o projétil (tomate) após a colisão com qualquer objeto
        Destroy(gameObject);
    }
}