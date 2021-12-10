using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonHover : MonoBehaviour, IPointerEnterHandler
    //IPointerExitHandler
{
    //Attach this script to the GameObject you would like to have mouse hovering detected on
    //https://gamedev.stackexchange.com/questions/116801/how-can-i-detect-that-the-mouse-is-over-a-button-so-that-i-can-display-some-ui-t
    //Detect if the Cursor starts to pass over the GameObject
    public void OnPointerEnter(PointerEventData pointerEventData)
    {
        FindObjectOfType<AudioManager>().PlayUninterrupted("HoverOn");
        //AudioAnywhere.PlayUninterruptedAnywhere("Hover On");
    }

    //Detect when Cursor leaves the GameObject
    /*public void OnPointerExit(PointerEventData pointerEventData)
    {
        FindObjectOfType<AudioManager>().PlayUninterrupted("HoverOff");
        //AudioAnywhere.PlayUninterruptedAnywhere("Hover Off");
    }*/
}
