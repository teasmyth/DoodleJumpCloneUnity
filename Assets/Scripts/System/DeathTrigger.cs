using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathTrigger : MonoBehaviour
{
    private GameSettings settings;
    private PlayerStat pS;
    private AudioManager audio;
    private float dist;

    private void Start()
    {
        audio = AudioManager.Instance;
        pS = PlayerStat.Instance;
        dist = Vector2.Distance(pS.playerGO.transform.position, transform.position);
        settings = GameSettings.Instance;
        //transform.position = new Vector3(transform.parent.position.x, transform.position.y, transform.parent.position.z);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            audio.Play("GameOver");
            settings.gameState = GameSettings.GameState.Over;
        }
    }
}
