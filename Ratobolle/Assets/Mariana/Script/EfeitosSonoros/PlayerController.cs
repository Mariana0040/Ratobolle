using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Efeitos Sonoros")]
    public AudioClip somDoPulo;
    public AudioClip somDeColeta; // Exemplo de outro som

    void Update()
    {
        // Exemplo 1: Tocar som de pulo ao apertar a barra de espaço
        if (Input.GetKeyDown(KeyCode.Space))
        {
            // Lógica do pulo do seu personagem iria aqui...
            Debug.Log("Jogador pulou!");

            // --- AQUI A MÁGICA ACONTECE ---
            // Verifica se o som foi definido no Inspector antes de tocar
            if (somDoPulo != null)
            {
                // Pede para o AudioManager tocar o nosso efeito sonoro
                AudioManager.Instance.PlaySFX(somDoPulo);
            }
        }
    }

    // Exemplo 2: Tocar som ao colidir com um item coletável
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Coletavel"))
        {
            Debug.Log("Pegou um item!");
            Destroy(other.gameObject); // Destrói o item

            // Pede para o AudioManager tocar o som de coleta
            if (somDeColeta != null)
            {
                AudioManager.Instance.PlaySFX(somDeColeta);
            }
        }
    }
    
}