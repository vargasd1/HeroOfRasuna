using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellInteraction : MonoBehaviour
{
    public Transform player;
    public GameObject collisionFlash;
    public GameObject stunnedPart;
    public GameObject stunPart;
    public Rigidbody rb;
    public string spellType;
    public GameObject fractured;
    public GameObject xpOrb;

    //Grenade Vars
    public Vector3 positionA;
    public Vector3 positionB;
    public Vector3 handle;
    public AnimationCurve tweenSpeed;
    private float percent = 0;
    private float tweenLength = .5f;
    private float tweenTimer = 0;

    private void Start()
    {
        player = FindObjectOfType<PlayerManager>().gameObject.transform;
        if (spellType == "attack")
        {
            rb = GetComponent<Rigidbody>();
            rb.velocity = transform.forward * 20;
        }
    }

    private void Update()
    {
        if (spellType == "grenade")
        {
            tweenTimer += Time.deltaTime;
            float p = tweenTimer / tweenLength;

            percent = tweenSpeed.Evaluate(p);
            transform.position = CalcPositionOnCurve(percent);
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
                    enem.health -= 34;
                    enem.alreadyHitByPlayer = true;
                    enem.anim.SetTrigger("Hit");
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
                        Vector3 force = new Vector3(Random.Range(-10f, 10f), 0f, Random.Range(-10f, 10f)).normalized * 200;
                        xp.GetComponent<Rigidbody>().AddForce(force);
                    }
                    Destroy(hit.gameObject);
                }
                break;
            case "grenade":
                if (hit.gameObject.tag == "Ground")
                {
                    Vector3 targetLoc = new Vector3(transform.position.x, transform.position.y + 0.15f, transform.position.z);
                    GameObject stun = Instantiate(stunPart, targetLoc, Quaternion.Euler(-90, 0, 0)) as GameObject;
                    stun.GetComponent<SpellInteraction>().spellType = "stun";
                    Destroy(gameObject);
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

    private Vector3 CalcPositionOnCurve(float percent)
    {
        // pC = lerp between pA and handle
        Vector3 positionC = AnimMath.Lerp(positionA, handle, percent);

        // pD = lerp between handle and pB
        Vector3 positionD = AnimMath.Lerp(handle, positionB, percent);

        // pF = lerp between pC and pD
        Vector3 positionE = AnimMath.Lerp(positionC, positionD, percent);

        return positionE;
    }
}
