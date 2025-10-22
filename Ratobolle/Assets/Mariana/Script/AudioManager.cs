using UnityEngine;
using System.Collections.Generic;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Fontes de �udio")]
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource sfxSource;

    [Header("Volumes Padr�o")]
    [Range(0f, 1f)] public float defaultMusicVolume = 0.8f;
    [Range(0f, 1f)] public float defaultSFXVolume = 1f;

    // Volumes atuais (para acesso externo)
    public float CurrentMusicVolume { get; private set; }
    public float CurrentSFXVolume { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeVolumes();

            // Garante que a m�sica n�o pare ao recarregar cenas
            musicSource.ignoreListenerPause = true;
            musicSource.ignoreListenerVolume = true;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void InitializeVolumes()
    {
        // Carrega volumes salvos ou usa padr�o
        CurrentMusicVolume = PlayerPrefs.GetFloat("MusicVolume", defaultMusicVolume);
        CurrentSFXVolume = PlayerPrefs.GetFloat("SFXVolume", defaultSFXVolume);

        // Aplica volumes
        musicSource.volume = CurrentMusicVolume;
        sfxSource.volume = CurrentSFXVolume;
    }

    // Controle de M�sica
    public void SetMusicVolume(float volume)
    {
        CurrentMusicVolume = Mathf.Clamp01(volume);
        musicSource.volume = CurrentMusicVolume;
        PlayerPrefs.SetFloat("MusicVolume", CurrentMusicVolume);
    }

    // Controle de SFX
    public void SetSFXVolume(float volume)
    {
        CurrentSFXVolume = Mathf.Clamp01(volume);
        sfxSource.volume = CurrentSFXVolume;
        PlayerPrefs.SetFloat("SFXVolume", CurrentSFXVolume);
    }

    // Toca um efeito sonoro
    public void PlaySFX(AudioClip clip)
    {
        sfxSource.PlayOneShot(clip, CurrentSFXVolume);
    }

    // Toca m�sica
    public void PlayMusic(AudioClip clip)
    {
        if (musicSource.clip == clip) return;

        musicSource.clip = clip;
        musicSource.Play();
    }

    // --- NOVA FUN��O PARA SONS 3D ---
    // Toca um efeito sonoro em uma posi��o espec�fica no mundo
    public void PlaySFXAtLocation(AudioClip clip, Vector3 position)
    {
        // Cria uma fonte de �udio tempor�ria na posi��o, toca o som e a destr�i.
        AudioSource.PlayClipAtPoint(clip, position, CurrentSFXVolume);
    }
}