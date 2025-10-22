using UnityEngine;

public class BotaoSair : MonoBehaviour
{
    // Esta função será chamada pelo botão
    public void SairDoJogo()
    {
        Debug.Log("Botão de sair clicado!"); // Mensagem para testar no Editor
        Application.Quit();
    }
}