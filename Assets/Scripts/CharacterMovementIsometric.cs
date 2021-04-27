using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterMovementIsometric : MonoBehaviour
{
    private CharacterController controller;
    private Animator anim;
    private PlayerManager playerMan;

    private bool groundedPlayer;
    private bool doOnce = true;

    // bool for animation activation 
    public bool isMoving = false;

    private float playerSpeed = 5.0f;

    private Vector3 northSouthDir, eastWestDir, playerVelocity;
    private Vector3 move;
    private Vector3 pointToLook;

    //variables for overclock
    public GameObject overlay;
    public Image overclockCD;
    float slowdownFactor = .05f;
    float overclockTime = 5f;
    float overclockTransitionTime = 2f;
    float overclockCDTime = 10f;
    public static bool overclock = false;
    public static bool overclockTransition = false;

    public Camera mainCam;

    private void Start()
    {
        //////////////////////////////////////////////////////////// Get Character Controller Off the Player
        controller = gameObject.GetComponent<CharacterController>();
        anim = gameObject.GetComponent<Animator>();
        playerMan = gameObject.GetComponent<PlayerManager>();
        anim.updateMode = AnimatorUpdateMode.UnscaledTime;
    }

    void Update()
    {

        Ray cameraRay = mainCam.ScreenPointToRay(Input.mousePosition);
        Plane groundPlane = new Plane(Vector3.up, Vector3.zero);
        float rayLength;

        if (groundPlane.Raycast(cameraRay, out rayLength) && !isMoving)
        {
            pointToLook = cameraRay.GetPoint(rayLength);
            transform.LookAt(new Vector3(pointToLook.x, transform.position.y, pointToLook.z));
        }

        //////////////////////////////////////////////////////////// Player Grounded & Speed
        groundedPlayer = controller.isGrounded;

        if (groundedPlayer)
        {
            //////////////////////////////////////////////////////////// Sprinting Speed
            playerVelocity.y = 0f;
            if (Input.GetKey(KeyCode.LeftShift))
            {
                if (doOnce)
                {
                    doOnce = false;
                }
                playerSpeed = 15.0f;
            }
            else
            {
                //////////////////////////////////////////////////////////// Walking Speed
                doOnce = true;
                playerSpeed = 8.0f;
            }
        }
        else
        {
            //////////////////////////////////////////////////////////// In air Speed
            playerSpeed = 5.0f;
        }

        // If isAttacking DONT FUCKING MOVE
        if (anim.GetCurrentAnimatorStateInfo(0).IsName("Attack 1") || anim.GetCurrentAnimatorStateInfo(0).IsName("Attack 2") || anim.GetCurrentAnimatorStateInfo(0).IsName("Attack 3"))
        {
            playerSpeed = 0f;
        }

        //////////////////////////////////////////////////////////// Player Movement X & Z
        if (Input.GetButton("Horizontal") || Input.GetButton("Vertical"))
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
                anim.SetBool("isMoving", true);
                isMoving = true;
                gameObject.transform.forward = move;
            }
        }
        else
        {
            anim.SetBool("isMoving", false);
            isMoving = false;
            move = Vector3.zero;
        }


        //activate overclock
        if (Input.GetKeyDown(KeyCode.R))
        {
            if (!overclock && overclockCDTime <= 0f) SlowTime();
        }

        //display time
        if (Input.GetKeyDown(KeyCode.T))
        {
            Debug.Log("Fixed Delta Time: " + Time.fixedDeltaTime + "\nFixed Unscaled Delta Time: " + Time.fixedUnscaledDeltaTime);
        }
    }//Update()



    void FixedUpdate()
    {
        //////////////////////////////////////////////////////////// Final Movement
        if (playerVelocity.y >= 15) playerVelocity.y = 15;
        //playerVelocity.y += (gravityValue * Time.fixedUnscaledDeltaTime);
        Vector3 finalMovementVector = new Vector3(move.x * playerSpeed, playerVelocity.y, move.z * playerSpeed);
        controller.Move(finalMovementVector * Time.fixedUnscaledDeltaTime);

        //decrement overclock time
        if (overclock)
        {
            Debug.Log("Overclock");
            overclockTime -= Time.fixedUnscaledDeltaTime;
            if (overclockTime <= 0f)
            {
                //start transitioning back
                overclockTransition = true;
                overclock = false;
                overclockTime = 5f;
            }
        }
        //transition the time back to normal
        else if (overclockTransition)
        {
            Debug.Log("Overclock Transition");
            overclockTransitionTime -= Time.fixedUnscaledDeltaTime;
            //slowly return back to normal time. increase pitches of sounds as time goes back
            Time.timeScale += (1f / overclockTransitionTime) * Time.unscaledDeltaTime;
            Time.timeScale = Mathf.Clamp(Time.timeScale, 0f, 1f);//prevents timeScale from going above 1/below 0
            Time.fixedDeltaTime = Time.timeScale * .02f;

            if (overclockTransitionTime <= 1f)
            {
                //set everything back to normal time
                Debug.Log("Overclock off");
                overclockTransition = false;
                overclockTransitionTime = 2f;
                Time.timeScale = 1f;
                Time.fixedDeltaTime = Time.timeScale * .02f;
                overlay.SetActive(false);
            }
        }
        else
        {
            //only edit the cooldown shadows/effects for OVERCLOCK SPECIFICALLY here when not actively using overclock
            overclockCDTime -= Time.fixedUnscaledDeltaTime;
            if (overclockCDTime < 0f) overclockCDTime = 0f;
            overclockCD.rectTransform.sizeDelta = new Vector2(70, Mathf.Lerp(0, 70, overclockCDTime / 10f));
        }
    }//FixedUpdate()



    public void SlowTime()
    {
        //slow down time
        overclockTime = 5f;
        Time.timeScale = slowdownFactor;
        Time.fixedDeltaTime = Time.timeScale * .02f;
        overclock = true;
        overlay.SetActive(true);
        overclockCDTime = 10f;

        //slowing down decrease pitches of sounds - adjust for later
    }//SlowTime()
}
