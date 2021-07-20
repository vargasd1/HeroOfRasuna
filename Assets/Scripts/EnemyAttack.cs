using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
    private GameObject player;
    private PlayerManager playerManager;
    private CharacterMovementIsometric playerMove;
    private EnemyAI self;

    void Start()
    {
        player = FindObjectOfType<PlayerManager>().gameObject;
        playerManager = player.GetComponent<PlayerManager>();
        playerMove = player.GetComponent<CharacterMovementIsometric>();
        self = gameObject.GetComponentInParent<EnemyAI>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player" && self.anim.GetCurrentAnimatorStateInfo(0).IsName("enemyPunch") && !self.alreadyHitPlayer)
        {
            playerManager.playerTargetHealth -= 25;
            playerMove.playerHit = true;
            playerManager.anim.SetTrigger("Hit");
            self.alreadyHitPlayer = true;
        }
    }
}
