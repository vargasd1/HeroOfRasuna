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
    public GameObject pauseMenuUI, settingsMenuUI, canvasUI, blur;
    public AudioMixer audioMixer;
    public Image loadingScreen;
    private bool fadeIn = true;
    public float alpha = 1;

    private GameObject player;
    private Animator playerAnim;
    private PlayerMovement playerMove;

    Resolution[] resolutions;
    public TMPro.TMP_Dropdown resolutionDropdown;
    int curRes = 0;

    void Start()
    {
        resolutions = Screen.resolutions;
        resolutionDropdown.ClearOptions();

        List<string> resStr = new List<string>();
        for(int i = 0; i < resolutions.Length; ++i)
        {
            string option = resolutions[i].width + " x " + resolutions[i].height;
            if ((resolutions[i].width == 2560 && resolutions[i].height == 1440) || (resolutions[i].width == 1920 && resolutions[i].height == 1080) || (resolutions[i].width == 1280 && resolutions[i].height == 720))
            {
                resStr.Add(option);
                curRes++;
            }

            //if(resolutions[i].width == Screen.width && resolutions[i].height == Screen.height)
            //{
                //curRes = i;
            //}
        }

        resolutionDropdown.AddOptions(resStr);
        resolutionDropdown.value = curRes;
        resolutionDropdown.RefreshShownValue();

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
        if (SceneManager.GetActiveScene().buildIndex != 4)
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
        settingsMenuUI.SetActive(false);
        canvasUI.SetActive(true);
        blur.SetActive(false);
    }

    void Pause()
    {
        pauseMenuUI.SetActive(true);
        settingsMenuUI.SetActive(false);
        canvasUI.SetActive(false);
        blur.SetActive(true);
        playerAnim.SetFloat("speedMult", 0);
        Time.timeScale = 0f;
        GamePaused = true;
    }

    public void Settings()
    {
        Debug.Log("Opening settings");
        pauseMenuUI.SetActive(false);
        settingsMenuUI.SetActive(true);
    }

    public void MainMenu()
    {
        Time.timeScale = 1f;
        Debug.Log("Loading main menu");
        SceneManager.LoadScene("MainMenu");
    }

    public void QuitGame()
    {
        Debug.Log("Quitting game");
        Application.Quit();
    }

    public void ReturnToPause()
    {
        settingsMenuUI.SetActive(false);
        pauseMenuUI.SetActive(true);
    }

    public void SetVolume(float vol)
    {
        Debug.Log(vol);
        audioMixer.SetFloat("MasterVolume", vol);
    }

    public void SetQuality(int q)
    {
        QualitySettings.SetQualityLevel(q);
        Debug.Log("Changed quality");
    }

    public void SetFullscreen(bool f)
    {
        Screen.fullScreen = !Screen.fullScreen;
    }

    public void SetResolution(int r)
    {
        Resolution res = resolutions[r];
        Screen.SetResolution(res.width, res.height, Screen.fullScreen);
    }
}
