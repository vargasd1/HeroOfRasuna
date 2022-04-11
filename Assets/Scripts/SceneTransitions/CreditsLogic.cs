using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using UnityEngine.SceneManagement;

/// <summary>
/// This script is used to send the player back to the Main Menu when the video is done.
/// Also if the player clicks during credits, player goes to Main Menu.
/// 
/// ATTATCHED TO: Video Player
/// </summary>

public class CreditsLogic : MonoBehaviour
{

    private VideoPlayer vid;
    public RawImage vidMat;
    private float timer = 1.5f;

    // Start is called before the first frame update
    void Start()
    {
        vid = GetComponent<VideoPlayer>();

        vid.url = System.IO.Path.Combine(Application.streamingAssetsPath, "HoR_Credits_Revised.mp4");
        vid.Play();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        // Timer to not let Player skip right away, as well as lets video start before checking !vid.isPlaying
        if (timer <= 0)
        {
            if (!vid.isPlaying)
            {
                SceneManager.LoadScene(0);
            }

            if (Input.GetMouseButtonDown(0))
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
