﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    private PlayerManager self;
    public GameObject fracturedPot;
    public GameObject fracturedBowl;
    public GameObject fractruedVase;
    private PlayerMovement playerMove;
    public GameObject xpOrb;
    private bool enemIsRanged = false;
    private EnemyAI enem;
    private EnemyRangedAI enemR;
    private SuravAI boss;
    private float soundChoice;

    void Start()
    {
        self = gameObject.GetComponentInParent<PlayerManager>();
        playerMove = gameObject.GetComponentInParent<PlayerMovement>();
    }

    private void OnTriggerStay(Collider hit)
    {
        if (playerMove.isAttacking)
        {
            if (hit.tag == "Enemy")
            {
                if (hit.gameObject.GetComponent<EnemyAI>() != null)
                {
                    enem = hit.gameObject.GetComponent<EnemyAI>();
                    enemIsRanged = false;
                }
                else if (hit.gameObject.GetComponent<EnemyRangedAI>() != null)
                {
                    enemR = hit.gameObject.GetComponent<EnemyRangedAI>();
                    enemIsRanged = true;
                }

                if (enemIsRanged)
                {
                    if (!enemR.alreadyHitByPlayer)
                    {
                        enemR.health -= 34;
                        enemR.hitDelay = 0.15f;
                        enemR.alreadyHitByPlayer = true;
                        enemR.anim.SetTrigger("Hit");
                        enemR.state = EnemyRangedAI.State.hitStunned;
                    }
                }
                else
                {
                    if (!enem.alreadyHitByPlayer)
                    {
                        enem.health -= 34;
                        enem.hitDelay = 0.15f;
                        enem.alreadyHitByPlayer = true;
                        enem.anim.SetTrigger("Hit");
                        enem.state = EnemyAI.State.hitStunned;
                    }
                }
            }
            else if (hit.tag == "Boss")
            {
                boss = hit.gameObject.GetComponent<SuravAI>();
                if (!boss.wasHitByPlayer)
                {
                    boss.health -= 800;
                    //boss.health -= 20;
                    boss.wasHitByPlayer = true;
                    boss.hitDelay = 0.2f;
                }
            }
            else if (hit.tag == "Prop1")
            {
                GameObject frac = Instantiate(fracturedPot, hit.gameObject.transform.position, Quaternion.identity, null);
                frac.transform.localScale = hit.gameObject.transform.localScale/100;
                soundChoice = UnityEngine.Random.Range(0, 2);
                if (soundChoice < 1) FindObjectOfType<AudioManager>().PlayUninterrupted("PotteryBreak1");
                else FindObjectOfType<AudioManager>().PlayUninterrupted("PotteryBreak2");
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

            else if (hit.tag == "Prop2")
            {
                GameObject frac = Instantiate(fracturedBowl, hit.gameObject.transform.position, Quaternion.identity, null);
                soundChoice = UnityEngine.Random.Range(0, 2);
                if (soundChoice < 1) FindObjectOfType<AudioManager>().PlayUninterrupted("PotteryBreak1");
                else FindObjectOfType<AudioManager>().PlayUninterrupted("PotteryBreak2");
                frac.transform.localScale = hit.gameObject.transform.localScale/100;
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
            else if (hit.tag == "Prop3")
            {
                GameObject frac = Instantiate(fractruedVase, hit.gameObject.transform.position, Quaternion.identity, null);
                soundChoice = UnityEngine.Random.Range(0, 2);
                if (soundChoice < 1) FindObjectOfType<AudioManager>().PlayUninterrupted("PotteryBreak1");
                else FindObjectOfType<AudioManager>().PlayUninterrupted("PotteryBreak2");
                frac.transform.localScale = hit.gameObject.transform.localScale/100;
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
