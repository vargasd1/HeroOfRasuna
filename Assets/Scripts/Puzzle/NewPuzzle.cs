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
    //private bool fadeIn = false;
    //private bool fadeOut = false;
    public Image UICover;
    private bool doOnce = true;
    //private float alpha = 0;

    void Start()
    {
        inventory = Inventory.instance;
        ui = FindObjectOfType<InventoryUI>();
        puzzleScript = FindObjectOfType<PuzzleActive>();
        UICover = GameObject.FindGameObjectWithTag("loadingScreen").GetComponent<Image>();
    }

    public override void Interact()//called when the player presses the interact button (G)
    {
        if (!puzzleScript.finishPuzzle)
        {
            //call parent function
            base.Interact();

            //switch to puzzle mode
            if (!camPuzzle.activeSelf && discInner.activeSelf && discMid.activeSelf && discOuter.activeSelf)
            {
                //if overclocked, stop overclock
                PlayerMovement playerMove = FindObjectOfType<PlayerMovement>();
                if (playerMove.overclock || playerMove.overclockTransition)
                {
                    playerMove.overclockTime = 5f;
                    playerMove.overclockTransitionTime = 2f;
                    playerMove.overclock = false;
                    playerMove.overclockTransition = false;
                    Time.timeScale = 1f;
                    Time.fixedDeltaTime = Time.timeScale * .02f;
                    AudioManager aud = FindObjectOfType<AudioManager>();
                    aud.Stop("Overclock");
                    aud.ChangePitch("Overclock", 1f);
                    aud.ResetSounds();
                }
                if (doOnce)
                {
                    StartCoroutine(switchToPuzzle());
                    doOnce = false;
                }
            }
            //exit puzzle mode
            else if (camPuzzle.activeSelf)
            {
                StartCoroutine(exitPuzzle());
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
        playerObj.GetComponent<PlayerManager>().isInPuzzle = true;
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

        yield break;
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

        puzzleScript.fadeOut = false;
        puzzleScript.fadeIn = true;

        yield return new WaitForSecondsRealtime(1.5f);

        canvasUI.SetActive(true);
        playerObj.GetComponent<PlayerManager>().isInPuzzle = false;
        playerObj.GetComponent<PlayerMovement>().isCutScene = false;
        doOnce = true;

        yield break;
    }
}
