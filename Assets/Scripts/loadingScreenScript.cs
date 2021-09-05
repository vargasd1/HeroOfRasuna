using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class loadingScreenScript : MonoBehaviour
{

    public Image progressBar;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(loadAsyncOperation());
    }

    IEnumerator loadAsyncOperation()
    {
        AsyncOperation gameLevel = SceneManager.LoadSceneAsync(4);

        while (gameLevel.progress < 1)
        {
            progressBar.transform.localScale = new Vector3(1, gameLevel.progress, 1);
            yield return new WaitForEndOfFrame();
        }
    }
}