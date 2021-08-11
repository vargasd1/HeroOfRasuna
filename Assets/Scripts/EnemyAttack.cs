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

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag == "Player" && self.isAttacking && !playerManager.isInvinc)
        {
            playerManager.playerTargetHealth -= 25;
            playerManager.isInvinc = true;
            playerManager.anim.SetTrigger("Hit");
            playerMove.playerHit = true;
        }
    }
}
