using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    private PlayerManager self;
    public GameObject fractured;
    private CharacterMovementIsometric playerMove;
    public GameObject xpOrb;

    void Start()
    {
        self = gameObject.GetComponentInParent<PlayerManager>();
        playerMove = gameObject.GetComponentInParent<CharacterMovementIsometric>();
    }

    private void OnTriggerStay(Collider hit)
    {
        if (playerMove.isAttacking)
        {
            if (hit.tag == "Enemy")
            {
                EnemyAI ai = hit.gameObject.GetComponent<EnemyAI>();
                if (!ai.alreadyHitByPlayer)
                {
                    ai.health -= 34;
                    ai.alreadyHitByPlayer = true;
                    ai.anim.SetTrigger("Hit");
                    ai.state = EnemyAI.State.hitStunned;
                }
            }
            else if (hit.tag == "Prop")
            {
                GameObject frac = Instantiate(fractured, hit.gameObject.transform.position, hit.gameObject.transform.rotation, null);
                foreach (Rigidbody rb in frac.GetComponentsInChildren<Rigidbody>())
                {
                    Vector3 force = (rb.transform.position - transform.position).normalized * 75;
                    rb.AddForce(force);
                }
                for (int i = 0; i < Random.Range(2, 4); i++)
                {
                    GameObject xp = Instantiate(xpOrb, new Vector3(transform.position.x, transform.position.y, transform.position.z), transform.rotation, null);
                    Vector3 force = new Vector3(Random.Range(-10f, 10f), 0f, Random.Range(-10f, 10f)).normalized * 200;
                    xp.GetComponent<Rigidbody>().AddForce(force);
                }
                Destroy(hit.gameObject);
            }
        }
    }
}
