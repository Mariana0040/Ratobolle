// SaveSystem.cs
using UnityEngine;
using System.IO;
using System;

public static class SaveSystem
{
    private static readonly string SAVE_FOLDER = Application.persistentDataPath + "/Saves/";
    //private static readonly string SETTINGS_FILE = "settings.save";
    //private static readonly string GAME_FILE = "game.save";

    public static void Initialize()
    {
        if (!Directory.Exists(SAVE_FOLDER))
        {
            Directory.CreateDirectory(SAVE_FOLDER);
        }
    }

    public static void Save(GameData data, string fileName)
    {
        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(SAVE_FOLDER + fileName, json);
    }

    public static GameData Load(string fileName)
    {
        string path = SAVE_FOLDER + fileName;
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            return JsonUtility.FromJson<GameData>(json);
        }
        return null;
    }

    public static bool SaveExists(string fileName)
    {
        return File.Exists(SAVE_FOLDER + fileName);
    }

    public static void DeleteSave(string fileName)
    {
        if (SaveExists(fileName))
        {
            File.Delete(SAVE_FOLDER + fileName);
        }
    }
}

[System.Serializable]
public class GameData
{
    // Configurações
    public bool musicEnabled = true;
    public bool sfxEnabled = true;
    public int languageIndex = 0;

    // Dados do jogo (adicione seus próprios campos)
    // public int playerLevel;
    // public Vector3 playerPosition;
}