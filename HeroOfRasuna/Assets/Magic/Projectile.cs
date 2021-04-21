using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public GameObject collisionFlash;
    public Rigidbody rb;

    

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.velocity = transform.forward * 20;
    }

    void OnTriggerEnter(Collider hit)
    {
        Debug.Log(hit);
        /*
        if (hit.gameObject.tag == "Enemy")
        {
            hit.gameObject.GetComponent<EnemyHealth>().health--;
        }
        */

        if(hit.gameObject.tag != "Player")
        {
            Destroy(gameObject);
            Instantiate(collisionFlash, transform.position, transform.rotation);
        }

    }
}
