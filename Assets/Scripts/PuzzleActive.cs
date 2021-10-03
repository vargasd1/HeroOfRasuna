using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuzzleActive : MonoBehaviour
{
    public GameObject discOuter, discMid, discInner, camPuzzle, highlightInner, highlightMid, highlightOuter;
    int selectedPiece = 0;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    void Update()
    {
        //move the puzzle pieces in puzzle mode
        if (camPuzzle.activeSelf)
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
                        discInner.transform.Rotate(-2.0f, 0.0f, 0.0f, Space.Self);
                        break;
                    case 1:
                        discMid.transform.Rotate(-2.0f, 0.0f, 0.0f, Space.Self);
                        break;
                    case 2:
                        discOuter.transform.Rotate(-2.0f, 0.0f, 0.0f, Space.Self);
                        break;
                }
            }
            else if (Input.GetKey(KeyCode.D))
            {
                switch (selectedPiece)
                {
                    case 0:
                        discInner.transform.Rotate(2.0f, 0.0f, 0.0f, Space.Self);
                        break;
                    case 1:
                        discMid.transform.Rotate(2.0f, 0.0f, 0.0f, Space.Self);
                        break;
                    case 2:
                        discOuter.transform.Rotate(2.0f, 0.0f, 0.0f, Space.Self);
                        break;
                }
            }
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
