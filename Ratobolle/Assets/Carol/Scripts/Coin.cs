using UnityEngine;

public class Coin : MonoBehaviour
{
    [Tooltip("Valor desta moeda.")]
    public int value = 1; // A maioria das moedas valer� 1, mas voc� pode ter moedas maiores

    [Tooltip("Efeito de part�cula a ser instanciado ao coletar (opcional).")]
    public GameObject collectionEffectPrefab;

    [Tooltip("Som a ser tocado ao coletar (opcional).")]
    public AudioClip collectionSound;

    // Voc� pode adicionar mais coisas aqui, como anima��es de giro, etc.

    void Start()
    {
        // Garante que tem um Collider para ser detectado
        Collider col = GetComponent<Collider>();
        if (col == null)
        {
            Debug.LogError($"Moeda '{gameObject.name}' n�o possui um Collider! N�o poder� ser coletada.", this);
        }
        // � crucial que o collider da moeda seja um Trigger para coleta suave
        else if (!col.isTrigger)
        {
            Debug.LogWarning($"Collider na moeda '{gameObject.name}' n�o est� marcado como 'Is Trigger'. Recomenda-se marcar 'Is Trigger' para coleta suave.", this);
            // Considere marcar automaticamente: col.isTrigger = true;
        }
    }

    // Fun��o a ser chamada pelo jogador quando a moeda � coletada
    public void Collect(AudioSource playerAudioSource) // Passamos o AudioSource do jogador para tocar o som
    {
        // Instancia o efeito de coleta, se houver
        if (collectionEffectPrefab != null)
        {
            Instantiate(collectionEffectPrefab, transform.position, Quaternion.identity);
        }

        // Toca o som de coleta, se houver e se o jogador tiver um AudioSource
        if (collectionSound != null && playerAudioSource != null)
        {
            playerAudioSource.PlayOneShot(collectionSound);
        }
        else if (collectionSound != null && playerAudioSource == null)
        {
            // Fallback: Toca o som na posi��o da moeda se o jogador n�o tiver AudioSource
            // Isso � menos ideal para sons 2D, mas funciona para sons 3D.
            AudioSource.PlayClipAtPoint(collectionSound, transform.position);
        }

        // Destr�i o objeto da moeda
        Destroy(gameObject);
    }
}