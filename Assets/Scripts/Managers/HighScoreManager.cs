using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HighScoreManager : MonoBehaviour
{
    public static HighScoreManager Instance;

    public GameObject scorePanel;
    public GameObject gameOverPanel;
    public TextMeshProUGUI submitText;
    public GameObject inputFieldText;
    public float templateHeight = 50f;

    private Transform entryContainer;
    private Transform entryTemplate;
    private List<Transform> highscoreEntryTransformList;
    private bool loaded = false;
    private bool submitted = false;
    private bool skipColorClear = false;

    [HideInInspector]public int highestScore;
    public bool resetLeaderboard = false;

    private PlayerStat pS;
    private UIManager ui;
    private GameSettings settings;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        pS = PlayerStat.Instance;
        ui = UIManager.Instance;
        settings = GameSettings.Instance;
        entryContainer = gameOverPanel.transform.Find("highscoreEntryContainer");
        entryTemplate = entryContainer.Find("highscoreEntryTemplate");
        entryTemplate.gameObject.SetActive(false);

        //try
        //{
        //    string jsonString = PlayerPrefs.GetString("highscoreTable");
        //    Highscores highscores = JsonUtility.FromJson<Highscores>(jsonString);
        //}
        //catch
        //{
        //    ResetLeaderboard();
        //    Debug.Log("Highscore was missing, leaderboard reset");
        //}

        string jsonString = PlayerPrefs.GetString("highscoreTable");
        Highscores highscores = JsonUtility.FromJson<Highscores>(jsonString);

        if (highscores == null)
        {
            ResetLeaderboard();
        }


        if (resetLeaderboard)
        {
            ResetLeaderboard();
        }
        if (!ui.isMainMenu)   pS.highestScore = ListLeaderBoardElements()[0].score;
    }

    private void Update()
    {
        if (gameOverPanel.activeInHierarchy)
        {
            if (loaded)
            {
                return;
            }
            if (!skipColorClear)
            {
                ClearColors();
            }
            MakeLeaderboardElements();
            loaded = true;
        }
    }

    private void ClearColors()
    {
        string jsonString = PlayerPrefs.GetString("highscoreTable");
        Highscores highscores = JsonUtility.FromJson<Highscores>(jsonString);

        for (int i = 0; i < highscores.highscoreEntryList.Count; i++)
        {
            highscores.highscoreEntryList[i].activePlayer = false;
        }

        string json = JsonUtility.ToJson(highscores);
        PlayerPrefs.SetString("highscoreTable", json);
        PlayerPrefs.Save();
    }

    public int PredictPlacement(int activePlayerScore)
    {
        string jsonString = PlayerPrefs.GetString("highscoreTable");
        Highscores highscores = JsonUtility.FromJson<Highscores>(jsonString);

        HighscoreEntry tmp = new HighscoreEntry { name = "TMP", score = activePlayerScore, activePlayer = true };
        highscores.highscoreEntryList.Add(tmp);

        for (int i = 0; i < highscores.highscoreEntryList.Count; i++)
        {
            for (int j = i + 1; j < highscores.highscoreEntryList.Count; j++)
            {
                if (highscores.highscoreEntryList[j].score > highscores.highscoreEntryList[i].score)
                {
                    HighscoreEntry tmpEntry = highscores.highscoreEntryList[i];
                    highscores.highscoreEntryList[i] = highscores.highscoreEntryList[j];
                    highscores.highscoreEntryList[j] = tmpEntry;
                }
            }
        }
        int rank = 0;

        for (int i = 0; i < highscores.highscoreEntryList.Count; i++)
        {
            if (highscores.highscoreEntryList[i].activePlayer)
            {
                rank = i + 1;
            }
        }

        if (submitted)
        {
            rank -= 1;
        }

        return rank;
    }

    /// <summary>
    /// First clear the previous elements. Then, makes the leaderboard elements and shows top 10.
    /// </summary>
    private void MakeLeaderboardElements()
    {
        //Clears the leaderboard before every load so it doesn't look weird when new score is submitted
        for (int i = 1; i < entryContainer.childCount; i++)
        {
            Destroy(entryContainer.GetChild(i).gameObject);
        }
        //Puts the elements from the list on the leaderboard screen
        highscoreEntryTransformList = new List<Transform>();
        int tmpInt = ListLeaderBoardElements().Count;
        if (tmpInt > 10)
        {
            tmpInt = 10;
        }

        for (int i = 0; i < tmpInt; i++)
        {
            CreateHighscoreEntryTransform(ListLeaderBoardElements()[i], entryContainer, highscoreEntryTransformList, ListLeaderBoardElements()[i].activePlayer);
        }
    }

    /// <summary>
    /// Lists and sorts top scores in the game
    /// </summary>
    public List<HighscoreEntry> ListLeaderBoardElements()
    {
        string jsonString = PlayerPrefs.GetString("highscoreTable");
        Highscores highscores = JsonUtility.FromJson<Highscores>(jsonString);

        // Sorting the list by score
        for (int i = 0; i < highscores.highscoreEntryList.Count; i++)
        {
            for (int j = i + 1; j < highscores.highscoreEntryList.Count; j++)
            {

                if (highscores.highscoreEntryList[j].score > highscores.highscoreEntryList[i].score)
                {
                    HighscoreEntry tmp = highscores.highscoreEntryList[i];
                    highscores.highscoreEntryList[i] = highscores.highscoreEntryList[j];
                    highscores.highscoreEntryList[j] = tmp;
                }
            }
        }
        return highscores.highscoreEntryList;
    }

    private void CreateHighscoreEntryTransform(HighscoreEntry highscoreEntry, Transform container, List<Transform> transformList, bool activePlayer)
    {
        Transform entryTransform = Instantiate(entryTemplate, container);
        RectTransform entryRectTransform = entryTransform.GetComponent<RectTransform>();
        entryRectTransform.anchoredPosition = new Vector2(0, -templateHeight * transformList.Count);
        entryTransform.gameObject.SetActive(true);

        int rank = transformList.Count + 1;

        string rankString;
        switch (rank)
        {
            default: rankString = rank + "TH"; break;
            case 1: rankString = "1ST"; break;
            case 2: rankString = "2ND"; break;
            case 3: rankString = "3RD"; break;
        }
        entryTransform.Find("posText").GetComponent<TextMeshProUGUI>().text = rankString;

        int score = highscoreEntry.score;
        entryTransform.Find("scoreText").GetComponent<TextMeshProUGUI>().text = score.ToString();

        string entryName = highscoreEntry.name;
        entryTransform.Find("nameText").GetComponent<TextMeshProUGUI>().text = entryName;

        entryTransform.Find("background").gameObject.SetActive(rank % 2 == 1);
        if (activePlayer)
        {
            Image image = entryTransform.Find("background").gameObject.GetComponent<Image>();
            image.gameObject.SetActive(true);
            image.color = new Color(0, 150, 255, 1);
        }

        transformList.Add(entryTransform);
    }

    public void AddingHighscoreEntry(int score, string name, bool activePlayer)
    {
        // Make a new highscore entry
        HighscoreEntry highscoreEntry = new HighscoreEntry { score = score, name = name, activePlayer = activePlayer };

        // Load saved highscores
        string jsonString = PlayerPrefs.GetString("highscoreTable");
        Highscores highscores = JsonUtility.FromJson<Highscores>(jsonString);

        // Add the new entry to highscores
        highscores.highscoreEntryList.Add(highscoreEntry);

        // Save updated highscores
        string json = JsonUtility.ToJson(highscores);
        PlayerPrefs.SetString("highscoreTable", json);
        PlayerPrefs.Save();
    }

    public void SubmitScore()
    {
        if (settings.gameState == GameSettings.GameState.Over && !submitted)
        {
            //Setting every other score to be non-active
            for (int i = 0; i < ListLeaderBoardElements().Count; i++)
            {
                ListLeaderBoardElements()[i].activePlayer = false;
            }

            string tmpName = "PLAYER";
            if (inputFieldText.GetComponent<TMP_InputField>().text != "")
            {
                tmpName = inputFieldText.GetComponent<TMP_InputField>().text;
            }

            submitted = true;
            //submitText.text = "Saved!";
            submitText.transform.parent.GetComponent<Image>().SetNativeSize();
            submitText.transform.parent.GetComponent<Button>().interactable = false;
            skipColorClear = true;
            AddingHighscoreEntry(pS.score, tmpName, true);
            loaded = false;
        }
    }
    private void ResetLeaderboard()
    {
        PlayerPrefs.DeleteKey("highscoreTable");
        HighscoreEntry tmp = new HighscoreEntry { name = "RESET", score = 0, activePlayer = false };
        List<HighscoreEntry> highscoreEntries = new List<HighscoreEntry>();
        highscoreEntries.Add(tmp);

        Highscores highscores = new Highscores { highscoreEntryList = highscoreEntries };
        string jsonString = JsonUtility.ToJson(highscores);

        PlayerPrefs.SetString("highscoreTable", jsonString);
        PlayerPrefs.Save();
        resetLeaderboard = false;
    }

    /// <summary>
    /// Single highscore entry
    /// </summary>
    ///
    [System.Serializable]
    public class HighscoreEntry
    {
        public int score;
        public string name;
        public bool activePlayer;
    }
    private class Highscores
    {
        public List<HighscoreEntry> highscoreEntryList;
    }
}