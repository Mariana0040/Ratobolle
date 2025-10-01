using UnityEngine;
using UnityEngine.UI; // Se o painel tiver componentes de UI que precisam ser gerenciados, mas para ativar/desativar o GameObject não é estritamente necessário.
using TMPro;
public class RecipeBookToggle : MonoBehaviour
{
    [Header("Painel do Livro de Receitas")]
    [Tooltip("Arraste aqui o GameObject do Painel que representa o livro de receitas aberto.")]
    [SerializeField] private GameObject recipeBookPanel; // O GameObject que você quer mostrar/esconder

    [Header("Referências de UI Adicionais")] // 2. ADICIONE ESTE CABEÇALHO (Opcional)
    [Tooltip("Arraste aqui o texto que deve aparecer quando o livro estiver aberto.")]
    [SerializeField] private TextMeshProUGUI exitBookPrompt; // 3. ADICIONE ESTA LINHA para a referência do texto


    // Opcional: Para controlar o estado e evitar spam de logs
    private bool isRecipeBookOpen = false;

    void Start()
    {
        // Garante que o painel do livro de receitas e o painel principal estejam definidos
        if (recipeBookPanel == null)
        {
            Debug.LogError("O Painel do Livro de Receitas (RecipeBookPanel) não foi definido no Inspetor!", this);
            enabled = false; // Desativa o script para evitar erros
            return;
        }

        // Garante que ambos comecem desativados
        recipeBookPanel.SetActive(false);
        if (exitBookPrompt != null)
        {
            exitBookPrompt.gameObject.SetActive(false);
        }

        // Começa com o livro de receitas fechado (invisível)
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

    // Função para alternar a visibilidade do livro de receitas
    public void ToggleRecipeBook()
    {
        if (recipeBookPanel == null) return; // Segurança

        isRecipeBookOpen = !isRecipeBookOpen; // Inverte o estado
        recipeBookPanel.SetActive(isRecipeBookOpen); // Ativa ou desativa o GameObject do painel

        if (isRecipeBookOpen)
        {
            Debug.Log("Livro de Receitas Aberto!");
            // Opcional: Pausar o jogo, mostrar o cursor do mouse, etc.
            // Time.timeScale = 0f; // Exemplo de pausar o jogo
            // Cursor.lockState = CursorLockMode.None;
            // Cursor.visible = true;
            // 4. ADICIONE ESTE BLOCO: Mostra o texto quando o livro abre
            if (exitBookPrompt != null)
            {
                exitBookPrompt.gameObject.SetActive(true);
            }
        }
        else
        {
            Debug.Log("Livro de Receitas Fechado!");
            // Opcional: Despausar o jogo, esconder o cursor, etc.
            // Time.timeScale = 1f;
            // Cursor.lockState = CursorLockMode.Locked;
            // Cursor.visible = false;

            // 5. ADICIONE ESTE BLOCO: Esconde o texto quando o livro fecha
            if (exitBookPrompt != null)
            {
                exitBookPrompt.gameObject.SetActive(false);
            }
        }
    }

    // Você pode chamar esta função de um botão de UI também, se quiser um botão para fechar
    public void CloseRecipeBook()
    {
        if (recipeBookPanel == null) return;

        if (isRecipeBookOpen) // Só faz algo se estiver aberto
        {
            isRecipeBookOpen = false;
            recipeBookPanel.SetActive(false);
            Debug.Log("Livro de Receitas Fechado (pelo botão/função).");
            // Time.timeScale = 1f;
            // Cursor.lockState = CursorLockMode.Locked;
            // Cursor.visible = false;

            // 6. ADICIONE ESTE BLOCO: Garante que o texto se esconda aqui também
            if (exitBookPrompt != null)
            {
                exitBookPrompt.gameObject.SetActive(false);
            }
        }
    }
}