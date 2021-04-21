using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyMovement : MonoBehaviour
{

    public GameObject player;
    NavMeshAgent agent;
    Transform target;
    public float lookRadius = 10f, waitTime, wonderTime, startWaitTime, wonderRadius = 50f;

    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        target = player.transform;
    }

    // Update is called once per frame
    void Update()
    {
        float distance = Vector3.Distance(target.position, transform.position);

        if(distance <= lookRadius)
        {
            agent.SetDestination(target.position);
            agent.speed = 5;
        }
        else
        {
            Vector3 randPos = RandomNavSphere(transform.position, wonderRadius, -1);
            agent.SetDestination(randPos);
            agent.speed = 1;
            if (target != player.transform) target = player.transform;
        }
    }
    public static Vector3 RandomNavSphere(Vector3 origin, float distance, int layermask)
    {
        Vector3 randomDirection = Random.insideUnitSphere * distance;

        randomDirection += origin;

        NavMeshHit navHit;

        NavMesh.SamplePosition(randomDirection, out navHit, distance, layermask);

        return navHit.position;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, wonderRadius);
    }
}
