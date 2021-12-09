using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// This script is used to start the cutscene when the player picks up the sword.
/// 
/// ATTATCHED TO: SwordPickup
/// </summary>

public class CameraFocusEnemy : MonoBehaviour
{
    // Cutscene Vars
    public GameObject enemyDoor;
    public NavMeshObstacle navMeshBlock;
    public GameObject enemySpawnPoint;
    private PlayerMovement player;
    public GameObject playerSword;

    // Enemy Vars
    public GameObject enemy;
    public GameObject rangedEnemy;
    public GameObject enemyRagdoll;
    private GameObject enemRag;
    public GameObject enemyWallFade;

    // Camera Vars
    private CameraFollow camFol;
    public GameObject enemyFocus;
    public GameObject cameraObject;

    // Misc Vars
    private bool doOnce = false;
    private bool moveDoorUp = false;
    private bool moveDoorDown = false;
    public GameObject particle;
    private float bounceCounter = 0.8f;
    private bool pickedUp = false;

    private void Start()
    {
        player = FindObjectOfType<PlayerMovement>();
        Physics.IgnoreLayerCollision(14, 13);
    }

    private void Update()
    {
        // Move door up or down
        if (moveDoorUp)
        {
            float step = 2 * Time.fixedUnscaledDeltaTime;// Time.deltaTime;
            enemyDoor.transform.position = Vector3.MoveTowards(enemyDoor.transform.position, new Vector3(23, 4, 0), step);
        }
        else if (moveDoorDown)
        {
            float step = 2 * Time.fixedUnscaledDeltaTime;// Time.deltaTime;
            enemyDoor.transform.position = Vector3.MoveTowards(enemyDoor.transform.position, new Vector3(23, 0, 0), step);
        }

        // Bounce and Spin Sword
        if (!pickedUp)
        {
            bounceCounter += Time.deltaTime;
            if (bounceCounter >= Mathf.PI * 2) bounceCounter -= Mathf.PI * 2;
            transform.position = new Vector3(-5, 1 + Mathf.Sin(bounceCounter) * 0.4f, 0);

            transform.Rotate(0, 1, 0);
        }

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            //if overclocked, reset overclock
            if (player.overclock || player.overclockTransition)
            {

                player.overclockTime = 5f;
                player.overclockTransitionTime = 2f;
                player.overclock = false;
                player.overclockTransition = false;
                Time.timeScale = 1f;
                Time.fixedDeltaTime = Time.timeScale * .02f;
                AudioManager aud = FindObjectOfType<AudioManager>();
                aud.Stop("Overclock");
                aud.ChangePitch("Overclock", 1f);
                aud.ResetSounds();
            }

            if (!doOnce) StartCoroutine(panBack());
        }
    }

    IEnumerator panBack()
    {
        // Destroys sword, gives player sword, and make player unable to move
        pickedUp = true;
        transform.position = new Vector3(100, 100, 100);
        Destroy(particle);
        playerSword.SetActive(true);
        player.GetComponent<PlayerManager>().hasSword = true;
        doOnce = true;
        camFol = cameraObject.GetComponent<CameraFollow>();
        camFol.cameraMoveSpeed = 10;
        player.isCutScene = true;

        // Spawn enemies and their wall fade hitbox
        GameObject newEnem = Instantiate(enemy, enemySpawnPoint.transform.position, new Quaternion(0, 0, 0, 0)) as GameObject;
        GameObject enemWallFade = Instantiate(enemyWallFade, newEnem.transform.position, Quaternion.Euler(-45, 225, 0), null) as GameObject;
        enemWallFade.GetComponent<EnemyToWallCollision>().targetEnemy = newEnem;
        GameObject newEnem2 = Instantiate(enemy, enemySpawnPoint.transform.position, new Quaternion(0, 0, 0, 0));
        GameObject enemWallFade2 = Instantiate(enemyWallFade, newEnem2.transform.position, Quaternion.Euler(-45, 225, 0), null) as GameObject;
        enemWallFade2.GetComponent<EnemyToWallCollision>().targetEnemy = newEnem2;

        yield return new WaitForSecondsRealtime(1);

        // Set the enemyFocus gameObject as the target for the camera
        camFol.CameraFollowObject = enemyFocus;

        // Forceablly change enemy states and make them move to location
        newEnem.GetComponent<EnemyAI>().isWandering = true;
        newEnem.GetComponent<EnemyAI>().state = EnemyAI.State.Wander;
        newEnem.GetComponent<EnemyAI>().wanderDest = new Vector3(15, 0, 6);
        newEnem.GetComponent<NavMeshAgent>().SetDestination(new Vector3(15, 0, 6));
        newEnem2.GetComponent<EnemyAI>().isWandering = true;
        newEnem2.GetComponent<EnemyAI>().state = EnemyAI.State.Wander;
        newEnem2.GetComponent<EnemyAI>().wanderDest = new Vector3(15, 0, -6);
        newEnem2.GetComponent<NavMeshAgent>().SetDestination(new Vector3(15, 0, -6));

        yield return new WaitForSecondsRealtime(1);

        // Open Door and remove navMeshBlock
        moveDoorUp = true;
        moveDoorDown = false;
        navMeshBlock.enabled = false;

        yield return new WaitForSecondsRealtime(4);

        // Close Door and enable navMeshBlock
        moveDoorDown = true;
        moveDoorUp = false;
        navMeshBlock.enabled = true;

        // Spawn in Ragdoll
        Quaternion rot = Quaternion.Euler(90, -90, 0);

        enemRag = Instantiate(enemyRagdoll, new Vector3(20, 25, 0), rot);

        yield return new WaitForSecondsRealtime(3);

        // Set camera focus back to the player
        camFol.CameraFollowObject = player.gameObject;

        yield return new WaitForSecondsRealtime(1);

        // Destroy ragdoll and spawn actual ranged enemy in and make idle
        GameObject newEnem3 = Instantiate(rangedEnemy, new Vector3(enemRag.transform.position.x, 0, enemRag.transform.position.z), new Quaternion(0, 0, 0, 0));
        GameObject enemWallFade3 = Instantiate(enemyWallFade, newEnem3.transform.position, Quaternion.Euler(-45, 225, 0), null) as GameObject;
        enemWallFade3.GetComponent<EnemyToWallCollision>().targetEnemy = newEnem3;

        newEnem3.GetComponent<EnemyRangedAI>().state = EnemyRangedAI.State.Idle;

        Destroy(enemRag);
        moveDoorDown = false;
        Destroy(gameObject);

        // Let player move
        player.isCutScene = false;
    }
}
