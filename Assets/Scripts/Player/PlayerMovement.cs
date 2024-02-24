using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private Rigidbody2D rb;
    private Player player;
    private GameSettings settings;
    private PlayerStat pS;
    private BoxCollider2D col;
    private SpriteRenderer spriteRenderer;
    private AudioManager audio;

    public LayerMask platformLayer;

    private float movementInput;
    private float acceleration;
    private float movementChange;
    private float jumpForce;
    private float bigJumpTimer;

    [SerializeField] private float currentSpeed;

    // Start is called before the first frame update
    void Start()
    {
        audio = AudioManager.Instance;
        rb = this.GetComponent<Rigidbody2D>();
        player = gameObject.GetComponent<Player>();
        pS = PlayerStat.Instance;
        col = gameObject.GetComponent<BoxCollider2D>();
        settings = GameSettings.Instance;
        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        rb.mass = pS.gravity;

        jumpForce = pS.jumpForce;
    }

    // Update is called once per frame
    void Update()
    {
        movementInput = Input.GetAxisRaw("Horizontal");
        Flip();

        if (pS.acceleration > 0)
        {
            if (movementInput != movementChange)
            {
                acceleration = pS.accelerationMinimum;
            }
            acceleration += Time.fixedDeltaTime * pS.acceleration;
            if (acceleration >= 1)
            {
                acceleration = 1;
            }
            movementChange = movementInput;
            movementInput *= acceleration;
        }

        if (settings.gameState != GameSettings.GameState.Over)
        {
            if (!pS.autoJump && IsGrounded() && Input.GetKeyDown(pS.jumpButton))
            {
                Vector2 velocity = rb.velocity;
                velocity.y = pS.jumpForce;
                if (transform.rotation.z != 0)
                {
                    transform.rotation = Quaternion.Euler(0, 0, 0);
                }
                audio.Play("Jump");
                rb.velocity = Vector2.up * jumpForce;
            }
            if (pS.flying && rb.velocity.y <= 0)
            {
                pS.flying = false;
            }
            if (!IsGrounded())
            {
                transform.rotation = Quaternion.Euler(0, 0, 0);
            }
            pS.isGrounded = IsGrounded();
            if (pS.biggerJumps)
            {
                //StartCoroutine(TemporaryBiggerJumpsCO());
                bigJumpTimer -= Time.deltaTime;
                if (bigJumpTimer <= 0)
                {
                    jumpForce = pS.jumpForce;
                    pS.biggerJumps = false;
                }
            }

        }


        transform.position += new Vector3(movementInput * pS.movementSpeed * Time.deltaTime, 0);

    }

    private void FixedUpdate()
    {
        //Left & Right Movement
        //rb.velocity = new Vector2(movementInput * pS.movementSpeed, rb.velocity.y);
        //currentSpeed = rb.velocity.x;
    }

    private IEnumerator DisableAndEnableCollider(GameObject platform, float duration)
    {
        platform.GetComponent<Collider2D>().enabled = false;
        yield return new WaitForSeconds(duration);
        platform.GetComponent<Collider2D>().enabled = true;
    }

    public void TemporaryBiggerJump()
    {
        bigJumpTimer = settings.temporaryJumpBoostDuration;
        jumpForce = settings.temporaryJumpBoostAmount;
        pS.biggerJumps = true;
    }

    public IEnumerator TemporaryBiggerJumpsCO()
    {
        jumpForce = settings.temporaryJumpBoostAmount;
        yield return new WaitForSeconds(settings.temporaryJumpBoostDuration);
        jumpForce = pS.jumpForce;
        pS.biggerJumps = false;
    }

    public bool IsGrounded()
    {
        // Checking ground from both edges and the middle of the collider
        Vector3 groundCheckRootMin = new Vector3(col.bounds.min.x, col.bounds.center.y - col.bounds.size.y / 2 + 0.1f, col.bounds.center.z);
        Vector3 groundCheckRootCenter = new Vector3(col.bounds.center.x, col.bounds.center.y - col.bounds.size.y / 2 + 0.1f, col.bounds.center.z);
        Vector3 groundCheckRootMax = new Vector3(col.bounds.max.x, col.bounds.center.y - col.bounds.size.y / 2 + 0.1f, col.bounds.center.z);

        if (pS.visualizeGroundCheck)
        {
            Debug.DrawRay(groundCheckRootMin, -transform.up * pS.groundCheckDistance);
            Debug.DrawRay(groundCheckRootCenter, -transform.up * pS.groundCheckDistance);
            Debug.DrawRay(groundCheckRootMax, -transform.up * pS.groundCheckDistance);
        }

        RaycastHit2D hitMin = Physics2D.Raycast(groundCheckRootMin, -transform.up, pS.groundCheckDistance, platformLayer);
        RaycastHit2D hitCenter = Physics2D.Raycast(groundCheckRootCenter, -transform.up, pS.groundCheckDistance, platformLayer);
        RaycastHit2D hitMax = Physics2D.Raycast(groundCheckRootMax, -transform.up, pS.groundCheckDistance, platformLayer);

        if (hitMin.collider != null)
        {
            return hitMin.collider.CompareTag("Platform");
        }
        else if (hitCenter.collider != null)
        {
            return hitCenter.collider.CompareTag("Platform");
        }
        else if (hitMax.collider != null)
        {
            return hitMax.collider.CompareTag("Platform");
        }
        else
        {
            return false;
        }
    }

    private void Flip()
    {
        if (movementInput > 0)
        {
            if (pS.spriteLooksRight)
            {
                spriteRenderer.flipX = false;
            }
            else if (!pS.spriteLooksRight)
            {
                spriteRenderer.flipX = true;
            }
        }
        if (movementInput < 0)
        {
            if (pS.spriteLooksRight)
            {
                spriteRenderer.flipX = true;
            }
            else if (!pS.spriteLooksRight)
            {
                spriteRenderer.flipX = false;
            }
        }

    }
}
