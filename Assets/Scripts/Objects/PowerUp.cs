using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUp : MonoBehaviour
{

    private GameSettings settings;
    private PlayerStat pS;
    private AudioManager audio;
    public enum PowerupType { FlyUp, TemporaryBiggerJump  }
    public PowerupType powerupType;

    // Start is called before the first frame update
    void Start()
    {
        settings = GameSettings.Instance;
        pS = PlayerStat.Instance;
        audio = AudioManager.Instance;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !pS.flying)
        {
            if (powerupType == PowerupType.FlyUp)
            {
                audio.Play("Salt");
                collision.GetComponent<Rigidbody2D>().AddForce(new Vector2(0, settings.jumpBoostForce * 100));
                pS.flying = true;
            }
            else if (powerupType == PowerupType.TemporaryBiggerJump)
            {
                audio.Play("Sprout");
                collision.GetComponent<PlayerMovement>().TemporaryBiggerJump();
            }
            Destroy(gameObject);
        }
    }
}