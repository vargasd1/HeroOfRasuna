using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PuzzleActive : MonoBehaviour
{
    public GameObject discOuter, discMid, discInner, camPuzzle, playerObj, puzzleUI, canvasUI, camMain, camGate;
    float inRot, midRot, outRot, startIn, startMid, startOut, time = 0f;
    int selectedPiece = 0;
    public bool finishPuzzle = false;
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
            if (Mathf.Abs(discInner.transform.rotation.z) <= 6f * (Mathf.PI / 180f) && Mathf.Abs(discMid.transform.rotation.z) <= 6f * (Mathf.PI / 180f) && Mathf.Abs(discOuter.transform.rotation.z) <= 6f * (Mathf.PI / 180f))
            {
                finishPuzzle = true;
                startIn = GetAngle(discInner.transform.eulerAngles.z);
                startMid = GetAngle(discMid.transform.eulerAngles.z);
                startOut = GetAngle(discOuter.transform.eulerAngles.z);
            }
            //allow player to move puzzle
            else
            {
                if (Input.GetKeyDown(KeyCode.W))
                {
                    ++selectedPiece;
                    if (selectedPiece > 2) selectedPiece = 2;
                    HighlightPuzzle();
                }
                else if (Input.GetKeyDown(KeyCode.S))
                {
                    --selectedPiece;
                    if (selectedPiece < 0) selectedPiece = 0;
                    HighlightPuzzle();
                }
                else if (Input.GetKey(KeyCode.A))
                {
                    switch (selectedPiece)
                    {
                        case 0:
                            //discInner.transform.Rotate(0.0f, 0.0f, 2.0f, Space.Self);
                            discInner.transform.eulerAngles = discInner.transform.rotation.eulerAngles + new Vector3(0f, 0f, 2f);
                            break;
                        case 1:
                            //discMid.transform.Rotate(0.0f, 0.0f, 2.0f, Space.Self);
                            discMid.transform.eulerAngles = discMid.transform.rotation.eulerAngles + new Vector3(0f, 0f, 2f);
                            break;
                        case 2:
                            //discOuter.transform.Rotate(0.0f, 0.0f, 2.0f, Space.Self);
                            discOuter.transform.eulerAngles = discOuter.transform.rotation.eulerAngles + new Vector3(0f, 0f, 2f);
                            break;
                    }
                }
                else if (Input.GetKey(KeyCode.D))
                {
                    switch (selectedPiece)
                    {
                        case 0:
                            //discInner.transform.Rotate(0.0f, 0.0f, -2.0f, Space.Self);
                            discInner.transform.eulerAngles = discInner.transform.rotation.eulerAngles + new Vector3(0f, 0f, -2f);
                            break;
                        case 1:
                            //discMid.transform.Rotate(0.0f, 0.0f, -2.0f, Space.Self);
                            discMid.transform.eulerAngles = discMid.transform.rotation.eulerAngles + new Vector3(0f, 0f, -2f);
                            break;
                        case 2:
                            //discOuter.transform.Rotate(0.0f, 0.0f, -2.0f, Space.Self);
                            discOuter.transform.eulerAngles = discOuter.transform.rotation.eulerAngles + new Vector3(0f, 0f, -2f);
                            break;
                    }
                }
            }//else allow player to move puzzle
        }//if(camPuzzle.activeSelf)
    }//Update()

    void FixedUpdate()
    {
        if(finishPuzzle && time < 1f)
        {
            //make all rotations on puzzles 0
            discInner.transform.Rotate(0.0f, 0.0f, startIn, Space.Self);
            discMid.transform.Rotate(0.0f, 0.0f, startMid, Space.Self);
            discOuter.transform.Rotate(0.0f, 0.0f, startOut, Space.Self);

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

    void HighlightPuzzle()
    {
        //highlight the currently selected puzzle piece
        //
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
        playerObj.GetComponent<PlayerMovement>().isCutScene = false;
    }
}
