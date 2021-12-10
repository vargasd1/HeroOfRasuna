using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TooltipScript : MonoBehaviour
{
    //ATTACHED TO: each tooltip prefab in the firstFloor scene
    public int tip = 0;//change in Unity editor

    void OnTriggerStay(Collider other)
    {
        if (other.tag == "Player")
        {
            if (!other.GetComponent<PlayerMovement>().isCutScene)
            {
                ActiveOrNot(true);
            }
            else
            {
                ActiveOrNot(false);
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            ActiveOrNot(false);
            //removeHint();

        }
    }

    void ActiveOrNot(bool b)
    {
        switch (tip)
        {
            case 0:
                GameObject.Find("/Canvas - UI/Text/Text - Tooltip Move").SetActive(b);
                break;
            case 1:
                GameObject.Find("/Canvas - UI/Text/Text - Tooltip Run").SetActive(b);
                break;
            case 2:
                GameObject.Find("/Canvas - UI/Text/Text - Tooltip LClick").SetActive(b);
                break;
            case 3:
                GameObject.Find("/Canvas - UI/Text/Text - Tooltip Q").SetActive(b);
                break;
            case 4:
                GameObject.Find("/Canvas - UI/Text/Text - Tooltip RClick").SetActive(b);
                break;
            case 5:
                GameObject.Find("/Canvas - UI/Text/Text - Tooltip E").SetActive(b);
                break;
            case 6:
                GameObject.Find("/Canvas - UI/Text/Text - Tooltip R").SetActive(b);
                break;
            case 7:
                GameObject.Find("/Canvas - UI/Text/Text - Tooltip Pause").SetActive(b);
                break;
            case 8:
                GameObject.Find("/Canvas - UI/Text/Text - Tooltip Charge").SetActive(b);
                break;
        }
    }

 
}
