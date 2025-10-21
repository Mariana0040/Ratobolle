using UnityEngine;

public class TutorialTriggerVovo : MonoBehaviour
{
    // Caixa para a gente arrastar nosso Prefab da Vov�
    public GameObject prefabDaVovo;

    // --- MUDAN�A IMPORTANTE AQUI ---
    // Em vez de UMA string, agora temos um ARRAY (uma lista) de strings.
    // Os colchetes [] significam "uma cole��o de".
    [TextArea(3, 10)]
    public string[] mensagens;

    // Guarda a refer�ncia da vov� que foi criada
    private GameObject vovoInstanciada;
    private bool jaFoiAtivado = false;


    public void OnTriggerEnter(Collider other)
    {
        // Se o objeto que entrou for o jogador E o gatilho n�o foi ativado ainda
        if (other.CompareTag("Player") && !jaFoiAtivado)
        {
            jaFoiAtivado = true; // Marca como ativado

            // Se a vov� ainda n�o existe no mundo, vamos cri�-la
            // Usamos FindObjectOfType para evitar criar v�rias vov�s
            VovoGuiaController vovoExistente = Object.FindFirstObjectByType<VovoGuiaController>();
            if (vovoExistente == null)
            {
                // Cria uma c�pia da vov� na cena
                Vector3 posicaoParaAparecer = other.transform.position + other.transform.forward * 2f + Vector3.up;
                vovoInstanciada = Instantiate(prefabDaVovo, posicaoParaAparecer, Quaternion.identity);

                // Pega o script da vov� e diz para ela quem ela deve seguir (o jogador)
                vovoInstanciada.GetComponent<VovoGuiaController>().alvoParaSeguir = other.transform;
            }
            else
            {
                // Se a vov� j� existe, apenas pegamos a refer�ncia dela
                vovoInstanciada = vovoExistente.gameObject;
            }

            // Inicia a conversa, enviando a LISTA INTEIRA de mensagens
            vovoInstanciada.GetComponent<VovoGuiaController>().IniciarConversa(mensagens);
        }
    }
}