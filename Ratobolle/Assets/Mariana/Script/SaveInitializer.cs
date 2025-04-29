// SaveInitializer.cs
using UnityEngine;

public class SaveInitializer : MonoBehaviour
{
    void Awake()
    {
        SaveSystem.Initialize();

        // Carrega configurações ou cria novas
       /* if (!SaveSystem.SaveExists(SaveSystem.SETTINGS_FILE))
        {
            GameData defaultSettings = new GameData();
            SaveSystem.Save(defaultSettings, SaveSystem.SETTINGS_FILE);
        }*/
    }
}