using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuzzleSwitch : MonoBehaviour
{
    //Attached to Puzzle Platform > Switch 0/1/2

    public GameObject player;
    public GameObject puzzlePiece;
    public float[] destinations = {-18f, 2f, 12f};
    float currentX = 0f;
    float y = 0f;
    public float z = 0f;
    float time = 0f;
    int posCheck = 0;
    int nextPosCheck = 1;
    bool move = false;
    public bool correctPlace = false;
    public bool allowPuzzle = true;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.G) && checkPlayerDistance() && allowPuzzle)
        {
            Debug.Log("Moving piece");
            if (!move) move = true;
        }
        else if (Input.GetKeyDown(KeyCode.G)) Debug.Log("Not close enough");
    }

    // Update is called once per physics update
    void FixedUpdate()
    {
        if (move)
        {
            time += Time.fixedDeltaTime;
            movePiece();
            if(time > 3f)
            {
                move = false;
                time = 0f;
                ++posCheck;
                ++nextPosCheck;
                if (posCheck > destinations.Length -1) posCheck = 0;
                if (nextPosCheck > destinations.Length - 1) nextPosCheck = 0;

                if (posCheck == 1) correctPlace = true;
                else correctPlace = false;
            }
        }
    }

    void movePiece()
    {
        currentX = Mathf.Lerp(destinations[posCheck], destinations[nextPosCheck], time / 2.5f);
        puzzlePiece.transform.localPosition = new Vector3(currentX, y, z);
    }

    bool checkPlayerDistance()
    {
        if (Vector3.Distance(player.transform.position, transform.position) < 3f) return true;
        else return false;
    }
}
