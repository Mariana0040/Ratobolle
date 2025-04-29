// LocalizationManager.cs
using UnityEngine;
using System.Collections.Generic;

public class LocalizationManager : MonoBehaviour
{
    public static LocalizationManager Instance;

    public enum Language { Portuguese, English }
    public Language currentLanguage = Language.Portuguese;

    public Dictionary<string, string[]> localizationData = new Dictionary<string, string[]>();

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeDictionary();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void InitializeDictionary()
    {
        // Exemplo: Key | PT | EN
        localizationData.Add("start_button", new string[] { "Iniciar Jogo", "Start Game" });
    }
        
                                                                               

    public void ChangeLanguage(int languageIndex)
    {
        currentLanguage = (Language)languageIndex;
        // Atualizar todos os textos da cena
        foreach (LocalizedText text in FindObjectsOfType<LocalizedText>())
        {
            text.UpdateText();
        }
    }
}

