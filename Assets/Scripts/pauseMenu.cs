using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class pauseMenu : MonoBehaviour
{
    public static bool GamePaused = false;
    public GameObject pauseMenuUI;
    public Image loadingScreen;
    private bool fadeIn = true;
    public float alpha = 1;

    private GameObject player;
    private Animator playerAnim;
    private PlayerMovement playerMove;

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

        player = FindObjectOfType<PlayerManager>().gameObject;
        playerAnim = player.GetComponent<Animator>();
        playerMove = player.GetComponent<PlayerMovement>();
        playerMove.isCutScene = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!playerMove.isCutScene)
            {
                if (GamePaused) Resume();
                else Pause();
            }
        }

        if (fadeIn)
        {
            loadingScreen.color = new Color(0, 0, 0, alpha);

            if (alpha <= 0.35f)
            {
                alpha -= Time.unscaledDeltaTime;
                playerMove.isCutScene = false;
                if (alpha <= 0) fadeIn = false;
            }
            else
            {
                alpha -= Time.unscaledDeltaTime * 0.25f;
                playerMove.isCutScene = true;
            }
        }
    }

    private void OnLevelWasLoaded(int level)
    {
        if (SceneManager.GetActiveScene().buildIndex != 5)
        {
            player = FindObjectOfType<PlayerManager>().gameObject;
            playerAnim = player.GetComponent<Animator>();
            playerMove = player.GetComponent<PlayerMovement>();
            playerMove.isCutScene = true;
            alpha = 1;
            fadeIn = true;
        }
    }

    public void Resume()
    {
        Time.timeScale = 1f;
        playerAnim.SetFloat("speedMult", 1);
        GamePaused = false;
        pauseMenuUI.SetActive(false);
    }

    void Pause()
    {
        pauseMenuUI.SetActive(true);
        playerAnim.SetFloat("speedMult", 0);
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
