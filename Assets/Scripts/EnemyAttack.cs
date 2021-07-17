using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
    private GameObject player;
    private EnemyAI self;

    void Start()
    {
        player = FindObjectOfType<PlayerManager>().gameObject;
        self = gameObject.GetComponentInParent<EnemyAI>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player" && self.anim.GetCurrentAnimatorStateInfo(0).IsName("enemyPunch") && !self.alreadyHitPlayer)
        {
            player.GetComponent<PlayerManager>().playerTargetHealth -= 25;
            self.alreadyHitPlayer = true;
        }
    }
}
