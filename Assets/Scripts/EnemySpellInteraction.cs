using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpellInteraction : MonoBehaviour
{
    public PlayerManager player;
    private PlayerMovement playerMove;
    public GameObject collisionFlash;
    public Rigidbody rb;

    private void Start()
    {
        player = FindObjectOfType<PlayerManager>();
        playerMove = player.GetComponent<PlayerMovement>();
        rb = GetComponent<Rigidbody>();
        rb.velocity = transform.forward * 20;
    }

    void OnTriggerEnter(Collider hit)
    {
        if (hit.gameObject.tag == "Player")
        {
            player.playerTargetHealth -= 25;
            player.isInvinc = true;
            player.invincTimer = 5;
            player.anim.SetTrigger("Hit");
            playerMove.playerHit = true;
            Destroy(gameObject);
            Instantiate(collisionFlash, transform.position, transform.rotation);
        }
        if (hit.gameObject.tag != "Prop" && hit.gameObject.tag != "Boss" && hit.gameObject.tag != "Enemy" && hit.gameObject.tag != "Particles" && hit.gameObject.tag != "Ground" && hit.gameObject.tag != "MainCamera" && hit.gameObject.tag != "HitBox")
        {
            Destroy(gameObject);
            Instantiate(collisionFlash, transform.position, transform.rotation);
        }
    }
}
