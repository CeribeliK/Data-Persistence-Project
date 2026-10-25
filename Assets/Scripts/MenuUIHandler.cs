using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class MenuUIHandler : MonoBehaviour
{
    public TMP_InputField nameInputField;
    public TextMeshProUGUI bestScoreText;

    void Start()
    {
        LoadBestScoreToUI();

        // Preenche o campo de texto automaticamente com o último nome jogado
        if (nameInputField != null && DataManager.Instance != null)
        {
            if (!string.IsNullOrEmpty(DataManager.Instance.playerName))
            {
                nameInputField.text = DataManager.Instance.playerName;
            }
        }
    }

    private void LoadBestScoreToUI()
    {
        if (DataManager.Instance != null)
        {
            string name = DataManager.Instance.highScoreName;
            int score = DataManager.Instance.highFieldScore;

            if (string.IsNullOrEmpty(name))
            {
                bestScoreText.text = "Best Score : None : 0";
            }
            else
            {
                bestScoreText.text = $"Best Score : {name} : {score}";
            }
        }
    }

    public void StartNew()
    {
        if (nameInputField != null && DataManager.Instance != null)
        {
            // Salva o nome da última sessão no DataManager e no arquivo JSON
            DataManager.Instance.SaveLastPlayerName(nameInputField.text);
        }

        SceneManager.LoadScene("main");
    }

    public void Exit()
    {
#if UNITY_EDITOR
        EditorApplication.ExitPlaymode();
#else
        Application.Quit();
#endif
    }
}