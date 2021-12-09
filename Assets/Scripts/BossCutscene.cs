using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;

public class BossCutscene : MonoBehaviour
{

    private GameObject player;
    private PlayerMovement playerMove;
    private PlayerManager playerMan;
    private NavMeshAgent playerAI;
    private CharacterController playerCont;

    private SuravAI boss;

    private GameObject UI;
    private CameraFollow cam;
    public BossGate gate;
    public GameObject bossFocus;
    public GameObject bossObstacle;
    public GameObject mainObstacle;

    private bool startCutscene = false;
    private float slamDelay = 1f;
    private float camMoveDelay = 0f;
    private float playerMoveDelay = 0f;
    private bool doOnce = true;
    private bool movePlayer = false;
    public bool readyToSkip = false;
    public bool isTalking = false;
    public float startFightDelay = 2f;

    public int dialogueCount = 0;
    private BossDialogue dialogueUI;
    private Animator dialogueAnim;
    public GameObject nextPrompt;

    private float textStartDelay = 0.5f;
    private bool startOnce = true;

    // Start is called before the first frame update
    void Start()
    {
        player = FindObjectOfType<PlayerManager>().gameObject;
        playerMove = FindObjectOfType<PlayerMovement>();
        playerMan = FindObjectOfType<PlayerManager>();
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
        if (startCutscene)
        {
            if (doOnce)
            {
                //if overclocked, reset overclock
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

                playerMove.isCutScene = true;
                UI.SetActive(false);
                bossObstacle.GetComponent<NavMeshObstacle>().enabled = false;
                mainObstacle.GetComponent<NavMeshObstacle>().enabled = true;
                doOnce = false;
            }

            if (slamDelay <= 0)
            {
                gate.stopDoor = false;
                gate.closeDoor = true;
            }
            else
            {
                slamDelay -= Time.unscaledDeltaTime;
                gate.transform.position = gate.transform.position + UnityEngine.Random.insideUnitSphere * 0.025f;
                gate.openDoor = false;
                gate.stopDoor = true;
            }

            if (camMoveDelay >= 2)
            {
                cam.cameraMoveSpeed = 10;
                cam.CameraFollowObject = bossFocus;
                camMoveDelay = 2;
            }
            else
            {
                camMoveDelay += Time.unscaledDeltaTime;
            }

            if (playerMoveDelay >= 3)
            {
                movePlayer = true;
                playerMoveDelay = 3;
            }
            else
            {
                playerMoveDelay += Time.unscaledDeltaTime;
            }

            if (movePlayer)
            {
                player.transform.LookAt(new Vector3(70, 0, 0));
                playerMove.isCutSceneMoving = true;
                playerCont.enabled = false;
                playerAI.enabled = true;
                playerAI.SetDestination(new Vector3(65, 0, 0));

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

            if (isTalking)
            {
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
                    if (dialogueCount < 4)
                    {
                        nextPrompt.SetActive(true);

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

                if (dialogueCount >= 4 && readyToSkip)
                {
                    if (startFightDelay <= 0)
                    {
                        cam.CameraFollowObject = player;
                        playerMove.isCutScene = false;
                        UI.SetActive(true);
                        dialogueAnim.ResetTrigger("PopUp");
                        dialogueAnim.SetTrigger("PopDown");
                        boss.startFight = true;
                        isTalking = false;
                        startCutscene = false;
                        FindObjectOfType<AudioManager>().Stop("Exploration3");
                        FindObjectOfType<AudioManager>().Play("Boss Fight");
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
