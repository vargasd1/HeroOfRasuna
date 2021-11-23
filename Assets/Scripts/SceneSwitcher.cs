using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SceneSwitcher : MonoBehaviour
{

    public Image loadScreen;
    private bool fadeOut = false;
    private float alpha = 0;

    private void Start()
    {
        fadeOut = false;
        loadScreen.color = new Color(0, 0, 0, 0);
        alpha = 0;
        loadScreen.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (fadeOut)
        {
            alpha += Time.unscaledDeltaTime;
            loadScreen.color = new Color(0, 0, 0, alpha);
            if (loadScreen.color.a >= 1)
            {
                loadingScreenScript.scene = "FirstFloor";
                SceneManager.LoadScene(4);//must change depending on included scenes in build
            }
        }
    }

    public void switchToPlay()
    {
        fadeOut = true;
        loadScreen.gameObject.SetActive(true);
        FindObjectOfType<AudioManager>().Stop("Theme");
        FindObjectOfType<AudioManager>().Play("Ambient1");
        //AudioAnywhere.PlayAnywhere("Ambient1");
    }

    public void quitGame()
    {
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif

    }
}
