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
        MeteorShower,
        Defeated
    }
    public State state;


    // Navigation Variables
    private NavMeshAgent agent;
    public Transform player;
    public Animator anim;
    public bool hasSeenPlayer;
    public bool isWandering = false;
    public Vector3 wanderDest;
    public float wanderDelay;

    // Other Vars
    private float bounceCounter = 0.8f;
    private float health = 200;

    private bool lowerWeight = false;
    private bool raiseWeight = false;

    private float resetLayerAmt = 0;
    private float resetLayerAlpha = 0;

    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
        state = SuravAI.State.Talking;
        player = FindObjectOfType<PlayerManager>().gameObject.transform;
    }

    // Update is called once per frame
    void Update()
    {
        if (health <= 0)
        {
            state = State.Defeated;
        }
        else
        {
            // Make Surav Bounce up and down
            bounceCounter += Time.deltaTime;
            if (bounceCounter >= Mathf.PI * 2) bounceCounter -= Mathf.PI * 2;
            agent.baseOffset = 2 + Mathf.Sin(bounceCounter) * 0.4f;

            if (lowerWeight)
            {
                lowerLayerWeight();
            }

            if (raiseWeight)
            {
                raiseLayerWeight();
            }
        }

        switch (state)
        {
            case State.Talking:
                state = State.Idle;
                break;
            case State.Idle:
                if (wanderDelay > 0) wanderDelay -= Time.deltaTime;
                if (wanderDelay <= 0)
                {
                    state = State.MoveToNewLocation;
                }
                break;
            case State.MoveToNewLocation:
                findNewLocation();
                break;
            case State.MinigunAttack:
                break;
            case State.ShotgunAttack:
                break;
            case State.ShockwaveAttack:
                break;
            case State.MeteorShower:
                break;
            case State.Defeated:
                break;
        }

    }

    void findNewLocation()
    {
        NavMeshHit hit;
        Vector3 randomDir = UnityEngine.Random.insideUnitSphere * 20;
        randomDir += transform.position;
        NavMesh.SamplePosition(randomDir, out hit, 20, 9);

        if (!isWandering)
        {
            isWandering = true;
            wanderDest = hit.position;
            agent.SetDestination(wanderDest);
        }

        if (Vector3.Distance(transform.position, wanderDest) <= 2f)
        {
            state = State.Idle;
            agent.SetDestination(transform.position);
            wanderDelay = 5;
            isWandering = false;
        }
    }

    void lowerLayerWeight()
    {
        resetLayerAlpha += Time.deltaTime;
        resetLayerAmt = Mathf.Lerp(0.75f, 0, resetLayerAlpha);
        anim.SetLayerWeight(1, resetLayerAmt);

        if (resetLayerAmt <= 0.05f)
        {
            lowerWeight = false;
            anim.SetLayerWeight(1, 0);
            resetLayerAlpha = 0;
        }
    }

    void raiseLayerWeight()
    {
        resetLayerAlpha += Time.deltaTime;
        resetLayerAmt = Mathf.Lerp(0, 0.75f, resetLayerAlpha);
        anim.SetLayerWeight(1, resetLayerAmt);

        if (resetLayerAmt >= 0.7f)
        {
            raiseWeight = false;
            anim.SetLayerWeight(1, 0.75f);
            resetLayerAlpha = 0;
        }
    }

    void setRaiseLayerWeight()
    {
        lowerWeight = false;
        raiseWeight = true;
        resetLayerAlpha = 0;
    }

    void setLowerLayerWeight()
    {
        raiseWeight = false;
        lowerWeight = true;
        resetLayerAlpha = 0.75f;
    }
}
