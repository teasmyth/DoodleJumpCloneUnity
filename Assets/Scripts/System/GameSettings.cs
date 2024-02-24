using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSettings : MonoBehaviour
{
    public static GameSettings Instance;
    [Tooltip("Reloads game instantly and avoids all the menus, for quicker play testing.")]
    public bool instantReplay;
    public float scoreMultiplier;
    [HideInInspector] public bool reloadGame = false;

    [Header("General Settings")]
    public string gameSceneName;
    public string mainMenuSceneName;
    public float edgeDetection;
    public bool disableTutorial = false;
    public enum GameState { Playing, Over }
    public GameState gameState;


    [Header("Powerup Settings")]
    public float jumpBoostForce;
    public float temporaryJumpBoostAmount;
    public float temporaryJumpBoostDuration;

    [Header("Camera Settings")]
    [Space(10)]
    public float cameraFollowYAxisAllowance;
    public float cameraSmoothness;
    public float destroyObjectsBelowCamera;

    [Header("Platform Settings")]
    [Space(10)]
    public float infiniteSpawnAhead;
    public float crashingPlatformTimer;
    public enum CrashingPlaformBehaviour { TimerStartsOnce, TimerContOnStand, TimerResets}
    public CrashingPlaformBehaviour crashingPlaformBehaviour;

    private PlayerStat pS;
    private UIManager ui;

    private void Awake()
    {
        Instance = this;
        Application.targetFrameRate = 60;
    }

    private void Start()
    {
        pS = PlayerStat.Instance;
        ui = UIManager.Instance;
        Time.timeScale = 1f;
    }

    private void Update()
    {
        if (gameState == GameState.Over && instantReplay)
        {
            reloadGame = true;
        }
        else
        {
            reloadGame = false;
        }
        
    }
}
