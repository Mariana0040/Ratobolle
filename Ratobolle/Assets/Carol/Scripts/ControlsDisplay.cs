using UnityEngine;
using UnityEngine.UI; // Se precisar interagir com componentes UI diretamente

public class ControlsDisplay : MonoBehaviour
{
    public void ShowPanel()
    {
        if (!gameObject.activeSelf) // S� ativa se j� n�o estiver ativo
        {
            gameObject.SetActive(true);
            Debug.Log($"Painel '{gameObject.name}' mostrado.");
        }
    }

    // Esta fun��o ser� chamada para esconder o painel
    public void HidePanel()
    {
        if (gameObject.activeSelf) // S� desativa se j� n�o estiver inativo
        {
            gameObject.SetActive(false);
            Debug.Log($"Painel '{gameObject.name}' escondido.");
        }
    }

    // Opcional: garantir que come�a desativado
    void Awake()
    {
        // Se voc� j� desativou no editor, esta linha pode ser redundante ou �til como garantia.
        // No entanto, se o script que ativa/desativa � outro (como KitchenZone),
        // � melhor deixar o estado inicial ser controlado pelo editor.
        // Para este caso, vamos assumir que ele j� come�a desativado no editor.
    }
}