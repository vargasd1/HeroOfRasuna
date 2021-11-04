using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuzzleActive : MonoBehaviour
{
    public GameObject discOuter, discMid, discInner, camPuzzle, playerObj, puzzleUI, canvasUI, camMain;
    float inRot, midRot, outRot, startIn, startMid, startOut, time = 0f;
    int selectedPiece = 0;
    public bool finishPuzzle = false;

    void Update()
    {
        //move the puzzle pieces in puzzle mode
        if (camPuzzle.activeSelf)
        {
            //if puzzle is close enough to complete. Give the euler angles
            if(Mathf.Abs(discInner.transform.rotation.z) <= 6f * (Mathf.PI / 180f) && Mathf.Abs(discMid.transform.rotation.z) <= 6f * (Mathf.PI / 180f) && Mathf.Abs(discOuter.transform.rotation.z) <= 6f * (Mathf.PI / 180f))
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
                    switch(selectedPiece)
                    {
                        case 0:
                            discInner.transform.Rotate(0.0f, 0.0f, 2.0f, Space.Self);
                            break;
                        case 1:
                            discMid.transform.Rotate(0.0f, 0.0f, 2.0f, Space.Self);
                            break;
                        case 2:
                            discOuter.transform.Rotate(0.0f, 0.0f, 2.0f, Space.Self);
                            break;
                    }
                }
                else if (Input.GetKey(KeyCode.D))
                {
                    switch (selectedPiece)
                    {
                        case 0:
                            discInner.transform.Rotate(0.0f, 0.0f, -2.0f, Space.Self);
                            break;
                        case 1:
                            discMid.transform.Rotate(0.0f, 0.0f, -2.0f, Space.Self);
                            break;
                        case 2:
                            discOuter.transform.Rotate(0.0f, 0.0f, -2.0f, Space.Self);
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

        //only runs after puzzle is finished
        if(time >= 6f)// && time < 6.25f)
        {
            //disable view for staircase being unlocked
            finishPuzzle = true;//turn back on
            //enable player
            playerObj.SetActive(true);
            canvasUI.SetActive(true);
            camMain.SetActive(true);
            time += Time.fixedDeltaTime;
        }
        else if(time >= 1f)
        {
            finishPuzzle = false;//stops puzzle rotation
            //disable all puzzle parts, enable view for staircase being unlocked in the second floor
            puzzleUI.SetActive(false);
            camPuzzle.SetActive(false);
            //set normal camera to pan
            time += Time.fixedDeltaTime;
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
}
