using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class InGamePauseAndSettings : MonoBehaviour
{
    [Header("Painel de Pause/Configura��es")]
    [SerializeField] private GameObject pauseMenuPanel;

    [Header("Controles de Configura��es (UI)")]
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
        // L�gica de despausar o jogo
        Time.timeScale = 1f;
        isGamePaused = false;
        Debug.Log("Jogo Despausado.");
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // ***** PauseGame() AGORA EST� FORA E NO N�VEL CORRETO DA CLASSE *****
    void PauseGame()
    {
        if (pauseMenuPanel != null)
        {
            Debug.Log("Tentando ativar o PauseMenuPanel...");
            pauseMenuPanel.SetActive(true);
            // Adicione aqui a verifica��o se realmente ficou ativo, como fizemos antes:
            // if (pauseMenuPanel.activeInHierarchy) Debug.Log("PauseMenuPanel ATIVO!");
            // else Debug.LogError("PauseMenuPanel N�O ATIVO ap�s SetActive(true)!");
        }
        else
        {
            Debug.LogError("PauseMenuPanel � NULO! N�o foi arrastado no Inspetor.");
            return;
        }

        Time.timeScale = 0f;
        isGamePaused = true;
        Debug.Log("Jogo Pausado. Time.timeScale = 0.");
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        LoadSettings(); // Carrega as configura��es para mostrar os valores corretos na UI de pause
    }

    public void LoadMainMenu()
    {
        Time.timeScale = 1f;
        isGamePaused = false;
        SceneManager.LoadScene(0);
    }

    void LoadSettings()
    {
        // ... (L�gica de LoadSettings permanece a mesma) ...
        if (musicVolumeSlider != null) musicVolumeSlider.value = PlayerPrefs.GetFloat("MusicVolume", 0.75f);
        if (sfxVolumeSlider != null) sfxVolumeSlider.value = PlayerPrefs.GetFloat("SfxVolume", 0.75f);
        if (fullscreenToggle != null) fullscreenToggle.isOn = PlayerPrefs.GetInt("Fullscreen", Screen.fullScreen ? 1 : 0) == 1;
        if (resolutionDropdown != null) { /* L�gica do dropdown */ }
        Debug.Log("Configura��es carregadas no painel de pause.");
    }

    // --- FUN��ES CHAMADAS PELOS CONTROLES DA UI DE CONFIGURA��ES ---
    public void OnMusicVolumeChanged(float volume)
    {
        Debug.Log("Volume da M�sica (Pause Menu): " + volume);
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
        Debug.Log("Resolu��o Alterada (Pause Menu), �ndice: " + index);
        // ... (L�gica de aplicar resolu��o) ...
        PlayerPrefs.SetInt("ResolutionIndex", index);
        PlayerPrefs.Save();
    }
}