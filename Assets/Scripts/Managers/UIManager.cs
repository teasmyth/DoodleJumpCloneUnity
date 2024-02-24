using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class UIManager : MonoBehaviour
{
    public TextMeshProUGUI score, highestScore, yourScoreIsText, placement;
    private PlayerStat pS;
    private GameSettings settings;

    public GameObject gameOverPanel;
    public GameObject gameUIPanel;
    public GameObject mainMenuPanel;
    public bool isMainMenu;

    private bool gameEnded = false;
    public static UIManager Instance;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        pS = PlayerStat.Instance;
        settings = GameSettings.Instance;
    }

    // Update is called once per frame
    void Update()
    {
        if (settings.reloadGame)
        {
            ReloadScene();
        }
        if (isMainMenu)
        {
            return;
        }
        score.text = pS.score.ToString();
        highestScore.text = "Highscore " + pS.highestScore;

        if (gameOverPanel.activeInHierarchy && !gameEnded)
        {
            yourScoreIsText.text = "Your score is: " + pS.score;
            placement.text = "Your placement: " + HighScoreManager.Instance.PredictPlacement(pS.score);
            gameEnded = true;
        }

        if (settings.gameState == GameSettings.GameState.Over)
        {
            //Time.timeScale = 0.1f;
            gameUIPanel.SetActive(false);
            gameOverPanel.SetActive(true);
        }
    }

    public void Replay()
    {
        SceneManager.LoadScene(settings.gameSceneName);
    }

    public void ToMainMenu()
    {
        SceneManager.LoadScene(settings.mainMenuSceneName);
    }

    public void Play()
    {
        SceneManager.LoadScene(settings.gameSceneName);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void ReloadScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void CloseLeaderboard()
    {
        gameOverPanel.SetActive(false);
        mainMenuPanel.SetActive(true);
    }

    public void OpenLeaderboard()
    {
        mainMenuPanel.SetActive(false);
        gameOverPanel.SetActive(true);
    }
}
