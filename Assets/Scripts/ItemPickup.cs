using UnityEngine;
using UnityEngine.UI;

public class ItemPickup : Interactable
{
    public Item item;

    void Start()
    {
        item.name = gameObject.name;
    }

    public override void Interact()
    {
        //call parent function
        base.Interact();

        PickUp();
    }

    void PickUp()
    {
        Debug.Log("Picking up " + item.name);
        //Add to inventory
        Inventory.instance.Add(item);
        textPickup.SetActive(false);
        Destroy(gameObject);
    }
}
