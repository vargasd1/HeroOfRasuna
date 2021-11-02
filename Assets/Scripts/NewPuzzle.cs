using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//names in PlaceDisk() need to stay updated with object names

public class NewPuzzle : Interactable
{
    Inventory inventory;
    InventoryUI ui;
    public GameObject discOuter, discMid, discInner, camPuzzle, camMain, playerObj, canvasUI, puzzleUI;//, highlightInner, highlightMid, highlightOuter;
    public PuzzleActive puzzleScript;
    private bool fadeIn = false;
    private bool fadeOut = false;
    public Image UICover;
    private float alpha = 0;
    //int selectedPiece = 0;

    void Start()
    {
        inventory = Inventory.instance;
        ui = FindObjectOfType<InventoryUI>();
        puzzleScript = FindObjectOfType<PuzzleActive>();
        UICover = GameObject.FindGameObjectWithTag("loadingScreen").GetComponent<Image>();
    }

    /*void Update()
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
                Debug.Log("Holding A");
            }
            else if (Input.GetKey(KeyCode.D))
            {
                Debug.Log("Holding D");
            }
        }
    }*/

    public override void Interact()//called when the player presses the interact button (G)
    {
        if (!puzzleScript.finishPuzzle)
        {
            //call parent function
            base.Interact();

            //switch to puzzle mode
            if (!camPuzzle.activeSelf && discInner.activeSelf && discMid.activeSelf && discOuter.activeSelf)
            {
                StartCoroutine(switchToPuzzle());
                // Moved in to Coroutine

                //camMain.SetActive(false);
                //camPuzzle.SetActive(true);
                //playerObj.SetActive(false);
                //puzzleUI.SetActive(true);
                //canvasUI.SetActive(false);
            }
            //exit puzzle mode
            else if (camPuzzle.activeSelf)
            {
                StartCoroutine(exitPuzzle());
                // Moved in to Coroutine

                //playerObj.SetActive(true);
                //puzzleUI.SetActive(false);
                //canvasUI.SetActive(true);
                //camPuzzle.SetActive(false);
                //camMain.SetActive(true);
                //shouldn't need to unhighlight since they are children of puzzleUI
            }
            else
            {
                PlaceDisc();
            }
        }
    }

    void PlaceDisc()
    {
        if (inventory.items.Count > 0)
        {
            //place puzzles
            switch (inventory.items[0].name)
            {
                case "HOR_Puzzle_Piece3":
                    ui.ringOuter.SetActive(false);
                    discOuter.SetActive(true);
                    break;
                case "HOR_Puzzle_Piece2":
                    ui.ringMid.SetActive(false);
                    discMid.SetActive(true);
                    break;
                case "HOR_Puzzle_Piece1":
                    ui.ringInner.SetActive(false);
                    discInner.SetActive(true);
                    break;
            }
            inventory.Remove(inventory.items[0]);
        }
    }

    IEnumerator switchToPuzzle()
    {
        playerObj.GetComponent<PlayerMovement>().isCutScene = true;
        puzzleScript.fadeOut = true;
        puzzleScript.fadeIn = false;
        canvasUI.SetActive(false);

        yield return new WaitForSecondsRealtime(1);

        camMain.SetActive(false);
        camPuzzle.SetActive(true);
        playerObj.SetActive(false);

        puzzleScript.fadeOut = false;
        puzzleScript.fadeIn = true;

        yield return new WaitForSecondsRealtime(1.5f);

        puzzleUI.SetActive(true);
    }

    IEnumerator exitPuzzle()
    {
        puzzleScript.fadeOut = true;
        puzzleScript.fadeIn = false;
        puzzleUI.SetActive(false);


        yield return new WaitForSecondsRealtime(1);

        camMain.SetActive(true);
        camPuzzle.SetActive(false);
        playerObj.SetActive(true);
        playerObj.GetComponent<PlayerMovement>().isCutScene = true;

        puzzleScript.fadeOut = false;
        puzzleScript.fadeIn = true;

        yield return new WaitForSecondsRealtime(1.5f);

        canvasUI.SetActive(true);
        playerObj.GetComponent<PlayerMovement>().isCutScene = false;
    }

    /*void HighlightPuzzle()
    {
        //highlight the currently selected puzzle piece
        highlightInner.SetActive(false);
        highlightMid.SetActive(false);
        highlightOuter.SetActive(false);
        switch(selectedPiece)
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
    }*/
}
