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
        switch (spellType)
        {
            case "stun":
                if (hit.gameObject.tag == "Enemy" && hit.gameObject.tag != "Particles")
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
            case "attack":
                if (hit.gameObject.tag == "Enemy")
                {
                    hit.gameObject.GetComponent<EnemyAI>().health -= 25;
                }

                if (hit.gameObject.tag != "Player" && hit.gameObject.tag != "Particles" && hit.gameObject.tag != "Ground")
                {
                    Destroy(gameObject);
                    Instantiate(collisionFlash, transform.position, transform.rotation);
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
