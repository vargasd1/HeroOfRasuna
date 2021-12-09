using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

/// <summary>
/// This script is used to create the Enemies Ranged AI, letting him decide all his choices.
/// 
/// ATTATCHED TO: HoR_RangedEnemy_Base
/// </summary>
public class EnemyRangedAI : MonoBehaviour
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
    bool dieSoundOnce = false;

    // Attack Variables
    private float timeBetweenAttacks = 0f;
    public bool alreadyHitPlayer = false;
    public bool alreadyHitByPlayer = false;
    public float hitDelay = 0f;
    public bool isAttacking = false;
    public bool attackAnimIsPlaying = false;
    public GameObject spellObj;
    private Vector3 pointToLook;

    //Third Floor Variables
    private bool thirdLevel = false;
    private BossGate bossDoor;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
        state = State.Spawning;
        player = FindObjectOfType<PlayerManager>().gameObject.transform;
    }

    private void Start()
    {
        if (SceneManager.GetActiveScene().buildIndex == 3)
        {
            thirdLevel = true;
            bossDoor = FindObjectOfType<BossGate>();
        }
    }

    void Update()
    {
        // If dead, unstun and go to death state
        if (health <= 0)
        {
            anim.SetBool("Stunned", false);
            state = State.Dead;
        }

        // If the player dies, set them to null
        if (player && player.GetComponent<PlayerManager>().isDead) player = null;

        // If game isn't paused
        if (!pauseMenu.GamePaused)
        {
            // If already hit by the player, small delay where they can't take damage again immediatly
            if (alreadyHitByPlayer)
            {
                if (hitDelay <= 0)
                {
                    anim.ResetTrigger("Hit");
                    alreadyHitByPlayer = false;
                }
                else
                {
                    hitDelay -= Time.unscaledDeltaTime;
                }
            }

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

                    // Wander to new location after timer is done, or Chase if Player is in sight, or Attack if close enough
                    if (wanderDelay > 0) wanderDelay -= Time.deltaTime;
                    if (player)
                    {
                        if (Vector3.Distance(transform.position, player.position) > 10 && wanderDelay <= 0)
                        {
                            state = State.Wander;
                        }
                        if (Vector3.Distance(transform.position, player.position) <= 10 || hasSeenPlayer) state = State.Chasing;
                        if (Vector3.Distance(transform.position, player.position) <= 10) state = State.Attacking;
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
                    anim.SetBool("Stunned", true);

                    // Decrement stun timer
                    stunnedTimer -= Time.deltaTime;
                    if (stunnedTimer <= 0)
                    {
                        state = State.Idle;
                        isAttacking = false;
                        anim.ResetTrigger("Attack");
                        attackAnimIsPlaying = false;
                        anim.SetBool("Stunned", false);
                    }
                    break;
                case State.Dead:
                    // Play Death Animation
                    anim.SetBool("Stunned", false);
                    anim.SetTrigger("Died");
                    if (!dieSoundOnce)
                    {
                        FindObjectOfType<AudioManager>().PlayUninterrupted("EnemyDissolve");
                        dieSoundOnce = true;
                    }
                    // STOP MOVING
                    agent.speed = 0f;
                    // Spawn XP
                    if (!orbsDroppedOnce)
                    {
                        for (int i = 0; i < UnityEngine.Random.Range(3, 5); i++)
                        {
                            GameObject xp = Instantiate(xpOrb, new Vector3(transform.position.x, transform.position.y, transform.position.z), transform.rotation, null);
                            Vector3 force = new Vector3(UnityEngine.Random.Range(-10f, 10f), 0f, UnityEngine.Random.Range(-10f, 10f)).normalized * 200;
                            xp.GetComponent<Rigidbody>().AddForce(force);
                        }
                        orbsDroppedOnce = true;
                        player.gameObject.GetComponent<PlayerManager>().enemiesKilled++;
                        if (thirdLevel) bossDoor.enemiesKilled++;
                    }
                    // Remove Collider
                    GetComponent<CapsuleCollider>().enabled = false;
                    GetComponent<NavMeshAgent>().enabled = false;
                    break;
            }
        }
    }

    void WanderToLocation()
    {
        if (player)
        {
            // Chase player if close enough
            if (Vector3.Distance(transform.position, player.position) <= 10 || hasSeenPlayer)
            {
                state = State.Chasing;
            }
            else
            {
                // Find location in circle and set it as it's destination 
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

                if (Vector3.Distance(transform.position, wanderDest) <= agent.stoppingDistance)
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
            // If no player, just stand still
            state = State.Idle;
            agent.SetDestination(transform.position);
        }
    }

    void ChasePlayer()
    {
        if (player && health > 0 && (Vector3.Distance(transform.position, player.position) <= 10 || hasSeenPlayer))
        {
            // look at player and set destination to theirs, if close enough, attack, and if the player is far enough away, give up
            hasSeenPlayer = true;
            anim.SetBool("isMoving", true);
            agent.speed = 3.5f;
            transform.LookAt(player);
            if (Vector3.Distance(transform.position, player.position) >= 30) hasSeenPlayer = false;
            if (Vector3.Distance(transform.position, player.position) <= 10) state = State.Attacking;
            else agent.SetDestination(player.position);
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
            // Stop moving
            agent.SetDestination(transform.position);
            anim.SetBool("isMoving", false);

            if (timeBetweenAttacks <= 0)
            {
                // If the player is still close attack else chase
                if (Vector3.Distance(transform.position, player.position) <= 10)
                {
                    anim.SetTrigger("Attack");
                    timeBetweenAttacks = 10f;
                    agent.speed = 0f;
                }
                else
                {
                    if (!isAttacking && !attackAnimIsPlaying) state = State.Chasing;
                }
            }
            else
            {
                timeBetweenAttacks -= Time.fixedDeltaTime;
            }

            transform.LookAt(player);
        }
        else
        {
            state = State.Idle;
        }
    }

    /// <summary>
    /// This function resets the enemy's attack vars (animation event key)
    /// </summary>
    public void resetAttackEnemy()
    {
        isAttacking = false;
        anim.ResetTrigger("Attack");
        attackAnimIsPlaying = false;
    }

    /// <summary>
    /// This function resets the enemy's damage vars (animation event key)
    /// </summary>
    public void resetDamageEnemy()
    {
        state = State.Idle;
        anim.SetBool("HitAgain", false);
        attackAnimIsPlaying = false;
    }

    /// <summary>
    /// This function plays the hit sound effect and flips isAttacking (animation event key)
    /// </summary>
    public void isAttackingSet()
    {
        isAttacking = !isAttacking;
    }

    /// <summary>
    /// /// This function checks if the attack animation is playing (animation event key)
    /// </summary>
    public void isAttackPlaying()
    {
        attackAnimIsPlaying = true;
    }

    /// <summary>
    /// /// This function spawns a enemy projectile (animation event key)
    /// </summary>
    public void spawnSpell()
    {
        if (state != State.Stunned)
        {
            // Spawn prefab
            GameObject lightBlast = Instantiate(spellObj, transform.position + transform.forward, Quaternion.identity, null) as GameObject;

            // make y same height, so it doesn't fall up or down
            pointToLook = new Vector3(player.transform.position.x, transform.position.y + 1, player.transform.position.z);

            // make lightBlast prefab rotate towards click
            lightBlast.transform.LookAt(pointToLook);
            // addForce in the forward direction so the lightBlast moved towards click
            lightBlast.GetComponent<Rigidbody>().velocity = Vector3.zero + (lightBlast.transform.forward * 25f);
            FindObjectOfType<AudioManager>().PlayUninterrupted("EnemyProjectile");
        }
    }
}
