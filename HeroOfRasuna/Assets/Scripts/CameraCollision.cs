using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraCollision : MonoBehaviour
{
    public float minDistance = 1.0f;
    public float maxDistance = 6.0f;
    public float smooth = 10.0f;
    public Vector3 dollyDir;
    public Vector3 wallDir;
    public Vector3 dollyDirAdjusted;
    public float distance;
    public float cameraYOffset;
    private float camYOff;
    public LayerMask layer;
    // Start is called before the first frame update
    void Start()
    {
        dollyDir = transform.localPosition.normalized;
        distance = transform.localPosition.magnitude;
        camYOff = transform.localPosition.y + cameraYOffset;
        wallDir = new Vector3(transform.localPosition.x, camYOff, transform.localPosition.z);
    }

    // Update is called once per frame
    void Update()
    {
        // Sets cameras desired position where we set
        Vector3 desiredCameraPos = transform.parent.TransformPoint(dollyDir * maxDistance);
        RaycastHit hit;

        //Finds the distance between the camera and the player
        if (Physics.Linecast(transform.parent.position, desiredCameraPos, out hit))
        {
            if ((hit.collider.gameObject.layer) == 10 || hit.collider.gameObject.layer == 11)
            {
                distance = Mathf.Clamp((hit.distance * 0.1f), minDistance, maxDistance);
            }
        }
        else
        {
            distance = maxDistance;
        }

        //Moves camera updwards when distance becomes too close to player and would originally clip through the player
        if (distance == minDistance)
        {
            transform.localPosition = Vector3.Lerp(transform.localPosition, wallDir * distance, Time.deltaTime * smooth);
        }

        //Moves the camera from desired position to the position it's being moved
        transform.localPosition = Vector3.Lerp(transform.localPosition, dollyDir * distance, Time.deltaTime * smooth);
    }
}
