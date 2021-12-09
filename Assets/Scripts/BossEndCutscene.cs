using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

/// <summary>
/// This script is used to start and update the Boss' outro cutscene.
/// This stops the fight and fades to the boss' transition, which then fades to the dialouge,
/// and then fades to black and switches scene to credits.
/// 
/// ATTATCHED TO: BossFightCutsceneTrigger
/// </summary>

public class BossEndCutscene : MonoBehaviour
{

    // UI Vars
    public Image UICover;
    public GameObject mainUI;
    private bool fadeIn = false;
    private bool fadeOut = false;
    private float alpha = 0;

    // Boss and Player Vars
    private SuravAI boss;
    private PlayerMovement playerMove;
    private bool doOnce = true;

    // Camera Vars
    public Camera bossPlayerCam;
    public Camera bossCam;
    public Camera playerCam;
    private Camera mainCam;

    // NavMesh Var
    public GameObject bossSeatNav;

    // Boss Transition 
    public GameObject transition;
    public GameObject curedBoss;
    private GameObject transitionEffect;

    // Dialogue Vars
    public int dialogueCount = 0;
    private BossDialogue dialogueUI;
    private Animator dialogueAnim;
    public GameObject nextPrompt;
    public bool readyToSkip = false;
    public bool isTalking = false;
    private float textStartDelay = 1f;
    private bool startOnce = true;
    public TMPro.TMP_FontAsset curedFont;
    private float startFadeDelay = 4f;

    // Start is called before the first frame update
    void Start()
    {
        boss = FindObjectOfType<SuravAI>();
        playerMove = FindObjectOfType<PlayerMovement>();
        mainCam = Camera.main;
        dialogueUI = FindObjectOfType<BossDialogue>();
        dialogueAnim = dialogueUI.GetComponentInParent<Animator>();
        nextPrompt.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        // If the boss dies, begin the end cutscene
        if (boss.health <= 0)
        {
            if (doOnce)
            {
                StartCoroutine(StartDefeatCutscene());
                doOnce = false;
            }
        }

        // If the coroutine is finished start the dialogue
        if (isTalking)
        {
            // Small delay before he starts talking to let the text box pop up
            if (textStartDelay <= 0 && startOnce)
            {
                readyToSkip = false;
                dialogueUI.textChanged = true;
                startOnce = false;
            }
            else
            {
                textStartDelay -= Time.unscaledDeltaTime;
            }

            if (readyToSkip)
            {
                // Check if the player is on their last dialogue or not
                if (dialogueCount < 7)
                {
                    nextPrompt.SetActive(true);

                    // If the player clicks when the message is done, move on to next message
                    if (Input.GetKeyDown(KeyCode.Mouse0))
                    {
                        dialogueCount++;
                        dialogueUI.textChanged = true;
                        dialogueUI.txt.text = "";
                        readyToSkip = false;
                        dialogueUI.textSpeed = 0.025f;
                    }
                }
            }
            else
            {
                nextPrompt.SetActive(false);
            }

            // Switch case for what Surav says and when
            switch (dialogueCount)
            {
                case 0:
                    dialogueUI.story = "YOU HAVE MY THANKS, BRAVE ONE.";
                    break;
                case 1:
                    dialogueUI.story = "I WAS SENSITIVE TO MY ACTIONS. ONES THAT I COULD NOT CONTROL.";
                    break;
                case 2:
                    dialogueUI.story = "AT MY WEAKEST, WITH NO FAITH TO DRAW STRENGTH FROM, THE ALGORITHM FINALLY OVERTOOK ME.";
                    break;
                case 3:
                    dialogueUI.story = "YOU'VE SHOWN COURAGE AND CUNNING. NOT MANY CAN RESIST THE CORRUPTION OF THE ALGORITHM.";
                    break;
                case 4:
                    dialogueUI.story = "I SHALL BESTOW A SKILL ON YOU THAT WILL HELP IN THE TRIALS TO COME.";
                    break;
                case 5:
                    dialogueUI.story = "IF YOU WISH TO SUCCEED IN YOUR JOURNEY, YOU'LL NEED TO RESCUE THE REMAINING GUARDIANS.";
                    break;
                case 6:
                    dialogueUI.story = "WE CAN OFFER POWER THAT WILL AID YOU IN YOUR FIGHT.";
                    break;
                case 7:
                    dialogueUI.isFinalLine = true;
                    dialogueUI.story = "EVER ONWARD";
                    dialogueUI.story2 = "(EVER ONWARD!)";
                    break;
            }

            // If the dialogue is all finsihed
            if (dialogueCount >= 7 && readyToSkip)
            {
                // Small delay before the screen fades to black
                if (startFadeDelay <= 0)
                {
                    isTalking = false;
                    fadeOut = true;
                }
                else
                {
                    startFadeDelay -= Time.unscaledDeltaTime;
                }
            }
        }

        // Fades in the UI black screen
        if (fadeIn)
        {
            if (alpha <= 0.5f) alpha -= Time.unscaledDeltaTime;
            else alpha -= Time.unscaledDeltaTime * 0.75f;

            if (alpha <= 0) fadeIn = false;

            UICover.color = new Color(0, 0, 0, alpha);
        }

        // Fades out the UI black screen
        if (fadeOut)
        {
            alpha += Time.unscaledDeltaTime;
            UICover.color = new Color(0, 0, 0, alpha);

            if (dialogueUI.isFinalLine)
            {
                if (alpha >= 1)
                {
                    SceneManager.LoadScene("CreditScene");
                }
            }
        }
    }

    private IEnumerator StartDefeatCutscene()
    {
        // Turn off Character Controller, UI, Boss AI, and NavMeshObstacle near boss' seat
        playerMove.isCutScene = true;
        mainUI.SetActive(false);
        boss.GetComponent<NavMeshAgent>().enabled = false;
        playerMove.gameObject.GetComponent<CharacterController>().enabled = false;
        bossSeatNav.GetComponent<NavMeshObstacle>().enabled = false;
        fadeOut = true;

        yield return new WaitForSecondsRealtime(1f);

        // Move player and boss and rotate them to look at each other
        playerMove.gameObject.transform.position = new Vector3(81f, 0, 0);
        boss.gameObject.transform.position = new Vector3(88.754f, 2, 0);
        playerMove.gameObject.transform.LookAt(new Vector3(boss.gameObject.transform.position.x, playerMove.gameObject.transform.position.y, boss.gameObject.transform.position.z));
        boss.gameObject.transform.LookAt(new Vector3(playerMove.gameObject.transform.position.x, boss.gameObject.transform.position.y, playerMove.gameObject.transform.position.z));

        // Turn on boss' AI again to make him bounce and set his destination to where he is moved to
        boss.GetComponent<NavMeshAgent>().enabled = true;
        boss.GetComponent<NavMeshAgent>().SetDestination(boss.gameObject.transform.position);

        yield return new WaitForSecondsRealtime(0.5f);

        // Switch cameras and fade screen in
        mainCam.gameObject.SetActive(false);
        bossCam.gameObject.SetActive(true);
        fadeOut = false;
        fadeIn = true;

        yield return new WaitForSecondsRealtime(3);

        // Begin transition and play sound
        transitionEffect = Instantiate(transition, new Vector3(88.754f, 3.15f, 0), Quaternion.Euler(0, -90, 0), null) as GameObject;
        FindObjectOfType<AudioManager>().PlayUninterrupted("Transition");

        yield return new WaitForSecondsRealtime(4.55f);

        // Swap out corrupted and cured Surav
        Instantiate(curedBoss, new Vector3(88.754f, 3.15f, 0), Quaternion.Euler(0, -90, 0), null);
        Destroy(boss.gameObject);

        yield return new WaitForSecondsRealtime(1.3f);

        Destroy(transitionEffect);

        yield return new WaitForSecondsRealtime(1.5f);

        // Fade Camera Out
        fadeIn = false;
        fadeOut = true;

        yield return new WaitForSecondsRealtime(1.5f);

        // Move player forward into shot, switch cameras, and fade in
        playerMove.gameObject.transform.position = new Vector3(83f, 0, 0);
        bossCam.gameObject.SetActive(false);
        bossPlayerCam.gameObject.SetActive(true);
        fadeOut = false;
        fadeIn = true;

        yield return new WaitForSecondsRealtime(0.5f);

        // Change Font Properties 
        dialogueUI.txt.font = curedFont;
        dialogueUI.txt.characterSpacing = 0.75f;
        dialogueUI.txt.lineSpacing = -15;
        dialogueUI.txt.fontSize = 35;

        dialogueUI.txt.text = "";

        // Pop up text box
        dialogueAnim.SetTrigger("PopUp");

        isTalking = true;

        yield return new WaitForSecondsRealtime(1f);

        fadeIn = false;

        yield break;
    }
}
