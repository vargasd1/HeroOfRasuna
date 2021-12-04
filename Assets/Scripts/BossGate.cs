﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossGate : MonoBehaviour
{

    public int enemiesKilled = 0;
    public bool openDoor = false;
    private bool openOnce = true;
    public bool closeDoor = false;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (enemiesKilled >= 6)
        {
            openDoor = true;
        }

        if (openDoor && openOnce)
        {
            float step = 1.5f * Time.unscaledDeltaTime;
            transform.position = Vector3.MoveTowards(transform.position, new Vector3(31.74f, 5, 0), step);
            if (Vector3.Distance(transform.position, new Vector3(31.74f, 5, 0)) < .05f)
            {
                transform.position = new Vector3(31.74f, 5, 0);
                openDoor = false;
                openOnce = false;
            }
        }

        if (closeDoor)
        {
            openDoor = false;
            float step = 18 * Time.unscaledDeltaTime;
            transform.position = Vector3.MoveTowards(transform.position, new Vector3(31.74f, 0, 0), step);
            if (Vector3.Distance(transform.position, new Vector3(31.74f, 0, 0)) < .05f)
            {
                transform.position = new Vector3(31.74f, 0, 0);
                closeDoor = false;
            }
        }
    }
}