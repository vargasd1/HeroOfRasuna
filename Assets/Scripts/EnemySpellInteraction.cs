using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        surav = FindObjectOfType<SuravAI>().gameObject;
    }

    private void Update()
    {
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
            if (isShockwave)
            {
                player.playerTargetHealth -= 25;
                player.isInvinc = true;
                player.invincTimer = 1f;
            }
            else
            {
                if (isBossMinigun) player.playerTargetHealth -= 15;
                else player.playerTargetHealth -= 25;
                player.isInvinc = true;
                player.invincTimer = 1f;
                player.anim.SetTrigger("Hit");
                playerMove.playerHit = true;
                Destroy(gameObject);
                Instantiate(collisionFlash, transform.position, transform.rotation);
            }
        }
        if (hit.gameObject.tag != "Prop" && hit.gameObject.tag != "Boss" && hit.gameObject.tag != "Enemy" && hit.gameObject.tag != "Particles" && hit.gameObject.tag != "Ground" && hit.gameObject.tag != "MainCamera" && hit.gameObject.tag != "HitBox")
        {
            if (!isShockwave)
            {
                Destroy(gameObject);
                Instantiate(collisionFlash, transform.position, transform.rotation);
            }
        }
    }
}
