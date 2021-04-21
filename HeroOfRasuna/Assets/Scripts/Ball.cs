using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour
{
    Rigidbody rb;
    float speed = 3f;

    // Start is called before the first frame update
    void Start()
    {
        rb = gameObject.GetComponent<Rigidbody>();
        rb.velocity = new Vector3(speed, 0f, -speed);
    }

    // Update is called once per frame
    void Update()
    {
        //
    }

    void OnTriggerEnter(Collider other)
    {
        rb.velocity *= -1f;
    }
}
