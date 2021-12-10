using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SceneSwitcher : MonoBehaviour
{

    public Image loadScreen;
    private bool fadeOut = false;
    private bool fadeIn = false;
    private float alpha = 1;
    private string sceneName = "FirstFloor";

    private void Start()
    {
        fadeOut = false;
        fadeIn = true;
        alpha = 1;
    }

    private void Update()
    {
        if (fadeIn)
        {
            alpha -= Time.fixedDeltaTime * 0.8f;

            loadScreen.color = new Color(0, 0, 0, alpha);

            if (loadScreen.color.a <= 0)
            {
                fadeIn = false;
                loadScreen.gameObject.SetActive(false);
            }
        }

        if (fadeOut)
        {
            alpha += Time.fixedDeltaTime;
            loadScreen.color = new Color(0, 0, 0, alpha);
            if (loadScreen.color.a >= 1)
            {
                loadingScreenScript.scene = sceneName;
                SceneManager.LoadScene(4);//must change depending on included scenes in build
            }
        }
    }

    public void switchToPlay()
    {
        fadeOut = true;
        loadScreen.gameObject.SetActive(true);
        sceneName = "FirstFloor";
    }

    public void switchToCredits()
    {
        fadeOut = true;
        loadScreen.gameObject.SetActive(true);
        sceneName = "CreditScene";
    }

    public void quitGame()
    {
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif

    }
}
