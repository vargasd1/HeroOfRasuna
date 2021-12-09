using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraToWallCollision : MonoBehaviour
{
    private GameObject playerObject;
    private PlayerMovement playerMove;
    private Outline playerOutline;

    private void Start()
    {
        playerObject = FindObjectOfType<PlayerMovement>().gameObject;
        playerOutline = playerObject.GetComponentInChildren<Outline>();
        playerMove = playerObject.GetComponent<PlayerMovement>();
    }

    private void OnTriggerStay(Collider other)
    {
        // If the object collided with is a wall start to fade it out and turn on player outline
        if (other.gameObject.tag == "Wall" && !playerMove.isCutScene && !playerMove.isCutSceneMoving)
        {
            WallFade wall = other.gameObject.GetComponent<WallFade>();

            playerOutline.enabled = true;

            wall.SetMaterialTransparent();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // If the object that is no longer collided with is a wall disable the player's outine and fade wall back in
        WallFade wall = other.gameObject.GetComponent<WallFade>();

        if (other.gameObject.tag == "Wall")
        {
            playerOutline.enabled = false;

            wall.fadeOut = false;
            wall.fadeIn = true;
        }
    }
}
