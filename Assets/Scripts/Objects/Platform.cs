using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Platform : MonoBehaviour
{
    public bool tiny;
    public bool crashing;

    public GameObject root, crystal;

    //Crashing platform sprites
    public Sprite defaultSprite, smallDamagedSprite, bigDamageSprite, almostDamagedSprite, damagedSprite;

    private PlayerStat pS;
    private GameSettings settings;
    private SpriteRenderer spriteRenderer;
    private float timer;
    private float crashingTimer;
    private bool startTimer = false;
    private bool playerStanding = false;
    private AudioManager audio;

    private Collider2D col;
    private Rigidbody2D rb;

    private void Start()
    {
        pS = PlayerStat.Instance;
        settings = GameSettings.Instance;
        col = gameObject.GetComponent<Collider2D>();
        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        defaultSprite = spriteRenderer.sprite;
        audio = AudioManager.Instance;

        crashingTimer = settings.crashingPlatformTimer;
        timer = crashingTimer;
    }

    private void Update()
    {
        TinyDisabler();

        if (crashing)
        {
            ChangeSprite();
        }

        switch (settings.crashingPlaformBehaviour)
        {
            case GameSettings.CrashingPlaformBehaviour.TimerStartsOnce:
                if (startTimer)
                {
                    timer -= Time.unscaledDeltaTime;
                }
                break;
            case GameSettings.CrashingPlaformBehaviour.TimerContOnStand:
                if (startTimer && playerStanding)
                {
                    timer -= Time.unscaledDeltaTime;
                }
                break;
            case GameSettings.CrashingPlaformBehaviour.TimerResets:
                if (startTimer && playerStanding)
                {
                    timer -= Time.unscaledDeltaTime;
                }
                else if (!playerStanding)
                {
                    timer = crashingTimer;
                }
                break;
        }

        if (timer <= 0)
        {
            Destroy(gameObject);
        }

        if (rb != null)
        {

            PlatformDrag(rb, pS.slopedPlatformDrag);
        }
    }

    private void LateUpdate()
    {
        if (settings.destroyObjectsBelowCamera != 0 && transform.position.y < Camera.main.transform.position.y - settings.destroyObjectsBelowCamera)
        {
            Destroy(gameObject);
        }

        if (settings.gameState == GameSettings.GameState.Over && transform.position.y < Camera.main.transform.position.y + 20 - settings.destroyObjectsBelowCamera)
        {
            Destroy(gameObject);
        }
    }

    public void ChangeSprite()
    {
        if (defaultSprite != null && smallDamagedSprite != null && bigDamageSprite != null && almostDamagedSprite != null && damagedSprite != null)
        {
            if (timer <= crashingTimer * 4 / 5 && timer > crashingTimer * 3 / 5)
            {
                spriteRenderer.sprite = smallDamagedSprite;
            }
            else if (timer <= crashingTimer * 3 / 5 && timer > crashingTimer * 2 / 5)
            {
                spriteRenderer.sprite = bigDamageSprite;
                audio.Play("Nervous");
            }
            else if (timer <= crashingTimer * 2 / 5 && timer > crashingTimer * 1 / 5)
            {
                spriteRenderer.sprite = bigDamageSprite;
            }
            if (timer <= crashingTimer * 1 / 5)
            {
                spriteRenderer.sprite = almostDamagedSprite;
            }
        }
        else
        {
            Debug.Log("Crashing platform sprites missing, will not change sprites.");
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && collision.relativeVelocity.y <= 0f)
        {
            rb = collision.collider.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                if (tiny && pS.legConfiguration != Player.LegConfiguration.Tiny)
                {
                    return;
                }
                if (pS.autoJump)
                {
                    Vector2 velocity = rb.velocity;
                    velocity.y = pS.jumpForce;
                    rb.velocity = velocity;
                }
                playerStanding = true;
                if (crashing)
                {
                    startTimer = true;
                }
            }
        }
    }

    void TinyDisabler()
    {
        if (pS.legConfiguration != Player.LegConfiguration.Tiny && tiny)
        {
            col.enabled = false;
        }
        else if (pS.legConfiguration == Player.LegConfiguration.Tiny && tiny)
        {
            col.enabled = true;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && playerStanding)
        {
            playerStanding = false;
            if (settings.crashingPlaformBehaviour == GameSettings.CrashingPlaformBehaviour.TimerContOnStand || settings.crashingPlaformBehaviour == GameSettings.CrashingPlaformBehaviour.TimerResets)
            {
                startTimer = false;
            }
            rb = null;
        }
    }

    private void PlatformDrag(Rigidbody2D rb, float amount)
    {
        if (transform.rotation.z != 0)
        {
            float direction = Mathf.Sign(transform.rotation.z);
            float slope = transform.rotation.z;
            Vector2 forceToAdd = new Vector2();
            if (direction > 0)
            {
                forceToAdd = -transform.right * (slope / amount) * pS.movementSpeed;
                rb.gameObject.transform.position += (Vector3)forceToAdd;
            }
            else if (direction < 0)
            {
                forceToAdd = transform.right * (slope / amount) * pS.movementSpeed;
                rb.gameObject.transform.position -= (Vector3)forceToAdd;
            }
            Debug.Log(direction);
        }

    }
}