using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class InGamePauseAndSettings : MonoBehaviour
{
    [Header("Painel de Pause/Configurações")]
    [SerializeField] private GameObject pauseMenuPanel;

    [Header("Controles de Configurações (UI)")]
    [SerializeField] private Slider musicVolumeSlider;
    [SerializeField] private Slider sfxVolumeSlider;
    [SerializeField] private Toggle fullscreenToggle;
    [SerializeField] private TMP_Dropdown resolutionDropdown;

    public static bool isGamePaused = false;

    void Start()
    {
        if (pauseMenuPanel != null)
        {
            pauseMenuPanel.SetActive(false);
        }
        Time.timeScale = 1f;
        isGamePaused = false;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        LoadSettings();
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
                PauseGame(); // Agora esta chamada deve funcionar
            }
        }
    }

    public void ResumeGame()
    {
        if (pauseMenuPanel != null)
        {
            Debug.Log("Tentando desativar o PauseMenuPanel...");
            pauseMenuPanel.SetActive(false);
        }
        // Lógica de despausar o jogo
        Time.timeScale = 1f;
        isGamePaused = false;
        Debug.Log("Jogo Despausado.");
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // ***** PauseGame() AGORA ESTÁ FORA E NO NÍVEL CORRETO DA CLASSE *****
    void PauseGame()
    {
        if (pauseMenuPanel != null)
        {
            Debug.Log("Tentando ativar o PauseMenuPanel...");
            pauseMenuPanel.SetActive(true);
            // Adicione aqui a verificação se realmente ficou ativo, como fizemos antes:
            // if (pauseMenuPanel.activeInHierarchy) Debug.Log("PauseMenuPanel ATIVO!");
            // else Debug.LogError("PauseMenuPanel NÃO ATIVO após SetActive(true)!");
        }
        else
        {
            Debug.LogError("PauseMenuPanel é NULO! Não foi arrastado no Inspetor.");
            return;
        }

        Time.timeScale = 0f;
        isGamePaused = true;
        Debug.Log("Jogo Pausado. Time.timeScale = 0.");
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        LoadSettings(); // Carrega as configurações para mostrar os valores corretos na UI de pause
    }

    public void LoadMainMenu()
    {
        Time.timeScale = 1f;
        isGamePaused = false;
        SceneManager.LoadScene(0);
    }

    void LoadSettings()
    {
        // ... (Lógica de LoadSettings permanece a mesma) ...
        if (musicVolumeSlider != null) musicVolumeSlider.value = PlayerPrefs.GetFloat("MusicVolume", 0.75f);
        if (sfxVolumeSlider != null) sfxVolumeSlider.value = PlayerPrefs.GetFloat("SfxVolume", 0.75f);
        if (fullscreenToggle != null) fullscreenToggle.isOn = PlayerPrefs.GetInt("Fullscreen", Screen.fullScreen ? 1 : 0) == 1;
        if (resolutionDropdown != null) { /* Lógica do dropdown */ }
        Debug.Log("Configurações carregadas no painel de pause.");
    }

    // --- FUNÇÕES CHAMADAS PELOS CONTROLES DA UI DE CONFIGURAÇÕES ---
    public void OnMusicVolumeChanged(float volume)
    {
        Debug.Log("Volume da Música (Pause Menu): " + volume);
        PlayerPrefs.SetFloat("MusicVolume", volume);
        PlayerPrefs.Save();
    }

    public void OnSFXVolumeChanged(float volume)
    {
        Debug.Log("Volume SFX (Pause Menu): " + volume);
        PlayerPrefs.SetFloat("SfxVolume", volume);
        PlayerPrefs.Save();
    }

    public void OnFullscreenToggle(bool isOn)
    {
        Debug.Log("Fullscreen (Pause Menu): " + isOn);
        Screen.fullScreen = isOn;
        PlayerPrefs.SetInt("Fullscreen", isOn ? 1 : 0);
        PlayerPrefs.Save();
    }

    public void OnResolutionChanged(int index)
    {
        Debug.Log("Resolução Alterada (Pause Menu), Índice: " + index);
        // ... (Lógica de aplicar resolução) ...
        PlayerPrefs.SetInt("ResolutionIndex", index);
        PlayerPrefs.Save();
    }
}