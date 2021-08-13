using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class XpMagnetizeToPlayer : MonoBehaviour
{

    GameObject player;
    float absorbTimer = 1;

    // Start is called before the first frame update
    void Start()
    {
        // Ignore Collision Boxes/Spheres/Etc. of specified Layers
        Physics.IgnoreLayerCollision(10, 11);
        Physics.IgnoreLayerCollision(11, 12);
        Physics.IgnoreLayerCollision(11, 11);
        // Find Player
        player = FindObjectOfType<PlayerManager>().gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        // If spawn Delay is <= 0 then start absorption
        if (absorbTimer <= 0)
        {
            // If the player is close enough start to follow
            if (Vector3.Distance(player.transform.position, transform.position) < 5)
            {
                // Lerp to the players position
                transform.position = AnimMath.Lerp(transform.position, player.transform.position, 0.13f);
                // If super close to the player be absorbed
                if (Vector3.Distance(player.transform.position, transform.position) < 0.5f)
                {
                    player.GetComponent<PlayerMovement>().overclockChargedAmt += 5;
                    Destroy(gameObject);
                }
            }
        }
        else
        {
            // If spawn Delay is > 0 then timer goes down
            absorbTimer -= Time.deltaTime;
        }
    }
}
