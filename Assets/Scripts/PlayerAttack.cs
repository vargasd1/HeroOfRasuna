using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    private PlayerManager self;
    public GameObject fractured;
    private CharacterMovementIsometric playerMove;

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
                if (self.attackNum > 0 && !ai.alreadyHitByPlayer)
                {
                    ai.health -= 34;
                    ai.alreadyHitByPlayer = true;
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
                Destroy(hit.gameObject);
            }
        }
    }
}
