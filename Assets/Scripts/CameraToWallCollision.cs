using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraToWallCollision : MonoBehaviour
{
    private GameObject playerObject;
    private Outline playerOutline;

    private void Start()
    {
        playerObject = FindObjectOfType<PlayerMovement>().gameObject;
        playerOutline = playerObject.GetComponentInChildren<Outline>();
    }

    private void OnTriggerStay(Collider other)
    {
        // If the object collided with is a wall start to fade it out and turn on player outline
        if (other.gameObject.tag == "Wall")
        {
            playerOutline.enabled = true;
            other.gameObject.GetComponent<WallFade>().SetMaterialTransparent();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // If the object that is no longer collided with is a wall disable the player's outine and fade wall back in
        WallFade walls = other.gameObject.GetComponent<WallFade>();
        if (other.gameObject.tag == "Wall")
        {
            playerOutline.enabled = false;
            walls.fadeOut = false;
            walls.fadeIn = true;
        }
    }
}
