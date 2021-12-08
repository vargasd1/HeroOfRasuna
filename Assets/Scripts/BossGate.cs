using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BossGate : MonoBehaviour
{

    public int enemiesKilled = 0;
    public bool openDoor = false;
    private bool openOnce = true;
    public bool closeDoor = false;
    public GameObject dust;
    private bool doOnce = true;
    public bool stopDoor = false;

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
    private bool startOnce = true;


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

            playerMove.overclock = false;
            playerMove.overclockTransition = false;
            playerMove.overclockTransitionTime = 2f;
            Time.timeScale = 1f;
            Time.fixedDeltaTime = Time.timeScale * .02f;
            playerMove.overclockTime = 5;
            playerMove.overclockChargedAmt = 0f;
            AudioManager aud = FindObjectOfType<AudioManager>();
            aud.Stop("Overclock");
            aud.ChangePitch("Overclock", 1f);
            aud.ResetSounds();

            StartCoroutine(StartBossCutscene());
            startOnce = false;
        }

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

        if (stopDoor)
        {
            transform.position = transform.position;
        }

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
    }

    private IEnumerator StartBossCutscene()
    {
        mainUI.SetActive(false);

        yield return new WaitForSecondsRealtime(2);

        fadeOut = true;
        fadeIn = false;

        yield return new WaitForSecondsRealtime(1);

        transform.rotation = Quaternion.Euler(0, 90, 0);
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

        transform.rotation = Quaternion.Euler(0, -90, 0);
        camDoor.gameObject.SetActive(false);
        camMain.gameObject.SetActive(true);

        fadeOut = false;
        fadeIn = true;

        yield return new WaitForSecondsRealtime(2);

        mainUI.SetActive(true);
        playerMove.isCutScene = false;

        yield break;
    }
}
