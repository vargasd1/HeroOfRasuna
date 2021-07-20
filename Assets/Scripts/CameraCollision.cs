using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraCollision : MonoBehaviour
{
    private GameObject playerObject;
    private Outline playerOutline;

    private void Start()
    {
        playerObject = FindObjectOfType<CharacterMovementIsometric>().gameObject;
        playerOutline = playerObject.GetComponentInChildren<Outline>();
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag == "Wall")
        {
            playerOutline.enabled = true;
            other.gameObject.GetComponent<WallFade>().SetMaterialTransparent();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        WallFade walls = other.gameObject.GetComponent<WallFade>();
        if (other.gameObject.tag == "Wall")
        {
            playerOutline.enabled = false;
            walls.fadeOut = false;
            walls.fadeIn = true;
        }
    }
}
