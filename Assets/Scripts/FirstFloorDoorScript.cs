using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
    //private AudioManager audioScript;


    // Start is called before the first frame update
    void Awake()
    {
        player = FindObjectOfType<PlayerManager>().gameObject;
        playerManager = player.GetComponent<PlayerManager>();
        playerMove = player.GetComponent<PlayerMovement>();
        //audioScript = FindObjectOfType<AudioManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (playerManager.enemiesKilled >= 3 && !doOnce)
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

            StartCoroutine(doorCutScene());
            doOnce = true;
        }

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

        if (openDoor)
        {
            float step = 1 * Time.fixedUnscaledDeltaTime;// Time.deltaTime;
            transform.position = Vector3.MoveTowards(transform.position, new Vector3(-11.42f, 5, -17.53f), step);
        }
    }

    IEnumerator doorCutScene()
    {
        mainUI.SetActive(false);

        yield return new WaitForSecondsRealtime(2);

        fadeOut = true;
        fadeIn = false;

        yield return new WaitForSecondsRealtime(1);

        camMain.gameObject.SetActive(false);
        camDoor.gameObject.SetActive(true);

        fadeOut = false;
        fadeIn = true;

        yield return new WaitForSecondsRealtime(1);

        openDoor = true;
        fadeIn = false;

        yield return new WaitForSecondsRealtime(3);

        fadeOut = true;

        yield return new WaitForSecondsRealtime(1);

        camDoor.gameObject.SetActive(false);
        camMain.gameObject.SetActive(true);

        fadeOut = false;
        fadeIn = true;

        yield return new WaitForSecondsRealtime(2);

        mainUI.SetActive(true);
        playerMove.isCutScene = false;
    }
}
