using UnityEngine.Audio;
using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

// ATTACHED TO: AudioManager prefab. Prefab should ONLY be placed in the start screen

public class AudioManager : MonoBehaviour
{
    public Sound[] sounds;
    public static AudioManager instance;
    public AudioMixer mixer;

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

        // add audio source component to each sound
        foreach(Sound s in sounds)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;
            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
            s.source.loop = s.loop;
            s.source.outputAudioMixerGroup = s.audioMixerGroup;
        }
    }

    // called first
    void OnEnable()
    {
        Debug.Log("OnEnable called");
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void Start()
    {
        //Debug.Log("Start theme");
        //Play("Theme");
    }

    // play BGM
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log("OnSceneLoaded: " + scene.name);
        switch (SceneManager.GetActiveScene().buildIndex)//case numbers change by build
        {
            case 1:
                Stop("Theme");
                Stop("Exploration1");
                Play("Exploration1");
                break;
            case 2:
                Stop("Exploration1");
                Stop("Exploration2");
                Play("Exploration2");
                break;
            case 3:
                Stop("Exploration2");
                Stop("Exploration3");
                Stop("Boss Fight");
                Play("Exploration3");
                break;
            case 4:
                break;
            default:
                Stop("Exploration1");
                Stop("Exploration2");
                Stop("Exploration3");
                Stop("Boss Fight");
                Play("Theme");
                break;
        }
    }

    // play sound based on input name
    public void Play(string name)
    {
        // TO USE: FindObjectOfType<AudioManager>().Play(name);
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null)
        {
            Debug.Log("Sound: " + name + " not found");
            return;
        }

        //if (pauseMenu.GamePaused) s.source.pitch = .5f;
        //else s.source.pitch = 1;

        s.source.Play();
    }

    // stop playing a currently playing sound
    public void Stop(string name)
    {
        // TO USE: FindObjectOfType<AudioManager>().Stop(name);

        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null)
        {
            Debug.Log("Sound: " + name + " not found");
            return;
        }
        s.source.Stop();
    }

    IEnumerator playSoundWithDelay(string clip, float delay, Sound s)
    {
        yield return new WaitForSecondsRealtime(delay);
        //if (pauseMenu.GamePaused) s.source.pitch = .5f;
        //else s.source.pitch = 1;
        //s.source.Play();
        s.source.PlayOneShot(s.source.clip, s.source.volume);
    }

    public void PlayInSeconds(string name, float seconds)
    {
        // TO USE: FindObjectOfType<AudioManager>().PlayInSeconds(name, delay);
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null)
        {
            Debug.Log("Sound: " + name + " not found");
            return;
        }
        else
        {
            StartCoroutine(playSoundWithDelay(name, seconds, s));
        }
    }

    public void PlayUninterrupted(string name)
    {
        // TO USE: FindObjectOfType<AudioManager>().PlayUninterrupted(name);
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null)
        {
            Debug.Log("Sound: " + name + " not found");
            return;
        }
        else
        {
            //if (pauseMenu.GamePaused) s.source.pitch = .5f;
            //else s.source.pitch = 1;
            Debug.Log("Sound: " + name);
            s.source.PlayOneShot(s.source.clip, s.source.volume);
        }
    }

    public void ChangePitch(string name, float pitch)
    {
        // TO USE: FindObjectOfType<AudioManager>().ChangePitch(name, pitch);
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null)
        {
            Debug.Log("Sound: " + name + " not found");
            return;
        }
        else
        {
            Debug.Log("Changing pitch of " + name);
            s.source.pitch = pitch;
        }
    }

    public void SlowSounds()
    {
        // TO USE: FindObjectOfType<AudioManager>().SlowSounds();
        //currently only slows abilities (4-6) and (15-18)
        for(int x = 4; x < 7; ++x)
        {
            Sound s = sounds[x];
            s.source.pitch = .4f;
        }
        for (int x = 17; x < 19; ++x)
        {
            Sound s = sounds[x];
            s.source.pitch = .4f;
        }
        //Surav's abilities need to be pitched to the slowed time, but since there's a transition, just use .1
        for (int x = 15; x < 17; ++x)
        {
            Sound s = sounds[x];
            s.source.pitch = .1f;
        }
    }

    public void ResetSounds()
    {
        //currently only slows abilities (4-6) and (15-18)
        for (int x = 4; x < 7; ++x)
        {
            Sound s = sounds[x];
            s.source.pitch = 1f;
        }
        for (int x = 15; x < 19; ++x)
        {
            Sound s = sounds[x];
            s.source.pitch = 1f;
        }
    }
}
