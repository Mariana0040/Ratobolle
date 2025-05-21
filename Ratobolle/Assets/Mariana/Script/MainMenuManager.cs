// MainMenuManager.cs
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{
    [Header("Pain�is")]
    public GameObject confirmationPanel;
    public GameObject exitConfirmationPanel; // Novo painel para sair
    public GameObject settingsPanel;

    [Header("Componentes do Painel")]
    [SerializeField] private TextMeshProUGUI confirmationText;
    [SerializeField] private Button confirmButton;
    [SerializeField] private Button cancelButton;

    [Header("Bot�es")]
    public Button newGameButton;
    public Button continueButton;
    public Button quitButton;

    private enum ConfirmationMode { NewGame, Quit }
    private ConfirmationMode currentMode;

    void Start()
    {
        confirmationPanel.SetActive(false);
        settingsPanel.SetActive(true);

        confirmButton.onClick.AddListener(OnConfirm);
        cancelButton.onClick.AddListener(OnCancel);

        //s� para teste
        {
            PlayerPrefs.SetInt("SavedGame", 1); // For�a a exist�ncia de progresso
            PlayerPrefs.Save();

            confirmationPanel.SetActive(false);
            settingsPanel.SetActive(true);

            confirmButton.onClick.AddListener(OnConfirm);
            cancelButton.onClick.AddListener(OnCancel);
        }

    }

    // Chamado pelo bot�o "Sair"
    public void OnQuitPressed()
    {
        currentMode = ConfirmationMode.Quit;
        UpdateConfirmationUI(
            "Deseja salvar o progresso?",
            "Sim",
            "N�o"
        );
        confirmationPanel.SetActive(true);
    }

    // Chamado pelo bot�o "Novo Jogo"
    public void OnStartGame()
    {
        //if (PlayerPrefs.HasKey("SavedGame"))
        //{
        // currentMode = ConfirmationMode.NewGame;
        //UpdateConfirmationUI(
        // "Deseja salvar seu progresso?",
        //"Confirmar",
        //"Cancelar"
        //);
        //confirmationPanel.SetActive(true);
        //}
        //else
        //{
        //  StartNewGame();
        // }
        {
            Debug.Log("Bot�o Jogar pressionado");

            if (PlayerPrefs.HasKey("SavedGame"))
            {
                Debug.Log("Progresso encontrado, mostrando confirma��o.");
                currentMode = ConfirmationMode.NewGame;
                UpdateConfirmationUI(
                    "Deseja salvar seu progresso?",
                    "Confirmar",
                    "Cancelar"
                );
                confirmationPanel.SetActive(true);
            }
            else
            {
                Debug.Log("Nenhum progresso salvo. Iniciando novo jogo.");
                StartNewGame();
            }
        }

    }

    // Atualiza textos do painel de confirma��o
    private void UpdateConfirmationUI(string message, string confirmLabel, string cancelLabel)
    {
        confirmationText.text = message;
        confirmButton.GetComponentInChildren<TextMeshProUGUI>().text = confirmLabel;
        cancelButton.GetComponentInChildren<TextMeshProUGUI>().text = cancelLabel;
    }

    // A��o do bot�o "Sim" ou "Confirmar"
    public void OnConfirm()
    {
        confirmationPanel.SetActive(false);

        if (currentMode == ConfirmationMode.NewGame)
        {
            StartNewGame();
        }
        else if (currentMode == ConfirmationMode.Quit)
        {
            SaveGame(); // Salva antes de sair
            QuitGame();
        }
    }

    // A��o do bot�o "N�o" ou "Cancelar"
    public void OnCancel()
    {
        confirmationPanel.SetActive(false);

        if (currentMode == ConfirmationMode.Quit)
        {
            QuitGame(); // Sai sem salvar
        }
    }

    // Chamado pelo bot�o "Jogar"
    public void OnPlayPressed()
    {
        if (PlayerPrefs.HasKey("SavedGame"))
        {
            currentMode = ConfirmationMode.NewGame;
            UpdateConfirmationUI(
                "Deseja sobrescrever seu progresso salvo?",
                "Confirmar",
                "Cancelar"
            );
            confirmationPanel.SetActive(true);
        }
        else
        {
            StartNewGame();
        }
    }

    // M�todo para salvar o progresso
    private void SaveGame()
    {
        PlayerPrefs.SetInt("SavedGame", 1); // Exemplo: salva um flag
        PlayerPrefs.Save(); // Garante o salvamento imediato
    }

    // M�todo para iniciar novo jogo
    public void StartNewGame()
    {
        PlayerPrefs.DeleteAll();
        SceneManager.LoadScene("Game");
    }
    public void StartTutorial()
    {
        SceneManager.LoadScene(1);
    }
    public void ContinueGame()
    {
        SceneManager.LoadScene("Game");
    }

    public void OpenSettings()
    {
        settingsPanel.SetActive(true);
    }

    public void CloseSettings()
    {
        settingsPanel.SetActive(false);
    }

    public void QuitGame()
    {
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}

