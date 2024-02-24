using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    private Transform target;
    private GameSettings settings;
    private Vector3 currentVelocity;

    // Start is called before the first frame update
    void Start()
    {
        settings = GameSettings.Instance;
        target = GameObject.FindGameObjectWithTag("Player").transform;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        Vector3 newPos = new Vector3(transform.position.x, target.position.y, transform.position.z);
        if (settings.gameState == GameSettings.GameState.Playing && target.position.y > transform.position.y + settings.cameraFollowYAxisAllowance)
        {
            transform.position = newPos;
        }
        else if (settings.gameState == GameSettings.GameState.Over)
        {
            transform.position = newPos;
        }
    }
}
