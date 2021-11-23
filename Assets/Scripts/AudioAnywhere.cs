using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AudioAnywhere
{
    public static void PlayAnywhere(string name)
    {
        Scene scn = SceneManager.GetSceneByName("DontDestroyOnLoad");
        GameObject[] gameObjects = scn.GetRootGameObjects();
        GameObject audio = null;
        foreach (GameObject s in gameObjects)
        {
            if (s.name == "AudioManager") audio = s;
        }

        audio.GetComponent<AudioManager>().Play(name);
    }

    public static void PlayUninterruptedAnywhere(string name)
    {
        Scene scn = SceneManager.GetSceneByName("DontDestroyOnLoad");
        GameObject[] gameObjects = scn.GetRootGameObjects();
        GameObject audio = null;
        foreach(GameObject s in gameObjects)
        {
            if (s.name == "AudioManager") audio = s;
        }

        audio.GetComponent<AudioManager>().PlayUninterrupted(name);
    }
}
