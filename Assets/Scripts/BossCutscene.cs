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
    private BossGate gate;
    public GameObject bossFocus;
    public GameObject bossObstacle;
    public GameObject mainObstacle;

    private bool startCutscene = false;
    private float slamDelay = 1f;
    private float camMoveDelay = 0f;
    private float playerMoveDelay = 0f;
    private bool doOnce = true;
    private bool movePlayer = false;
    public float talkDelay = 3f;
    public bool isTalking = false;
    public float startFightDelay = 3f;

    public int dialogueCount = 0;
    private Vector3 gateDefaultPos = new Vector3(31.74f, 5, 0);

    // Start is called before the first frame update
    void Start()
    {
        player = FindObjectOfType<PlayerManager>().gameObject;
        playerMove = FindObjectOfType<PlayerMovement>();
        playerMan = FindObjectOfType<PlayerManager>();
        boss = FindObjectOfType<SuravAI>();
        UI = FindObjectOfType<HealthBar>().gameObject;
        cam = FindObjectOfType<CameraFollow>();
        gate = FindObjectOfType<BossGate>();
        playerAI = player.GetComponent<NavMeshAgent>();
        playerCont = player.GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (startCutscene)
        {
            if (doOnce)
            {
                playerMove.isCutScene = true;
                UI.SetActive(false);
                bossObstacle.GetComponent<NavMeshObstacle>().enabled = false;
                mainObstacle.GetComponent<NavMeshObstacle>().enabled = true;
                doOnce = false;
            }

            if (slamDelay <= 0)
            {
                gate.closeDoor = true;
            }
            else
            {
                slamDelay -= Time.unscaledDeltaTime;
                gate.transform.position = gateDefaultPos + UnityEngine.Random.insideUnitSphere * 0.0420f;
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
                    movePlayer = false;
                    isTalking = true;
                }
            }

            if (isTalking)
            {
                if (talkDelay <= 0)
                {
                    if (Input.GetKeyDown(KeyCode.Mouse0))
                    {
                        dialogueCount++;
                        talkDelay = 3f;
                    }
                }
                else
                {
                    talkDelay -= Time.unscaledDeltaTime;
                }

                if (dialogueCount >= 3 && talkDelay < 0)
                {
                    if (startFightDelay <= 0)
                    {
                        cam.CameraFollowObject = player;
                        playerMove.isCutScene = false;
                        UI.SetActive(true);
                        boss.startFight = true;
                        isTalking = false;
                        startCutscene = false;
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
