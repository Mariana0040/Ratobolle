using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Collider))]
public class ProjetilTomate : MonoBehaviour
{
    [Header("Configurações do Impacto")]
    [SerializeField] private string tagDoAlvo = "Player";
    [SerializeField] private float penalidadeDeTempo = 10f;
    [SerializeField] private GameObject efeitoDeImpacto;

    // --- REFERÊNCIA AO SOM (NÃO MAIS AO AUDIOSOURCE) ---
    [Header("Áudio")]
    [Tooltip("O som que toca quando o tomate atinge algo.")]
    [SerializeField] private AudioClip somDeImpacto;

    public Transform pontoDeRespawn;
    private TemporizadorDeQueijo temporizador;

    // Não precisa mais de referência ao AudioSource aqui

    void Start()
    {
        temporizador = Object.FindFirstObjectByType<TemporizadorDeQueijo>();
        Destroy(gameObject, 8f);
    }

    private void OnCollisionEnter(Collision collision)
    {
        // --- CHAMA O AUDIOMANAGER PARA TOCAR O SOM ---
        // A lógica de PlayClipAtPoint é útil, mas vamos manter a simplicidade
        // e usar o SFXSource central do AudioManager.
        if (AudioManager.Instance != null && somDeImpacto != null)
        {
            AudioManager.Instance.PlaySFX(somDeImpacto);
        }

        if (collision.gameObject.CompareTag(tagDoAlvo))
        {
            // ... (lógica de dano e respawn permanece a mesma) ...
        }

        Destroy(gameObject);
    }
}