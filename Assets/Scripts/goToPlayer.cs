using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class goToPlayer : MonoBehaviour
{

    GameObject player;
    float absorbTimer = 1;

    // Start is called before the first frame update
    void Start()
    {
        Physics.IgnoreLayerCollision(10, 11);
        Physics.IgnoreLayerCollision(11, 12);
        player = FindObjectOfType<PlayerManager>().gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        if (absorbTimer <= 0)
        {
            if (Vector3.Distance(player.transform.position, transform.position) < 5)
            {
                transform.position = AnimMath.Lerp(transform.position, player.transform.position, 0.13f);
                if (Vector3.Distance(player.transform.position, transform.position) < 0.5f)
                {
                    Destroy(gameObject);
                }
            }
        }
        else
        {
            absorbTimer -= Time.deltaTime;
        }
    }
}
