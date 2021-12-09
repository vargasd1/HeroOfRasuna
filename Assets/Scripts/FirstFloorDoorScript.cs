using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// This script opens the door once all the enemies on the first floor have been killed.
/// 
/// ATTATCHED TO: HoR_Stairs_Gate (First Floor)
/// </summary>
public class FirstFloorDoorScript : MonoBehaviour
{
    private GameObject player;
    private PlayerManager playerManager;
    private PlayerMovement playerMove;
    public Camera camMain;
    public Camera camDoor;
    public Image UICover;
    public GameObject mainUI;
    private bool fadeIn = false;
    private bool fadeOut = false;
    private float alpha = 0;
    private bool openDoor = false;
    private bool doOnce = false;

    // Start is called before the first frame update
    void Awake()
    {
        player = FindObjectOfType<PlayerManager>().gameObject;
        playerManager = player.GetComponent<PlayerManager>();
        playerMove = player.GetComponent<PlayerMovement>();
    }

    // Update is called once per frame
    void Update()
    {
        if (playerManager.enemiesKilled >= 3 && !doOnce)
        {
            // Stops player from moving
            playerMove.isCutScene = true;

            // Stops overclock if player is using it
            if (playerMove.overclock || playerMove.overclockTransition)
            {
                playerMove.overclockTime = 5f;
                playerMove.overclockTransitionTime = 2f;
                playerMove.overclock = false;
                playerMove.overclockTransition = false;
                Time.timeScale = 1f;
                Time.fixedDeltaTime = Time.timeScale * .02f;
                AudioManager aud = FindObjectOfType<AudioManager>();
                aud.Stop("Overclock");
                aud.ChangePitch("Overclock", 1f);
                aud.ResetSounds();
            }

            StartCoroutine(doorCutScene());
            doOnce = true;
        }

        // Checks to fade in or out the Camera
        if (fadeIn)
        {
            if (alpha <= 0.5f) alpha -= Time.unscaledDeltaTime;
            else alpha -= Time.unscaledDeltaTime * 0.75f;

            if (alpha <= 0) fadeIn = false;

            UICover.color = new Color(0, 0, 0, alpha);
        }

        if (fadeOut)
        {
            alpha += Time.unscaledDeltaTime;
            UICover.color = new Color(0, 0, 0, alpha);
        }

        // Opening the door using MoveTowards
        if (openDoor)
        {
            float step = 1 * Time.fixedUnscaledDeltaTime;
            transform.position = Vector3.MoveTowards(transform.position, new Vector3(-11.42f, 5, -17.53f), step);
        }
    }

    IEnumerator doorCutScene()
    {
        // Hide UI
        mainUI.SetActive(false);

        yield return new WaitForSecondsRealtime(2);

        // Fade out camera
        fadeOut = true;
        fadeIn = false;

        yield return new WaitForSecondsRealtime(1);

        // Swap cameras and fade back in
        camMain.gameObject.SetActive(false);
        camDoor.gameObject.SetActive(true);

        fadeOut = false;
        fadeIn = true;

        yield return new WaitForSecondsRealtime(1);

        // Open door
        openDoor = true;
        fadeIn = false;

        yield return new WaitForSecondsRealtime(3);

        // Fade out camera
        fadeOut = true;

        yield return new WaitForSecondsRealtime(1);

        // Switch cameras back and fade back in
        camDoor.gameObject.SetActive(false);
        camMain.gameObject.SetActive(true);

        fadeOut = false;
        fadeIn = true;

        yield return new WaitForSecondsRealtime(2);

        // Turn UI back on and let player move
        mainUI.SetActive(true);
        playerMove.isCutScene = false;
    }
}
