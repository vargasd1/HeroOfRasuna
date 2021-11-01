using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//ATTACHED TO: HoR_Stairs_Gate in level 2
//repurposed from old test puzzle in test scene

public class SecondFloorDoorScript : MonoBehaviour
{
    public GameObject gate;
    PuzzleActive puzzleScript;
    Vector3 openPosition;
    Vector3 closedPosition;
    //float time = 0f;
    bool open = false;
    void Start()
    {
        closedPosition = gate.transform.position;
        openPosition = new Vector3(transform.position.x, transform.position.y + 5f, transform.position.z);
        puzzleScript = FindObjectOfType<PuzzleActive>();
    }

    void FixedUpdate()
    {
        if (puzzleScript.finishPuzzle)
        {
            open = true;
        }

        if (open)
        {
            //time += Time.fixedUnscaledDeltaTime;
            //if (time >= 1.5f && time <= 5.5f) openDoor();
            //openDoor();
        }
    }

    void openDoor()
    {
        //open the gate from 1.5 - 5.5 seconds
        //Debug.Log("Moving");

        //gate.transform.position = Vector3.Lerp(closedPosition, openPosition, (time - 1.5f) / 4f);

        //Moves Towards is less gittery and less code
        float step = 1 * Time.fixedUnscaledDeltaTime;// Time.deltaTime;
        gate.transform.position = Vector3.MoveTowards(transform.position, new Vector3(11.36f, 5, 17.54f), step);
    }
}
