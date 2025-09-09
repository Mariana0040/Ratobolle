using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Collider))]
public class ProjetilTomate : MonoBehaviour
{
    [Header("Configura��es do Impacto")]
    [SerializeField] private string tagDoAlvo = "Player";
    [SerializeField] private float penalidadeDeTempo = 10f;
    [SerializeField] private GameObject efeitoDeImpacto;

    // --- REFER�NCIA AO SOM (N�O MAIS AO AUDIOSOURCE) ---
    [Header("�udio")]
    [Tooltip("O som que toca quando o tomate atinge algo.")]
    [SerializeField] private AudioClip somDeImpacto;

    public Transform pontoDeRespawn;
    private TemporizadorDeQueijo temporizador;

    // N�o precisa mais de refer�ncia ao AudioSource aqui

    void Start()
    {
        temporizador = Object.FindFirstObjectByType<TemporizadorDeQueijo>();
        Destroy(gameObject, 8f);
    }

    private void OnCollisionEnter(Collision collision)
    {
        // --- CHAMA O AUDIOMANAGER PARA TOCAR O SOM ---
        // A l�gica de PlayClipAtPoint � �til, mas vamos manter a simplicidade
        // e usar o SFXSource central do AudioManager.
        if (AudioManager.Instance != null && somDeImpacto != null)
        {
            AudioManager.Instance.PlaySFX(somDeImpacto);
        }

        if (collision.gameObject.CompareTag(tagDoAlvo))
        {
            // ... (l�gica de dano e respawn permanece a mesma) ...
        }

        Destroy(gameObject);
    }
}