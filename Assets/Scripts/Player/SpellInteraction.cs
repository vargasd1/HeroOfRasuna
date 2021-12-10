using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// This script controls when the player's spell interactions
/// 
/// ATTATCHED TO: VFX_LightBurst, VFX_StunGrenade, VFX_Stun (Prefabs)
/// </summary>
public class SpellInteraction : MonoBehaviour
{
    // Player and base spell Vars
    public Transform player;
    public GameObject collisionFlash;
    public GameObject stunnedPart;
    public GameObject stunPart;
    public Rigidbody rb;
    public string spellType;

    // Fractured Pots
    public GameObject fracturedBowl;
    public GameObject fracturedPot;
    public GameObject fracturedVase;
    public GameObject xpOrb;

    // Enemies Vars
    private bool enemIsRanged = false;
    private EnemyAI enem;
    private EnemyRangedAI enemR;
    private SuravAI boss;

    // Sound Var
    private float soundChoice;

    //Grenade Vars
    public Vector3 positionA;
    public Vector3 positionB;
    public Vector3 handle;
    public AnimationCurve tweenSpeed;
    private float percent = 0;
    private float tweenLength = .5f;
    private float tweenTimer = 0;

    private void Start()
    {
        player = FindObjectOfType<PlayerManager>().gameObject.transform;
        boss = FindObjectOfType<SuravAI>();
        if (spellType == "attack")
        {
            rb = GetComponent<Rigidbody>();
            rb.velocity = transform.forward * 20;
        }
    }

    private void Update()
    {
        // Moves the grenade along the set path
        if (spellType == "grenade")
        {
            tweenTimer += Time.deltaTime;
            float p = tweenTimer / tweenLength;

            percent = tweenSpeed.Evaluate(p);
            transform.position = CalcPositionOnCurve(percent);
        }
    }

    void OnTriggerEnter(Collider hit)
    {
        // Checks which enemy type was hit
        if (hit.gameObject.GetComponent<EnemyAI>() != null)
        {
            enem = hit.gameObject.GetComponent<EnemyAI>();
            enemIsRanged = false;
        }
        else if (hit.gameObject.GetComponent<EnemyRangedAI>() != null)
        {
            enemR = hit.gameObject.GetComponent<EnemyRangedAI>();
            enemIsRanged = true;
        }

        // Switch case for what type of spell was spawned
        switch (spellType)
        {
            case "stun":
                if (hit.gameObject.tag == "Enemy" && hit.gameObject.tag != "Particles")
                {
                    // If it hits an enemy, checks what kind of enemy and stuns them, spawning stunned particles
                    if (enemIsRanged)
                    {
                        if (enemR.state != EnemyRangedAI.State.Stunned)
                        {
                            enemR.stunnedTimer = 4f;
                            enemR.state = EnemyRangedAI.State.Stunned;
                            enemR.GetComponent<NavMeshAgent>().SetDestination(enemR.transform.position);
                            Vector3 stunnedPos = new Vector3(enemR.transform.position.x, enemR.transform.position.y + 2, enemR.transform.position.z);
                            Instantiate(stunnedPart, stunnedPos, Quaternion.Euler(-90, 0, 0), enemR.gameObject.transform);
                        }
                    }
                    else
                    {
                        if (enem.state != EnemyAI.State.Stunned)
                        {
                            enem.stunnedTimer = 4f;
                            enem.state = EnemyAI.State.Stunned;
                            enem.GetComponent<NavMeshAgent>().SetDestination(enem.transform.position);
                            Vector3 stunnedPos = new Vector3(enem.transform.position.x, enem.transform.position.y + 2, enem.transform.position.z);
                            Instantiate(stunnedPart, stunnedPos, Quaternion.Euler(-90, 0, 0), enem.gameObject.transform);
                        }
                    }
                }
                else if (hit.gameObject.tag == "Boss" && hit.gameObject.tag != "Particles")
                {
                    // If it hits boss, stun and spawn stunned particles
                    boss.stunTimer = 3f;
                    Vector3 stunnedPos = new Vector3(boss.transform.position.x, 6, boss.transform.position.z);
                    GameObject stunpart = Instantiate(stunnedPart, stunnedPos, Quaternion.Euler(-90, 0, 0), boss.gameObject.transform) as GameObject;
                    stunpart.transform.localScale = new Vector3(2, 2, 2);

                }
                break;
            case "attack":
                if (hit.gameObject.tag != "Player" && hit.gameObject.tag != "Particles" && hit.gameObject.tag != "Ground" && hit.gameObject.tag != "MainCamera" && hit.gameObject.tag != "HitBox" && hit.gameObject.name != "HOR_Puzzle_Piece1" && hit.gameObject.name != "HOR_Puzzle_Piece2" && hit.gameObject.name != "HOR_Puzzle_Piece3")
                {
                    // If it hits nothing important, destroy object and spawn impact flash
                    Destroy(gameObject);
                    Instantiate(collisionFlash, transform.position, transform.rotation);
                }
                switch (hit.gameObject.tag)
                {
                    // Switch Case for what it hits, if it's an enemy deal damage, if it's a pot break it
                    case "Enemy":
                        if (enemIsRanged)
                        {
                            enemR.health -= 34;
                            enemR.alreadyHitByPlayer = true;
                            enemR.hitDelay = 0.15f;
                            enemR.anim.SetTrigger("Hit");
                            enemR.state = EnemyRangedAI.State.hitStunned;
                        }
                        else
                        {
                            enem.health -= 34;
                            enem.alreadyHitByPlayer = true;
                            enem.hitDelay = 0.15f;
                            enem.anim.SetTrigger("Hit");
                            enem.state = EnemyAI.State.hitStunned;
                        }
                        break;
                    case "Boss":
                        if (boss.state != SuravAI.State.Talking)
                        {
                            boss = hit.gameObject.GetComponent<SuravAI>();
                            boss.health -= 20;
                            boss.wasHitByPlayer = true;
                            boss.hitDelay = 0.5f;
                        }
                        break;

                    case "Prop1":
                        // Spawn fractured prop
                        GameObject frac = Instantiate(fracturedPot, hit.gameObject.transform.position, Quaternion.identity, null);
                        frac.transform.localScale = hit.gameObject.transform.localScale / 100;

                        // Play sound
                        soundChoice = UnityEngine.Random.Range(0, 2);
                        if (soundChoice < 1) FindObjectOfType<AudioManager>().PlayUninterrupted("PotteryBreak1");
                        else FindObjectOfType<AudioManager>().PlayUninterrupted("PotteryBreak2");

                        // Give some force to pottery
                        foreach (Rigidbody rb in frac.GetComponentsInChildren<Rigidbody>())
                        {
                            Vector3 force = (rb.transform.position - transform.position).normalized * 75;
                            rb.AddForce(force);
                        }

                        // Spawn XP
                        for (int i = 0; i < Random.Range(2, 4); i++)
                        {
                            GameObject xp = Instantiate(xpOrb, new Vector3(transform.position.x, transform.position.y, transform.position.z), transform.rotation, null);
                            Vector3 force = new Vector3(Random.Range(-10f, 10f), 0f, Random.Range(-10f, 10f)).normalized * 200;
                            xp.GetComponent<Rigidbody>().AddForce(force);
                        }
                        Destroy(hit.gameObject);
                        break;
                    case "Prop2":
                        // Spawn fractured prop
                        GameObject frac2 = Instantiate(fracturedBowl, hit.gameObject.transform.position, Quaternion.identity, null);
                        frac2.transform.localScale = hit.gameObject.transform.localScale / 100;

                        // Play sound
                        soundChoice = UnityEngine.Random.Range(0, 2);
                        if (soundChoice < 1) FindObjectOfType<AudioManager>().PlayUninterrupted("PotteryBreak1");
                        else FindObjectOfType<AudioManager>().PlayUninterrupted("PotteryBreak2");

                        // Give some force to pottery
                        foreach (Rigidbody rb in frac2.GetComponentsInChildren<Rigidbody>())
                        {
                            Vector3 force = (rb.transform.position - transform.position).normalized * 75;
                            rb.AddForce(force);
                        }

                        // Spawn XP
                        for (int i = 0; i < Random.Range(2, 4); i++)
                        {
                            GameObject xp = Instantiate(xpOrb, new Vector3(transform.position.x, transform.position.y, transform.position.z), transform.rotation, null);
                            Vector3 force = new Vector3(Random.Range(-10f, 10f), 0f, Random.Range(-10f, 10f)).normalized * 200;
                            xp.GetComponent<Rigidbody>().AddForce(force);
                        }
                        Destroy(hit.gameObject);
                        break;
                    case "Prop3":
                        // Spawn fractured prop
                        GameObject frac3 = Instantiate(fracturedVase, hit.gameObject.transform.position, Quaternion.identity, null);
                        frac3.transform.localScale = hit.gameObject.transform.localScale / 100;

                        // Play sound
                        soundChoice = UnityEngine.Random.Range(0, 2);
                        if (soundChoice < 1) FindObjectOfType<AudioManager>().PlayUninterrupted("PotteryBreak1");
                        else FindObjectOfType<AudioManager>().PlayUninterrupted("PotteryBreak2");


                        // Give some force to pottery
                        foreach (Rigidbody rb in frac3.GetComponentsInChildren<Rigidbody>())
                        {
                            Vector3 force = (rb.transform.position - transform.position).normalized * 75;
                            rb.AddForce(force);
                        }

                        // Spawn XP
                        for (int i = 0; i < Random.Range(2, 4); i++)
                        {
                            GameObject xp = Instantiate(xpOrb, new Vector3(transform.position.x, transform.position.y, transform.position.z), transform.rotation, null);
                            Vector3 force = new Vector3(Random.Range(-10f, 10f), 0f, Random.Range(-10f, 10f)).normalized * 200;
                            xp.GetComponent<Rigidbody>().AddForce(force);
                        }
                        Destroy(hit.gameObject);
                        break;
                    default:
                        break;
                }
                break;
            case "grenade":
                if (hit.gameObject.tag == "Ground")
                {
                    // If the grenade hits the ground, play sound and spawn stun sphere
                    FindObjectOfType<AudioManager>().PlayUninterrupted("Stun");
                    Vector3 targetLoc = new Vector3(transform.position.x, transform.position.y + 0.15f, transform.position.z);
                    GameObject stun = Instantiate(stunPart, targetLoc, Quaternion.Euler(0, 0, 0)) as GameObject;
                    stun.GetComponent<SpellInteraction>().spellType = "stun";
                    Destroy(gameObject);
                }
                break;
        }
    }

    private void OnCollisionStay(Collision hit)
    {
        switch (spellType)
        {
            case "stun":
                if (hit.gameObject.tag == "Enemy" && hit.gameObject.tag != "Particles")
                {
                    if (enemIsRanged)
                    {
                        if (enemR.state != EnemyRangedAI.State.Stunned)
                        {
                            enemR.stunnedTimer = 4f;
                            enemR.state = EnemyRangedAI.State.Stunned;
                            enemR.GetComponent<NavMeshAgent>().SetDestination(enemR.transform.position);
                            Vector3 stunnedPos = new Vector3(enemR.transform.position.x, enemR.transform.position.y + 2, enemR.transform.position.z);
                            Instantiate(stunnedPart, stunnedPos, Quaternion.Euler(-90, 0, 0), enemR.gameObject.transform);
                        }
                    }
                    else
                    {
                        if (enem.state != EnemyAI.State.Stunned)
                        {
                            enem.stunnedTimer = 4f;
                            enem.state = EnemyAI.State.Stunned;
                            enem.GetComponent<NavMeshAgent>().SetDestination(enem.transform.position);
                            Vector3 stunnedPos = new Vector3(enem.transform.position.x, enem.transform.position.y + 2, enem.transform.position.z);
                            Instantiate(stunnedPart, stunnedPos, Quaternion.Euler(-90, 0, 0), enem.gameObject.transform);
                        }
                    }
                }
                break;
        }
    }

    /// <summary>
    /// This function Lerps the grenade from the player and to the where the player clicked in the arc
    /// </summary>
    private Vector3 CalcPositionOnCurve(float percent)
    {
        // pC = lerp between pA and handle
        Vector3 positionC = AnimMath.Lerp(positionA, handle, percent);

        // pD = lerp between handle and pB
        Vector3 positionD = AnimMath.Lerp(handle, positionB, percent);

        // pF = lerp between pC and pD
        Vector3 positionE = AnimMath.Lerp(positionC, positionD, percent);

        return positionE;
    }
}
