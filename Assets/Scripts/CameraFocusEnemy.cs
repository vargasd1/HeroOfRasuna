using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CameraFocusEnemy : MonoBehaviour
{
    public GameObject enemyFocus;
    public GameObject cameraObject;
    public GameObject enemyDoor;
    public NavMeshObstacle navMeshBlock;
    public GameObject enemySpawnPoint;
    private PlayerMovement player;
    public GameObject playerSword;
    public GameObject enemy;
    public GameObject rangedEnemy;
    public GameObject enemyRagdoll;
    private GameObject enemRag;
    private CameraFollow camFol;
    public GameObject enemyWallFade;
    private bool doOnce = false;
    private bool moveDoorUp = false;
    private bool moveDoorDown = false;
    private bool moveButtonDown = false;
    private AudioManager audioScript;
    public GameObject particle;
    private float bounceCounter = 0.8f;
    private bool pickedUp = false;

    private void Start()
    {
        player = FindObjectOfType<PlayerMovement>();
        Physics.IgnoreLayerCollision(14, 13);
        audioScript = FindObjectOfType<AudioManager>();
    }

    private void Update()
    {
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
        pickedUp = true;
        transform.position = new Vector3(100, 100, 100);
        Destroy(particle);
        playerSword.SetActive(true);
        player.GetComponent<PlayerManager>().hasSword = true;
        doOnce = true;
        camFol = cameraObject.GetComponent<CameraFollow>();
        camFol.cameraMoveSpeed = 10;
        player.isCutScene = true;
        GameObject newEnem = Instantiate(enemy, enemySpawnPoint.transform.position, new Quaternion(0, 0, 0, 0)) as GameObject;
        GameObject enemWallFade = Instantiate(enemyWallFade, newEnem.transform.position, Quaternion.Euler(-45, 225, 0), null) as GameObject;
        enemWallFade.GetComponent<EnemyToWallCollision>().targetEnemy = newEnem;
        GameObject newEnem2 = Instantiate(enemy, enemySpawnPoint.transform.position, new Quaternion(0, 0, 0, 0));
        GameObject enemWallFade2 = Instantiate(enemyWallFade, newEnem2.transform.position, Quaternion.Euler(-45, 225, 0), null) as GameObject;
        enemWallFade2.GetComponent<EnemyToWallCollision>().targetEnemy = newEnem2;

        yield return new WaitForSecondsRealtime(1);

        camFol.CameraFollowObject = enemyFocus;

        newEnem.GetComponent<EnemyAI>().isWandering = true;
        newEnem.GetComponent<EnemyAI>().state = EnemyAI.State.Wander;
        newEnem.GetComponent<EnemyAI>().wanderDest = new Vector3(15, 0, 6);
        newEnem.GetComponent<NavMeshAgent>().SetDestination(new Vector3(15, 0, 6));
        newEnem2.GetComponent<EnemyAI>().isWandering = true;
        newEnem2.GetComponent<EnemyAI>().state = EnemyAI.State.Wander;
        newEnem2.GetComponent<EnemyAI>().wanderDest = new Vector3(15, 0, -6);
        newEnem2.GetComponent<NavMeshAgent>().SetDestination(new Vector3(15, 0, -6));

        yield return new WaitForSecondsRealtime(1);

        moveDoorUp = true;
        moveDoorDown = false;
        navMeshBlock.enabled = false;
        moveButtonDown = false;

        yield return new WaitForSecondsRealtime(4);

        moveDoorDown = true;
        moveDoorUp = false;
        navMeshBlock.enabled = true;

        Quaternion rot = Quaternion.Euler(90, -90, 0);

        enemRag = Instantiate(enemyRagdoll, new Vector3(20, 25, 0), rot);

        yield return new WaitForSecondsRealtime(3);

        camFol.CameraFollowObject = player.gameObject;

        yield return new WaitForSecondsRealtime(1);

        GameObject newEnem3 = Instantiate(rangedEnemy, new Vector3(enemRag.transform.position.x, 0, enemRag.transform.position.z), new Quaternion(0, 0, 0, 0));
        GameObject enemWallFade3 = Instantiate(enemyWallFade, newEnem3.transform.position, Quaternion.Euler(-45, 225, 0), null) as GameObject;
        enemWallFade3.GetComponent<EnemyToWallCollision>().targetEnemy = newEnem3;

        newEnem3.GetComponent<EnemyRangedAI>().state = EnemyRangedAI.State.Idle;

        Destroy(enemRag);
        player.isCutScene = false;
        moveDoorDown = false;
        Destroy(gameObject);
    }
}
