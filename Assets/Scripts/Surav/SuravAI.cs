﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// This script is used to create Surav's AI, letting him decide all his choices.
/// 
/// ATTATCHED TO: Surav_Corrupted_Base (Prefab)
/// </summary>
public class SuravAI : MonoBehaviour
{
    // State Machine Variables
    public enum State
    {
        Talking,
        Idle,
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

    // Attack Vars
    public GameObject spellObj;
    public GameObject solarFlareObj;
    public GameObject chargeObj;
    public GameObject meteorObj;
    private Vector3 pointToLook;
    private float miniGunTimer = 0.3f;
    private float shotGunTimer = 0.75f;
    private float shotCount = 0;
    private float shockwaveWindUpTimer = 4;
    private bool spawnChargeOnce = true;
    private bool spawnShockOnce = true;
    private bool spawnMeteorOnce = true;
    public bool wasHitByPlayer = false;
    public float hitDelay = 0f;
    private float minigunWindUp = 2f;
    private float shotgunWindUp = 2f;
    private bool startMinigun = false;
    private bool startShotgun = false;
    private GameObject shockwaveCharge;
    public float stunTimer = 0;
    public PlayerMovement playerMove;

    // Other Vars
    private float bounceCounter = 0.8f;
    public float health = 500;

    public float attackDelay = 5;

    public bool phaseTwo = false;
    public bool applyTextureOnce = true;
    public SkinnedMeshRenderer[] skinnedRends;
    public MeshRenderer[] meshRends;
    public Material phaseTwoText;
    bool defeatedOnce = false;

    // Intro Cutscene Var
    public bool startFight = false;
    public GameObject bossRoomObst;
    public GameObject mainRoomObst;

    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
        state = SuravAI.State.Talking;
        player = FindObjectOfType<PlayerManager>().gameObject.transform;
        playerMove = FindObjectOfType<PlayerMovement>();
    }

    // Update is called once per frame
    void Update()
    {
        // Make Surav Bounce up and down
        bounceCounter += Time.deltaTime;
        if (bounceCounter >= Mathf.PI * 2) bounceCounter -= Mathf.PI * 2;
        agent.baseOffset = 1 + Mathf.Sin(bounceCounter) * 0.4f;

        if (health <= 0)
        {
            state = State.Defeated;
            if (!defeatedOnce)
            {
                // Stops overclock
                AudioManager aud = FindObjectOfType<AudioManager>();
                if (playerMove.overclock || playerMove.overclockTransition)
                {
                    playerMove.overclockTime = 5f;
                    playerMove.overclockTransitionTime = 2f;
                    playerMove.overclock = false;
                    playerMove.overclockTransition = false;
                    Time.timeScale = 1f;
                    Time.fixedDeltaTime = Time.timeScale * .02f;
                    aud.Stop("Overclock");
                    aud.ChangePitch("Overclock", 1f);
                    aud.ResetSounds();
                }
                NextFloorPlayer.ResetValues();
                if (shockwaveCharge != null) Destroy(shockwaveCharge);
                aud.Stop("SolarFlare");
                aud.Stop("ArrowRain");
                aud.Stop("EnemyProjectile");
                aud.Stop("Boss Fight");
                aud.Play("Exploration3");
                defeatedOnce = true;
            }
        }
        else
        {
            // Start second phase and apply red shader
            if (health <= 400)
            {
                if (applyTextureOnce)
                {
                    phaseTwo = true;
                    foreach (SkinnedMeshRenderer i in skinnedRends)
                    {
                        i.material = phaseTwoText;
                    }
                    foreach (MeshRenderer i in meshRends)
                    {
                        i.material = phaseTwoText;
                    }
                    applyTextureOnce = false;
                }
            }

            // Always look at player
            if (player)
            {
                Vector3 lookVector = player.position;
                lookVector.y = transform.position.y;
                transform.LookAt(lookVector);
            }

            // Hit timer
            if (wasHitByPlayer)
            {
                if (hitDelay <= 0)
                {
                    wasHitByPlayer = false;
                }
                else
                {
                    hitDelay -= Time.unscaledDeltaTime;
                }
            }

            if (stunTimer <= 0)
            {
                switch (state)
                {
                    case State.Talking:
                        if (startFight) state = State.Idle;
                        break;
                    case State.Idle:
                        if (attackDelay > 0)
                        {
                            attackDelay -= Time.deltaTime;
                            findNewLocation();
                        }
                        else
                        {
                            pickAttack();
                            isWandering = false;
                        }
                        break;
                    case State.MinigunAttack:
                        MinigunAttack();
                        break;
                    case State.ShotgunAttack:
                        ShotgunAttack();
                        break;
                    case State.ShockwaveAttack:
                        ShockwaveAttack();
                        break;
                    case State.MeteorShower:
                        MeteorAttack();
                        break;
                    case State.Defeated:
                        break;
                }
            }
            else
            {
                // If is stunned, resets vars
                state = State.Idle;
                attackDelay = 2f;
                stunTimer -= Time.deltaTime;
                Destroy(shockwaveCharge);
                agent.SetDestination(transform.position);

                spawnShockOnce = true;
                spawnChargeOnce = true;
                spawnMeteorOnce = true;

                if (!phaseTwo) shockwaveWindUpTimer = 3;
                else shockwaveWindUpTimer = 1f;

                if (!phaseTwo) minigunWindUp = 2f;
                else minigunWindUp = 0.5f;

                if (!phaseTwo) shotgunWindUp = 2f;
                else shotgunWindUp = 0.5f;

                shotCount = 0;
                miniGunTimer = 0.3f;
                startMinigun = false;
                shotGunTimer = 0.75f;
                startShotgun = false;
            }
        }
    }

    private void pickAttack()
    {
        int rand = Mathf.FloorToInt(UnityEngine.Random.Range(0, 10));

        // If the player is "close"
        if (Vector3.Distance(player.transform.position, transform.position) <= 5)
        {
            switch (rand)
            {
                case 0:
                case 1:
                case 2:
                case 3:
                    state = State.ShockwaveAttack;
                    spawnShockOnce = true;
                    spawnChargeOnce = true;
                    if (!phaseTwo) shockwaveWindUpTimer = 3;
                    else shockwaveWindUpTimer = 1f;
                    break;
                case 4:
                case 5:
                case 6:
                    state = State.MinigunAttack;
                    spawnChargeOnce = true;
                    if (!phaseTwo) minigunWindUp = 2f;
                    else minigunWindUp = 0.5f;
                    break;
                case 7:
                case 8:
                    state = State.ShotgunAttack;
                    spawnChargeOnce = true;
                    if (!phaseTwo) shotgunWindUp = 2f;
                    else shotgunWindUp = 0.5f;
                    break;
                case 9:
                    state = State.MeteorShower;
                    spawnMeteorOnce = true;
                    break;
            }
        }
        else // If player is "far"
        {
            switch (rand)
            {
                case 0:
                    state = State.ShockwaveAttack;
                    spawnShockOnce = true;
                    spawnChargeOnce = true;
                    if (!phaseTwo) shockwaveWindUpTimer = 3;
                    else shockwaveWindUpTimer = 1f;
                    break;
                case 1:
                case 2:
                case 3:
                case 4:
                    state = State.MinigunAttack;
                    spawnChargeOnce = true;
                    if (!phaseTwo) minigunWindUp = 2f;
                    else minigunWindUp = 0.5f;
                    break;
                case 5:
                case 6:
                case 7:
                case 8:
                    state = State.ShotgunAttack;
                    spawnChargeOnce = true;
                    if (!phaseTwo) shotgunWindUp = 2f;
                    else shotgunWindUp = 0.5f;
                    break;
                case 9:
                    state = State.MeteorShower;
                    spawnMeteorOnce = true;
                    break;
            }
        }
    }

    private void ShockwaveAttack()
    {
        // Spawn charge up
        if (spawnChargeOnce)
        {
            shockwaveCharge = Instantiate(chargeObj, transform.position, Quaternion.identity, null) as GameObject;
            shockwaveCharge.GetComponent<EnemySpellInteraction>().isChargeUp = true;
            spawnChargeOnce = false;
            FindObjectOfType<AudioManager>().PlayUninterrupted("SolarFlare");
        }

        // Spawn shockwave
        if (shockwaveWindUpTimer <= 0 && health > 0)
        {
            if (spawnShockOnce)
            {
                GameObject shockwave = Instantiate(solarFlareObj, new Vector3(transform.position.x, 0, transform.position.z), Quaternion.identity, null) as GameObject;
                shockwave.GetComponentInChildren<EnemySpellInteraction>().isShockwave = true;
                spawnShockOnce = false;
                state = State.Idle;
                if (!phaseTwo) attackDelay = 4;//check for animation time and reset this
                else attackDelay = 1.5f;
            }
            Destroy(shockwaveCharge);
        }
        else
        {
            shockwaveWindUpTimer -= Time.deltaTime;
        }
    }

    private void MinigunAttack()
    {
        // Spawn charge up
        if (spawnChargeOnce)
        {
            shockwaveCharge = Instantiate(chargeObj, transform.position, Quaternion.identity, null) as GameObject;
            shockwaveCharge.GetComponent<EnemySpellInteraction>().isChargeUp = true;
            spawnChargeOnce = false;
        }

        // Wind up before shooting
        if (minigunWindUp <= 0)
        {
            startMinigun = true;
        }
        else
        {
            minigunWindUp -= Time.unscaledDeltaTime;
        }

        // Start spawning projectiles
        if (startMinigun)
        {
            if (miniGunTimer > 0) miniGunTimer -= Time.deltaTime;

            if (miniGunTimer <= 0 && shotCount < 10)
            {
                SpawnProjectile();
                miniGunTimer = 0.25f;
                shotCount++;
                FindObjectOfType<AudioManager>().PlayUninterrupted("EnemyProjectile");
            }
            else if (miniGunTimer <= 0 && shotCount >= 10)
            {
                state = State.Idle;
                attackDelay = 1.5f;
                shotCount = 0;
                miniGunTimer = 0.3f;
                startMinigun = false;
                Destroy(shockwaveCharge);
            }
        }
    }

    private void ShotgunAttack()
    {
        // Spawn charge up
        if (spawnChargeOnce)
        {
            shockwaveCharge = Instantiate(chargeObj, transform.position, Quaternion.identity, null) as GameObject;
            shockwaveCharge.GetComponent<EnemySpellInteraction>().isChargeUp = true;
            spawnChargeOnce = false;
        }

        // Wind up before shooting
        if (shotgunWindUp <= 0)
        {
            startShotgun = true;
        }
        else
        {
            shotgunWindUp -= Time.unscaledDeltaTime;
        }

        // Starts spawning projectiles
        if (startShotgun)
        {
            if (shotGunTimer > 0) shotGunTimer -= Time.deltaTime;

            if (shotGunTimer <= 0 && shotCount < 4)
            {
                SpawnTripleProjectile();
                shotGunTimer = 0.75f;
                shotCount++;
                FindObjectOfType<AudioManager>().PlayUninterrupted("EnemyProjectile");
            }
            else if (shotGunTimer <= 0 && shotCount >= 4)
            {
                state = State.Idle;
                attackDelay = 1.5f;
                shotCount = 0;
                shotGunTimer = 0.75f;
                startShotgun = false;
                Destroy(shockwaveCharge);
            }
        }

    }

    private void MeteorAttack()
    {
        // Spawn prefab
        if (spawnMeteorOnce)
        {
            GameObject meteor = Instantiate(meteorObj, new Vector3(player.transform.position.x, 0, player.transform.position.z), Quaternion.identity, null) as GameObject;
            spawnMeteorOnce = false;
            state = State.Idle;
            FindObjectOfType<AudioManager>().PlayUninterrupted("ArrowRain");
            if (!phaseTwo) attackDelay = 3;//check for animation time and reset this
            else attackDelay = 1;
        }
    }

    private void findNewLocation()
    {
        // Find random position
        NavMeshHit hit;
        Vector3 randomDir = UnityEngine.Random.insideUnitSphere * 20;
        randomDir += transform.position;
        NavMesh.SamplePosition(randomDir, out hit, 20, 9);

        // Move to position
        if (!isWandering)
        {
            isWandering = true;
            wanderDest = hit.position;
            agent.SetDestination(wanderDest);
        }

        if (Vector3.Distance(transform.position, wanderDest) <= 2f)
        {
            agent.SetDestination(transform.position);
        }
    }

    private void SpawnProjectile()
    {
        // Spawn prefab
        GameObject lightBlast = Instantiate(spellObj, new Vector3(transform.position.x, 1, transform.position.z) + transform.forward, Quaternion.identity, null) as GameObject;
        lightBlast.GetComponent<EnemySpellInteraction>().isBossMinigun = true;

        // make y same height, so it doesn't fall up or down
        pointToLook = new Vector3(player.transform.position.x, 1, player.transform.position.z);

        // make lightBlast prefab rotate towards player
        lightBlast.transform.LookAt(pointToLook);

        // addForce in the forward direction so the lightBlast moved towards click
        if (!phaseTwo) lightBlast.GetComponent<Rigidbody>().velocity = (lightBlast.transform.forward * 25f * (2f / 3f));
        else lightBlast.GetComponent<Rigidbody>().velocity = (lightBlast.transform.forward * 25f * (8f / 3f));
    }

    private void SpawnTripleProjectile()
    {
        // Spawn prefab
        GameObject lightBlast = Instantiate(spellObj, new Vector3(transform.position.x, 1, transform.position.z) + transform.forward, Quaternion.identity, null) as GameObject;
        GameObject lightBlast2 = Instantiate(spellObj, new Vector3(transform.position.x, 1, transform.position.z) + transform.forward, Quaternion.identity, null) as GameObject;
        GameObject lightBlast3 = Instantiate(spellObj, new Vector3(transform.position.x, 1, transform.position.z) + transform.forward, Quaternion.identity, null) as GameObject;

        // make y same height, so it doesn't fall up or down
        pointToLook = new Vector3(player.transform.position.x, 1, player.transform.position.z);

        // make lightBlast prefab rotate towards player
        lightBlast.transform.LookAt(pointToLook);
        lightBlast2.transform.LookAt(pointToLook);
        lightBlast3.transform.LookAt(pointToLook);

        lightBlast2.transform.Rotate(0, 45, 0);
        lightBlast3.transform.Rotate(0, -45, 0);

        // addForce in the forward direction so the lightBlast moved towards click
        if (!phaseTwo)
        {
            lightBlast.GetComponent<Rigidbody>().velocity = (lightBlast.transform.forward * 25f);
            lightBlast2.GetComponent<Rigidbody>().velocity = (lightBlast2.transform.forward * 25f);
            lightBlast3.GetComponent<Rigidbody>().velocity = (lightBlast3.transform.forward * 25f);
        }
        else
        {
            lightBlast.GetComponent<Rigidbody>().velocity = (lightBlast.transform.forward * 25f * (8f / 3f));
            lightBlast2.GetComponent<Rigidbody>().velocity = (lightBlast2.transform.forward * 25f * (8f / 3f));
            lightBlast3.GetComponent<Rigidbody>().velocity = (lightBlast3.transform.forward * 25f * (8f / 3f));
        }
    }
}
