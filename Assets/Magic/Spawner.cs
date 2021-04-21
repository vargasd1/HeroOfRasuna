using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public GameObject projectile;
    
    void Start()
    {
        
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            GameObject lightBlast = Instantiate(projectile, transform.position, transform.rotation, null) as GameObject;
            Rigidbody rb = lightBlast.GetComponent<Rigidbody>();
            rb.velocity = transform.forward * 20;
            //lightBlast.transform.parent = null;
        }
    }
}
