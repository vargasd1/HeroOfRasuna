using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This script is used to make the camera zoom in and out right before the boss fight
/// 
/// ATTATCHED TO: BossCameraZoom (ThirdFloor)
/// </summary>


public class BossCameraZoom : MonoBehaviour
{
    private bool moveCamera = false;
    private GameObject player;

    private void Start()
    {
        // Get the Player
        player = FindObjectOfType<PlayerMovement>().gameObject;
    }

    private void Update()
    {
        // If the camera isn't turned off map the camera's zoom (orthographicSize property) to the player's position in the X
        if (moveCamera && Camera.main != null)
        {
            Camera.main.orthographicSize = AnimMath.Map(player.gameObject.transform.position.x, 22.7f, 32.1f, 8, 12);
        }
    }


    private void OnTriggerEnter(Collider other)
    {
        // If the player enters the trigger box, start to zoom camera
        if (other.gameObject.tag == "Player")
        {
            moveCamera = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // If the player exits the trigger box, check which side the player is on, and stop zooming the camera
        if (other.gameObject.tag == "Player")
        {
            if (other.gameObject.transform.position.x < 30)
            {
                moveCamera = false;
                Camera.main.orthographicSize = 8.0f;
            }
            else
            {
                moveCamera = false;
                Camera.main.orthographicSize = 12.0f;
            }
        }
    }
}
