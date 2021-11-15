using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//ATTACHED TO: player gameobject

public class PlayerMovement : MonoBehaviour
{
    private CharacterController controller;
    public Animator anim;
    private PlayerManager playerMan;

    private bool groundedPlayer;
    private bool doOnce = true;

    // bool for animation activation 
    public bool isMoving = false;
    public bool isAttacking = false;
    public bool isCasting = false;
    public bool playerHit = false;
    public bool isDead = false;
    public bool attackAnimPlaying = false;
    public bool isCutScene = false;

    public float playerSpeed = 5.0f;

    private Vector3 northSouthDir, eastWestDir, playerVelocity;
    private Vector3 move;
    private Vector3 pointToLook;
    private float targetRotationX;
    private float targetRotationZ;
    private float tempRotationX;
    private float tempRotationZ;
    private bool doRotation = false;

    //variables for overclock
    public GameObject overlay;
    public Image overclockCD;
    float slowdownFactor = .05f;
    public float overclockTime = 5f;
    public float overclockTransitionTime = 2f;
    //float overclockCDTime = 0f;
    public bool overclock = false;
    public bool overclockTransition = false;
    public float overclockChargedAmt = 100f;

    //public Camera mainCam;
    private AudioManager audioScript;

    private void Start()
    {
        //////////////////////////////////////////////////////////// Get Character Controller Off the Player
        controller = gameObject.GetComponent<CharacterController>();
        anim = gameObject.GetComponent<Animator>();
        playerMan = gameObject.GetComponent<PlayerManager>();
        anim.updateMode = AnimatorUpdateMode.UnscaledTime;
        audioScript = FindObjectOfType<AudioManager>();
    }

    void Update()
    {
        isDead = playerMan.isDead;

        if (!isDead && !pauseMenu.GamePaused && !isCutScene)
        {
            //////////////////////////////////////////////////////////// Player Grounded & Speed
            groundedPlayer = controller.isGrounded;

            if (doRotation)
            {
                //tempRotationX = AnimMath.Slide(0, targetRotationX, .01f);
                //tempRotationZ = AnimMath.Slide(0, targetRotationZ, .01f);

                transform.LookAt(new Vector3(targetRotationX, transform.position.y, targetRotationZ));
                doRotation = false;
            }

            if (groundedPlayer)
            {
                //////////////////////////////////////////////////////////// Sprinting Speed
                playerVelocity.y = 0f;
                if (!playerHit && !isAttacking && !attackAnimPlaying && !isCasting)
                {
                    if (Input.GetKey(KeyCode.LeftShift))
                    {
                        if (doOnce)
                        {
                            doOnce = false;
                        }
                        if (playerSpeed < 9) playerSpeed += Time.fixedUnscaledDeltaTime * 25;
                        else playerSpeed = 9f;
                        anim.SetFloat("Speed", playerSpeed);
                    }
                    else
                    {
                        //////////////////////////////////////////////////////////// Walking Speed
                        doOnce = true;
                        if (playerSpeed > 6) playerSpeed -= Time.fixedUnscaledDeltaTime * 25;
                        else playerSpeed = 6f;
                        anim.SetFloat("Speed", playerSpeed);
                    }
                }
                else
                {
                    playerSpeed = 0f;
                }
            }

            //////////////////////////////////////////////////////////// Player Movement X & Z
            if ((Input.GetButton("Horizontal") || Input.GetButton("Vertical")) && !isDead)
            {
                northSouthDir = Camera.main.transform.forward;
                northSouthDir += new Vector3(0, .7f, 0);
                northSouthDir = Vector3.Normalize(northSouthDir);
                eastWestDir = Quaternion.Euler(new Vector3(0, 90, 0)) * northSouthDir;
                move = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));

                move = Quaternion.Euler(new Vector3(0, 90, 0)) * northSouthDir;

                Vector3 rightMovement = eastWestDir * playerSpeed * Input.GetAxis("Horizontal");
                Vector3 upMovement = northSouthDir * playerSpeed * Input.GetAxis("Vertical");

                move = Vector3.Normalize(rightMovement + upMovement);


                if (move != Vector3.zero)
                {
                    // Set animation parameter and variable of "isMoving" to false
                    anim.SetBool("isMoving", true);
                    isMoving = true;
                    // Find the look at rotation for the player
                    Quaternion toRotation = Quaternion.LookRotation(move, Vector3.up);
                    // Slerp the player's rotation towards the toRotation
                    transform.rotation = Quaternion.RotateTowards(transform.rotation, toRotation, 1000 * Time.unscaledDeltaTime);
                }
            }
            else
            {
                // If the player isn't moving set animation parameter and variable of "isMoving" to false
                anim.SetBool("isMoving", false);
                isMoving = false;
                // Make the move Vector = (0,0,0)
                move = Vector3.zero;
            }

            //activate overclock
            if (Input.GetKeyDown(KeyCode.R) && !isDead)
            {
                if (!overclock && overclockChargedAmt >= 100)
                {
                    audioScript.Play("Overclock");
                    SlowTime();
                }
            }

            //display time
            if (Input.GetKeyDown(KeyCode.T))
            {
                Debug.Log("Fixed Delta Time: " + Time.fixedDeltaTime + "\nFixed Unscaled Delta Time: " + Time.fixedUnscaledDeltaTime);
            }
        }
        else if (!isDead && !pauseMenu.GamePaused && isCutScene)
        {
            anim.SetBool("isMoving", false);
            playerSpeed = 0;
            anim.SetFloat("Speed", playerSpeed);
        }
        //(!isDead && !isPaused)
        else
        {
            playerSpeed = 0;
            anim.SetFloat("Speed", playerSpeed);
        }
    }//Update()

    void FixedUpdate()
    {
        if (!isDead && !pauseMenu.GamePaused && !isCutScene)
        {
            //////////////////////////////////////////////////////////// Final Movement
            if (playerVelocity.y >= 15) playerVelocity.y = 15;
            Vector3 finalMovementVector = new Vector3(move.x * playerSpeed, playerVelocity.y, move.z * playerSpeed);
            controller.Move(finalMovementVector * Time.fixedUnscaledDeltaTime);

            //decrement overclock time
            if (overclock)
            {
                //Debug.Log("Overclock");
                overclockTime -= Time.fixedUnscaledDeltaTime;
                overclockChargedAmt = ((overclockTime + 1f) / 6f) * 100f;
                if (overclockTime <= 0f)
                {
                    //start transitioning back
                    overclockTransition = true;
                    overclock = false;
                    overclockTime = 5f;
                }
                audioScript.ChangePitch("Overclock", .9f);
                audioScript.SlowSounds();
            }
            //transition the time back to normal
            else if (overclockTransition)
            {
                Debug.Log("Overclock Transition");
                overclockTransitionTime -= Time.fixedUnscaledDeltaTime;
                overclockChargedAmt = ((overclockTransitionTime - 1f) / 6f) * 100f;
                //slowly return back to normal time. increase pitches of sounds as time goes back
                Time.timeScale += (1f / overclockTransitionTime) * Time.unscaledDeltaTime;
                Time.timeScale = Mathf.Clamp(Time.timeScale, 0f, 1f);//prevents timeScale from going above 1/below 0
                Time.fixedDeltaTime = Time.timeScale * .02f;
                audioScript.ChangePitch("Overclock", 2f + Time.timeScale);

                if (overclockTransitionTime <= 1f)
                {
                    //set everything back to normal time
                    //Debug.Log("Overclock off");
                    overclockTransition = false;
                    overclockTransitionTime = 2f;
                    Time.timeScale = 1f;
                    Time.fixedDeltaTime = Time.timeScale * .02f;
                    overlay.SetActive(false);
                    overclockChargedAmt = 0f;
                    audioScript.Stop("Overclock");
                    audioScript.ChangePitch("Overclock", 1f);
                    audioScript.ResetSounds();
                }
            }
            else
            {
                //only edit the cooldown shadows/effects for OVERCLOCK SPECIFICALLY here when not actively using overclock
                //overclockCDTime -= Time.fixedUnscaledDeltaTime;
                //if (overclockCDTime < 0f) overclockCDTime = 0f;
                //float chargeHeight = AnimMath.Map((100f - overclockChargedAmt) / 1.4285f, 0, 100, 70, 0);
                //float chargeHeight = Mathf.Lerp(70f, 0f, (overclockChargedAmt / 100f));
                //overclockCD.rectTransform.sizeDelta = new Vector2(70, chargeHeight);
                //overclockCD.fillAmount = Mathf.Clamp(overclockChargedAmt / 100f, 0f, 1f);
                //Debug.Log("chargeHeight: " + chargeHeight);
            }
            overclockCD.fillAmount = Mathf.Clamp(overclockChargedAmt / 100f, 0f, 1f);
        }//(!isDead && !isPaused)
    }//FixedUpdate()

    public void SlowTime()
    {
        //slow down time
        overclockTime = 5f;
        Time.timeScale = slowdownFactor;
        Time.fixedDeltaTime = Time.timeScale * .02f;
        overclock = true;
        overclockChargedAmt = 0;
        overlay.SetActive(true);
        //overclockCDTime = 10f;


        //slowing down decrease pitches of sounds - adjust for later
    }//SlowTime()

    public void lookAtMouse()
    {
        //////////////////////////////////////////////////////////// This makes player look at mouse cursor
        Ray cameraRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        Plane groundPlane = new Plane(Vector3.up, Vector3.zero);
        float rayLength;

        if (groundPlane.Raycast(cameraRay, out rayLength))
        {
            pointToLook = cameraRay.GetPoint(rayLength);

            targetRotationX = pointToLook.x;
            targetRotationZ = pointToLook.z;

            doRotation = true;
        }
    }

    public void ResetDamage()
    {
        // in Animator sets the Trigger parameter back to false
        anim.ResetTrigger("Hit");
        playerHit = false;
    }

    public void ResetAttack()
    {
        // in Animator sets the Trigger parameter back to false
        anim.ResetTrigger("Hit");
        playerHit = false;
        playerMan.isCheckingForClick = false;
        attackAnimPlaying = false;
        playerMan.canAttack = true;
        playerMan.attackNum = 0;
        anim.SetInteger("swingCount", 0);
    }

    public void isAttackingSet()
    {
        // Used for the animator to tell the player script if to move or not, and when they frames of the attack are out
        isAttacking = !isAttacking;
        playerMan.canAttack = true;
    }

    public void isCastingSet()
    {
        isCasting = !isCasting;
        playerMan.canAttack = true;
    }

    public void attackAnimSet()
    {
        // Used for the animator to tell the player script if the attack animations are playing or not
        attackAnimPlaying = true;
    }
}
