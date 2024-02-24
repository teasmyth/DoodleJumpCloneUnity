using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialManager : MonoBehaviour
{
    public int disableFirstTutorialScore;
    public GameObject tutorialPanel;
    private GameSettings settings;
    private PlayerStat pS;

    // Start is called before the first frame update
    void Start()
    {
        pS = PlayerStat.Instance;
        settings = GameSettings.Instance;
    }

    // Update is called once per frame
    void Update()
    {
        if (HighScoreManager.Instance.gameOverPanel.transform.parent.gameObject.activeInHierarchy || settings.disableTutorial)
        {
            tutorialPanel.SetActive(false);
        }

        if (pS.score > disableFirstTutorialScore)
        {
            tutorialPanel.SetActive(false);
        }
    }
}
