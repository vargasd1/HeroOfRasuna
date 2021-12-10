using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class loadingScreenScript : MonoBehaviour
{
    public static string scene;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(loadAsyncOperation());
    }

    IEnumerator loadAsyncOperation()
    {
        AsyncOperation gameLevel = SceneManager.LoadSceneAsync(scene);

        while (gameLevel.progress < 1)
        {
            yield return new WaitForEndOfFrame();
        }
    }
}