using UnityEngine;
using UnityEngine.UI; // Se o painel tiver componentes de UI que precisam ser gerenciados, mas para ativar/desativar o GameObject n�o � estritamente necess�rio.

public class RecipeBookToggle : MonoBehaviour
{
    [Header("Painel do Livro de Receitas")]
    [Tooltip("Arraste aqui o GameObject do Painel que representa o livro de receitas aberto.")]
    [SerializeField] private GameObject recipeBookPanel; // O GameObject que voc� quer mostrar/esconder

    // Opcional: Para controlar o estado e evitar spam de logs
    private bool isRecipeBookOpen = false;

    void Start()
    {
        // Garante que o painel do livro de receitas e o painel principal estejam definidos
        if (recipeBookPanel == null)
        {
            Debug.LogError("O Painel do Livro de Receitas (RecipeBookPanel) n�o foi definido no Inspetor!", this);
            enabled = false; // Desativa o script para evitar erros
            return;
        }

        // Come�a com o livro de receitas fechado (invis�vel)
        recipeBookPanel.SetActive(false);
        isRecipeBookOpen = false;
    }

    void Update()
    {
        // Verifica se a tecla 'R' foi pressionada neste frame
        if (Input.GetKeyDown(KeyCode.R))
        {
            ToggleRecipeBook();
        }
    }

    // Fun��o para alternar a visibilidade do livro de receitas
    public void ToggleRecipeBook()
    {
        if (recipeBookPanel == null) return; // Seguran�a

        isRecipeBookOpen = !isRecipeBookOpen; // Inverte o estado
        recipeBookPanel.SetActive(isRecipeBookOpen); // Ativa ou desativa o GameObject do painel

        if (isRecipeBookOpen)
        {
            Debug.Log("Livro de Receitas Aberto!");
            // Opcional: Pausar o jogo, mostrar o cursor do mouse, etc.
            // Time.timeScale = 0f; // Exemplo de pausar o jogo
            // Cursor.lockState = CursorLockMode.None;
            // Cursor.visible = true;
        }
        else
        {
            Debug.Log("Livro de Receitas Fechado!");
            // Opcional: Despausar o jogo, esconder o cursor, etc.
            // Time.timeScale = 1f;
            // Cursor.lockState = CursorLockMode.Locked;
            // Cursor.visible = false;
        }
    }

    // Voc� pode chamar esta fun��o de um bot�o de UI tamb�m, se quiser um bot�o para fechar
    public void CloseRecipeBook()
    {
        if (recipeBookPanel == null) return;

        if (isRecipeBookOpen) // S� faz algo se estiver aberto
        {
            isRecipeBookOpen = false;
            recipeBookPanel.SetActive(false);
            Debug.Log("Livro de Receitas Fechado (pelo bot�o/fun��o).");
            // Time.timeScale = 1f;
            // Cursor.lockState = CursorLockMode.Locked;
            // Cursor.visible = false;
        }
    }
}