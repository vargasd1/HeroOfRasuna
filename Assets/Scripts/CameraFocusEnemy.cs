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
    public GameObject enemy;
    public GameObject rangedEnemy;
    public GameObject enemyRagdoll;
    public GameObject topButtonPart;
    private GameObject enemRag;
    private CameraFollow camFol;
    private bool doOnce = false;
    private bool moveDoorUp = false;
    private bool moveDoorDown = false;
    private bool moveButtonDown = false;
    private AudioManager audioScript;

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

        if (moveButtonDown)
        {
            float step = 0.075f * Time.fixedUnscaledDeltaTime;// Time.deltaTime;
            topButtonPart.transform.position = Vector3.MoveTowards(topButtonPart.transform.position, new Vector3(-5, 0.066f, 0), step);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            player.overclock = false;
            player.overclockTransition = false;
            player.overclockTransitionTime = 2f;
            Time.timeScale = 1f;
            Time.fixedDeltaTime = Time.timeScale * .02f;
            player.overclockTime = 5;
            player.overclockChargedAmt = 0f;
            /*audioScript.Stop("Overclock");
            audioScript.ChangePitch("Overclock", 1f);
            audioScript.ResetSounds();*/
            AudioManager aud = FindObjectOfType<AudioManager>();
            aud.Stop("Overclock");
            aud.ChangePitch("Overclock", 1f);
            aud.ResetSounds();

            if (!doOnce) StartCoroutine(panBack());
        }
    }

    IEnumerator panBack()
    {
        doOnce = true;
        camFol = cameraObject.GetComponent<CameraFollow>();
        camFol.cameraMoveSpeed = 10;
        player.isCutScene = true;
        var newEnem = Instantiate(enemy, enemySpawnPoint.transform.position, new Quaternion(0, 0, 0, 0));
        var newEnem2 = Instantiate(enemy, enemySpawnPoint.transform.position, new Quaternion(0, 0, 0, 0));

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
        moveButtonDown = false;
        topButtonPart.transform.position = new Vector3(-5, 0.066f, 0);

        Quaternion rot = Quaternion.Euler(90, -90, 0);

        enemRag = Instantiate(enemyRagdoll, new Vector3(20, 25, 0), rot);

        yield return new WaitForSecondsRealtime(3);

        camFol.CameraFollowObject = player.gameObject;

        yield return new WaitForSecondsRealtime(1);

        var newEnem3 = Instantiate(rangedEnemy, new Vector3(enemRag.transform.position.x, 0, enemRag.transform.position.z), new Quaternion(0, 0, 0, 0));
        newEnem3.GetComponent<EnemyRangedAI>().state = EnemyRangedAI.State.Idle;
        Destroy(enemRag);
        player.isCutScene = false;
        moveDoorDown = false;
    }
}
