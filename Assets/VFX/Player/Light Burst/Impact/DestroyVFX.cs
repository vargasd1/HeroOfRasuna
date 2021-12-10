using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class DestroyVFX : MonoBehaviour
{

    
    public float timer = 1;
   
    

    // Update is called once per frame
    void Update()
    {
       if(timer <= 0)
        {
            Destroy(gameObject);
        }
        else
        {
            timer -= Time.deltaTime;
        }

    }

    
}
