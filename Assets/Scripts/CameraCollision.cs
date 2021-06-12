using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraCollision : MonoBehaviour
{
    public GameObject PlayerObject;

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag == "Wall")
        {
            PlayerObject.GetComponentInChildren<Outline>().enabled = true;
            other.gameObject.GetComponent<WallFade>().SetMaterialTransparent();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        PlayerObject.GetComponentInChildren<Outline>().enabled = false;
        other.gameObject.GetComponent<WallFade>().fadeOut = false;
        other.gameObject.GetComponent<WallFade>().fadeIn = true;
    }
}
