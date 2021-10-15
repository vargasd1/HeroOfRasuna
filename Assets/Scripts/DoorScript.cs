using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//ATTACHD TO: HoR_Gate (1) in level 2
//repurposed from old test puzzle in test scene

public class DoorScript : MonoBehaviour
{
    public GameObject gate;
    PuzzleActive puzzleScript;
    Vector3 openPosition;
    Vector3 closedPosition;
    float time = 0f;
    bool open = false;
    void Start()
    {
        closedPosition = gate.transform.position;
        openPosition = new Vector3(transform.position.x, transform.position.y - 7f, transform.position.z);
        puzzleScript = FindObjectOfType<PuzzleActive>();
    }

    void FixedUpdate()
    {
        if (puzzleScript.finishPuzzle)
        {
            open = true;
            
        }

        if(open)
        {
            time += Time.fixedUnscaledDeltaTime;
            if (time >= 1.5f && time <= 5.5f) openDoor();
        }
    }

    void openDoor()
    {
        //open the gate from 1.5 - 5.5 seconds
        Debug.Log("Moving");
        gate.transform.position = Vector3.Lerp(closedPosition, openPosition, (time - 1.5f) / 4f);
    }
}
