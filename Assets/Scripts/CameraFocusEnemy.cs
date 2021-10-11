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
    private GameObject enemRag;
    private CameraFollow camFol;
    private bool doOnce = false;
    private bool moveDoorUp = false;
    private bool moveDoorDown = false;

    private void Start()
    {
        player = FindObjectOfType<PlayerMovement>();
        Physics.IgnoreLayerCollision(14, 13);
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
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            if (!doOnce) StartCoroutine(panBack());
        }
    }

    IEnumerator panBack()
    {
        doOnce = true;
        camFol = cameraObject.GetComponent<CameraFollow>();
        camFol.cameraMoveSpeed = 10;
        camFol.CameraFollowObject = enemyFocus;
        player.isCutScene = true;
        var newEnem = Instantiate(enemy, enemySpawnPoint.transform.position, new Quaternion(0, 0, 0, 0));
        var newEnem2 = Instantiate(enemy, enemySpawnPoint.transform.position, new Quaternion(0, 0, 0, 0));

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

        yield return new WaitForSecondsRealtime(4);

        moveDoorDown = true;
        moveDoorUp = false;
        navMeshBlock.enabled = true;

        Quaternion rot = Quaternion.Euler(90, -90, 0);

        enemRag = Instantiate(enemyRagdoll, new Vector3(20, 25, 0), rot);

        yield return new WaitForSecondsRealtime(3);

        camFol.CameraFollowObject = player.gameObject;

        yield return new WaitForSecondsRealtime(1);

        var newEnem3 = Instantiate(rangedEnemy, new Vector3(enemRag.transform.position.x, 0, enemRag.transform.position.z), new Quaternion(0, 0, 0, 0));
        newEnem3.GetComponent<EnemyRangedAI>().state = EnemyRangedAI.State.Idle;
        Destroy(enemRag);
        player.isCutScene = false;

    }
}
