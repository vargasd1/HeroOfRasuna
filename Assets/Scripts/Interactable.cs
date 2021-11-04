using UnityEngine;
using UnityEngine.UI;

public class Interactable : MonoBehaviour
{
    public float radius = 3f;
    public Transform interactionTransform;//put in the interaction point in Unity editor
    public Transform player;
    public GameObject textPickup, textPlace;

    public virtual void Interact()
    {
        //this method is meant to be overwritten
        //Debug.Log("Interacting with " + transform.name);
    }

    void Update()
    {
        //check if the player is interacting with the object
        float distance = Vector3.Distance(player.position, interactionTransform.position);
        if(Input.GetKeyDown(KeyCode.G) && distance <= radius)
        {
            Interact();
        }
    }
    void OnDrawGizmosSelected()
    {
        //for use in editor
        //keep editor from giving errors
        if (interactionTransform == null) interactionTransform = transform;
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(interactionTransform.position, radius);
    }

    void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.name == "Player")
        {
            if (interactionTransform.name == "Puzzle (empty)")
            {
                if (!FindObjectOfType<PuzzleActive>().finishPuzzle) textPlace.SetActive(true);
            }
            else textPickup.SetActive(true);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.name == "Player")
        {
            if (interactionTransform.name == "Puzzle (empty)") textPlace.SetActive(false);
            else textPickup.SetActive(false);
        }
    }

    void OnTriggerStay(Collider other)
    {
        if (other.gameObject.name == "Player")
        {
            if (interactionTransform.name == "Puzzle (empty)")
            {
                if (!FindObjectOfType<PuzzleActive>().finishPuzzle) textPlace.SetActive(true);
                else textPlace.SetActive(false);
            }
            else textPickup.SetActive(true);
        }
    }
}
