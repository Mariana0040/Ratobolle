using UnityEngine;

public class InteracaoGeladeira : MonoBehaviour
{
    public ComportamentoChefe chefe; // Arraste o objeto do Chefe para cá no Inspector
    private bool estaAberta = false; // Para evitar spam de notificações

    // Método que você chamaria quando o player interage (abre) a geladeira
    public void AbrirGeladeira()
    {
        if (!estaAberta)
        {
            estaAberta = true;
            Debug.Log("Geladeira foi aberta!");
            // Notifica o chefe
            if (chefe != null)
            {
                chefe.NotificarGeladeiraAberta(this.transform);
            }
            else
            {
                Debug.LogWarning("Referência ao Chefe não atribuída na geladeira!");
            }

            // Opcional: Depois de um tempo, ou quando o chefe chegar, você pode "fechar" visualmente
            // E resetar 'estaAberta' para false. Por enquanto, o chefe fará isso.
        }
    }

    // Você precisará chamar AbrirGeladeira() de onde o jogador interage.
    // Exemplo: Se o jogador apertar 'E' perto da geladeira.
    void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player") && Input.GetKeyDown(KeyCode.E)) // Supondo que o player tenha a tag "Player"
        {
            AbrirGeladeira();
            // Você também pode adicionar uma animação de abrir a geladeira aqui
        }
    }

    // Método para o chefe chamar quando ele "fechar" a geladeira
    public void FecharGeladeiraVisualmente()
    {
        estaAberta = false;
        Debug.Log("Geladeira visualmente fechada.");
        // Adicione aqui a lógica visual para fechar a geladeira (animação, etc.)
    }
}