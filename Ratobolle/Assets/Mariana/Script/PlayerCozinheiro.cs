using UnityEngine;
using System.Collections;

public class PlayerCozinheiro : MonoBehaviour
{
    [Header("Configura��o de Cozinha")]
    [Tooltip("O tempo que leva para preparar e entregar a comida.")]
    public float tempoDePreparo = 10.0f;
    [Tooltip("A que dist�ncia o jogador precisa estar do cliente para interagir.")]
    public float raioDeInteracao = 2.0f;
    public KeyCode teclaDeInteracao = KeyCode.E;

    // Controle interno
    private bool temTodosIngredientes = false; // Mude isso para true quando o jogador pegar tudo
    private bool estaCozinhando = false;

    void Update()
    {
        // Apenas um exemplo para teste. Voc� deve implementar sua l�gica de coleta de ingredientes.
        if (Input.GetKeyDown(KeyCode.I))
        {
            temTodosIngredientes = true;
            Debug.Log("Jogador pegou todos os ingredientes!");
        }

        // Tenta cozinhar ao apertar a tecla
        if (Input.GetKeyDown(teclaDeInteracao))
        {
            if (!temTodosIngredientes)
            {
                Debug.Log("Faltam ingredientes!");
                return;
            }
            if (estaCozinhando)
            {
                Debug.Log("J� estou cozinhando!");
                return;
            }

            TentarEntregarComida();
        }
    }

    void TentarEntregarComida()
    {
        // Encontra todos os colliders pr�ximos ao jogador
        Collider[] collidersProximos = Physics.OverlapSphere(transform.position, raioDeInteracao);

        foreach (var col in collidersProximos)
        {
            // Tenta pegar o componente do cliente rato
            ClienteRatoAI cliente = col.GetComponent<ClienteRatoAI>();

            // Se encontrou um cliente e ele est� esperando pedido...
            if (cliente != null)
            {
                // Inicia o processo de cozinhar para este cliente
                StartCoroutine(PrepararEEntregar(cliente));
                return; // Sai do loop para n�o servir m�ltiplos clientes de uma vez
            }
        }

        Debug.Log("Nenhum cliente esperando por perto.");
    }

    private IEnumerator PrepararEEntregar(ClienteRatoAI clienteAlvo)
    {
        estaCozinhando = true;
        Debug.Log($"Iniciando preparo para o cliente na mesa {clienteAlvo.gameObject.name}...");

        // Aqui voc� pode tocar uma anima��o de cozinhar no jogador

        yield return new WaitForSeconds(tempoDePreparo);

        Debug.Log("Comida pronta! Entregando...");
        clienteAlvo.ReceberComida(5f); // O rato come por 5 segundos

        // Reseta o estado
        temTodosIngredientes = false;
        estaCozinhando = false;
    }

    // Desenha uma esfera no editor para visualizar o raio de intera��o
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, raioDeInteracao);
    }
}