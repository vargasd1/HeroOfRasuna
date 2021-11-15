using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class XpMagnetizeToPlayer : MonoBehaviour
{

    public PlayerMovement player;
    float absorbTimer = 1;

    // Start is called before the first frame update
    void Start()
    {
        // Find Player
        player = FindObjectOfType<PlayerMovement>().GetComponent<PlayerMovement>();
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
                    player.overclockChargedAmt += 5;
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
