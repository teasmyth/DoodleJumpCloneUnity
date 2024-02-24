using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStat : MonoBehaviour
{
    public static PlayerStat Instance;
    public GameObject playerGO;
    public HighScoreManager highScoreManager;

    [Space(5)]
    [Header("Buttons")]
    public KeyCode jumpButton;
    public KeyCode legSwapButton;
    public KeyCode legSwapAltButton;

    [Space(5)]
    [Header("Variables")]
    [Space(10)]
    public float movementSpeed;
    [Tooltip("If 0, acceleration is disabled.")]
    public float acceleration;
    public float accelerationMinimum;
    public float jumpForce;
    public float gravity;
    [Tooltip("The bigger the less slippery the platforms will get, increasing the drag.")]
    public float slopedPlatformDrag;
    public bool autoJump;
    public bool spriteLooksRight;

    [Space(5)]
    [Header("Player Debug")]
    [Space(10)]
    public int highestScore;
    public int score;
    public Player.LegConfiguration legConfiguration;
    public bool isGrounded;
    public float groundCheckDistance;
    public bool visualizeGroundCheck;
    public bool biggerJumps;
    public bool flying;

    private void Awake()
    {
        Instance = this;
        if (playerGO == null)
        {
            playerGO = GameObject.FindGameObjectWithTag("Player");
        }
    }

    private void Update()
    {
        if (score > highestScore)
        {
            highestScore = score;
        }
    }
}
