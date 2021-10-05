using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    // State Machine Variables
    public enum State
    {
        Spawning,
        Idle,
        Chasing,
        Attacking,
        Wander,
        Stunned,
        hitStunned,
        Dead,
    }
    public State state;

    // Navigation Variables
    public Transform player;
    private NavMeshAgent agent;
    public Animator anim;
    public bool hasSeenPlayer;
    public bool isWandering = false;
    public Vector3 wanderDest;
    public float wanderDelay;

    // Health Variables
    public float health = 100f;
    public float stunnedTimer = 0f;
    private float spawnTimer = 2f;
    public GameObject xpOrb;
    public bool orbsDroppedOnce = false;

    // Attack Variables
    private float timeBetweenAttacks = 0f;
    public bool alreadyHitPlayer = false;
    public bool alreadyHitByPlayer = false;
    public bool isAttacking = false;
    public bool attackAnimIsPlaying = false;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
        state = State.Spawning;
        player = FindObjectOfType<PlayerManager>().gameObject.transform;
    }

    private void Start() { }

    void Update()
    {
        if (health <= 0) state = State.Dead;
        if (player && player.GetComponent<PlayerManager>().isDead) player = null;
        if (!pauseMenu.GamePaused)
        {
            switch (state)
            {
                case State.Spawning:
                    // Play spawn animation
                    // When completed, goes to Idle state
                    spawnTimer -= Time.fixedDeltaTime;
                    anim.SetBool("isMoving", false);
                    if (spawnTimer <= 0) state = State.Idle;
                    break;
                case State.Idle:
                    // Reset navMesh && state
                    agent.speed = 6f;
                    anim.SetBool("isMoving", false);
                    if (wanderDelay > 0) wanderDelay -= Time.deltaTime;
                    if (player)
                    {
                        if (Vector3.Distance(transform.position, player.position) > 10 && wanderDelay <= 0)
                        {
                            state = State.Wander;
                        }
                        if (Vector3.Distance(transform.position, player.position) <= 10 || hasSeenPlayer) state = State.Chasing;
                        if (Vector3.Distance(transform.position, player.position) <= agent.stoppingDistance) state = State.Attacking;
                    }
                    break;
                case State.Wander:
                    agent.speed = 4f;
                    WanderToLocation();
                    break;
                case State.Chasing:
                    agent.speed = 5f;
                    ChasePlayer();
                    break;
                case State.Attacking:
                    AttackPlayer();
                    break;
                case State.hitStunned:
                    agent.speed = 0f;
                    break;
                case State.Stunned:
                    // Stops moving instantly
                    agent.speed = 0f;
                    anim.SetBool("isMoving", false);
                    // Decrement stun timer
                    stunnedTimer -= Time.deltaTime;
                    if (stunnedTimer <= 0) state = State.Idle;
                    break;
                case State.Dead:
                    // Play Death Animation
                    anim.SetTrigger("Died");
                    // STOP MOVING
                    agent.speed = 0f;
                    if (!orbsDroppedOnce)
                    {
                        for (int i = 0; i < UnityEngine.Random.Range(3, 5); i++)
                        {
                            GameObject xp = Instantiate(xpOrb, new Vector3(transform.position.x, transform.position.y, transform.position.z), transform.rotation, null);
                            Vector3 force = new Vector3(UnityEngine.Random.Range(-10f, 10f), 0f, UnityEngine.Random.Range(-10f, 10f)).normalized * 200;
                            xp.GetComponent<Rigidbody>().AddForce(force);
                        }
                        orbsDroppedOnce = true;
                    }
                    // Remove Collider
                    GetComponent<CapsuleCollider>().enabled = false;
                    break;
            }
        }
    }

    void WanderToLocation()
    {
        if (player)
        {
            if (Vector3.Distance(transform.position, player.position) <= 10 || hasSeenPlayer)
            {
                state = State.Chasing;
            }
            else
            {
                NavMeshHit hit;
                anim.SetBool("isMoving", true);
                Vector3 randomDir = UnityEngine.Random.insideUnitSphere * 10;
                randomDir += transform.position;
                NavMesh.SamplePosition(randomDir, out hit, 10, 9);

                if (!isWandering)
                {
                    isWandering = true;
                    wanderDest = hit.position;
                    agent.SetDestination(wanderDest);
                }

                if (Vector3.Distance(transform.position, wanderDest) <= 1.5f)
                {
                    state = State.Idle;
                    agent.SetDestination(transform.position);
                    wanderDelay = 5;
                    isWandering = false;
                }
            }
        }
        else
        {
            state = State.Idle;
            agent.SetDestination(transform.position);
        }
    }

    void ChasePlayer()
    {
        if (player && health > 0 && (Vector3.Distance(transform.position, player.position) <= 10 || hasSeenPlayer))
        {
            hasSeenPlayer = true;
            anim.SetBool("isMoving", true);
            agent.speed = 3.5f;
            transform.LookAt(player);
            if (Vector3.Distance(transform.position, player.position) <= agent.stoppingDistance) state = State.Attacking;
            else agent.SetDestination(player.position);
            if (Vector3.Distance(transform.position, player.position) >= 30) hasSeenPlayer = false;

        }
        else
        {
            state = State.Idle;
            agent.SetDestination(transform.position);
        }
    }

    void AttackPlayer()
    {
        if (player)
        {
            if (timeBetweenAttacks <= 0)
            {
                anim.SetBool("isMoving", false);
                alreadyHitPlayer = false;
                if (Vector3.Distance(transform.position, player.position) <= agent.stoppingDistance)
                {
                    anim.SetTrigger("Attack");
                    timeBetweenAttacks = 3f;
                    agent.speed = 0f;
                }
                else
                {
                    if (!isAttacking && !attackAnimIsPlaying) state = State.Chasing;
                }
            }

            transform.LookAt(player);

            if (timeBetweenAttacks >= 0) timeBetweenAttacks -= Time.fixedDeltaTime;
        }
        else
        {
            state = State.Idle;
        }
    }

    public void resetAttackEnemy()
    {
        state = State.Idle;
        isAttacking = false;
        anim.ResetTrigger("Attack");
        attackAnimIsPlaying = false;
    }

    public void resetHitStun()
    {
        alreadyHitByPlayer = false;
        anim.ResetTrigger("Hit");
    }

    public void resetDamageEnemy()
    {
        state = State.Idle;
        anim.ResetTrigger("Hit");
        alreadyHitByPlayer = false;
        anim.SetBool("HitAgain", false);
        attackAnimIsPlaying = false;
    }
    public void isAttackingSet()
    {
        isAttacking = !isAttacking;
    }

    public void isAttackPlaying()
    {
        attackAnimIsPlaying = true;
    }
}
