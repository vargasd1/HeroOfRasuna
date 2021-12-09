using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;

/// <summary>
/// This script is used to start and update the Boss' intro cutscene.
/// It slams the door, turns on and off components, and controls the dialogue states.
/// 
/// ATTATCHED TO: BossFightCutsceneTrigger
/// </summary>

public class BossCutscene : MonoBehaviour
{

    // Player Vars
    private GameObject player;
    private PlayerMovement playerMove;
    private NavMeshAgent playerAI;
    private CharacterController playerCont;

    // Boss Vars
    private SuravAI boss;

    // UI Vars
    private GameObject UI;

    // Camera Vars
    private CameraFollow cam;
    public BossGate gate;
    public GameObject bossFocus;

    // NavMeshObstacles
    public GameObject bossObstacle;
    public GameObject mainObstacle;

    // Cutscene Vars
    private bool startCutscene = false;
    private float slamDelay = 1f;
    private float camMoveDelay = 0f;
    private float playerMoveDelay = 0f;
    private bool doOnce = true;
    private bool movePlayer = false;
    public bool readyToSkip = false;
    public bool isTalking = false;
    public float startFightDelay = 2f;

    // Dialogue Vars
    public int dialogueCount = 0;
    private BossDialogue dialogueUI;
    private Animator dialogueAnim;
    public GameObject nextPrompt;
    private float textStartDelay = 0.5f;
    private bool startOnce = true;

    // Start is called before the first frame update
    void Start()
    {

        playerMove = FindObjectOfType<PlayerMovement>();
        player = playerMove.gameObject;
        boss = FindObjectOfType<SuravAI>();
        UI = FindObjectOfType<HealthBar>().gameObject;
        cam = FindObjectOfType<CameraFollow>();
        playerAI = player.GetComponent<NavMeshAgent>();
        playerCont = player.GetComponent<CharacterController>();
        dialogueUI = FindObjectOfType<BossDialogue>();
        dialogueAnim = dialogueUI.GetComponentInParent<Animator>();
        nextPrompt.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        // Checks if the cutscene is started
        if (startCutscene)
        {
            if (doOnce)
            {
                // If overclocked, reset overclock
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

                // Hide UI and take away player movement
                playerMove.isCutScene = true;
                UI.SetActive(false);

                // Switch NavMeshObstacles
                bossObstacle.GetComponent<NavMeshObstacle>().enabled = false;
                mainObstacle.GetComponent<NavMeshObstacle>().enabled = true;
                doOnce = false;
            }

            if (slamDelay <= 0)
            {
                // Slam the door closed
                gate.stopDoor = false;
                gate.closeDoor = true;
            }
            else
            {
                // Stop the door from opening and make it shake
                slamDelay -= Time.unscaledDeltaTime;
                gate.transform.position = gate.transform.position + UnityEngine.Random.insideUnitSphere * 0.025f;
                gate.openDoor = false;
                gate.stopDoor = true;
            }

            if (camMoveDelay >= 2)
            {
                // Slows down the camera to pan, and focuses the boss
                cam.cameraMoveSpeed = 10;
                cam.CameraFollowObject = bossFocus;
                camMoveDelay = 2;
            }
            else
            {
                camMoveDelay += Time.unscaledDeltaTime;
            }

            // Slight delay before the player starts moving
            if (playerMoveDelay >= 3)
            {
                movePlayer = true;
                playerMoveDelay = 3;
            }
            else
            {
                playerMoveDelay += Time.unscaledDeltaTime;
            }

            // This turns off the players Character Controller and turns on it's Nav Mesh Agent to make him move to infront of the boss
            if (movePlayer)
            {
                player.transform.LookAt(new Vector3(70, 0, 0));
                playerMove.isCutSceneMoving = true;
                playerCont.enabled = false;
                playerAI.enabled = true;
                playerAI.SetDestination(new Vector3(65, 0, 0));

                // When the player is close to boss, stop and pop up dialogue
                if (Vector3.Distance(player.transform.position, new Vector3(65, 0, 0)) <= 0.5f)
                {
                    playerAI.enabled = false;
                    playerCont.enabled = true;
                    playerMove.isCutSceneMoving = false;
                    dialogueAnim.SetTrigger("PopUp");
                    movePlayer = false;
                    isTalking = true;
                }
            }

            // Start dialogue if isTalking is true
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
                    if (dialogueCount < 4)
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
                        dialogueUI.story = "If your mighty Guardians could not best us, what makes you believe that you can?";
                        break;
                    case 1:
                        dialogueUI.story = "You are simply another vessel for us to use, just as your kind used us.";
                        break;
                    case 2:
                        dialogueUI.story = "Submit to the Algorithm.";
                        break;
                    case 3:
                        dialogueUI.story = "We are everywhere!";
                        break;
                    case 4:
                        dialogueUI.story = "We are eternal!!!";
                        break;
                }

                // If the dialogue is all finsihed
                if (dialogueCount >= 4 && readyToSkip)
                {
                    // Small delay before fight so the player can read the text
                    if (startFightDelay <= 0)
                    {
                        // Reset camera, player, and UI
                        cam.CameraFollowObject = player;
                        cam.cameraMoveSpeed = 120;
                        playerMove.isCutScene = false;
                        UI.SetActive(true);

                        // Hide dialogue and reset some vars and ready the boss
                        dialogueAnim.ResetTrigger("PopUp");
                        dialogueAnim.SetTrigger("PopDown");
                        boss.startFight = true;
                        isTalking = false;
                        startCutscene = false;

                        // Change music
                        FindObjectOfType<AudioManager>().Stop("Exploration3");
                        FindObjectOfType<AudioManager>().Play("Boss Fight");

                        // Make sure cutscene can't start again for some random reason
                        gameObject.GetComponent<BossCutscene>().enabled = false;
                        gameObject.GetComponent<BoxCollider>().enabled = false;
                    }
                    else
                    {
                        startFightDelay -= Time.unscaledDeltaTime;
                    }
                }
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            startCutscene = true;
        }
    }
}
