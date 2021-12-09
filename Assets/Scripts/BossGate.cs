using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// This script is used to play the cutscene once all enemies on the third floor are defeated.
/// It opens the door and has the code to slam the door behind the character and spawn the dust.
/// 
/// ATTATCHED TO: HoR_Gate (ThirdFloor)
/// </summary>

public class BossGate : MonoBehaviour
{
    // Start Cutscene Vars
    public int enemiesKilled = 0;
    private bool startOnce = true;

    // Moving Door Vars
    public bool openDoor = false;
    private bool openOnce = true;
    public bool closeDoor = false;
    public GameObject dust;
    private bool doOnce = true;
    public bool stopDoor = false;

    // Player Vars
    private GameObject player;
    private PlayerManager playerManager;
    private PlayerMovement playerMove;

    // Camera Vars
    public Camera camMain;
    public Camera camDoor;

    // UI Vars
    public Image UICover;
    public GameObject mainUI;
    private bool fadeIn = false;
    private bool fadeOut = false;
    private float alpha = 0;

    private void Start()
    {
        player = FindObjectOfType<PlayerManager>().gameObject;
        playerManager = player.GetComponent<PlayerManager>();
        playerMove = player.GetComponent<PlayerMovement>();
    }

    // Update is called once per frame
    void Update()
    {
        if (enemiesKilled >= 6 && !closeDoor && startOnce)
        {
            playerMove.isCutScene = true;

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

            StartCoroutine(StartBossCutscene());
            startOnce = false;
        }


        // Open the door, and only open it once
        if (openDoor && openOnce && !stopDoor)
        {
            float step = 1.5f * Time.unscaledDeltaTime;
            transform.position = Vector3.MoveTowards(transform.position, new Vector3(31.74f, 5, 0), step);
            if (Vector3.Distance(transform.position, new Vector3(31.74f, 5, 0)) < .05f)
            {
                transform.position = new Vector3(31.74f, 5, 0);
                openDoor = false;
                openOnce = false;
            }
        }

        // Stops the door in it's place for when it shakes
        if (stopDoor)
        {
            transform.position = transform.position;
        }

        // Slams door behind and spawns dust particles once it's close to it's location
        if (closeDoor)
        {
            openDoor = false;
            float step = 18 * Time.unscaledDeltaTime;
            transform.position = Vector3.MoveTowards(transform.position, new Vector3(31.74f, 0, 0), step);
            if (Vector3.Distance(transform.position, new Vector3(31.74f, 0, 0)) < .05f)
            {
                transform.position = new Vector3(31.74f, 0, 0);
                if (doOnce)
                {
                    Instantiate(dust, new Vector3(31.74f, 0, 0), Quaternion.Euler(-90, 90, 0), null);
                    FindObjectOfType<AudioManager>().PlayUninterrupted("DoorSlam");
                    doOnce = false;
                }
                closeDoor = false;
            }
        }

        // Fade in the UI
        if (fadeIn)
        {
            if (alpha <= 0.5f) alpha -= Time.unscaledDeltaTime;
            else alpha -= Time.unscaledDeltaTime * 0.75f;

            if (alpha <= 0) fadeIn = false;

            UICover.color = new Color(0, 0, 0, alpha);
        }

        // Fade out the UI
        if (fadeOut)
        {
            alpha += Time.unscaledDeltaTime;
            UICover.color = new Color(0, 0, 0, alpha);
        }
    }

    private IEnumerator StartBossCutscene()
    {
        // Hide player HUD
        mainUI.SetActive(false);

        yield return new WaitForSecondsRealtime(2);

        // Fade screen out
        fadeOut = true;
        fadeIn = false;

        yield return new WaitForSecondsRealtime(1);

        // Flip door for texture purpose, switch cameras, and fade in
        transform.rotation = Quaternion.Euler(0, 90, 0);
        camMain.gameObject.SetActive(false);
        camDoor.gameObject.SetActive(true);

        fadeOut = false;
        fadeIn = true;

        yield return new WaitForSecondsRealtime(1);

        // Open door
        openDoor = true;
        fadeIn = false;

        yield return new WaitForSecondsRealtime(3);

        // Fade out
        fadeOut = true;

        yield return new WaitForSecondsRealtime(1);

        // Flip door back around, switch cameras back, and fade in
        transform.rotation = Quaternion.Euler(0, -90, 0);
        camDoor.gameObject.SetActive(false);
        camMain.gameObject.SetActive(true);

        fadeOut = false;
        fadeIn = true;

        yield return new WaitForSecondsRealtime(2);

        // Let player move and turn on main HUD
        mainUI.SetActive(true);
        playerMove.isCutScene = false;

        yield break;
    }
}
