using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

public class BossEndCutscene : MonoBehaviour
{

    // Outro Cutscene
    public Image UICover;
    public GameObject mainUI;
    private bool fadeIn = false;
    private bool fadeOut = false;
    private SuravAI boss;
    private PlayerMovement playerMove;
    private bool doOnce = true;

    public Camera bossPlayerCam;
    public Camera bossCam;
    public Camera playerCam;
    private Camera mainCam;
    public GameObject bossSeatNav;

    private float alpha = 0;
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
        if (boss.health <= 0)
        {
            if (doOnce)
            {
                StartCoroutine(StartDefeatCutscene());
                doOnce = false;
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
                if (dialogueCount < 7)
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

            if (dialogueCount >= 7 && readyToSkip)
            {
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
        playerMove.isCutScene = true;
        mainUI.SetActive(false);
        boss.GetComponent<NavMeshAgent>().enabled = false;
        playerMove.gameObject.GetComponent<CharacterController>().enabled = false;
        bossSeatNav.GetComponent<NavMeshObstacle>().enabled = false;
        fadeOut = true;

        yield return new WaitForSecondsRealtime(1f);

        playerMove.gameObject.transform.position = new Vector3(81f, 0, 0);
        boss.gameObject.transform.position = new Vector3(88.754f, 2, 0);
        playerMove.gameObject.transform.LookAt(new Vector3(boss.gameObject.transform.position.x, playerMove.gameObject.transform.position.y, boss.gameObject.transform.position.z));
        boss.gameObject.transform.LookAt(new Vector3(playerMove.gameObject.transform.position.x, boss.gameObject.transform.position.y, playerMove.gameObject.transform.position.z));
        boss.GetComponent<NavMeshAgent>().enabled = true;
        boss.GetComponent<NavMeshAgent>().SetDestination(boss.gameObject.transform.position);

        yield return new WaitForSecondsRealtime(0.5f);

        mainCam.gameObject.SetActive(false);
        bossCam.gameObject.SetActive(true);
        fadeOut = false;
        fadeIn = true;

        yield return new WaitForSecondsRealtime(3);

        transitionEffect = Instantiate(transition, new Vector3(88.754f, 3.15f, 0), Quaternion.Euler(0, -90, 0), null) as GameObject;
        FindObjectOfType<AudioManager>().PlayUninterrupted("Transition");

        yield return new WaitForSecondsRealtime(4.55f);

        Instantiate(curedBoss, new Vector3(88.754f, 3.15f, 0), Quaternion.Euler(0, -90, 0), null);
        Destroy(boss.gameObject);

        yield return new WaitForSecondsRealtime(1.3f);

        Destroy(transitionEffect);

        yield return new WaitForSecondsRealtime(1.5f);

        fadeIn = false;
        fadeOut = true;

        yield return new WaitForSecondsRealtime(1.5f);

        playerMove.gameObject.transform.position = new Vector3(83f, 0, 0);
        bossCam.gameObject.SetActive(false);
        bossPlayerCam.gameObject.SetActive(true);
        fadeOut = false;
        fadeIn = true;

        yield return new WaitForSecondsRealtime(0.5f);

        // Font Stuff
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
