using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.SceneManagement;

public class CreditsLogic : MonoBehaviour
{

    private VideoPlayer vid;
    private float timer = 2;

    // Start is called before the first frame update
    void Start()
    {
        vid = GetComponent<VideoPlayer>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (timer <= 0)
        {
            if (!vid.isPlaying)
            {
                SceneManager.LoadScene(0);
            }
        }
        else
        {
            timer -= Time.fixedDeltaTime;
        }
    }
}
