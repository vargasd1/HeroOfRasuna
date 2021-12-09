using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossCameraZoom : MonoBehaviour
{
    private bool moveCamera = false;
    private GameObject player;

    private void Start()
    {
        player = FindObjectOfType<PlayerMovement>().gameObject;
    }

    private void Update()
    {
        if (moveCamera && Camera.main != null)
        {
            Camera.main.orthographicSize = AnimMath.Map(player.gameObject.transform.position.x, 22.7f, 32.1f, 8, 12);
        }
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            moveCamera = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
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
