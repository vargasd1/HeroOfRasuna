using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventMng : MonoBehaviour
{
    public static EventMng instance;
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
    }
}
