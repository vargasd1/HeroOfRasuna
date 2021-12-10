using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This script controls when the player is attacking and the interactions with their attack
/// 
/// ATTATCHED TO: Player (AttackHitbox)
/// </summary>
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
        // Checks if player is attacking
        if (playerMove.isAttacking)
        {
            // Checks if it hits an enemy
            if (hit.tag == "Enemy")
            {
                // What kind of enemy was hit
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

                // Deal damage to enemy and set them to hitstun
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
                // Deal damage to boss and give them little invinc
                boss = hit.gameObject.GetComponent<SuravAI>();
                if (!boss.wasHitByPlayer)
                {
                    boss.health -= 20;
                    boss.wasHitByPlayer = true;
                    boss.hitDelay = 0.2f;
                }
            }
            else if (hit.tag == "Prop1")
            {
                // Spawn fractured prop
                GameObject frac = Instantiate(fracturedPot, hit.gameObject.transform.position, Quaternion.identity, null);
                frac.transform.localScale = hit.gameObject.transform.localScale / 100;

                // Play sound
                soundChoice = UnityEngine.Random.Range(0, 2);
                if (soundChoice < 1) FindObjectOfType<AudioManager>().PlayUninterrupted("PotteryBreak1");
                else FindObjectOfType<AudioManager>().PlayUninterrupted("PotteryBreak2");

                // Give some force to pottery
                foreach (Rigidbody rb in frac.GetComponentsInChildren<Rigidbody>())
                {
                    Vector3 force = (rb.transform.position - transform.position).normalized * 75;
                    rb.AddForce(force);
                }

                // Spawn XP
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
                // Spawn fractured prop
                GameObject frac = Instantiate(fracturedBowl, hit.gameObject.transform.position, Quaternion.identity, null);
                frac.transform.localScale = hit.gameObject.transform.localScale / 100;

                // Play sound
                soundChoice = UnityEngine.Random.Range(0, 2);
                if (soundChoice < 1) FindObjectOfType<AudioManager>().PlayUninterrupted("PotteryBreak1");
                else FindObjectOfType<AudioManager>().PlayUninterrupted("PotteryBreak2");

                // Give some force to pottery
                foreach (Rigidbody rb in frac.GetComponentsInChildren<Rigidbody>())
                {
                    Vector3 force = (rb.transform.position - transform.position).normalized * 75;
                    rb.AddForce(force);
                }

                // Spawn XP
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
                // Spawn fractured prop
                GameObject frac = Instantiate(fractruedVase, hit.gameObject.transform.position, Quaternion.identity, null);
                frac.transform.localScale = hit.gameObject.transform.localScale / 100;

                // Play sound
                soundChoice = UnityEngine.Random.Range(0, 2);
                if (soundChoice < 1) FindObjectOfType<AudioManager>().PlayUninterrupted("PotteryBreak1");
                else FindObjectOfType<AudioManager>().PlayUninterrupted("PotteryBreak2");


                // Give some force to pottery
                foreach (Rigidbody rb in frac.GetComponentsInChildren<Rigidbody>())
                {
                    Vector3 force = (rb.transform.position - transform.position).normalized * 75;
                    rb.AddForce(force);
                }

                // Spawn XP
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
