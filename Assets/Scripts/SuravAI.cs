using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SuravAI : MonoBehaviour
{
    // State Machine Variables
    public enum State
    {
        Talking,
        Idle,
        MoveToNewLocation,
        ShockwaveAttack,
        ShotgunAttack,
        MinigunAttack,
        Defeated
    }
    public State state;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
