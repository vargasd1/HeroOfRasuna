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
    GameObject shockwaveCharge;

    // Other Vars
    private float bounceCounter = 0.8f;
    public float health = 800;

    private bool lowerWeight = false;
    private bool raiseWeight = false;

    private float resetLayerAmt = 0;
    private float resetLayerAlpha = 0;
    public float attackDelay = 5;

    public bool phaseTwo = false;
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
    }

    // Update is called once per frame
    void Update()
    {
        if (health <= 0)
        {
            state = State.Defeated;
            if (!defeatedOnce)
            {
                FindObjectOfType<AudioManager>().Stop("Boss Fight");
                FindObjectOfType<AudioManager>().Play("Exploration3");
                defeatedOnce = true;
            }
        }
        else
        {
            // Make Surav Bounce up and down
            bounceCounter += Time.deltaTime;
            if (bounceCounter >= Mathf.PI * 2) bounceCounter -= Mathf.PI * 2;
            agent.baseOffset = 1 + Mathf.Sin(bounceCounter) * 0.4f;

            if (player)
            {
                Vector3 lookVector = player.position;
                lookVector.y = transform.position.y;
                transform.LookAt(lookVector);
            }

            if (lowerWeight)
            {
                lowerLayerWeight();
            }

            if (raiseWeight)
            {
                raiseLayerWeight();
            }

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

            if (health <= (health / 2))
            {
                phaseTwo = true;
            }
        }

        switch (state)
        {
            case State.Talking:
                if (startFight) state = State.Idle;
                break;
            case State.Idle:
                //findNewLocation();
                if (attackDelay > 0) attackDelay -= Time.deltaTime;
                if (attackDelay <= 0)
                {
                    pickAttack();
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
                FindObjectOfType<AudioManager>().Stop("SolarFlare");
                break;
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
                    else shockwaveWindUpTimer = 1.5f;
                    break;
                case 4:
                case 5:
                case 6:
                    state = State.MinigunAttack;
                    spawnChargeOnce = true;
                    if (!phaseTwo) minigunWindUp = 1.5f;
                    else minigunWindUp = 0.5f;
                    break;
                case 7:
                case 8:
                    state = State.ShotgunAttack;
                    spawnChargeOnce = true;
                    if (!phaseTwo) shotgunWindUp = 1.5f;
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
                    else shockwaveWindUpTimer = 1.5f;
                    break;
                case 1:
                case 2:
                case 3:
                case 4:
                    state = State.MinigunAttack;
                    spawnChargeOnce = true;
                    if (!phaseTwo) minigunWindUp = 1.5f;
                    else minigunWindUp = 0.5f;
                    break;
                case 5:
                case 6:
                case 7:
                case 8:
                    state = State.ShotgunAttack;
                    spawnChargeOnce = true;
                    if (!phaseTwo) shotgunWindUp = 1.5f;
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
        if (spawnChargeOnce)
        {
            shockwaveCharge = Instantiate(chargeObj, transform.position, Quaternion.identity, null) as GameObject;
            spawnChargeOnce = false;
            FindObjectOfType<AudioManager>().PlayUninterrupted("SolarFlare");
        }

        if (shockwaveWindUpTimer <= 0)
        {
            if (spawnShockOnce)
            {
                GameObject shockwave = Instantiate(solarFlareObj, new Vector3(transform.position.x, 0, transform.position.z), Quaternion.identity, null) as GameObject;
                spawnShockOnce = false;
                state = State.Idle;
                if (!phaseTwo) attackDelay = 5;//check for animation time and reset this
                else attackDelay = 2;
            }
        }
        else
        {
            shockwaveWindUpTimer -= Time.deltaTime;
        }
    }

    private void MinigunAttack()
    {
        if (spawnChargeOnce)
        {
            shockwaveCharge = Instantiate(chargeObj, transform.position, Quaternion.identity, null) as GameObject;
            spawnChargeOnce = false;
        }

        if (minigunWindUp <= 0)
        {
            Destroy(shockwaveCharge);
            startMinigun = true;
        }
        else
        {
            minigunWindUp -= Time.unscaledDeltaTime;
        }

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
                attackDelay = 1;
                shotCount = 0;
                miniGunTimer = 0.3f;
                startMinigun = false;
            }
        }
    }

    private void ShotgunAttack()
    {
        if (spawnChargeOnce)
        {
            shockwaveCharge = Instantiate(chargeObj, transform.position, Quaternion.identity, null) as GameObject;
            spawnChargeOnce = false;
        }

        if (shotgunWindUp <= 0)
        {
            Destroy(shockwaveCharge);
            startShotgun = true;
        }
        else
        {
            shotgunWindUp -= Time.unscaledDeltaTime;
        }

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
                attackDelay = 1;
                shotCount = 0;
                shotGunTimer = 0.75f;
                startShotgun = false;
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

    private void SpawnProjectile()
    {
        // Spawn prefab
        GameObject lightBlast = Instantiate(spellObj, new Vector3(transform.position.x, 1, transform.position.z), Quaternion.identity, null) as GameObject;
        lightBlast.GetComponent<EnemySpellInteraction>().isBossMinigun = true;

        // make y same height, so it doesn't fall up or down
        pointToLook = new Vector3(player.transform.position.x, 1, player.transform.position.z);

        // make lightBlast prefab rotate towards player
        lightBlast.transform.LookAt(pointToLook);
        // addForce in the forward direction so the lightBlast moved towards click
        if (!phaseTwo) lightBlast.GetComponent<Rigidbody>().AddForce(lightBlast.transform.forward * 1000);
        else lightBlast.GetComponent<Rigidbody>().AddForce(lightBlast.transform.forward * 4000);
    }

    private void SpawnTripleProjectile()
    {
        // Spawn prefab
        GameObject lightBlast = Instantiate(spellObj, new Vector3(transform.position.x, 1, transform.position.z), Quaternion.identity, null) as GameObject;
        GameObject lightBlast2 = Instantiate(spellObj, new Vector3(transform.position.x, 1, transform.position.z), Quaternion.identity, null) as GameObject;
        GameObject lightBlast3 = Instantiate(spellObj, new Vector3(transform.position.x, 1, transform.position.z), Quaternion.identity, null) as GameObject;

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
            lightBlast.GetComponent<Rigidbody>().AddForce(lightBlast.transform.forward * 1000);
            lightBlast2.GetComponent<Rigidbody>().AddForce(lightBlast2.transform.forward * 1000);
            lightBlast3.GetComponent<Rigidbody>().AddForce(lightBlast3.transform.forward * 1000);
        }
        else
        {
            lightBlast.GetComponent<Rigidbody>().AddForce(lightBlast.transform.forward * 4000);
            lightBlast2.GetComponent<Rigidbody>().AddForce(lightBlast2.transform.forward * 4000);
            lightBlast3.GetComponent<Rigidbody>().AddForce(lightBlast3.transform.forward * 4000);
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
