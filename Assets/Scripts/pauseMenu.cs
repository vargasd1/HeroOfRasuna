using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class pauseMenu : MonoBehaviour
{
    public static bool GamePaused = false;
    public GameObject pauseMenuUI;

    public Animator player;

    public static pauseMenu instance;

    // called right before Start() methods, so sounds can be called in Start()
    void Awake()
    {
        // don't destroy on scene change
        if (instance == null) instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);

        player = FindObjectOfType<PlayerManager>().gameObject.GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (GamePaused) Resume();
            else Pause();
        }
    }

    public void Resume()
    {
        Time.timeScale = 1f;
        player.SetFloat("speedMult", 1);
        GamePaused = false;
        pauseMenuUI.SetActive(false);
    }

    void Pause()
    {
        pauseMenuUI.SetActive(true);
        player.SetFloat("speedMult", 0);
        Time.timeScale = 0f;
        GamePaused = true;
    }

    public void Settings()
    {
        Debug.Log("Opening settings");
    }

    public void MainMenu()
    {
        Time.timeScale = 1f;
        Debug.Log("Loading main menu");
        SceneManager.LoadScene("Menu");
    }

    public void QuitGame()
    {
        Debug.Log("Quitting game");
        Application.Quit();
    }
}
