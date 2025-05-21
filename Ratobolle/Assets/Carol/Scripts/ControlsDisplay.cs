using UnityEngine;
using UnityEngine.UI; // Se precisar interagir com componentes UI diretamente

public class ControlsDisplay : MonoBehaviour
{
    public void ShowPanel()
    {
        if (!gameObject.activeSelf) // Só ativa se já não estiver ativo
        {
            gameObject.SetActive(true);
            Debug.Log($"Painel '{gameObject.name}' mostrado.");
        }
    }

    // Esta função será chamada para esconder o painel
    public void HidePanel()
    {
        if (gameObject.activeSelf) // Só desativa se já não estiver inativo
        {
            gameObject.SetActive(false);
            Debug.Log($"Painel '{gameObject.name}' escondido.");
        }
    }

    // Opcional: garantir que começa desativado
    void Awake()
    {
        // Se você já desativou no editor, esta linha pode ser redundante ou útil como garantia.
        // No entanto, se o script que ativa/desativa é outro (como KitchenZone),
        // é melhor deixar o estado inicial ser controlado pelo editor.
        // Para este caso, vamos assumir que ele já começa desativado no editor.
    }
}