using UnityEngine;

public class InteracaoGeladeira : MonoBehaviour
{
    public ComportamentoChefe chefe; // Arraste o objeto do Chefe para c� no Inspector
    private bool estaAberta = false; // Para evitar spam de notifica��es

    // M�todo que voc� chamaria quando o player interage (abre) a geladeira
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
                Debug.LogWarning("Refer�ncia ao Chefe n�o atribu�da na geladeira!");
            }

            // Opcional: Depois de um tempo, ou quando o chefe chegar, voc� pode "fechar" visualmente
            // E resetar 'estaAberta' para false. Por enquanto, o chefe far� isso.
        }
    }

    // Voc� precisar� chamar AbrirGeladeira() de onde o jogador interage.
    // Exemplo: Se o jogador apertar 'E' perto da geladeira.
    void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player") && Input.GetKeyDown(KeyCode.E)) // Supondo que o player tenha a tag "Player"
        {
            AbrirGeladeira();
            // Voc� tamb�m pode adicionar uma anima��o de abrir a geladeira aqui
        }
    }

    // M�todo para o chefe chamar quando ele "fechar" a geladeira
    public void FecharGeladeiraVisualmente()
    {
        estaAberta = false;
        Debug.Log("Geladeira visualmente fechada.");
        // Adicione aqui a l�gica visual para fechar a geladeira (anima��o, etc.)
    }
}