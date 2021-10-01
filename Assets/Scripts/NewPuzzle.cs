using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewPuzzle : Interactable
{
    Inventory inventory;
    public GameObject discOuter, discMid, discInner;
    InventoryUI ui;

    // Start is called before the first frame update
    void Start()
    {
        /*discOuter = transform.Find("Pillar3").Find("DiscOuter").GetComponent<GameObject>();
        discMid = transform.Find("Pillar2").Find("DiscMid").GetComponent<GameObject>();
        discInner = transform.Find("Pillar1").Find("DiscInner").GetComponent<GameObject>();*/
        inventory = Inventory.instance;
        ui = FindObjectOfType<InventoryUI>();
    }

    public override void Interact()
    {
        //call parent function
        base.Interact();

        PlaceDisc();
    }

    void PlaceDisc()
    {
        if(inventory.items.Count == 0)
        {
            Debug.Log("Inventory empty");
        }
        else
        {
            switch(inventory.items[0].name)
            {
                case "Test item 1":
                    ui.ringOuter.SetActive(false);
                    discOuter.SetActive(true);
                    Debug.Log("Placing outer");
                    break;
                case "Test item 2":
                    ui.ringMid.SetActive(false);
                    discMid.SetActive(true);
                    Debug.Log("Placing mid");
                    break;
                case "Test item 3":
                    ui.ringInner.SetActive(false);
                    discInner.SetActive(true);
                    Debug.Log("Placing inner");
                    break;
            }
            inventory.Remove(inventory.items[0]);
        }
    }
}
