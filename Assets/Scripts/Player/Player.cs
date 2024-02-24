using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    private BoxCollider2D col;
    private PlayerStat pS;
    private GameSettings settings;

    public GameObject gameCamera;

    //public enum GameState { Playing, Over }
    //public GameState gameState;

    public enum LegConfiguration { Normal, Tiny }
    public LegConfiguration legConfiguration;

    public Sprite normalSprite, normalWithBranch;
    public Sprite tinyLegSprite, tinyWithBranch;

    private Vector2 originalCol;
    private int score = 0;
    private int prevScore = 0;

    // Start is called before the first frame update
    void Start()
    {
        col = gameObject.GetComponent<BoxCollider2D>();
        pS = PlayerStat.Instance;
        settings = GameSettings.Instance;
        originalCol = col.size;
        settings.gameState = GameSettings.GameState.Playing;
    }

    private void Update()
    {
        score = Mathf.RoundToInt(Camera.main.transform.position.y * settings.scoreMultiplier);

        if (score > prevScore)
        {
            pS.score = score;
            prevScore = score;
        }
        else
        {
            pS.score = prevScore;
        }
        pS.legConfiguration = legConfiguration;

        if (settings.gameState == GameSettings.GameState.Over)
        {
            StartCoroutine(Spin());        }

        if (pS.biggerJumps)
        {
            if (legConfiguration == LegConfiguration.Normal)
            {
                gameObject.GetComponent<SpriteRenderer>().sprite = normalWithBranch;
            }
            else if (legConfiguration == LegConfiguration.Tiny)
            {
                gameObject.GetComponent<SpriteRenderer>().sprite = tinyWithBranch;
            }
        }
        else if (!pS.biggerJumps)
        {
            if (legConfiguration == LegConfiguration.Normal)
            {
                gameObject.GetComponent<SpriteRenderer>().sprite = normalSprite;
            }
            else if (legConfiguration == LegConfiguration.Tiny)
            {
                gameObject.GetComponent<SpriteRenderer>().sprite = tinyLegSprite;
            }
        }

        if ((Input.GetKeyDown(pS.legSwapButton) || Input.GetKeyDown(pS.legSwapAltButton)) && legConfiguration == LegConfiguration.Normal)
        {
            legConfiguration = LegConfiguration.Tiny;
            col.size = new Vector2(col.size.x / 2, col.size.y);
        }
        else if ((Input.GetKeyUp(pS.legSwapButton) || Input.GetKeyUp(pS.legSwapAltButton)) && legConfiguration == LegConfiguration.Tiny)
        {
            legConfiguration = LegConfiguration.Normal;
            col.size = originalCol;
        }
    }

    IEnumerator Spin()
    {
        yield return new WaitForSeconds(0.1f);

            Rigidbody2D rb = gameObject.GetComponent<Rigidbody2D>();

            var rotationVector = transform.rotation.eulerAngles;
            rotationVector.z += 5;
            transform.rotation = Quaternion.Euler(rotationVector);
            rb.freezeRotation = true;
            rb.gravityScale = 0;
            rb.velocity = Vector2.zero;
            gameObject.GetComponent<Collider2D>().enabled = false;
        
    }
}
