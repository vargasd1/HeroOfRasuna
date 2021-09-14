using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class loadNextLevel : MonoBehaviour
{

    public Image loadScreen;
    private bool fadeOut = false;
    private float alpha = 0;
    private PlayerMovement player;

    private void Start()
    {
        player = FindObjectOfType<PlayerMovement>();
    }

    private void Update()
    {
        if (fadeOut)
        {
            alpha += Time.unscaledDeltaTime;
            loadScreen.color = new Color(0, 0, 0, alpha);
            if (loadScreen.color.a >= 1)
            {
                SceneManager.LoadScene(5);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            player.isCutScene = true;
            fadeOut = true;
            loadScreen.gameObject.SetActive(true);
            switch (SceneManager.GetActiveScene().buildIndex)
            {
                case 2:
                    loadingScreenScript.scene = "SecondFloor";
                    break;
                case 3:
                    loadingScreenScript.scene = "ThirdFloor";
                    break;
                default:
                    loadingScreenScript.scene = "MainMenu";
                    break;
            }

        }
    }
}
