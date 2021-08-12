﻿using System.Collections;
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
        Stunned,
        hitStunned,
        Dead,
    }
    public State state;

    // Navigation Variables
    public Transform player;
    private NavMeshAgent agent;
    public Animator anim;

    // Health Variables
    public float health = 100f;
    public float stunnedTimer = 0f;
    private float spawnTimer = 2f;

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
        if (!player.GetComponent<PlayerManager>().isPaused)
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
                    agent.speed = 3.5f;
                    agent.acceleration = 8f;
                    anim.SetBool("isMoving", false);
                    if (player)
                    {
                        if (Vector3.Distance(transform.position, player.position) <= agent.stoppingDistance + 1f) state = State.Attacking;
                        else state = State.Chasing;
                    }
                    break;
                case State.Chasing:
                    ChasePlayer();
                    break;
                case State.Attacking:
                    AttackPlayer();
                    break;
                case State.hitStunned:
                    agent.speed = 0f;
                    agent.acceleration = 1000f;
                    anim.SetTrigger("Hit");
                    break;
                case State.Stunned:
                    // Stops moving instantly
                    agent.speed = 0f;
                    agent.acceleration = 1000f;
                    anim.SetBool("isMoving", false);
                    // Decrement stun timer
                    stunnedTimer -= Time.fixedDeltaTime;
                    if (stunnedTimer <= 0) state = State.Idle;
                    break;
                case State.Dead:
                    // Play Death Animation
                    anim.SetTrigger("Died");
                    // STOP MOVING
                    agent.speed = 0f;
                    agent.acceleration = 1000f;
                    // Remove Collider
                    GetComponent<CapsuleCollider>().enabled = false;
                    break;
            }
        }
    }

    void ChasePlayer()
    {
        if (player && health > 0)
        {
            anim.SetBool("isMoving", true);
            agent.speed = 3.5f;
            agent.acceleration = 8f;
            agent.SetDestination(player.position);
            transform.LookAt(player);
            if (agent.remainingDistance <= agent.stoppingDistance) state = State.Attacking;
        }
        else
        {
            state = State.Idle;
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
                if (Vector3.Distance(transform.position, player.position) <= agent.stoppingDistance + 1f)
                {
                    anim.SetTrigger("Attack");
                    timeBetweenAttacks = 3f;
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

    public void resetDamageEnemy()
    {
        state = State.Idle;
        anim.ResetTrigger("Hit");
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
