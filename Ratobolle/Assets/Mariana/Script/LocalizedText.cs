// LocalizedText.cs
using UnityEngine;
using UnityEngine.UI;

public class LocalizedText : MonoBehaviour
{
    public string key;

    void Start()
    {
        UpdateText();
    }

    public void UpdateText()
    {
        Text textComponent = GetComponent<Text>();
        if (LocalizationManager.Instance.localizationData.ContainsKey(key))
        {
            string[] translations = LocalizationManager.Instance.localizationData[key];
            textComponent.text = translations[(int)LocalizationManager.Instance.currentLanguage];
        }
    }
}
