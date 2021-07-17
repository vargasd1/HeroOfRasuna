using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    private PlayerManager self;

    void Start()
    {
        self = gameObject.GetComponentInParent<PlayerManager>();
    }

    private void OnTriggerEnter(Collider hit)
    {
        if (hit.tag == "Enemy")
        {
            EnemyAI ai = hit.gameObject.GetComponent<EnemyAI>();
            if (self.attackNum > 0 && !ai.alreadyHitByPlayer)
            {
                ai.health -= 34;
                ai.alreadyHitByPlayer = true;
            }
        }
    }
}
