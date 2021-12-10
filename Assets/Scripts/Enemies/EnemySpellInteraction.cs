using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// This script tells the enemy spells what to do once they're spawned
/// 
/// ATTATCHED TO: VFX_SolarFlare and VFX_EnemyRanged (prefabs)
/// </summary>
public class EnemySpellInteraction : MonoBehaviour
{
    public PlayerManager player;
    private PlayerMovement playerMove;
    public GameObject collisionFlash;
    public Rigidbody rb;
    public bool isBossMinigun = false;
    private float killTimer = 10f;
    public bool isShockwave = false;
    public bool isChargeUp = false;
    private GameObject surav;

    private void Start()
    {
        player = FindObjectOfType<PlayerManager>();
        playerMove = player.GetComponent<PlayerMovement>();
        rb = GetComponent<Rigidbody>();

        // If third floor, get Surav info
        if (SceneManager.GetActiveScene().buildIndex == 3) surav = FindObjectOfType<SuravAI>().gameObject;
    }

    private void Update()
    {
        // If the projectile goes out of map or breaks, just destroy gameobject after some time
        if (killTimer <= 0)
        {
            Destroy(gameObject);
        }
        else
        {
            killTimer -= Time.deltaTime;
        }

        if (isChargeUp)
        {
            transform.position = surav.transform.position;
        }
    }

    void OnTriggerEnter(Collider hit)
    {
        if (hit.gameObject.tag == "Player" && !player.isInvinc)
        {
            // If it's the shockwave attack spawned by surav deal 25 damage to player
            if (isShockwave)
            {
                player.playerTargetHealth -= 25;
                player.isInvinc = true;
                player.invincTimer = 1f;
            }
            else
            {
                // If it's suravs minigun attack, deal less damage, else deal 25 damage to player
                if (isBossMinigun) player.playerTargetHealth -= 15;
                else player.playerTargetHealth -= 25;

                // Set player to hit state
                player.isInvinc = true;
                player.invincTimer = 1f;
                player.anim.SetTrigger("Hit");
                playerMove.playerHit = true;

                // Destroy projectile and spawn impact flash
                Destroy(gameObject);
                Instantiate(collisionFlash, transform.position, transform.rotation);
            }
        }
        if (hit.gameObject.tag != "Prop" && hit.gameObject.tag != "Boss" && hit.gameObject.tag != "Enemy" && hit.gameObject.tag != "Particles" && hit.gameObject.tag != "Ground" && hit.gameObject.tag != "MainCamera" && hit.gameObject.tag != "HitBox")
        {
            // If hits anything destroy gameObject if it's not shockwave
            if (!isShockwave)
            {
                Destroy(gameObject);
                Instantiate(collisionFlash, transform.position, transform.rotation);
            }
        }
    }
}
