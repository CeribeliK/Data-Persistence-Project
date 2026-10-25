using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.InputSystem; // Mantém o New Input System

public class MainManager : MonoBehaviour
{
    public Brick BrickPrefab;
    public int LineCount = 6;
    public Rigidbody Ball;

    public Text ScoreText;
    public Text BestScoreText; // Novo: Texto para exibir o recorde na tela do jogo
    public GameObject GameOverText;

    private bool m_Started = false;
    private int m_Points;
    private bool m_GameOver = false;

    private string playerName; // Novo: Guarda o nome do jogador da sessão atual

    // MIGRATED: InputAction replaces Input.GetKeyDown(KeyCode.Space)
    private InputAction m_LaunchAction;

    void Awake()
    {
        m_LaunchAction = new InputAction("Launch", InputActionType.Button, "<Keyboard>/space");
    }

    void OnEnable()
    {
        m_LaunchAction.Enable();
    }

    void OnDisable()
    {
        m_LaunchAction.Disable();
    }

    // Start is called before the first frame update
    void Start()
    {
        // 1. Pega o nome digitado no menu e atualiza o display do recorde
        if (DataManager.Instance != null)
        {
            playerName = DataManager.Instance.playerName;
            UpdateBestScoreDisplay();
        }

        const float step = 0.6f;
        int perLine = Mathf.FloorToInt(4.0f / step);

        int[] pointCountArray = new[] { 1, 1, 2, 2, 5, 5 };
        for (int i = 0; i < LineCount; ++i)
        {
            for (int x = 0; x < perLine; ++x)
            {
                Vector3 position = new Vector3(-1.5f + step * x, 2.5f + i * 0.3f, 0);
                var brick = Instantiate(BrickPrefab, position, Quaternion.identity);
                brick.PointValue = pointCountArray[i];
                brick.onDestroyed.AddListener(AddPoint);
            }
        }
    }

    private void Update()
    {
        if (!m_Started)
        {
            if (m_LaunchAction.WasPressedThisFrame())
            {
                m_Started = true;
                float randomDirection = Random.Range(-1.0f, 1.0f);
                Vector3 forceDir = new Vector3(randomDirection, 1, 0);
                forceDir.Normalize();

                Ball.transform.SetParent(null);
                Ball.AddForce(forceDir * 2.0f, ForceMode.VelocityChange);
            }
        }
        else if (m_GameOver)
        {
            if (m_LaunchAction.WasPressedThisFrame())
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            }
        }
    }

    void AddPoint(int point)
    {
        m_Points += point;

        // 2. Exibe o nome do jogador junto com o score na tela
        ScoreText.text = $"Score : {m_Points}";

        // 3. Verifica se bateu o recorde em tempo real
        CheckAndUpdateHighScore();
    }

    void CheckAndUpdateHighScore()
    {
        if (DataManager.Instance != null)
        {
            if (m_Points > DataManager.Instance.highFieldScore)
            {
                // Salva o novo recorde via JSON no DataManager
                DataManager.Instance.SaveHighScore(playerName, m_Points);
                UpdateBestScoreDisplay();
            }
        }
    }

    void UpdateBestScoreDisplay()
    {
        if (DataManager.Instance != null && BestScoreText != null)
        {
            string bestName = DataManager.Instance.highScoreName;
            int bestScore = DataManager.Instance.highFieldScore;

            if (string.IsNullOrEmpty(bestName))
            {
                BestScoreText.text = "Best Score : None : 0";
            }
            else
            {
                BestScoreText.text = $"Best Score : {bestName} : {bestScore}";
            }
        }
    }

    public void GameOver()
    {
        m_GameOver = true;
        GameOverText.SetActive(true);
    }
}