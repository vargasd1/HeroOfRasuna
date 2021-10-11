using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuzzleActive : MonoBehaviour
{
    public GameObject discOuter, discMid, discInner, camPuzzle, highlightInner, highlightMid, highlightOuter;
    float inRot, midRot, outRot, startIn, startMid, startOut, time = 0f;
    int selectedPiece = 0;
    public bool finishPuzzle = false;

    void Update()
    {
        //move the puzzle pieces in puzzle mode
        if (camPuzzle.activeSelf)
        {
            //if puzzle is close enough to complete
            if(Mathf.Abs(discInner.transform.rotation.z) <= 6f * (Mathf.PI / 180f) && Mathf.Abs(discMid.transform.rotation.z) <= 6f * (Mathf.PI / 180f) && Mathf.Abs(discOuter.transform.rotation.z) <= 6f * (Mathf.PI / 180f))
            {
                finishPuzzle = true;
                startIn = discInner.transform.rotation.z;
                startMid = discMid.transform.rotation.z;
                startOut = discOuter.transform.rotation.z;
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
                else if (Input.GetKey(KeyCode.D))
                {
                    switch (selectedPiece)
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
            }//else allow player to move puzzle
        }//if(camPuzzle.activeSelf)
    }//Update()

    void FixedUpdate()
    {
        if(finishPuzzle)
        {
            //adjust all rotations to be 0
            if (time > 1f) time = 1f;
            inRot = Mathf.Lerp(startIn, 0f, time);
            midRot = Mathf.Lerp(startMid, 0f, time);
            outRot = Mathf.Lerp(startOut, 0f, time);

            discInner.transform.rotation = new Quaternion(0f, 0f, inRot, Quaternion.identity.w);
            discMid.transform.rotation = new Quaternion(0f, 0f, midRot, Quaternion.identity.w);
            discOuter.transform.rotation = new Quaternion(0f, 0f, outRot, Quaternion.identity.w);

            time += Time.fixedDeltaTime;
        }

        if(time >= 6f)
        {
            //disable view for staircase being unlocked
            //enable player
            /*playerObj.SetActive(true);
            canvasUI.SetActive(true);
            camMain.SetActive(true);*/
        }
        else if(time >= 1f)
        {
            finishPuzzle = false;
            //disable all puzzle parts
            /*puzzleUI.SetActive(false);
            camPuzzle.SetActive(false);*/
            //enable view for staircase being unlocked in the second floor
        }
    }

    void HighlightPuzzle()
    {
        //highlight the currently selected puzzle piece
        highlightInner.SetActive(false);
        highlightMid.SetActive(false);
        highlightOuter.SetActive(false);
        switch (selectedPiece)
        {
            case 0:
                highlightInner.SetActive(true);
                break;
            case 1:
                highlightMid.SetActive(true);
                break;
            case 2:
                highlightOuter.SetActive(true);
                break;
        }
    }
}
