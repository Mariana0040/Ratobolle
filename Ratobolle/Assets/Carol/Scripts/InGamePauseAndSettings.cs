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

    // Não precisa de referência ao AudioManager, pois ele é um Singleton (Instance)

    void Start()
    {
        if (pauseMenuPanel != null) pauseMenuPanel.SetActive(false);
        Time.timeScale = 1f;
        isGamePaused = false;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // Puxa as configurações salvas para os sliders
        //LoadSettings();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isGamePaused) ResumeGame();
            else PauseGame();
        }
    }

    // --- FUNÇÕES CHAMADAS PELOS SLIDERS (AGORA COMANDAM O AUDIOMANAGER) ---
    public void OnMusicVolumeChanged(float volume)
    {
        // Manda o comando para o AudioManager ajustar a música
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.SetMusicVolume(volume);
        }
    }

    public void OnSFXVolumeChanged(float volume)
    {
        // Manda o comando para o AudioManager ajustar os SFX
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.SetSFXVolume(volume);
        }
    }

    // A função LoadSettings agora busca os valores DIRETO do AudioManager, que já os leu do PlayerPrefs
    void LoadSettings()
    {
        if (AudioManager.Instance != null)
        {
            if (musicVolumeSlider != null) musicVolumeSlider.value = AudioManager.Instance.CurrentMusicVolume;
            if (sfxVolumeSlider != null) sfxVolumeSlider.value = AudioManager.Instance.CurrentSFXVolume;
        }

        if (fullscreenToggle != null) fullscreenToggle.isOn = Screen.fullScreen;
        // ... (outras lógicas de load) ...
    }

    // O resto do seu script permanece o mesmo (Resume, Pause, etc.)
    #region Lógica de Pause e Menus
    public void ResumeGame()
    {
        if (pauseMenuPanel != null) pauseMenuPanel.SetActive(false);
        Time.timeScale = 1f;
        isGamePaused = false;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void PauseGame()
    {
        if (pauseMenuPanel != null) pauseMenuPanel.SetActive(true);
        Time.timeScale = 0f;
        isGamePaused = true;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        LoadSettings(); // Atualiza os sliders com os valores atuais ao pausar
    }

    public void LoadMainMenu()
    {
        Time.timeScale = 1f;
        isGamePaused = false;
        SceneManager.LoadScene(0);
    }

    public void OnFullscreenToggle(bool isOn)
    {
        Screen.fullScreen = isOn;
        PlayerPrefs.SetInt("Fullscreen", isOn ? 1 : 0);
        PlayerPrefs.Save();
    }

    public void OnResolutionChanged(int index)
    {
        PlayerPrefs.SetInt("ResolutionIndex", index);
        PlayerPrefs.Save();
    }
    #endregion
}