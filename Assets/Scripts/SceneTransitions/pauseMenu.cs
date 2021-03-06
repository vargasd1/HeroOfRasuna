using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;

//ATTACHED TO: Pause Canvas gameobject/prefab

public class pauseMenu : MonoBehaviour
{
    public static bool GamePaused = false;
    public GameObject pauseMenuUI, canvasUI, blur;
    public AudioMixer audioMixer;
    public Image loadingScreen;
    private bool fadeIn = true;
    public float alpha = 1;

    private GameObject player;
    private Animator playerAnim;
    private PlayerMovement playerMove;

    void Start()
    {
        canvasUI = GameObject.Find("Canvas - UI");
    }

    // called right before Start() methods, so sounds can be called in Start()
    void Awake()
    {
        player = FindObjectOfType<PlayerManager>().gameObject;
        playerAnim = player.GetComponent<Animator>();
        playerMove = player.GetComponent<PlayerMovement>();
        playerMove.isCutScene = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
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
        if (SceneManager.GetActiveScene().buildIndex < 4 && SceneManager.GetActiveScene().buildIndex > 0)
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
        canvasUI.SetActive(true);
        blur.SetActive(false);
    }

    void Pause()
    {
        pauseMenuUI.SetActive(true);
        canvasUI.SetActive(false);
        blur.SetActive(true);
        playerAnim.SetFloat("speedMult", 0);
        Time.timeScale = 0f;
        GamePaused = true;
    }

    public void MainMenu()
    {
        Time.timeScale = 1f;
        NextFloorPlayer.ResetValues();
        SceneManager.LoadScene("MainMenu");
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void SetVolume(float vol)
    {
        audioMixer.SetFloat("MasterVolume", vol);
    }

    public void SetFullscreen()
    {
        Screen.fullScreen = !Screen.fullScreen;
    }
}
