using UnityEngine;
using UnityEngine.UI;

//placed on Canvas- UI

public class InventoryUI : MonoBehaviour
{
    Inventory inventory;
    public GameObject ringOuter, ringMid, ringInner;

    // Start is called before the first frame update
    void Start()
    {
        /*ringOuter = transform.Find("Ring_Outer").GetComponent<GameObject>();
        ringMid = transform.Find("Ring_Mid").GetComponent<GameObject>();
        ringInner = transform.Find("Ring_Inner").GetComponent<GameObject>();*/
        inventory = Inventory.instance;
        inventory.onItemChangedCallback += UpdateUI;
    }

    void UpdateUI()
    {
        //Debug.Log("Updating UI");

        //loop through and check for items
        for (int i = 0; i < inventory.items.Count; ++i)
        {
            //Debug.Log(inventory.items[i].name);
            switch(inventory.items[i].name)
            {
                case "Test item 1":
                    ringOuter.SetActive(true);
                    break;
                case "Test item 2":
                    ringMid.SetActive(true);
                    break;
                case "Test item 3":
                    ringInner.SetActive(true);
                    break;
            }
        }
    }
}
