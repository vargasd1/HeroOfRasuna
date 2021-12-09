using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This script is used to turn on the damage hitbox for the enemy and damage player if close enough
/// 
/// ATTATCHED TO: HoR_Enemy_Base (AttackHitbox)
/// </summary>
public class EnemyAttack : MonoBehaviour
{
    private GameObject player;
    private PlayerManager playerManager;
    private PlayerMovement playerMove;
    private EnemyAI self;

    void Start()
    {
        player = FindObjectOfType<PlayerManager>().gameObject;
        playerManager = player.GetComponent<PlayerManager>();
        playerMove = player.GetComponent<PlayerMovement>();
        self = gameObject.GetComponentInParent<EnemyAI>();
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag == "Player" && self.isAttacking && !playerManager.isInvinc && self.state != EnemyAI.State.Stunned)
        {
            // Damage player and give them invinc and make them do "Hit" animation
            playerManager.playerTargetHealth -= 25;
            playerManager.isInvinc = true;
            playerManager.invincTimer = 2f;
            playerManager.anim.SetTrigger("Hit");
            playerMove.playerHit = true;
        }
    }
}
