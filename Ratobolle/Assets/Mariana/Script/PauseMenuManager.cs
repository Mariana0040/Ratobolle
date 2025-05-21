using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenuManager : MonoBehaviour
{
    [Header("Configurações de Cena")]
    [Tooltip("Nome ou Build Index da cena do Menu Principal.")]
    [SerializeField] private string mainMenuSceneName = "MainMenu"; // Ou use o build index 0
    // Se usar build index: [SerializeField] private int mainMenuSceneIndex = 0;

    public static bool isGamePaused = false;

    [Header("UI de Pause (Opcional)")]
    [SerializeField] private GameObject pauseIndicatorPanel;

    private bool isMenuSceneLoaded = false;

    void Start()
    {
        ResumeGameInitial(); // Garante estado inicial correto
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isGamePaused)
            {
                ResumeGame();
            }
            else
            {
                PauseGameAndShowSettings();
            }
        }
    }

    void ResumeGameInitial() // Chamado no Start para garantir o estado
    {
        isGamePaused = false;
        Time.timeScale = 1f;
        if (pauseIndicatorPanel != null) pauseIndicatorPanel.SetActive(false);
        GamePauseState.IsLoadedForSettings = false; // Garante que o estado global está resetado
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void ResumeGame()
    {
        Debug.Log("Jogo Despausado!");
        isGamePaused = false;
        Time.timeScale = 1f;
        GamePauseState.IsLoadedForSettings = false; // Reseta o estado global

        if (pauseIndicatorPanel != null) pauseIndicatorPanel.SetActive(false);

        if (isMenuSceneLoaded)
        {
            SceneManager.UnloadSceneAsync(mainMenuSceneName);
            // SceneManager.UnloadSceneAsync(mainMenuSceneIndex); // se usar índice
            isMenuSceneLoaded = false;
        }
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void PauseGameAndShowSettings()
    {
        Debug.Log("Jogo Pausado! Carregando Configurações do Menu Principal...");
        isGamePaused = true;
        Time.timeScale = 0f;
        GamePauseState.IsLoadedForSettings = true; // AVISA o menu principal

        if (pauseIndicatorPanel != null) pauseIndicatorPanel.SetActive(true);

        if (!IsSceneLoaded(mainMenuSceneName)) // Verifica se a cena já não está carregada
        {
            SceneManager.LoadScene(mainMenuSceneName, LoadSceneMode.Additive);
            // SceneManager.LoadScene(mainMenuSceneIndex, LoadSceneMode.Additive); // se usar índice
            isMenuSceneLoaded = true;
        }
        else
        {
            // Se já estiver carregada (improvável neste fluxo, mas por segurança),
            // talvez precise reativar o painel de settings no MainMenuManager
            // Isso exigiria uma referência direta ou um sistema de eventos mais robusto.
            // Por agora, assumimos que ela não estará carregada.
        }
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    // Função auxiliar para verificar se uma cena está carregada
    bool IsSceneLoaded(string sceneName)
    {
        for (int i = 0; i < SceneManager.sceneCount; i++)
        {
            if (SceneManager.GetSceneAt(i).name == sceneName)
            {
                return true;
            }
        }
        return false;
    }

    // Esta função será chamada pelo botão "Voltar" do MainMenuManager
    // quando o menu foi carregado aditivamente.
    public void HandleBackFromSettings()
    {
        ResumeGame();
    }
}
