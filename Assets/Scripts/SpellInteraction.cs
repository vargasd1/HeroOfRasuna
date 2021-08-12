using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellInteraction : MonoBehaviour
{
    public Transform player;
    public GameObject collisionFlash;
    public GameObject stunnedPart;
    public Rigidbody rb;
    public string spellType;
    public GameObject fractured;
    public GameObject xpOrb;

    private void Start()
    {
        if (spellType == "attack")
        {
            rb = GetComponent<Rigidbody>();
            rb.velocity = transform.forward * 20;
        }
    }

    void OnTriggerEnter(Collider hit)
    {
        EnemyAI enem = hit.gameObject.GetComponent<EnemyAI>();
        switch (spellType)
        {
            case "stun":
                if (hit.gameObject.tag == "Enemy" && hit.gameObject.tag != "Particles")
                {

                    if (enem.state != EnemyAI.State.Stunned)
                    {
                        enem.stunnedTimer = 4f;
                        enem.state = EnemyAI.State.Stunned;
                        Vector3 stunnedPos = new Vector3(enem.transform.position.x, enem.transform.position.y + 2, enem.transform.position.z);
                        Instantiate(stunnedPart, stunnedPos, Quaternion.Euler(-90, 0, 0), enem.gameObject.transform);
                    }
                }
                break;
            case "attack":
                if (hit.gameObject.tag == "Enemy")
                {
                    enem.health -= 25;
                    enem.state = EnemyAI.State.hitStunned;
                }
                if (hit.gameObject.tag != "Player" && hit.gameObject.tag != "Particles" && hit.gameObject.tag != "Ground" && hit.gameObject.tag != "MainCamera" && hit.gameObject.tag != "HitBox")
                {
                    Destroy(gameObject);
                    Instantiate(collisionFlash, transform.position, transform.rotation);
                }
                if (hit.tag == "Prop")
                {
                    GameObject frac = Instantiate(fractured, hit.gameObject.transform.position, hit.gameObject.transform.rotation, null);
                    foreach (Rigidbody rb in frac.GetComponentsInChildren<Rigidbody>())
                    {
                        Vector3 force = (rb.transform.position - transform.position).normalized * 75;
                        rb.AddForce(force);
                    }
                    for (int i = 0; i < Random.Range(2, 4); i++)
                    {
                        GameObject xp = Instantiate(xpOrb, new Vector3(transform.position.x, transform.position.y, transform.position.z), transform.rotation, null);
                        Vector3 force = new Vector3(Random.Range(-5f, 5f), 0f, Random.Range(-5f, 5f)).normalized * 150;
                        xp.GetComponent<Rigidbody>().AddForce(force);
                    }
                    Destroy(hit.gameObject);
                }
                break;
        }
    }

    private void OnCollisionStay(Collision hit)
    {
        switch (spellType)
        {
            case "stun":
                if (hit.gameObject.tag == "Enemy")
                {
                    EnemyAI enem = hit.gameObject.GetComponent<EnemyAI>();
                    if (enem.state != EnemyAI.State.Stunned)
                    {
                        enem.stunnedTimer = 4f;
                        enem.state = EnemyAI.State.Stunned;
                        Vector3 stunnedPos = new Vector3(enem.transform.position.x, enem.transform.position.y + 2, enem.transform.position.z);
                        Instantiate(stunnedPart, stunnedPos, Quaternion.Euler(-90, 0, 0), enem.gameObject.transform);
                    }
                }
                break;
        }
    }
}
