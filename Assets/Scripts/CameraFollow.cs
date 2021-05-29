using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class CameraFollow : MonoBehaviour
{
    public float cameraMoveSpeed = 120.0f;
    public GameObject CameraFollowObject;
    Transform target;
    Vector3 FollowPOS;
    public float clampAngle = 80.0f;
    public float inputSensitivity = 150.0f;
    public GameObject CameraObject;
    public GameObject PlayerObject;
    public float camDistanceXToPlayer;
    public float camDistanceYToPlayer;
    public float camDistanceZToPlayer;
    public float mouseX;
    public float mouseY;
    public float finalInputX;
    public float finalInputY;
    public float smoothX;
    public float smoothY;
    private float rotY = 0.0f;
    private float rotX = 0.0f;
    public LayerMask layerMask;
    private Quaternion targetRotation1 = Quaternion.Euler(0, -90, 0);
    private Quaternion targetRotation2 = Quaternion.Euler(0, 270, 0);
    private Quaternion targetRotation3 = Quaternion.Euler(0, 70, 0);
    private Quaternion targetRotation4 = Quaternion.Euler(0, -22, 0);
    private Quaternion targetRotation5 = Quaternion.Euler(0, 50, 0);
    private Quaternion targetRotation6 = Quaternion.Euler(0, 12, 0);
    private List<GameObject> hitObjs = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        Vector3 rot = transform.localRotation.eulerAngles;
        rotY = rot.y;
        rotX = rot.x;

        //Locks screen and hides mouse
        // Cursor.lockState = CursorLockMode.Locked;
        //Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
        Application.targetFrameRate = 60;

        //Exit Game
        if (Input.GetKey(KeyCode.Escape))
        {
            Application.Quit();
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#endif
        }
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

            //Cast Ray from camera to player and check collision between the two
            RaycastHit hit;
            if (Physics.Raycast(target.transform.position + new Vector3(0, 1, 0), Camera.main.transform.position, out hit))
            {
                if (hit.collider.gameObject.tag == "Wall")
                {
                    if (hitObjs.Count > 0)
                    {
                        foreach (GameObject obj in hitObjs)
                        {
                            if (hit.collider.gameObject != obj)
                            {
                                hit.collider.gameObject.GetComponent<WallFade>().SetMaterialTransparent();
                                hitObjs.Add(hit.collider.gameObject);
                            }
                        }
                    }
                    else
                    {
                        hit.collider.gameObject.GetComponent<WallFade>().SetMaterialTransparent();
                        hitObjs.Add(hit.collider.gameObject);
                    }
                }
            }
            else
            {
                if (hitObjs.Count > 0)
                {
                    for (int i = 0; i < hitObjs.Count; i++)
                    {
                        hitObjs[i].gameObject.GetComponent<WallFade>().fadeOut = false;
                        hitObjs[i].gameObject.GetComponent<WallFade>().fadeIn = true;
                        hitObjs.Remove(hitObjs[i]);
                    }
                }
            }
        }
    }
}
