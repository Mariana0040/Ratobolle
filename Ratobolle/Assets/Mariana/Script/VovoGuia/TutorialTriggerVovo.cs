using UnityEngine;

public class TutorialTriggerVovo : MonoBehaviour
{
    // Caixa para a gente arrastar nosso Prefab da Vovó
    public GameObject prefabDaVovo;

    // --- MUDANÇA IMPORTANTE AQUI ---
    // Em vez de UMA string, agora temos um ARRAY (uma lista) de strings.
    // Os colchetes [] significam "uma coleção de".
    [TextArea(3, 10)]
    public string[] mensagens;

    // Guarda a referência da vovó que foi criada
    private GameObject vovoInstanciada;
    private bool jaFoiAtivado = false;


    public void OnTriggerEnter(Collider other)
    {
        // Se o objeto que entrou for o jogador E o gatilho não foi ativado ainda
        if (other.CompareTag("Player") && !jaFoiAtivado)
        {
            jaFoiAtivado = true; // Marca como ativado

            // Se a vovó ainda não existe no mundo, vamos criá-la
            // Usamos FindObjectOfType para evitar criar várias vovós
            VovoGuiaController vovoExistente = Object.FindFirstObjectByType<VovoGuiaController>();
            if (vovoExistente == null)
            {
                // Cria uma cópia da vovó na cena
                Vector3 posicaoParaAparecer = other.transform.position + other.transform.forward * 2f + Vector3.up;
                vovoInstanciada = Instantiate(prefabDaVovo, posicaoParaAparecer, Quaternion.identity);

                // Pega o script da vovó e diz para ela quem ela deve seguir (o jogador)
                vovoInstanciada.GetComponent<VovoGuiaController>().alvoParaSeguir = other.transform;
            }
            else
            {
                // Se a vovó já existe, apenas pegamos a referência dela
                vovoInstanciada = vovoExistente.gameObject;
            }

            // Inicia a conversa, enviando a LISTA INTEIRA de mensagens
            vovoInstanciada.GetComponent<VovoGuiaController>().IniciarConversa(mensagens);
        }
    }
}