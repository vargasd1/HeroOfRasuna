using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

/// <summary>
/// This script lets the camera follow whatever its target is
/// 
/// ATTATCHED TO: Camera (Prefab)
/// </summary>

public class CameraFollow : MonoBehaviour
{
    public float cameraMoveSpeed = 120.0f;
    public GameObject CameraFollowObject;
    Transform target;

    // Start is called before the first frame update
    void Start()
    {
        CameraFollowObject = FindObjectOfType<PlayerManager>().gameObject;
        Application.targetFrameRate = 120;
    }

    void LateUpdate()
    {
        CameraUpdater();
    }

    private void CameraUpdater()
    {
        //Sets camera follow target
        if (CameraFollowObject)
        {
            target = CameraFollowObject.transform;

            //Moves the camera around the object based on speed (step)
            float step = cameraMoveSpeed * Time.fixedUnscaledDeltaTime;// Time.deltaTime;
            transform.position = Vector3.MoveTowards(transform.position, target.position, step);
        }
    }
}
