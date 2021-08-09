using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class propBreaking : MonoBehaviour
{
    public GameObject fractured;


    public void OnTriggerEnter(Collider hit)
    {
        if(hit.tag == "Player"){
            breakIt();
        }
    }

    public void breakIt()
    {
        Instantiate(fractured, transform.position, transform.rotation);
        Destroy(gameObject);
    }

}
