using UnityEngine;
using System.IO;

public class DataManager : MonoBehaviour
{
    public static DataManager Instance;

    public string playerName;
    public string highScoreName;
    public int highFieldScore;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        LoadHighScore();
    }

    [System.Serializable]
    private class SaveData
    {
        public string highScoreName;
        public int highFieldScore;
        public string lastPlayerName; // ADICIONADO: Guarda o último nome jogado
    }

    public void SaveHighScore(string name, int score)
    {
        highScoreName = name;
        highFieldScore = score;

        SaveData data = new SaveData();
        data.highScoreName = highScoreName;
        data.highFieldScore = highFieldScore;
        data.lastPlayerName = playerName; // Salva o nome atual na estrutura

        string json = JsonUtility.ToJson(data);
        File.WriteAllText(Application.persistentDataPath + "/savefile.json", json);
    }

    public void LoadHighScore()
    {
        string path = Application.persistentDataPath + "/savefile.json";
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            SaveData data = JsonUtility.FromJson<SaveData>(json);

            highScoreName = data.highScoreName;
            highFieldScore = data.highFieldScore;
            playerName = data.lastPlayerName; // Carrega o último nome jogado para a memória
        }
    }

    // ADICIONADO: Método separado para salvar apenas o último nome digitado ao clicar em Start
    public void SaveLastPlayerName(string name)
    {
        playerName = name;

        SaveData data = new SaveData();
        data.highScoreName = highScoreName;
        data.highFieldScore = highFieldScore;
        data.lastPlayerName = playerName;

        string json = JsonUtility.ToJson(data);
        File.WriteAllText(Application.persistentDataPath + "/savefile.json", json);
    }
}