// MainMenuManager.cs
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{
    [Header("Painéis")]
    public GameObject confirmationPanel;
    public GameObject exitConfirmationPanel; // Novo painel para sair
    public GameObject settingsPanel;

    [Header("Componentes do Painel")]
    [SerializeField] private TextMeshProUGUI confirmationText;
    [SerializeField] private Button confirmButton;
    [SerializeField] private Button cancelButton;

    [Header("Botões")]
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

        //só para teste
        {
            PlayerPrefs.SetInt("SavedGame", 1); // Força a existência de progresso
            PlayerPrefs.Save();

            confirmationPanel.SetActive(false);
            settingsPanel.SetActive(true);

            confirmButton.onClick.AddListener(OnConfirm);
            cancelButton.onClick.AddListener(OnCancel);
        }

    }

    // Chamado pelo botão "Sair"
    public void OnQuitPressed()
    {
        currentMode = ConfirmationMode.Quit;
        UpdateConfirmationUI(
            "Deseja salvar o progresso?",
            "Sim",
            "Não"
        );
        confirmationPanel.SetActive(true);
    }

    // Chamado pelo botão "Novo Jogo"
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
            Debug.Log("Botão Jogar pressionado");

            if (PlayerPrefs.HasKey("SavedGame"))
            {
                Debug.Log("Progresso encontrado, mostrando confirmação.");
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

    // Atualiza textos do painel de confirmação
    private void UpdateConfirmationUI(string message, string confirmLabel, string cancelLabel)
    {
        confirmationText.text = message;
        confirmButton.GetComponentInChildren<TextMeshProUGUI>().text = confirmLabel;
        cancelButton.GetComponentInChildren<TextMeshProUGUI>().text = cancelLabel;
    }

    // Ação do botão "Sim" ou "Confirmar"
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

    // Ação do botão "Não" ou "Cancelar"
    public void OnCancel()
    {
        confirmationPanel.SetActive(false);

        if (currentMode == ConfirmationMode.Quit)
        {
            QuitGame(); // Sai sem salvar
        }
    }

    // Chamado pelo botão "Jogar"
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

    // Método para salvar o progresso
    private void SaveGame()
    {
        PlayerPrefs.SetInt("SavedGame", 1); // Exemplo: salva um flag
        PlayerPrefs.Save(); // Garante o salvamento imediato
    }

    // Método para iniciar novo jogo
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

