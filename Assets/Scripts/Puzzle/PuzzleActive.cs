using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PuzzleActive : MonoBehaviour
{
    public GameObject discOuter, discMid, discInner, camPuzzle, playerObj, puzzleUI, canvasUI, camMain, camGate;
    public Image innerImg, midImg, outImg;
    float inRot, midRot, outRot, startIn, startMid, startOut, time = 0f;
    int selectedPiece = 0;
    public bool finishPuzzle = false;
    bool moveLeft = false;
    bool moveRight = false;
    private Image UICover;
    public bool fadeIn = false;
    public bool fadeOut = false;
    private float alpha = 0;
    private bool openDoor = false;
    public GameObject gate;
    private bool doOnce = false;

    private void Awake()
    {
        UICover = GameObject.FindGameObjectWithTag("loadingScreen").GetComponent<Image>();
    }

    void Update()
    {
        //move the puzzle pieces in puzzle mode
        if (camPuzzle.activeSelf)
        {
            //if puzzle is close enough to complete. Give the euler angles
            //if (Mathf.Abs(discInner.transform.rotation.z) <= 6f * (Mathf.PI / 180f) && Mathf.Abs(discMid.transform.rotation.z) <= 6f * (Mathf.PI / 180f) && Mathf.Abs(discOuter.transform.rotation.z) <= 6f * (Mathf.PI / 180f))
            if ((discInner.transform.eulerAngles.z <= 6f || discInner.transform.eulerAngles.z >= 354f) &&
                (discMid.transform.eulerAngles.z <= 6f || discMid.transform.eulerAngles.z >= 354f) &&
                (discOuter.transform.eulerAngles.z <= 6f || discOuter.transform.eulerAngles.z >= 354f))
            {
                finishPuzzle = true;
                startIn = GetAngle(discInner.transform.eulerAngles.z);
                startMid = GetAngle(discMid.transform.eulerAngles.z);
                startOut = GetAngle(discOuter.transform.eulerAngles.z);
            }
            //allow player to move puzzle
            //else
            else
            {
                if (Input.GetKeyDown(KeyCode.W))
                {
                    switch (selectedPiece)
                    {
                        case 0:
                            innerImg.rectTransform.localScale = Vector3.one;
                            midImg.rectTransform.localScale = Vector3.one * 2f;
                            break;
                        case 1:
                            midImg.rectTransform.localScale = Vector3.one;
                            outImg.rectTransform.localScale = Vector3.one * 2f;
                            break;
                    }
                    ++selectedPiece;
                    if (selectedPiece > 2) selectedPiece = 2;
                }
                else if (Input.GetKeyDown(KeyCode.S))
                {
                    switch (selectedPiece)
                    {
                        case 1:
                            innerImg.rectTransform.localScale = Vector3.one * 2f;
                            midImg.rectTransform.localScale = Vector3.one;
                            break;
                        case 2:
                            midImg.rectTransform.localScale = Vector3.one * 2f;
                            outImg.rectTransform.localScale = Vector3.one;
                            break;
                    }
                    --selectedPiece;
                    if (selectedPiece < 0) selectedPiece = 0;
                }
                else if (Input.GetKey(KeyCode.A))
                {
                    moveLeft = true;
                }
                else if (Input.GetKey(KeyCode.D))
                {
                    moveRight = true;
                }
                else if (Input.GetKey(KeyCode.T))
                {
                    switch (selectedPiece)
                    {
                        case 0:
                            Debug.Log("Inner: " + discInner.transform.eulerAngles.z);
                            break;
                        case 1:
                            Debug.Log("Mid: " + discMid.transform.eulerAngles.z);
                            break;
                        case 2:
                            Debug.Log("Outer: " + discOuter.transform.eulerAngles.z);
                            break;
                    }
                }
                else if (Input.GetKeyUp(KeyCode.A))
                {
                    moveLeft = false;
                }
                else if (Input.GetKeyUp(KeyCode.D))
                {
                    moveRight = false;
                }
            }//else allow player to move puzzle
        }//if(camPuzzle.activeSelf)
    }//Update()

    void FixedUpdate()
    {
        if (!finishPuzzle)
        {
            if (moveLeft)
            {
                switch (selectedPiece)
                {
                    case 0:
                        //discInner.transform.Rotate(0.0f, 0.0f, 2.0f, Space.Self);
                        discInner.transform.eulerAngles = discInner.transform.rotation.eulerAngles + new Vector3(0f, 0f, 2f);
                        break;
                    case 1:
                        discMid.transform.eulerAngles = discMid.transform.rotation.eulerAngles + new Vector3(0f, 0f, 2f);
                        break;
                    case 2:
                        discOuter.transform.eulerAngles = discOuter.transform.rotation.eulerAngles + new Vector3(0f, 0f, 2f);
                        break;
                }
            }
            else if (moveRight)
            {
                switch (selectedPiece)
                {
                    case 0:
                        discInner.transform.eulerAngles = discInner.transform.rotation.eulerAngles + new Vector3(0f, 0f, -2f);
                        break;
                    case 1:
                        discMid.transform.eulerAngles = discMid.transform.rotation.eulerAngles + new Vector3(0f, 0f, -2f);
                        break;
                    case 2:
                        discOuter.transform.eulerAngles = discOuter.transform.rotation.eulerAngles + new Vector3(0f, 0f, -2f);
                        break;
                }
            }
        }

        if (finishPuzzle && time < 1f)
        {
            //make all rotations on puzzles 0
            /*discInner.transform.Rotate(0.0f, 0.0f, startIn, Space.Self);
            discMid.transform.Rotate(0.0f, 0.0f, startMid, Space.Self);
            discOuter.transform.Rotate(0.0f, 0.0f, startOut, Space.Self);*/

            discInner.transform.eulerAngles = discInner.transform.rotation.eulerAngles + new Vector3(0f, 0f, startIn);
            discMid.transform.eulerAngles = discMid.transform.rotation.eulerAngles + new Vector3(0f, 0f, startMid);
            discOuter.transform.eulerAngles = discOuter.transform.rotation.eulerAngles + new Vector3(0f, 0f, startOut);

            time += Time.fixedDeltaTime;
        }

        if (time >= 1f)
        {
            //disable all puzzle parts, enable view for staircase being unlocked in the second floor
            if (!doOnce) StartCoroutine(openDoorCutscene());
            doOnce = true;
            time += Time.fixedDeltaTime;
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
            float step = 2f * Time.fixedUnscaledDeltaTime;// Time.deltaTime;
            gate.transform.position = Vector3.MoveTowards(gate.transform.position, new Vector3(11.36f, 5, 17.54f), step);
        }
    }

    float GetAngle(float starting)
    {
        //given the euler angle, give the correct adjustment
        //[0, 6]
        if (starting <= 6f) return -starting * Time.fixedDeltaTime;
        //[354, 360]
        else return (360f - starting) * Time.fixedDeltaTime;
    }

    IEnumerator openDoorCutscene()
    {
        //disable all puzzle parts, enable view for staircase being unlocked in the second floor
        puzzleUI.SetActive(false);

        fadeOut = true;
        fadeIn = false;

        yield return new WaitForSecondsRealtime(1);

        camPuzzle.SetActive(false);
        camGate.SetActive(true);
        playerObj.SetActive(true);
        playerObj.GetComponent<PlayerMovement>().isCutScene = true;

        fadeOut = false;
        fadeIn = true;

        yield return new WaitForSecondsRealtime(1);

        openDoor = true;
        fadeIn = false;

        yield return new WaitForSecondsRealtime(3);

        fadeOut = true;

        yield return new WaitForSecondsRealtime(1);

        camGate.SetActive(false);
        camMain.SetActive(true);

        fadeOut = false;
        fadeIn = true;

        yield return new WaitForSecondsRealtime(2);

        canvasUI.SetActive(true);
        playerObj.GetComponent<PlayerManager>().isInPuzzle = false;
        playerObj.GetComponent<PlayerMovement>().isCutScene = false;
    }
}
