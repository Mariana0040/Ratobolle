using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class VolumeControls : MonoBehaviour
{
    [Header("Referências")]
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider sfxSlider;
    [SerializeField] private TMP_Text musicVolumeText;
    [SerializeField] private TMP_Text sfxVolumeText;

    private void Start()
    {
        // Garante que o AudioManager está carregado
        if (AudioManager.Instance == null)
        {
            SceneManager.LoadScene("MainMenu"); // Nome da cena do AudioManager
            return;
        }
        // Inicializa sliders com valores atuais
        musicSlider.value = AudioManager.Instance.CurrentMusicVolume;
        sfxSlider.value = AudioManager.Instance.CurrentSFXVolume;

        // Atualiza textos
        UpdateVolumeTexts();

        // Configura eventos
        musicSlider.onValueChanged.AddListener(SetMusicVolume);
        sfxSlider.onValueChanged.AddListener(SetSFXVolume);

      
    }

    private void SetMusicVolume(float value)
    {
        AudioManager.Instance.SetMusicVolume(value);
        UpdateVolumeTexts();
    }

    private void SetSFXVolume(float value)
    {
        AudioManager.Instance.SetSFXVolume(value);
        UpdateVolumeTexts();
    }

    private void UpdateVolumeTexts()
    {
        musicVolumeText.text = $"Música: {Mathf.RoundToInt(AudioManager.Instance.CurrentMusicVolume * 100)}%";
        sfxVolumeText.text = $"SFX: {Mathf.RoundToInt(AudioManager.Instance.CurrentSFXVolume * 100)}%";
    }
}