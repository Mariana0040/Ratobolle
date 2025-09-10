using UnityEngine;

public class GatilhoDaCozinha : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        // Verifica se quem entrou no gatilho foi o jogador (usando a tag "Player")
        if (other.CompareTag("Player"))
        {
            // Notifica o gerenciador
            GerenciadorRestaurante.Instance.RegistrarEntradaNaCozinha();

            // Opcional: Desativa o gatilho para não contar várias vezes se o jogador ficar parado na porta
            // GetComponent<Collider>().enabled = false; 
        }
    }
}