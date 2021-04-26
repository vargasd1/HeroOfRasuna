using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorScript : MonoBehaviour
{
    //Attached to Door
    public GameObject puzzle0, puzzle1, puzzle2;
    Vector3 openPosition;
    Vector3 closedPosition;
    float time = 0f;

    void Start()
    {
        closedPosition = transform.position;
        openPosition = new Vector3(transform.position.x, transform.position.y + 10f, transform.position.z);
    }

    void FixedUpdate()
    {
        if (puzzle0.GetComponent<PuzzleSwitch>().correctPlace == true
            && puzzle1.GetComponent<PuzzleSwitch>().correctPlace == true
            && puzzle2.GetComponent<PuzzleSwitch>().correctPlace == true)
        {
            openDoor();
            time += Time.fixedUnscaledDeltaTime;
            puzzle0.GetComponent<PuzzleSwitch>().allowPuzzle = false;
            puzzle1.GetComponent<PuzzleSwitch>().allowPuzzle = false;
            puzzle2.GetComponent<PuzzleSwitch>().allowPuzzle = false;
        }
    }

    void openDoor()
    {
        transform.position = Vector3.Lerp(closedPosition, openPosition, time / 2.5f);
    }
}
