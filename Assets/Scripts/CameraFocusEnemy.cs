using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFocusEnemy : MonoBehaviour
{
    public GameObject enemyFocus;
    public GameObject cameraObject;
    public PlayerMovement player;
    private CameraFollow camFol;
    private bool doOnce = false;

    private void Start()
    {
        player = FindObjectOfType<PlayerMovement>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            if (!doOnce) StartCoroutine(panBack());
        }
    }

    IEnumerator panBack()
    {
        doOnce = true;
        camFol = cameraObject.GetComponent<CameraFollow>();
        camFol.cameraMoveSpeed = 10;
        camFol.CameraFollowObject = enemyFocus;
        player.isCutScene = true;

        yield return new WaitForSecondsRealtime(3);

        camFol.CameraFollowObject = player.gameObject;
        player.isCutScene = false;
    }
}
